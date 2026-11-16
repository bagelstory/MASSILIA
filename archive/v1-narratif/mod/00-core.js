/* CHOUF — 00-core : état G, hasard reproductible, horloges, sauvegarde, journal.
   Aucune valeur d'équilibrage ici : tout vient de DATA (data/*.json injecté par build.py). */

var D = window.DATA;            // données du jeu
var B = D.equilibrage;           // raccourci équilibrage
var G = null;                    // état de partie
var SAVE_KEY = "chouf_v1";

/** Horloge réelle injectable (le simulateur la remplace). */
var CLOCK = { now: function () { return Date.now(); } };

/** Index des listes de données par id. */
var IDX = {};
["zones", "personnages", "equipes", "roles", "competences", "evenements", "missions"].forEach(function (k) {
  IDX[k] = {};
  D[k].liste.forEach(function (x) { IDX[k][x.id] = x; });
});
IDX.chapitres = {};
D.calendrier.chapitres.forEach(function (c, i) { c.i = i; IDX.chapitres[c.id] = c; });
IDX.familles = {};
(D.personnages.familles || []).forEach(function (f) { IDX.familles[f.id] = f; });

/** Hasard reproductible (mulberry32), l'état tient dans G.seed. */
function rnd() {
  var t = (G.seed = (G.seed + 0x6D2B79F5) | 0);
  t = Math.imul(t ^ (t >>> 15), t | 1);
  t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
  return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
}
function rint(a, b) { return a + Math.floor(rnd() * (b - a + 1)); }
function pick(arr) { return arr[Math.floor(rnd() * arr.length)]; }
function clamp(v, a, b) { return v < a ? a : v > b ? b : v; }

/** Date de campagne → nombre de semaines depuis l'an 0 (comparaisons faciles). */
function wk(c) { return c.annee * 52 + (c.semaine - 1); }
function wkToCal(n) { return { annee: Math.floor(n / 52), semaine: (n % 52) + 1 }; }
function nowWk() { return wk(G.cal); }

/** Âge du protagoniste à la semaine courante. */
function age() {
  var n = G.protagoniste.naissance;
  return Math.floor((nowWk() - wk(n)) / 52);
}
function estMineur() { return age() < D.equilibrage.evenements.mineur_age; }

/** Nom affiché d'un personnage : surnom si l'usage du quartier, sinon prénom. */
function pnjNom(id) {
  var p = IDX.personnages[id];
  if (!p) return id;
  var pr = p.prenom === "{famille}" ? famillePrenom(id) : p.prenom;
  return p.surnom || pr;
}
function pnjNomComplet(id) {
  var p = IDX.personnages[id];
  if (!p) return id;
  var pr = p.prenom === "{famille}" ? famillePrenom(id) : p.prenom;
  var nom = p.nom || (p.groupe === "famille" && p.prenom === "{famille}" ? IDX.familles[G.famille].nom : "");
  var s = pr + (nom ? " " + nom : "");
  return p.surnom ? s + " « " + p.surnom + " »" : s;
}
function famillePrenom(id) {
  var f = IDX.familles[G.famille];
  return f && f[id] ? f[id].prenom : id;
}
function pnjAge(id) {
  var p = IDX.personnages[id];
  var a = p.age_2021 != null ? p.age_2021 : (IDX.familles[G.famille][id] || {}).age_2021;
  return a == null ? null : a + (G.cal.annee - 2021);
}

/** Journal : chaque changement significatif garde sa cause (règle UX du GDD V7 §6). */
function log(texte, type, cause) {
  G.journal.unshift({ a: G.cal.annee, s: G.cal.semaine, t: texte, type: type || "info", cause: cause || null });
  if (G.journal.length > 300) G.journal.length = 300;
}

