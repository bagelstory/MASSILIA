/* CHOUF v2 — 30-factions : l'IA des équipes rivales (§5). Chaque cycle, chaque faction :
   gagne et dépense, recrute lentement, choisit un objectif (contre-attaquer, se rétracter, étendre, maintenir),
   agit sur un ou plusieurs quartiers. Alliances temporaires, fragmentation, disparition. */

function forceAction(f) {
  var a = B.factions.force_action;
  return a[0] + f.agressivite * a[1] + f.effectif * a[2];
}
/** Revenu d'une faction : ses quartiers selon sa présence. */
function revenuFaction(fid) {
  var t = 0;
  VILLE.ordre.forEach(function (id) { t += G.q[id].flux * (G.q[id].adv[fid] || 0) / 100; });
  return t * B.factions.revenu_ratio;
}
/** Défense du joueur dans un quartier (opérateurs Force sur place, surveillance). */
function defense(id) {
  var red = 0;
  opsActifs().forEach(function (o) {
    if (o.dispo === "transit" || o.pos !== id) return;
    if (o.tache === "securiser") red += B.controle.force_reduit_attaque * efficacite(o, "securiser");
    if (o.tache === "surveiller") red += B.taches.surveiller.reduction_attaque * efficacite(o, "surveiller");
  });
  return clamp(red, 0, 0.85);
}

/** Choisit l'objectif et la cible d'une faction (§5.3). Retourne la liste des quartiers visés. */
function decider(fid) {
  var f = G.factions[fid], F = B.factions, terr = territoireFaction(fid), cibles = [];
  var eqp = IDX.equipes[fid];
  // attaquée : un de ses quartiers où le joueur dépasse le seuil
  var attaques = terr.concat(VILLE.ordre.filter(function (id) { return (G.q[id].adv[fid] || 0) >= 15; }))
    .filter(function (id) { return G.q[id].ctl > F.attaque_detecte; });
  if (attaques.length) { f.objectif = "contre"; cibles = attaques; }
  else if (f.budget < F.budget_max * F.retraite_budget * (eqp.budget / F.budget_max + 0.5)) { f.objectif = "retraite"; cibles = []; }
  else {
    // faiblesse : quartier voisin de son territoire, libre ou peu tenu par le joueur
    var voisins = {};
    terr.forEach(function (id) { VILLE.adj[id].forEach(function (v) { if ((G.q[v].adv[fid] || 0) < 30) voisins[v] = 1; }); });
    var faibles = Object.keys(voisins).filter(function (v) {
      var s = G.q[v]; return s.ctl < 40 && !Object.keys(s.adv).some(function (g) { return g !== fid && allies(fid, g) && s.adv[g] > 30; });
    });
    if (faibles.length && rnd() < 0.25 + f.agressivite * 0.12) {
      f.objectif = "etendre";
      faibles.sort(function (a, b) { return (G.q[a].ctl + advTotal(G.q[a])) - (G.q[b].ctl + advTotal(G.q[b])); });
      cibles = faibles;
    } else { f.objectif = "maintenir"; cibles = []; }
  }
  var n = Math.max(1, Math.ceil(f.effectif / F.actions_par_effectif));
  cibles = cibles.slice(0, n);
  f.cible = cibles[0] || null;
  return cibles;
}

/** Fait jouer toutes les factions pour ce cycle. */
function tourFactions() {
  var F = B.factions, att = [];
  Object.keys(G.factions).forEach(function (fid) {
    var f = G.factions[fid];
    if (!f.vivante) return;
    f.budget = clamp(f.budget + revenuFaction(fid) - f.effectif * F.cout_effectif, 0, F.budget_max);
    if (f.budget > F.recrutement_seuil + F.recrutement_cout && rnd() < 0.25) { f.budget -= F.recrutement_cout; f.effectif++; }
    var cibles = decider(fid);
    if (f.objectif === "retraite") {
      // se concentre : abandonne un peu de présence dans ses quartiers les plus faibles
      VILLE.ordre.forEach(function (id) { var s = G.q[id]; if (s.adv[fid] && s.adv[fid] < 25) s.adv[fid] = Math.max(0, s.adv[fid] - 4); });
      if (f.agressivite > 1 && rnd() < 0.3) f.agressivite--;
    }
    if (f.objectif === "maintenir" && f.agressivite < IDX.equipes[fid].agressivite && rnd() < 0.2) f.agressivite++;
    cibles.forEach(function (id) {
      var v = forceAction(f) * (1 - defense(id));
      var avant = G.q[id].ctl;
      var perte = gagnerPresence(id, fid, v);
      if (perte > 0.5) att.push({ f: fid, q: id, perte: perte, avant: avant });
      else if (avant === 0 && G.q[id].conn > 0 && v > 3 && rnd() < 0.3) log(IDX.equipes[fid].nom + " avance sur " + VILLE.q[id].nom + ".", "faction", id);
    });
    f.intention = cibles[0] ? { objectif: f.objectif, q: cibles[0] } : { objectif: f.objectif, q: null };
  });
  att.forEach(function (a) {
    log(IDX.equipes[a.f].nom + " attaque " + VILLE.q[a.q].nom + " : contrôle −" + Math.round(a.perte) + " %.", "attaque", a.q);
    if (G.q[a.q].ctl <= 0 && a.avant > 0) log(VILLE.q[a.q].nom + " est perdu.", "perte", a.q);
  });
  alliancesEtFragmentation();
}

function alliancesEtFragmentation() {
  var F = B.factions;
  G.alliances = G.alliances.filter(function (x) { return --x.reste > 0 || (log("Fin de la trêve entre " + IDX.equipes[x.a].nom + " et " + IDX.equipes[x.b].nom + ".", "faction"), false); });
  var vives = Object.keys(G.factions).filter(function (f) { return G.factions[f].vivante; });
  if (vives.length >= 2 && rnd() < F.alliance_proba) {
    var a = pick(vives), b = pick(vives.filter(function (x) { return x !== a; }));
    if (!allies(a, b)) {
      G.alliances.push({ a: a, b: b, reste: rint(F.alliance_duree[0], F.alliance_duree[1]) });
      log("Trêve entre " + IDX.equipes[a].nom + " et " + IDX.equipes[b].nom + ".", "faction");
    }
  }
  vives.forEach(function (fid) {
    var f = G.factions[fid], t = territoireFaction(fid).length;
    f.pic = Math.max(f.pic, t);
    var reste = VILLE.ordre.some(function (id) { return (G.q[id].adv[fid] || 0) > 1; });
    if (!reste) {
      f.vivante = false;
      log(IDX.equipes[fid].nom + " a disparu de la ville.", "disparition");
    } else if (!f.fragmentee && f.pic >= F.fragmentation_min && t > 0 && t <= f.pic * F.fragmentation_ratio) {
      f.fragmentee = true; f.effectif = Math.max(1, Math.round(f.effectif * 0.6)); f.agressivite = Math.min(5, f.agressivite + 1);
      log(IDX.equipes[fid].nom + " se divise : moins nombreux, plus nerveux.", "faction");
    }
  });
}
