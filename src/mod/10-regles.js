/* CHOUF — 10-regles : interpréteur des conditions et des effets écrits dans data/*.json.
   Grammaire (design/02-modele-donnees.md §12) :
   conditions : fait:ID · competence:ID>=N · relation:ID.champ>=N / !=valeur · zone:champ>=N ·
                police:enquete>=N · positionnement:x>=N · engagement_rompu:ID · a|b (ou)
   effets     : relation:ID.champ±N · competence:ID±N · scolarite.assiduite±N · etat:champ±N ·
                temps:energie±N · argent:propre±N · zone:champ±N · police:enquete±N ·
                positionnement:x±N · engagement:PNJ:ID:SEMAINES */

/** Remplace {pnj}, {mere}, {pnj.prenom}… dans un texte ou une règle. */
function fill(s, ctx) {
  if (!s) return s;
  return String(s).replace(/\{pnj\}/g, (ctx && ctx.pnj) || "pnj");
}
/** Texte affichable : {pnj} → nom du personnage. */
function fillTexte(s, ctx) {
  return String(s || "").replace(/\{(\w+)(?:\.(\w+))?\}/g, function (m, a) {
    if (a === "joueur") return G.protagoniste.prenom;
    var id = a === "pnj" ? (ctx && ctx.pnj) : a;
    return IDX.personnages[id] ? pnjNom(id) : m;
  });
}

/** Lit une valeur de l'état à partir d'une référence de règle. */
function lire(ref) {
  var m;
  if ((m = /^relation:(\w+)\.(\w+)$/.exec(ref))) { var r = G.relations[m[1]]; return r ? r[m[2]] : undefined; }
  if ((m = /^competence:(\w+)$/.exec(ref))) return G.protagoniste.competences[m[1]];
  if ((m = /^zone:(\w+)$/.exec(ref))) return G.zones[G.zone][m[1]];
  if (ref === "police:enquete") return G.police.enquete;
  if ((m = /^positionnement:(\w+)$/.exec(ref))) return G.protagoniste.positionnement[m[1]];
  if ((m = /^etat:(\w+)$/.exec(ref))) return G.protagoniste.etat[m[1]];
  if (ref === "scolarite.assiduite") return G.protagoniste.scolarite.assiduite;
  if ((m = /^membre:(\w+)\.(\w+)$/.exec(ref))) {
    var mb = G.organisation.membres.filter(function (x) { return x.role === m[1] && x.statut === "actif"; })[0];
    return mb ? mb[m[2]] : undefined;
  }
  return undefined;
}

/** Évalue une condition (chaîne). */
function cond(c, ctx) {
  if (!c) return true;
  c = fill(c, ctx);
  if (c.indexOf("|") >= 0) return c.split("|").some(function (x) { return cond(x, ctx); });
  var m;
  if ((m = /^fait:(\w+)$/.exec(c))) return !!G.evenements.faits[m[1]];
  if ((m = /^engagement_rompu:(\w+)$/.exec(c))) {
    return G.protagoniste.engagements.some(function (e) { return e.envers === m[1] && e.tenu === false; });
  }
  if ((m = /^(.+?)(>=|<=|!=|=|>|<)(.+)$/.exec(c))) {
    var v = lire(m[1]), w = isNaN(+m[3]) ? m[3] : +m[3];
    if (v === undefined) return m[2] === "!=";
    switch (m[2]) {
      case ">=": return v >= w; case "<=": return v <= w; case ">": return v > w; case "<": return v < w;
      case "=": return v == w; case "!=": return v != w;
    }
  }
  return false;
}
function conds(list, ctx) { return (list || []).every(function (c) { return cond(c, ctx); }); }

