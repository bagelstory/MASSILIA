/* CHOUF — 30-monde : missions, événements (cartes à choix, comme G.card dans KICK),
   engagements, et clôture de la semaine. */

/* ---------------- Missions ---------------- */

/** Missions disponibles dans la zone courante pour le chapitre courant. */
function missionsDispo() {
  var ch = IDX.chapitres[G.chapitre].i;
  return D.missions.liste.filter(function (m) {
    return (m.zone === "*" ? G.zone === G.protagoniste.zone_origine : m.zone === G.zone) &&
      IDX.chapitres[m.chapitre_min].i <= ch && (!m.condition || cond(m.condition));
  });
}
/** Niveau de maîtrise d'une mission (0..3). */
function maitrise(id) {
  var n = G.maitrise[id] || 0;
  return n >= 8 ? 3 : n >= 4 ? 2 : n >= 1 ? 1 : 0;
}
/** Convertit une clé de gain de mission en effet. */
function cleEffet(k, v) {
  if (k === "propre" || k === "sale") return "argent:" + k + (v >= 0 ? "+" : "") + v;
  if (k === "enquete") return "police:enquete" + (v >= 0 ? "+" : "") + v;
  return k + (v >= 0 ? "+" : "") + v;
}
/** Fait une mission. Retourne {ok, texte}. */
function faireMission(id) {
  var m = IDX.missions[id];
  if (!m) return { ok: false, texte: "Mission inconnue." };
  if (!depenser(m.cout)) return { ok: false, texte: "Pas assez d'énergie." };
  var bonus = 1 + 0.25 * maitrise(id);
  Object.keys(m.gains).forEach(function (k) {
    var v = m.gains[k];
    if (k === "propre" || k === "sale") v = Math.round(v * bonus);
    effet(cleEffet(k, v), {}, fillTexte(m.nom));
  });
  Object.keys(m.risques || {}).forEach(function (k) { effet(cleEffet(k, m.risques[k]), {}, m.nom); });
  if (m.fait && !G.evenements.faits[m.fait]) G.evenements.faits[m.fait] = { a: nowWk() };
  G.maitrise[id] = (G.maitrise[id] || 0) + 1;
  G.semaine_actions++;
  log(fillTexte(m.nom) + ".", "mission");
  save();
  return { ok: true, texte: fillTexte(m.voix || m.nom) };
}

/* ---------------- Engagements ---------------- */

function engagementsOuverts() {
  return G.protagoniste.engagements.filter(function (e) { return e.tenu === null; });
}
/** Tenir une promesse : coûte de l'énergie, renforce la confiance. */
function tenirEngagement(i) {
  var e = engagementsOuverts()[i];
  if (!e || !depenser({ energie: B.engagements.cout_tenir_energie })) return false;
  e.tenu = true;
  change("relation:" + e.envers + ".confiance", 6, "Promesse tenue");
  memoriser(e.envers + ":promesse_tenue");
  log("Tu as tenu ta promesse à " + pnjNom(e.envers) + ".", "engagement");
  G.semaine_actions++;
  save();
  return true;
}

/* ---------------- Événements ---------------- */

/** Peut-on tirer cet événement maintenant ? */
function evenementPossible(ev, ctx) {
  if (ev.once && G.evenements.faits[ev.id]) return false;
  var vu = G.evenements.vus[ev.id];
  if (vu != null && ev.delai_retour && nowWk() - vu < ev.delai_retour) return false;
  if (vu != null && !ev.delai_retour && ev.once !== false && ev.declencheur !== "programme") return false;
  return conds(ev.conditions, ctx);
}
/** Met un événement dans la file des cartes. */
function empiler(id, ctx) {
  G.file.push({ id: id, ctx: ctx || {} });
  G.evenements.vus[id] = nowWk();
}
/** Ouvre la carte suivante de la file. */
function carteSuivante() {
  if (G.carte || !G.file.length) return G.carte;
  G.carte = G.file.shift();
  return G.carte;
}
/** Options visibles d'une carte (celles dont la condition est remplie). */
function optionsCarte(c) {
  var ev = IDX.evenements[c.id];
  return ev.options.map(function (o, i) { return { o: o, i: i }; })
    .filter(function (x) { return !x.o.condition || cond(x.o.condition, c.ctx); });
}
/** Le joueur choisit l'option i (index dans ev.options). */
function choisir(i) {
  var c = G.carte;
  if (!c) return;
  var ev = IDX.evenements[c.id], o = ev.options[i];
  (o.effets || []).forEach(function (e) { effet(e, c.ctx, ev.titre); });
  memoriser(o.memoire, c.ctx);
  memoriser(ev.memoire, c.ctx);
  (o.programme || []).forEach(function (p) { programmer(p.id, p.dans, c.ctx); });
  G.evenements.faits[ev.id] = { a: nowWk(), option: i };
  if (ev.irreversible || o.irreversible) G.progression.irreversibles.push(ev.id);
  log(ev.titre + " — " + o.t + ".", "choix");
  G.carte = null;
  carteSuivante();
  save();
}

/** Événements fixés au calendrier (date précise). */
function declencherCalendrier() {
  D.evenements.liste.forEach(function (ev) {
    if (ev.declencheur === "calendrier" && ev.quand && wk(ev.quand) === nowWk() && evenementPossible(ev, {})) empiler(ev.id, {});
  });
  carteSuivante();
}

/** Rendez-vous de la ville actifs cette semaine (match de l'OM, mistral…). */
function villeCetteSemaine() {
  var s = G.cal.semaine, out = [];
  D.calendrier.ville.forEach(function (v) {
    if (v.semaines && (v.semaines.length === 1 ? s === v.semaines[0] : s >= v.semaines[0] && s <= v.semaines[1])) out.push(v.id);
    else if (v.frequence === "tous_les_15_jours_aout_mai" && (s >= 32 || s <= 21) && s % 2 === 0) out.push(v.id);
    else if (v.proba_semaine && rnd() < v.proba_semaine) out.push(v.id);
  });
  return out;
}

