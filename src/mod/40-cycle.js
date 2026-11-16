/* CHOUF v2 — 40-cycle : économie (§6), capacité (§7), flux internationaux (§8), indicateurs (§9),
   résolution d'un cycle (§10), événements, conditions de fin et bilan (§11).
   Ordre de résolution : saisons → opérateurs (transit, tâches, fatigue) → argent → factions → abandon →
   événements → indicateurs → trésorerie et dette → capacité → fin de partie. */

/* ---------- flux internationaux ---------- */
function offreFlux(id) { return B.flux.offres.filter(function (o) { return o.id === id; })[0]; }
function activerFlux(id) {
  var o = offreFlux(id);
  if (!o || G.flux.some(function (f) { return f.id === id && f.actif; })) return { ok: false, raison: "Déjà actif" };
  var cout = o.activation + o.engagement;
  if (G.tresorerie < cout) return { ok: false, raison: "Il faut " + cout + " € (activation + engagement)" };
  G.tresorerie -= cout;
  G.flux = G.flux.filter(function (f) { return f.id !== id; });
  G.flux.push({ id: id, actif: true, engage: o.engagement, fiabilite: o.fiabilite, exposition: o.exposition, instabilite: o.instabilite, cycles: 0, dernier: null });
  log("Flux activé : " + o.nom + " (−" + cout + " €).", "flux");
  save();
  return { ok: true };
}
function couperFlux(id) {
  var f = G.flux.filter(function (x) { return x.id === id && x.actif; })[0];
  if (!f) return;
  f.actif = false;
  log("Flux coupé : " + offreFlux(id).nom + ". " + f.engage + " € immobilisés perdus.", "flux");
  f.engage = 0;
  save();
}
function fluxActifs() { return G.flux.filter(function (f) { return f.actif; }); }

/* ---------- calculs du cycle (aussi utilisés pour l'aperçu du tableau de bord) ---------- */
/** Efficacité de pression appliquée aux revenus d'un quartier. */
function effRevenu(id) {
  var e = B.economie.revenu_sans_pression;
  opsActifs().forEach(function (o) { if (o.dispo !== "transit" && o.pos === id && o.tache === "presser") e = Math.max(e, efficacite(o, "presser")); });
  return e;
}
function reductionMaintien(id) {
  var r = 0;
  opsActifs().forEach(function (o) {
    if (o.dispo === "transit") return;
    if (o.pos === id && o.tache === "optimiser") r += B.taches.optimiser.baisse_maintien * efficacite(o, "optimiser");
    if ((o.pos === id || VILLE.adj[o.pos].indexOf(id) >= 0) && o.tache === "flux") r += B.taches.flux.baisse_maintien * efficacite(o, "flux") * (o.pos === id ? 1 : 0.5);
  });
  return clamp(r, 0, 0.8);
}
/** Prévision des entrées et sorties pour ce cycle (§6.2). */
function previsions() {
  var rev = 0, maint = 0, entr = 0, fluxRev = 0, fluxCout = 0, redEntr = 0;
  VILLE.ordre.forEach(function (id) {
    var s = G.q[id];
    if (s.ctl <= 0) return;
    var ch = s.ch.reduce(function (t, c) { return t + (c.proprio === "joueur" && c.ouvert && !c.saisi ? c.rent : 0); }, 0);
    rev += ch * (1 + s.inv * B.quartiers.investir_gain) * multSaison(id) * s.ctl / 100 * effRevenu(id);
    maint += s.maintien * (1 + s.inv * B.quartiers.investir_maintien) * (1 - reductionMaintien(id)) * s.ctl / 100;
  });
  opsActifs().forEach(function (o) {
    if (o.tache === "optimiser" && o.dispo !== "transit") redEntr += B.taches.optimiser.baisse_entretien * efficacite(o, "optimiser");
  });
  var nops = opsActifs().length;
  opsActifs().forEach(function (o) { entr += TYPES()[o.type].entretien; });
  entr *= 1 + Math.max(0, nops - 6) * (B.operateurs.entretien_progressif || 0);    // un gros réseau coûte plus cher par tête
  entr *= 1 - clamp(redEntr, 0, 0.3);
  fluxActifs().forEach(function (f) { var o = offreFlux(f.id); fluxRev += o.revenu * f.fiabilite / 100; fluxCout += o.cout; });
  var interets = G.dette * B.economie.dette_interet;
  return { revenus: Math.round(rev), maintien: Math.round(maint), entretien: Math.round(entr), flux_revenus: Math.round(fluxRev), flux_couts: fluxCout,
    interets: Math.round(interets), solde: Math.round(rev + fluxRev - maint - entr - fluxCout - interets) };
}