/** Applique un changement borné et écrit la cause dans le journal si elle est donnée. */
function change(ref, delta, cause) {
  var m, o, k, lo = 0, hi = 100;
  if ((m = /^relation:(\w+)\.(\w+)$/.exec(ref))) { connaitre(m[1]); o = G.relations[m[1]]; k = m[2]; o.contact = nowWk(); }
  else if ((m = /^competence:(\w+)$/.exec(ref))) { o = G.protagoniste.competences; k = m[1]; delta = gainCompetence(o[k], delta); }
  else if ((m = /^zone:(\w+)$/.exec(ref))) { o = G.zones[G.zone]; k = m[1]; }
  else if (ref === "police:enquete") { o = G.police; k = "enquete"; }
  else if ((m = /^positionnement:(\w+)$/.exec(ref))) { o = G.protagoniste.positionnement; k = m[1]; }
  else if ((m = /^etat:(\w+)$/.exec(ref))) { o = G.protagoniste.etat; k = m[1]; }
  else if (ref === "scolarite.assiduite") { o = G.protagoniste.scolarite; k = "assiduite"; }
  else if ((m = /^temps:(\w+)$/.exec(ref))) { o = G.temps; k = m[1]; hi = B.temps[k + "_max"] || 999; }
  else if ((m = /^argent:(\w+)$/.exec(ref))) { o = G.argent; k = m[1]; hi = 1e12; lo = -1e12; }
  else return 0;
  var avant = o[k] || 0;
  o[k] = clamp(Math.round((avant + delta) * 10) / 10, lo, hi);
  var d = o[k] - avant;
  if (cause && d && Math.abs(d) >= 3) log(libelle(ref) + " " + (d > 0 ? "+" : "") + Math.round(d), "chiffre", cause);
  return d;
}
/** Rendement décroissant des compétences au palier 4 (GDD V7 §12 C, option 3). */
function gainCompetence(v, delta) {
  if (delta <= 0) return delta;
  delta = Math.min(delta, B.competences.gain_max_par_evenement);
  return v >= B.competences.seuils[3] ? delta * B.competences.rendement_palier4 : delta;
}
function libelle(ref) {
  var m;
  if ((m = /^relation:(\w+)\.(\w+)$/.exec(ref))) return m[2].charAt(0).toUpperCase() + m[2].slice(1) + " de " + pnjNom(m[1]);
  if ((m = /^competence:(\w+)$/.exec(ref))) return IDX.competences[m[1]].nom;
  if ((m = /^zone:(\w+)$/.exec(ref))) return m[1].charAt(0).toUpperCase() + m[1].slice(1) + " du quartier";
  if (ref === "police:enquete") return "Attention de la police";
  if ((m = /^\w+:(\w+)$/.exec(ref))) return m[1].charAt(0).toUpperCase() + m[1].slice(1);
  return ref;
}

/** Applique un effet (chaîne). cause : texte court pour le journal. */
function effet(e, ctx, cause) {
  e = fill(e, ctx);
  var m;
  if ((m = /^engagement:(\w+):(\w+):(\d+)$/.exec(e))) {
    G.protagoniste.engagements.push({ id: m[2], envers: m[1], echeance: nowWk() + +m[3], tenu: null });
    log("Tu as promis quelque chose à " + pnjNom(m[1]) + ".", "engagement");
    return;
  }
  if ((m = /^(.+?)([+-]\d+(?:\.\d+)?)$/.exec(e))) change(m[1], +m[2], cause);
}

/** Mémoire : "pnj:drapeau" ou "*:drapeau" (fait de campagne). */
function memoriser(s, ctx) {
  if (!s) return;
  s = fill(s, ctx);
  var p = s.split(":");
  if (p[0] === "*") { if (G.progression.drapeaux.indexOf(p[1]) < 0) G.progression.drapeaux.push(p[1]); return; }
  connaitre(p[0]);
  var r = G.relations[p[0]];
  if (r && r.memoire.indexOf(p[1]) < 0) r.memoire.push(p[1]);
}

/** Palier (1..4) d'une compétence. */
function palier(v) {
  var s = B.competences.seuils, p = 1;
  for (var i = 1; i < s.length; i++) if (v >= s[i]) p = i + 1;
  return p;
}
