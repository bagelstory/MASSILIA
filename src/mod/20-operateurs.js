/* CHOUF v2 — 20-operateurs : recrutement, affectation (quartier + tâche), transit, fatigue, efficacité.
   Un opérateur est une fonction (§4) ; il porte un numéro, un visage et un surnom pris dans les personnages
   adultes du jeu, mais il n'a ni dialogue ni mémoire. */

function TYPES() { return B.operateurs.types; }

function creerOperateur(type, qid, comp) {
  var O = B.operateurs, visage = G.visages.length ? G.visages.shift() : null;
  var op = { id: "OP-" + ("00" + G.prochain_op++).slice(-3), type: type, comp: comp || rint(O.competence_recrue[0], O.competence_recrue[1]),
    xp: 0, fatigue: 0, dispo: "disponible", pos: qid, dest: null, transit: 0, tache: "repos", indispo: 0, visage: visage };
  G.operateurs.push(op);
  return op;
}
function opsActifs() { return G.operateurs.filter(function (o) { return o.dispo !== "perdu"; }); }
function opParId(id) { return G.operateurs.filter(function (o) { return o.id === id; })[0]; }
function nomOp(o) { return o.id + (o.visage && IDX.personnages[o.visage] ? " « " + (IDX.personnages[o.visage].surnom || IDX.personnages[o.visage].prenom) + " »" : ""); }

/** Coût de recrutement, et raison du refus éventuel (capacité §7, trésorerie §6.3). */
function peutRecruter(type) {
  var c = TYPES()[type].recrutement;
  if (opsActifs().length >= G.capacite) return { ok: false, raison: "Capacité atteinte (" + G.capacite + ")" };
  if (G.tresorerie < c) return { ok: false, raison: "Trésorerie insuffisante" };
  return { ok: true, cout: c };
}
function recruter(type) {
  var r = peutRecruter(type);
  if (!r.ok) return r;
  G.tresorerie -= r.cout;
  var op = creerOperateur(type, G.base);
  G.ind.stabilite = clamp(G.ind.stabilite + B.indicateurs.stabilite_recrue, 0, 100);
  log("Recrue : " + nomOp(op) + ", " + TYPES()[type].nom + " (−" + r.cout + " €).", "recrue", G.base);
  save();
  return { ok: true, op: op };
}

/** Distance en cycles entre deux quartiers (§4.4 : 1 si voisin, 2 sinon, 0 sur place). */
function dureeTransit(a, b) {
  if (a === b) return 0;
  return VILLE.adj[a].indexOf(b) >= 0 ? B.operateurs.transit_adjacent : B.operateurs.transit_loin;
}
/** Efficacité d'un opérateur sur une tâche (§4.4 spécialisation, fatigue, stabilité). */
function efficacite(o, tache) {
  var t = B.taches[tache], O = B.operateurs;
  if (!t || !t.type) return 0;
  var e = o.comp / O.competence_ref;
  if (t.type !== o.type) e *= O.hors_specialite;
  if (o.fatigue >= O.fatigue_seuil_baisse) e *= O.fatigue_effet;
  return r1(e);
}
/** Affecte un opérateur à un quartier et une tâche pour le cycle à venir. */
function affecter(opId, qid, tache) {
  var o = opParId(opId);
  if (!o || o.dispo === "perdu") return { ok: false, raison: "Opérateur indisponible" };
  if (o.dispo === "indisponible") return { ok: false, raison: "Épuisé : au repos ce cycle" };
  if (qid !== o.pos && !accessible(qid)) return { ok: false, raison: "Quartier inaccessible : il faut un voisin à " + B.controle.acces_voisin + " %" };
  o.tache = tache;
  if (qid !== o.pos) { o.dest = qid; o.transit = dureeTransit(o.pos, qid); o.dispo = "transit"; }
  else { o.dest = null; o.transit = 0; o.dispo = tache === "repos" ? "disponible" : "occupe"; }
  save();
  return { ok: true };
}
/** Quartier où l'opérateur agira (destination s'il voyage). */
function lieuOp(o) { return o.dest || o.pos; }