/* ---------- résolution ---------- */
function saisonsDuCycle() {
  var s = semaine(), out = [];
  B.evenements.saisons.forEach(function (x) {
    if (x.semaines && s >= x.semaines[0] && s <= x.semaines[1]) out.push(x.id);
    else if (x.pair_semaines && s % 2 === 0 && !(s >= x.hors_semaines[0] && s <= x.hors_semaines[1])) out.push(x.id);
    else if (x.proba && rnd() < x.proba) out.push(x.id);
  });
  return out;
}

/** Valide le cycle : le système résout tout et retourne le rapport (lignes courtes). */
function validerCycle() {
  if (G.fini) return null;
  var I = B.indicateurs, O = B.operateurs, T = B.taches, ind = G.ind;
  G.rapport = [];
  var avant = { tresorerie: G.tresorerie, territoire: territoire(), pression: ind.pression, stabilite: ind.stabilite, exposition: ind.exposition };
  var possede = {};
  VILLE.ordre.forEach(function (id) { possede[id] = G.q[id].ctl > 0; });
  G.saisons = saisonsDuCycle();
  G.saisons.forEach(function (sid) { if (sid !== "match") log(B.evenements.saisons.filter(function (x) { return x.id === sid; })[0].nom + " : revenus modifiés.", "saison"); });

  // 1. opérateurs
  var present = {};
  opsActifs().forEach(function (o) {
    if (o.dispo === "indisponible") {
      if (--o.indispo <= 0) { o.dispo = "disponible"; o.fatigue = O.fatigue_apres_indispo; o.tache = "repos"; }
      return;
    }
    if (o.dispo === "transit") {
      if (--o.transit > 0) return;
      o.pos = o.dest; o.dest = null; o.dispo = o.tache === "repos" ? "disponible" : "occupe";
      log(nomOp(o) + " arrive à " + VILLE.q[o.pos].nom + ".", "op", o.pos);
    }
    present[o.pos] = true;
    var t = T[o.tache], e = efficacite(o, o.tache), s = G.q[o.pos];
    if (!t || o.tache === "repos") { o.fatigue = Math.max(0, o.fatigue - O.fatigue_repos); return; }
    o.fatigue = clamp(o.fatigue + t.fatigue * O.fatigue_par_cycle_ratio * (ind.stabilite < I.stabilite_basse ? I.stabilite_basse_fatigue : 1), 0, 100);
    o.xp += O.progression_competence * (t.type === o.type ? 1 : 0.5);
    if (o.xp >= 1 && o.comp < 5) { o.xp = 0; o.comp++; log(nomOp(o) + " progresse : compétence " + o.comp + ".", "op"); }
    switch (o.tache) {
      case "securiser":
        var a = advTotal(s), baisse = Math.min(a, t.baisse_adverse * e);
        if (a > 0) for (var f in s.adv) s.adv[f] = Math.max(0, s.adv[f] - baisse * s.adv[f] / a);
        if (s.ctl > 0 || a > 0) gagnerControle(o.pos, t.gain_controle * e);
        ind.pression += t.pression; ind.exposition += t.exposition;
        break;
      case "presser":
        gagnerControle(o.pos, t.gain_controle * e);
        ind.pression += t.pression; ind.exposition += t.exposition;
        break;
      case "reperer":
        if (rnd() < Math.min(1, 0.4 + e * 0.5)) s.conn = Math.min(3, s.conn + t.connaissance);
        VILLE.adj[o.pos].forEach(function (v) { if (rnd() < 0.6 * e) G.q[v].conn = Math.min(3, Math.max(G.q[v].conn, 1)); });
        break;
      case "surveiller":
        s.conn = Math.max(s.conn, 2);
        VILLE.adj[o.pos].forEach(function (v) { G.q[v].vu = G.cycle + 1; });
        s.vu = G.cycle + 1;
        break;
      case "optimiser":
        ind.exposition -= (t.baisse_exposition || 0) * e;
        break;
      case "flux":
        fluxActifs().forEach(function (fl) { fl.fiabilite = clamp(fl.fiabilite + t.fiabilite_flux * e, 0, 100); });
        break;
    }
    if (o.fatigue >= 100) { o.dispo = "indisponible"; o.indispo = O.indispo_cycles; log(nomOp(o) + " est épuisé : indisponible 1 cycle.", "fatigue"); }
  });

  // 2. argent du cycle
  var p = previsions();
  var fluxNet = 0;
  fluxActifs().forEach(function (fl) {
    var o = offreFlux(fl.id), Fx = B.flux;
    fl.cycles++;
    ind.exposition += o.volume * Fx.exposition_par_volume * 0.5;
    fl.exposition = clamp(fl.exposition + o.volume * Fx.exposition_par_volume, 0, 100);
    fl.instabilite = clamp(fl.instabilite + Fx.instabilite_par_cycle, 0, 100);
    if (rnd() * 100 < fl.fiabilite) { fluxNet += o.revenu - o.cout; fl.dernier = "ok"; }
    else { fluxNet -= o.cout; fl.dernier = "perte"; fl.fiabilite = clamp(fl.fiabilite - 5, 0, 100); log(o.nom + " : livraison ratée, pas de revenu.", "flux"); }
    if (rnd() < fl.instabilite / Fx.rupture_div) {
      fl.actif = false;
      log("Rupture : " + o.nom + ". " + fl.engage + " € perdus, pression +8.", "rupture");
      G.dette += Math.round(fl.engage * 0.25); ind.pression += 8; fl.engage = 0;
      var lg = opsActifs().filter(function (x) { return x.type === "logistique" && x.dispo !== "transit"; })[0];
      if (lg && rnd() < 0.4) perdreOp(lg, "arrêté après la rupture du flux");
    } else if (fl.fiabilite < Fx.fiabilite_partenaire_perdu) {
      fl.actif = false; log("Le partenaire de " + o.nom + " disparaît. Flux perdu.", "rupture"); fl.engage = 0;
    }
  });
  var solde = p.revenus - p.maintien - p.entretien - p.interets + fluxNet;

  // 3. factions
  tourFactions();

  majCharbons();

  // 4. quartiers laissés seuls ou sans argent
  VILLE.ordre.forEach(function (id) {
    var s = G.q[id];
    if (s.ctl <= 0) { s.sans_op = 0; return; }
    if (present[id]) s.sans_op = 0; else s.sans_op++;
    if (s.sans_op > B.controle.abandon_cycles) {
      // un quartier sécurisé tient seul (plancher), les autres s'effritent sans présence
      if (statut(id) === "securise") s.ctl = Math.max(B.controle.securise_controle, s.ctl - 1);
      else s.ctl = Math.max(0, s.ctl - B.controle.abandon_baisse);
    }
    if (G.tresorerie <= 0 && G.dette > 0) s.ctl = Math.max(0, s.ctl - B.controle.degradation_sans_argent);
    if (ind.stabilite < I.stabilite_basse) s.ctl = Math.max(0, s.ctl - I.stabilite_basse_degradation);
  });

  // 5. événements
  evenementsCycle();

  // 6. indicateurs
  var gagnes = VILLE.ordre.filter(function (id) { return !possede[id] && G.q[id].ctl > 0; });
  gagnes.forEach(function (id) { log("Nouveau quartier : " + VILLE.q[id].nom + ".", "gain", id); });
  ind.pression += gagnes.length * I.pression_par_quartier_gagne;
  if (ind.exposition > I.exposition_alerte) { ind.pression += I.exposition_vers_pression; log("Exposition trop haute : la pression monte.", "alerte"); }
  ind.pression -= I.pression_baisse + ind.pression * I.pression_baisse_ratio; ind.exposition -= I.exposition_baisse + ind.exposition * I.exposition_baisse_ratio;
  ind.stabilite += (I.stabilite_vers - ind.stabilite) * I.stabilite_derive;
  opsActifs().forEach(function (o) { if (o.fatigue > 80) ind.stabilite -= I.stabilite_fatigue; });
  ind.stabilite += Math.min(4, Object.keys(present).filter(function (id) { return G.q[id].ctl > 0; }).length * I.stabilite_quartier_tenu);
  if (fluxActifs().length > B.flux.trop_de_flux) ind.stabilite -= (fluxActifs().length - B.flux.trop_de_flux) * I.stabilite_flux_sup;
  ind.pression = clamp(ind.pression, 0, 100); ind.exposition = clamp(ind.exposition, 0, 100); ind.stabilite = clamp(ind.stabilite, 0, 100);

  // 7. trésorerie et dette (§6.3)
  G.tresorerie += solde;
  if (G.tresorerie < 0) { G.dette += -G.tresorerie; G.tresorerie = 0; log("Trésorerie à zéro : dette +" + Math.round(G.dette) + " €.", "dette"); }
  else if (G.dette > 0) { var r = Math.min(G.dette, G.tresorerie * 0.5); G.dette -= r; G.tresorerie -= r; }
  G.tresorerie = Math.round(G.tresorerie); G.dette = Math.round(G.dette);
  G.cycles_zero = G.tresorerie <= 0 ? G.cycles_zero + 1 : 0;

  // 8. capacité (§7)
  if (G.cycle % B.capacite.tous_les === 0) { G.capacite++; log("Capacité +1 (" + G.capacite + ").", "capacite"); }
  var terr = territoire();
  B.capacite.seuils_territoire.forEach(function (sv) {
    if (terr >= sv && G.seuils_pris.indexOf(sv) < 0) { G.seuils_pris.push(sv); G.capacite++; log("Territoire " + sv + " % : capacité +1 (" + G.capacite + ").", "capacite"); }
  });

  verifierQuetes();

  // 9. bilan du cycle et fin
  G.dernier = { solde: Math.round(solde), revenus: p.revenus, couts: p.maintien + p.entretien + p.interets, flux: Math.round(fluxNet),
    d_territoire: r1(terr - avant.territoire), d_pression: r1(ind.pression - avant.pression), d_stabilite: r1(ind.stabilite - avant.stabilite), d_exposition: r1(ind.exposition - avant.exposition) };
  G.historique.push({ c: G.cycle, t: r1(terr), tr: G.tresorerie, p: Math.round(ind.pression) });
  verifierFin();
  var rap = G.rapport; G.rapport = null;
  G.cycle++;
  save();
  return { lignes: rap, d: G.dernier };
}