/** Nouvelle partie. prenom : choisi par le joueur. seed : facultatif (tests). */
function newGame(prenom, seed) {
  var T = B.temps, P = B.protagoniste;
  G = { v: 1, seed: (seed == null ? (CLOCK.now() & 0x7fffffff) : seed) | 0 };
  var departs = D.zones.liste.filter(function (z) { return z.depart; });
  var zo = pick(departs).id;
  G.famille = pick(D.personnages.familles).id;
  G.cal = { annee: T.semaine_depart.annee, semaine: T.semaine_depart.semaine };
  G.chapitre = D.calendrier.chapitres[0].id;
  G.temps = { energie: T.energie_depart, sangfroid: T.sangfroid_max, maj: CLOCK.now() };
  G.protagoniste = {
    prenom: (prenom || "Toi").trim().slice(0, 18) || "Toi",
    naissance: { annee: P.naissance_annee, semaine: rint(P.naissance_semaines[0], P.naissance_semaines[1]) },
    zone_origine: zo,
    competences: JSON.parse(JSON.stringify(P.competences_depart)),
    etat: JSON.parse(JSON.stringify(P.etat_depart)),
    scolarite: { statut: "lycee", assiduite: 70 },
    positionnement: { respect: 5, influence: 0, crainte: 0 },
    responsabilites: [], engagements: []
  };
  G.argent = { propre: rint(P.argent_propre_depart[0], P.argent_propre_depart[1]), sale: 0, dettes: [] };
  G.relations = {};
  B.relations.connus_depart.forEach(function (id) { connaitre(id, true); });
  G.zones = {};
  var vois = B.zones.voisines[zo] || [];
  D.zones.liste.forEach(function (z) {
    G.zones[z.id] = {
      connu: z.id === zo ? B.zones.connu_depart_zone_origine : vois.indexOf(z.id) >= 0 ? B.zones.connu_depart_voisines : B.zones.connu_depart_autres,
      influence: 0, tension: z.tension, police: z.police, humeur: z.humeur, plans: 0, coffre: 0, controle: z.controle
    };
  });
  G.zone = zo;                                   // zone affichée / où l'on agit
  G.organisation = { existe: false, nom: null, stabilite: 50, confiance_interne: 50, resilience: 40, ancrage: 50, membres: [], facades: [], reseaux: [] };
  G.police = { pression_ville: 30, enquete: 0, operation: null };
  G.rivaux = {};
  D.equipes.liste.forEach(function (e) { G.rivaux[e.id] = { force: e.force, relation: "neutre" }; });
  G.progression = { drapeaux: [], irreversibles: [], options: [], signaux: {} };
  G.evenements = { faits: {}, programmes: [], vus: {} };
  G.maitrise = {};                               // missions : nombre de fois réussies
  G.semaine_actions = 0;
  G.journal = []; G.messages = []; G.file = []; G.carte = null;
  G.absence = null;
  D.equilibrage.evenements.prologue.forEach(function (p) { programmer(p.id, p.dans, {}); });
  log("Rentrée " + G.cal.annee + ". Tu as " + age() + " ans. Tu habites " + IDX.zones[zo].nom + ".", "chapitre");
  declencherCalendrier();
  return G;
}

/** Rend un personnage connu (crée sa relation à partir des valeurs de départ). */
function connaitre(id, silencieux) {
  if (G.relations[id] || !IDX.personnages[id]) return;
  var r = IDX.personnages[id].relation_depart;
  G.relations[id] = { confiance: r.confiance, proximite: r.proximite, tension: r.tension, memoire: [], etat: "present", contact: nowWkSafe() };
  if (!silencieux) log("Tu connais maintenant " + pnjNomComplet(id) + ".", "rencontre");
}
function nowWkSafe() { return G && G.cal ? nowWk() : 0; }

function programmer(id, dans, ctx) {
  G.evenements.programmes.push({ id: id, a: nowWk() + dans, ctx: ctx || {} });
}

/* ---------- Sauvegarde (même principe que KICK : un seul objet JSON) ---------- */
var STORE = {
  get: function (k) { try { return window.localStorage.getItem(k); } catch (e) { return null; } },
  set: function (k, v) { try { window.localStorage.setItem(k, v); } catch (e) {} },
  del: function (k) { try { window.localStorage.removeItem(k); } catch (e) {} }
};
function save() { if (G) STORE.set(SAVE_KEY, JSON.stringify(G)); }
function hasSave() { return !!STORE.get(SAVE_KEY); }
function loadSave() {
  var s = STORE.get(SAVE_KEY);
  if (!s) return false;
  try { G = JSON.parse(s); } catch (e) { return false; }
  return migrate();
}
/** Migrations de sauvegarde par numéro de version. */
function migrate() {
  if (!G || G.v !== 1) return false;
  return true;
}
function eraseSave() { STORE.del(SAVE_KEY); G = null; }
