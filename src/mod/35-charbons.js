/* CHOUF v2 — 35-charbons : les points de vente de chaque quartier (abstraits : une rentabilité, un risque,
   un propriétaire), les zones verrouillées et les objectifs (quêtes chiffrées) qui les ouvrent. */

/* ---------- charbons ---------- */
/** Crée les charbons d'un quartier au début de la partie. */
function genererCharbons(id) {
  var C = B.charbons, z = IDX.zones[VILLE.q[id].zone], nb = C.nombre[z.bati.type] || [1, 2];
  var n = rint(nb[0], nb[1]), out = [], cells = VILLE.q[id].cells, r = rngDecor(id + ":ch");
  for (var i = 0; i < n; i++) {
    var ratio = rf(C.rentabilite_ratio[0], C.rentabilite_ratio[1]);
    var rent = Math.round(z.revenu * ratio / 10) * 10;
    var c = cells[Math.floor(r() * cells.length)];
    out.push({ nom: pick(C.noms), rent: rent, risque: ratio < C.risque_seuils[0] ? 1 : ratio < C.risque_seuils[1] ? 2 : 3,
      proprio: null, ouvert: false, saisi: 0, x: c[0] + 1, y: c[1] + 1 });
  }
  return out;
}
/** Potentiel d'un quartier : somme des rentabilités (sert aussi au revenu des factions). */
function fluxQuartier(id) {
  var s = G.q[id];
  return s.ch.reduce(function (t, c) { return t + c.rent; }, 0) * (1 + s.inv * B.quartiers.investir_gain);
}
function charbonsJoueur() {
  var n = 0;
  VILLE.ordre.forEach(function (id) { G.q[id].ch.forEach(function (c) { if (c.proprio === "joueur" && c.ouvert && !c.saisi) n++; }); });
  return n;
}
function coutCharbon(c) { return Math.round(c.rent * (c.proprio && c.proprio !== "joueur" ? B.charbons.reprise_ratio : B.charbons.ouverture_ratio)); }
/** Peut-on ouvrir ou reprendre ce charbon ? */
function peutCharbon(id, i) {
  var s = G.q[id], c = s.ch[i], C = B.charbons;
  if (!c) return { ok: false, raison: "?" };
  if (c.proprio === "joueur") return { ok: false, raison: "Déjà à toi" };
  if (c.saisi) return { ok: false, raison: "Saisi encore " + c.saisi + " cycle(s)" };
  if (!zoneOuverte(VILLE.q[id].zone)) return { ok: false, raison: "Zone verrouillée" };
  var reprise = c.proprio && c.proprio !== "joueur";
  var seuil = reprise ? C.seuil_reprise : C.seuil_ouverture;
  if (s.ctl < seuil) return { ok: false, raison: "Contrôle " + seuil + " % requis" };
  if (reprise && (s.adv[c.proprio] || 0) > s.ctl) return { ok: false, raison: "La faction est plus forte ici" };
  var cout = coutCharbon(c);
  if (G.tresorerie < cout) return { ok: false, raison: "Il faut " + cout + " €" };
  return { ok: true, cout: cout, reprise: reprise };
}
function prendreCharbon(id, i) {
  var r = peutCharbon(id, i);
  if (!r.ok) return r;
  var c = G.q[id].ch[i];
  G.tresorerie -= r.cout;
  if (r.reprise) { G.ind.pression = clamp(G.ind.pression + 3, 0, 100); G.ind.exposition = clamp(G.ind.exposition + 2, 0, 100); }
  log((r.reprise ? "Charbon repris à " + IDX.equipes[c.proprio].nom : "Charbon ouvert") + " : " + c.nom + ", " + VILLE.q[id].nom + " (−" + r.cout + " €).", "charbon", id);
  c.proprio = "joueur"; c.ouvert = true;
  verifierQuetes();
  save();
  return { ok: true };
}
function fermerCharbon(id, i) {
  var c = G.q[id].ch[i];
  if (!c || c.proprio !== "joueur") return;
  c.ouvert = !c.ouvert;
  log("Charbon " + (c.ouvert ? "rouvert" : "fermé") + " : " + c.nom + ", " + VILLE.q[id].nom + ".", "charbon", id);
  save();
}
/** Après le tour : les charbons suivent le rapport de force du quartier, la police en saisit. */
function majCharbons() {
  var C = B.charbons, E = B.evenements;
  VILLE.ordre.forEach(function (id) {
    var s = G.q[id], dom = factionDominante(s), z = IDX.zones[VILLE.q[id].zone];
    s.ch.forEach(function (c) {
      if (c.saisi) { c.saisi--; return; }
      if (c.proprio === "joueur" && s.ctl < C.perte_sous) {
        c.proprio = dom; c.ouvert = !!dom;
        log("Charbon perdu : " + c.nom + ", " + VILLE.q[id].nom + (dom ? " (" + IDX.equipes[dom].nom + ")" : "") + ".", "perte", id);
        return;
      }
      if (c.proprio && c.proprio !== "joueur" && (s.adv[c.proprio] || 0) < 5) { c.proprio = null; c.ouvert = false; }
      if (!c.proprio && dom && s.adv[dom] >= 30 && rnd() < 0.2) { c.proprio = dom; c.ouvert = true; }
      if (c.proprio === "joueur" && c.ouvert) {
        G.ind.pression += c.risque * C.pression_par_risque * 0.3;
        G.ind.exposition += c.risque * C.exposition_par_risque * 0.3;
        var p = C.saisie_base * c.risque * (G.ind.pression / 50) * (z.police / 60);
        if (rnd() < p) { c.saisi = C.saisie_cycles; log("Saisie de police : " + c.nom + ", " + VILLE.q[id].nom + " fermé " + C.saisie_cycles + " cycles.", "operation", id); }
      }
    });
  });
}