function perdreOp(o, raison) {
  o.dispo = "perdu"; o.tache = "repos";
  G.ind.stabilite = clamp(G.ind.stabilite - B.indicateurs.stabilite_perte, 0, 100);
  log(nomOp(o) + " (" + TYPES()[o.type].nom + ") perdu : " + raison + ".", "perteop", o.pos);
}

function evenementsCycle() {
  var E = B.evenements, ind = G.ind;
  var operation = ind.pression > B.indicateurs.pression_alerte && rnd() < E.operation_exterieure;
  if (operation) {
    ind.pression += E.operation_pression * 0.25;
    var tenus = VILLE.ordre.filter(function (id) { return G.q[id].ctl > 0; });
    tenus.sort(function () { return rnd() - 0.5; }).slice(0, 2).forEach(function (id) { G.q[id].ctl = Math.max(0, G.q[id].ctl - 10); });
    log("Opération extérieure : descentes dans tes quartiers, contrôle −10 dans deux d'entre eux.", "operation");
  }
  opsActifs().forEach(function (o) {
    if (o.dispo === "transit" || o.dispo === "perdu") return;
    var z = IDX.zones[VILLE.q[o.pos].zone];
    var pa = E.arrestation * (ind.pression / 100) * (z.police / 60) * (operation ? 2 : 1) * (o.tache === "repos" ? 0.3 : 1);
    if (rnd() < pa) { perdreOp(o, "arrêté"); return; }
    var s = G.q[o.pos], dom = factionDominante(s);
    if (o.type !== "force" && dom && s.adv[dom] > 50 && s.ctl > 0 && rnd() < E.elimination_contestee) perdreOp(o, "écarté par " + IDX.equipes[dom].nom);
  });
}