/* ---------------- Clôture de la semaine ---------------- */

/** Résout la semaine et passe à la suivante. Retourne le bilan affichable. */
function cloreSemaine() {
  if (G.carte) return null;                      // une décision attend
  var bilan = { lignes: [], cartes: 0 };
  var E = B.evenements, Z = B.zones;

  // 1. engagements échus
  G.protagoniste.engagements.forEach(function (e) {
    if (e.tenu === null && nowWk() >= e.echeance) {
      e.tenu = false;
      memoriser(e.envers + ":promesse_rompue");
      bilan.lignes.push("Tu n'as pas tenu ta promesse à " + pnjNom(e.envers) + ".");
      programmer("promesse_oubliee", 1, { pnj: e.envers });
    }
  });

  // 2. police : l'attention retombe lentement
  change("police:enquete", -B.police.enquete_baisse_semaine);

  // 3. zones : l'humeur revient vers la moyenne, la tension retombe
  Object.keys(G.zones).forEach(function (id) {
    var z = G.zones[id];
    z.humeur += (Z.humeur_derive_vers - z.humeur) * Z.humeur_derive / 10;
    z.tension = clamp(z.tension + Z.tension_derive, 0, 100);
  });

  // 4. relations : sans contact, la proximité baisse
  Object.keys(G.relations).forEach(function (id) {
    var r = G.relations[id];
    if (nowWk() - (r.contact || 0) > B.relations.semaines_sans_contact_avant_derive) r.proximite = clamp(r.proximite + B.relations.derive_proximite_sans_contact, 0, 100);
  });

  // 5. stress et santé
  var et = G.protagoniste.etat, S = B.stress;
  et.stress = clamp(et.stress + S.par_semaine_base + (G.semaine_actions === 0 ? S.repos_semaine : 0) + (G.police.enquete >= B.police.seuil_indice ? S.par_enquete_connue : 0), 0, 100);
  if (et.stress >= S.seuil_sante) { et.sante = clamp(et.sante - S.perte_sante_si_seuil, 0, 100); bilan.lignes.push("Tu dors mal. Ta santé baisse."); }

  // 6. avancer le calendrier
  var avantCh = G.chapitre, avantAge = age();
  var n = nowWk() + 1, c = wkToCal(n);
  G.cal.annee = c.annee; G.cal.semaine = c.semaine;
  G.semaine_actions = 0;
  G.temps.energie = Math.min(B.temps.energie_max, G.temps.energie + B.temps.energie_par_semaine);
  G.temps.sangfroid = Math.min(B.temps.sangfroid_max, G.temps.sangfroid + B.temps.sangfroid_par_semaine);
  if (age() > avantAge) bilan.lignes.push("Tu as " + age() + " ans.");
  majChapitre();
  if (G.chapitre !== avantCh) bilan.lignes.push("Nouveau chapitre : " + IDX.chapitres[G.chapitre].nom + ".");

  // 7. cartes : programmées, calendrier, rendez-vous de ville, seuils, tirage
  var max = E.cartes_max_par_semaine;
  function pousse(id, ctx) { if (bilan.cartes < max) { empiler(id, ctx); bilan.cartes++; } }
  G.evenements.programmes = G.evenements.programmes.filter(function (p) {
    if (p.a > nowWk()) return true;
    var ev = IDX.evenements[p.id];
    if (ev && bilan.cartes < max && evenementPossible(ev, p.ctx)) { pousse(p.id, p.ctx); return false; }
    return bilan.cartes >= max;                  // garde pour la semaine suivante si la file est pleine
  });
  D.evenements.liste.forEach(function (ev) {
    if (ev.declencheur === "calendrier" && ev.quand && wk(ev.quand) === nowWk() && evenementPossible(ev, {})) pousse(ev.id, {});
  });
  var ville = villeCetteSemaine();
  D.evenements.liste.forEach(function (ev) {
    if (ev.ville && ville.indexOf(ev.ville) >= 0 && evenementPossible(ev, {})) pousse(ev.id, {});
  });
  D.evenements.liste.forEach(function (ev) {
    if (ev.declencheur === "seuil" && evenementPossible(ev, {})) pousse(ev.id, {});
  });
  if (bilan.cartes < max && rnd() < E.proba_tirage_semaine) {
    var pool = D.evenements.liste.filter(function (ev) { return ev.declencheur === "tirage" && evenementPossible(ev, {}); });
    var tot = pool.reduce(function (a, ev) { return a + (ev.poids || 1); }, 0), r = rnd() * tot;
    for (var i = 0; i < pool.length; i++) { r -= pool[i].poids || 1; if (r <= 0) { pousse(pool[i].id, {}); break; } }
  }

  // 8. bilan mensuel
  if (G.cal.semaine % B.temps.bilan_mensuel_toutes === 0) bilan.mensuel = true;
  bilan.lignes.forEach(function (l) { log(l, "semaine"); });
  log("Semaine " + G.cal.semaine + ", " + G.cal.annee + ".", "semaine");
  carteSuivante();
  save();
  return bilan;
}

/** Chapitre courant d'après la date (les chapitres sont des cadres, GDD V7 §3). */
function majChapitre() {
  var ch = D.calendrier.chapitres;
  for (var i = ch.length - 1; i >= 0; i--) {
    if (G.cal.annee >= ch[i].annees[0]) { G.chapitre = ch[i].id; return; }
  }
}
/** La campagne est-elle terminée ? */
function finCampagne() { return nowWk() > wk(B.temps.semaine_fin); }
