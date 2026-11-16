/* CHOUF v2 — 00-core : état G, hasard reproductible, sauvegarde, journal.
   Gestion pure (design/05-gestion-pure.md) : pas de personnage incarné, des opérateurs, des quartiers,
   des factions, des cycles. Aucune valeur d'équilibrage ici : tout vient de DATA. */

var D = window.DATA, B = D.equilibrage, G = null, SAVE_KEY = "chouf_v2";

var IDX = {};
["zones", "personnages", "equipes"].forEach(function (k) {
  IDX[k] = {};
  D[k].liste.forEach(function (x) { IDX[k][x.id] = x; });
});

/** Hasard de la partie (mulberry32), son état tient dans G.seed : une partie rejouée avec la même graine est identique. */
function rnd() {
  var t = (G.seed = (G.seed + 0x6D2B79F5) | 0);
  t = Math.imul(t ^ (t >>> 15), t | 1);
  t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
  return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
}
function rint(a, b) { return a + Math.floor(rnd() * (b - a + 1)); }
function rf(a, b) { return a + rnd() * (b - a); }
function pick(a) { return a[Math.floor(rnd() * a.length)]; }
function clamp(v, a, b) { return v < a ? a : v > b ? b : v; }
function r1(v) { return Math.round(v * 10) / 10; }

/** Semaine de l'année du cycle courant (pour les saisons). */
function semaine() { return ((B.depart.semaine - 1 + G.cycle - 1) % 52) + 1; }
function annee() { return B.depart.annee + Math.floor((B.depart.semaine - 1 + G.cycle - 1) / 52); }
var MOIS = ["janvier", "février", "mars", "avril", "mai", "juin", "juillet", "août", "septembre", "octobre", "novembre", "décembre"];
function moisNom() { return MOIS[Math.min(11, Math.floor((semaine() - 1) / 4.35))]; }

/** Journal : une ligne courte par événement, avec sa cause (règle UX : comprendre pourquoi un indicateur bouge). */
function log(t, type, q) {
  G.journal.unshift({ c: G.cycle, t: t, type: type || "info", q: q || null });
  if (G.journal.length > 400) G.journal.length = 400;
  if (G.rapport) G.rapport.push({ t: t, type: type || "info", q: q || null });
}

/* ---------- Sauvegarde ---------- */
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
  if (!G || G.v !== 2) { G = null; return false; }
  return true;
}
function eraseSave() { STORE.del(SAVE_KEY); G = null; }