/** Conditions de fin (§11). */
function verifierFin() {
  var Fn = B.fin, terr = territoire();
  if (G.ind.pression >= B.indicateurs.pression_chute) return finir("chute");
  if (G.cycles_zero >= B.economie.faillite_cycles) return finir("faillite");
  if (terr >= Fn.victoire_territoire && G.tresorerie > 0) return finir("victoire");
  if (!opsActifs().length && G.tresorerie < Math.min.apply(0, Object.keys(TYPES()).map(function (t) { return TYPES()[t].recrutement; }))) return finir("faillite");
  if (G.cycle >= Fn.cycles_max) return finir("fin");
}
function finir(type) {
  var terr = territoire();
  var morts = Object.keys(G.factions).filter(function (f) { return !G.factions[f].vivante; }).length;
  var score = terr + Math.min(30, G.tresorerie / 2000) + morts * 6 + opsActifs().length * 2 - G.ind.pression / 5 - G.dette / 2000 + (type === "victoire" ? 20 : type === "fin" ? 0 : -25);
  var note = score >= 85 ? "A" : score >= 65 ? "B" : score >= 45 ? "C" : score >= 25 ? "D" : "E";
  G.fini = { type: type, cycles: G.cycle, territoire: r1(terr), tresorerie: G.tresorerie, dette: G.dette, operateurs: opsActifs().length,
    factions: Object.keys(G.factions).filter(function (f) { return G.factions[f].vivante; }).length, flux: fluxActifs().length,
    pression: Math.round(G.ind.pression), stabilite: Math.round(G.ind.stabilite), exposition: Math.round(G.ind.exposition), note: note };
  log("Fin de partie : " + type + ".", "fin");
}
function abandonner() { if (!G.fini) { finir("abandon"); save(); } }

/** Investir dans un quartier (§6.3). */
function coutInvestir(id) { return Math.round(Math.max(400, fluxQuartier(id)) * B.quartiers.investir_cout_ratio * (1 + G.q[id].inv)); }
function investir(id) {
  var s = G.q[id], c = coutInvestir(id);
  if (s.ctl <= 0) return { ok: false, raison: "Il faut tenir le quartier" };
  if (G.tresorerie < c) return { ok: false, raison: "Il faut " + c + " €" };
  G.tresorerie -= c; s.inv++;
  log("Investissement à " + VILLE.q[id].nom + " (−" + c + " €) : revenus et coûts en hausse.", "invest", id);
  save();
  return { ok: true };
}