/* ---------- zones verrouillées et objectifs ---------- */
function zoneOuverte(z) { return G.zonesOuvertes.indexOf(z) >= 0; }
/** Zones qu'on peut chercher à ouvrir : voisines d'une zone ouverte. */
function zonesProposees() {
  var out = {};
  G.zonesOuvertes.forEach(function (z) {
    VILLE.zones[z].forEach(function (id) { VILLE.adj[id].forEach(function (v) { var zz = VILLE.q[v].zone; if (!zoneOuverte(zz)) out[zz] = 1; }); });
  });
  return Object.keys(out);
}
/** Liste des objectifs actifs (ouverture de zones + liste fixe), avec leur avancement. */
function quetesActives() {
  var Q = D.quetes, out = [], nb = G.zonesOuvertes.length;
  zonesProposees().forEach(function (z) {
    var o = Q.ouverture_zone;
    out.push({ id: "zone_" + z, zone: z, nom: o.nom.replace("{zone}", IDX.zones[z].nom), objectifs: o.objectifs.map(function (x) {
      var y = JSON.parse(JSON.stringify(x)); if (y.n_par_zone) y.n += y.n_par_zone * (nb - 1); y.zone = z; return y;
    }), recompense: o.recompense });
  });
  Q.liste.forEach(function (q) { if (!G.quetesFaites[q.id]) out.push(q); });
  out.forEach(function (q) { q.avance = q.objectifs.map(avancement); q.fini = q.avance.every(function (a) { return a.ok; }); });
  return out;
}
function avancement(o) {
  var v = 0, but = o.n || o.v || 0, txt = "";
  switch (o.type) {
    case "quartiers_tenus": v = VILLE.ordre.filter(function (id) { return G.q[id].ctl >= o.seuil; }).length; txt = "Quartiers à " + o.seuil + " % ou plus"; break;
    case "connaissance_zone": v = VILLE.zones[o.zone].filter(function (id) { return G.q[id].conn >= o.niveau; }).length; txt = "Quartiers repérés dans " + IDX.zones[o.zone].court; break;
    case "connaissance": v = VILLE.ordre.filter(function (id) { return G.q[id].conn >= o.niveau; }).length; txt = "Quartiers bien connus (niveau " + o.niveau + ")"; break;
    case "charbons": v = charbonsJoueur(); txt = "Charbons ouverts"; break;
    case "tresorerie": v = G.tresorerie; txt = "Trésorerie (€)"; break;
    case "securises": v = VILLE.ordre.filter(function (id) { return statut(id) === "securise"; }).length; txt = "Quartiers sécurisés"; break;
    case "operateurs": v = opsActifs().length; txt = "Opérateurs"; break;
    case "territoire": v = Math.round(territoire()); txt = "Territoire (%)"; break;
    case "flux": v = fluxActifs().length; txt = "Flux actifs"; break;
  }
  return { txt: txt, v: Math.round(v), but: but, ok: v >= but };
}
/** Valide les objectifs atteints et donne les récompenses. Retourne les noms des objectifs validés. */
function verifierQuetes() {
  if (!G || G.fini) return [];
  var faits = [];
  quetesActives().forEach(function (q) {
    if (!q.fini) return;
    var r = q.recompense || {};
    if (q.zone) {
      G.zonesOuvertes.push(q.zone);
      VILLE.zones[q.zone].forEach(function (id) { G.q[id].conn = Math.max(G.q[id].conn, 1); });
      if (r.capacite_toutes_les && (G.zonesOuvertes.length - 1) % r.capacite_toutes_les === 0) { G.capacite++; }
    } else G.quetesFaites[q.id] = G.cycle;
    if (r.argent) G.tresorerie += r.argent;
    if (r.capacite) G.capacite += r.capacite;
    log("Objectif atteint : " + q.nom + (r.argent ? " (+" + r.argent + " €)" : "") + (r.capacite ? " (capacité +" + r.capacite + ")" : "") + (q.zone ? " : zone ouverte." : "."), "quete");
    faits.push(q.nom);
  });
  return faits;
}
