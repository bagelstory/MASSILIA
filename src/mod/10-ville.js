/* CHOUF v2 — 10-ville : les quartiers de Marseille (noms officiels de data/zones.json), leur découpage sur
   le plan, leurs voisinages, leur contrôle (joueur et factions), et la mise en place d'une partie.
   Le découpage est un schéma : chaque zone de jeu est partagée entre ses quartiers par regroupement de
   points (pas les frontières officielles exactes). Il ne dépend pas de la partie (hasard du décor). */

function rngDecor(txt) {
  var h = 2166136261;
  for (var i = 0; i < txt.length; i++) h = Math.imul(h ^ txt.charCodeAt(i), 16777619);
  return function () { h = (h + 0x6D2B79F5) | 0; var t = Math.imul(h ^ (h >>> 15), h | 1); t ^= t + Math.imul(t ^ (t >>> 7), t | 61); return ((t ^ (t >>> 14)) >>> 0) / 4294967296; };
}
function dansPoly(pt, poly) {
  var x = pt[0], y = pt[1], ins = false;
  for (var i = 0, j = poly.length - 1; i < poly.length; j = i++) {
    var xi = poly[i][0], yi = poly[i][1], xj = poly[j][0], yj = poly[j][1];
    if (((yi > y) !== (yj > y)) && x < (xj - xi) * (y - yi) / (yj - yi) + xi) ins = !ins;
  }
  return ins;
}

/** La ville : les 111 quartiers officiels (data/quartiers.json, tools/geo.py), rastérisés sur une grille du plan.
    VILLE.q[id] = {id, zone, nom, arr, cx, cy, n, cells[]}, VILLE.cell["x,y"] = id, VILLE.adj[id] = voisins. */
var VILLE = (function () {
  var PAS = 2, cell = {}, q = {}, ordre = [], adj = {};
  D.quartiers.liste.forEach(function (Q) {
    var xs = [], ys = [];
    Q.polys.forEach(function (r) { r.forEach(function (p) { xs.push(p[0]); ys.push(p[1]); }); });
    var o = { id: Q.id, zone: Q.zone, nom: Q.nom, arr: Q.arr, cx: 0, cy: 0, n: 0, cells: [] };
    for (var x = Math.floor(Math.min.apply(0, xs) / PAS) * PAS; x <= Math.max.apply(0, xs); x += PAS)
      for (var y = Math.floor(Math.min.apply(0, ys) / PAS) * PAS; y <= Math.max.apply(0, ys); y += PAS) {
        var k = x + "," + y;
        if (cell[k]) continue;
        var c = [x + PAS / 2, y + PAS / 2];
        if (Q.polys.some(function (r) { return dansPoly(c, r); })) { cell[k] = Q.id; o.cells.push([x, y]); o.cx += c[0]; o.cy += c[1]; o.n++; }
      }
    if (!o.n) {                                    // très petit quartier : au moins une case
      var cx = (Math.min.apply(0, xs) + Math.max.apply(0, xs)) / 2, cy = (Math.min.apply(0, ys) + Math.max.apply(0, ys)) / 2;
      var kx = Math.floor(cx / PAS) * PAS, ky = Math.floor(cy / PAS) * PAS;
      cell[kx + "," + ky] = Q.id; o.cells.push([kx, ky]); o.cx = cx; o.cy = cy; o.n = 1;
    } else { o.cx /= o.n; o.cy /= o.n; }
    // le centre doit tomber dans le quartier (quartiers en croissant) : sinon la case la plus proche du centre
    var ck = Math.floor(o.cx / PAS) * PAS + "," + Math.floor(o.cy / PAS) * PAS;
    if (cell[ck] !== Q.id) {
      var best = o.cells[0], bd = Infinity;
      o.cells.forEach(function (p) { var d = (p[0] - o.cx) * (p[0] - o.cx) + (p[1] - o.cy) * (p[1] - o.cy); if (d < bd) { bd = d; best = p; } });
      o.cx = best[0] + PAS / 2; o.cy = best[1] + PAS / 2;
    }
    q[Q.id] = o; ordre.push(Q.id); adj[Q.id] = {};
  });
  Object.keys(cell).forEach(function (k) {
    var a = cell[k], p = k.split(",").map(Number);
    [[PAS, 0], [0, PAS], [PAS, PAS], [PAS, -PAS]].forEach(function (d) {
      var b = cell[(p[0] + d[0]) + "," + (p[1] + d[1])];
      if (b && b !== a) { adj[a][b] = 1; adj[b][a] = 1; }
    });
  });
  Object.keys(adj).forEach(function (k) { adj[k] = Object.keys(adj[k]); });
  // les îles du Frioul : reliées au Vieux-Port et au Pharo (navettes maritimes)
  var iles = ordre.filter(function (id) { return q[id].nom === "Les Îles"; })[0];
  if (iles) ordre.filter(function (id) { return q[id].nom === "Le Pharo" || q[id].nom === "Hôtel de Ville"; }).forEach(function (v) {
    if (adj[iles].indexOf(v) < 0) { adj[iles].push(v); adj[v].push(iles); }
  });
  var zones = {};
  ordre.forEach(function (id) { (zones[q[id].zone] = zones[q[id].zone] || []).push(id); });
  return { PAS: PAS, cell: cell, q: q, ordre: ordre, adj: adj, zones: zones, taille: D.quartiers.taille };
})();

/** Quartier sous un point du plan. */
function quartierEn(x, y) {
  var P = VILLE.PAS, gx = Math.floor(x / P) * P, gy = Math.floor(y / P) * P;
  return VILLE.cell[gx + "," + gy] || VILLE.cell[(gx + P) + "," + gy] || VILLE.cell[gx + "," + (gy + P)] || null;
}

/* ---------- état des quartiers ---------- */
function advTotal(s) { var t = 0; for (var f in s.adv) t += s.adv[f]; return t; }
function libreDe(s) { return Math.max(0, 100 - s.ctl - advTotal(s)); }
function factionDominante(s) {
  var best = null, v = 0;
  for (var f in s.adv) if (s.adv[f] > v && G.factions[f] && G.factions[f].vivante) { v = s.adv[f]; best = f; }
  return best;
}
/** libre · joueur · securise · conteste · faction (règles §3.3). */
function statut(id) {
  var s = G.q[id], a = advTotal(s), C = B.controle;
  if (s.ctl <= 0 && a <= 0.5) return "libre";
  if (s.ctl >= C.securise_controle && a <= C.securise_adverse) return "securise";
  if (s.ctl > 0 && a > 0.5) return "conteste";
  if (s.ctl > 0) return "joueur";
  return "faction";
}
var STATUT_NOM = { libre: "Libre", joueur: "À toi", securise: "Sécurisé", conteste: "Contesté", faction: "Tenu par une faction" };
/** Le joueur peut-il y envoyer un opérateur ? (§3.4 : voisin à 50 % ou plus, ou déjà présent) */
function accessible(id, tache) {
  // le Repérage peut entrer dans une zone verrouillée, depuis un quartier voisin où tu es présent
  if (tache === "reperer" && G.q[id].ctl <= 0 && VILLE.adj[id].some(function (v) { return G.q[v].ctl > 0; })) return true;
  if (!zoneOuverte(VILLE.q[id].zone)) return false;
  if (G.q[id].ctl > 0) return true;
  return VILLE.adj[id].some(function (v) { return G.q[v].ctl >= B.controle.acces_voisin; });
}
/** Part de la ville contrôlée par le joueur (moyenne des contrôles). */
function territoire() {
  var t = 0;
  VILLE.ordre.forEach(function (id) { t += G.q[id].ctl; });
  return t / VILLE.ordre.length;
}
/** Ajoute du contrôle au joueur : d'abord la part libre, puis aux dépens des factions (à moitié). */
function gagnerControle(id, v) {
  var s = G.q[id], avant = s.ctl, libre = libreDe(s), pris = Math.min(v, libre);
  s.ctl += pris; v -= pris;
  if (v > 0) {
    var a = advTotal(s);
    if (a > 0) {
      var prise = Math.min(a, v * 0.5);
      for (var f in s.adv) s.adv[f] = Math.max(0, s.adv[f] - prise * s.adv[f] / a);
      s.ctl += prise;
    }
  }
  s.ctl = clamp(s.ctl, 0, 100);
  return s.ctl - avant;
}
/** Une faction gagne de la présence : part libre, puis aux dépens du joueur et des autres (sauf alliées). */
function gagnerPresence(id, f, v) {
  var s = G.q[id], libre = libreDe(s), pris = Math.min(v, libre), perteJoueur = 0;
  s.adv[f] = (s.adv[f] || 0) + pris; v -= pris;
  if (v > 0) {
    var cibles = s.ctl;
    for (var g in s.adv) if (g !== f && !allies(f, g)) cibles += s.adv[g];
    if (cibles > 0) {
      var prise = Math.min(cibles, v);
      perteJoueur = prise * s.ctl / cibles;
      s.ctl = Math.max(0, s.ctl - perteJoueur);
      for (var h in s.adv) if (h !== f && !allies(f, h)) s.adv[h] = Math.max(0, s.adv[h] - prise * s.adv[h] / cibles);
      s.adv[f] += prise;
    }
  }
  return perteJoueur;
}
function allies(a, b) {
  return G.alliances.some(function (x) { return (x.a === a && x.b === b) || (x.a === b && x.b === a); });
}

/** Revenu et coût d'un quartier pour ce cycle (saisons comprises). */
function multSaison(id) {
  var m = 1, z = VILLE.q[id].zone;
  (G.saisons || []).forEach(function (sid) {
    var s = B.evenements.saisons.filter(function (x) { return x.id === sid; })[0];
    if (s && (s.zones === "*" || s.zones.indexOf(z) >= 0)) m *= s.flux_mult;
  });
  return m;
}

/* ---------- nouvelle partie ---------- */
/** Prépare une ville et un réseau de départ. seed : facultatif (tests). */
function newGame(seed) {
  var Dp = B.depart, Q = B.quartiers;
  G = { v: 2, seed: (seed == null ? (Date.now() & 0x7fffffff) : seed) | 0 };
  G.cycle = 1; G.tresorerie = Dp.tresorerie; G.dette = 0; G.cycles_zero = 0;
  G.capacite = Dp.capacite; G.seuils_pris = []; G.ind = { pression: 15, stabilite: 55, exposition: 5 };
  G.operateurs = []; G.prochain_op = 1; G.visages = B.operateurs.visages.slice();
  G.factions = {}; G.alliances = []; G.flux = []; G.journal = []; G.rapport = null; G.fini = null; G.saisons = [];
  G.historique = []; G.quetesFaites = {}; G.zonesOuvertes = [];
  D.equipes.liste.forEach(function (e) {
    G.factions[e.id] = { id: e.id, budget: e.budget, effectif: e.effectif, agressivite: e.agressivite, objectif: "maintenir", cible: null,
      vivante: true, pic: 0, intention: null };
  });
  G.q = {};
  VILLE.ordre.forEach(function (id) {
    var z = IDX.zones[VILLE.q[id].zone];
    var s = { ctl: 0, adv: {}, conn: 0, ch: genererCharbons(id), inv: 0, sans_op: 0 };
    s.maintien = Math.round(s.ch.reduce(function (t, c) { return t + c.rent; }, 0) * B.charbons.maintien_ratio / 10) * 10 + 40;
    if (z.controle) s.adv[z.controle] = rint(Q.presence_faction[0], Q.presence_faction[1]);
    else D.equipes.liste.forEach(function (e) { if (rnd() < 0.3) s.adv[e.id] = rint(Q.presence_neutre[0], Q.presence_neutre[1]); });
    G.q[id] = s;
  });
  // présences voisines sur les bords des zones
  VILLE.ordre.forEach(function (id) {
    VILLE.adj[id].forEach(function (v) {
      var zc = IDX.zones[VILLE.q[v].zone].controle;
      if (zc && VILLE.q[v].zone !== VILLE.q[id].zone && !G.q[id].adv[zc] && rnd() < 0.5) G.q[id].adv[zc] = rint(Q.presence_voisine[0], Q.presence_voisine[1]);
    });
  });
  VILLE.ordre.forEach(function (id) { var s = G.q[id], a = advTotal(s); if (a > 100) for (var f in s.adv) s.adv[f] *= 100 / a; });
  // les factions tiennent les charbons là où elles sont fortes
  VILLE.ordre.forEach(function (id) {
    var s = G.q[id], dom = factionDominante(s);
    s.ch.forEach(function (c) { if (dom && s.adv[dom] >= 30) { c.proprio = dom; c.ouvert = true; } });
  });
  // base du joueur
  var zones = D.zones.liste.filter(function (z) { return z.depart; });
  var zb = pick(zones), qs = VILLE.zones[zb.id].slice();
  var cand = qs.filter(function (id) { return G.q[id].ch.length; });
  var base = pick(cand.length ? cand : qs), sb = G.q[base];
  G.zonesOuvertes = [zb.id];
  var nb = Dp.charbons_base, essai = 0;
  while (sb.ch.length < nb && essai++ < 10) sb.ch = sb.ch.concat(genererCharbons(base)).slice(0, Math.max(nb, sb.ch.length));
  sb.ch.forEach(function (c, i) { c.proprio = i < nb ? "joueur" : null; c.ouvert = i < nb; });
  G.base = base;
  sb.adv = {}; if (zb.controle) sb.adv[zb.controle] = Dp.presence_base_adverse;
  sb.ctl = Dp.controle_base; sb.conn = Dp.connaissance_base;
  qs.forEach(function (id) { G.q[id].conn = Math.max(G.q[id].conn, Dp.connaissance_zone); });
  VILLE.adj[base].forEach(function (id) { G.q[id].conn = Math.max(G.q[id].conn, 1); });
  Dp.operateurs.forEach(function (t) { creerOperateur(t, base, rint(Dp.competence[0], Dp.competence[1])); });
  D.equipes.liste.forEach(function (e) { G.factions[e.id].pic = territoireFaction(e.id).length; });
  log("Cycle 1. Ton réseau tient " + VILLE.q[base].nom + " (" + IDX.zones[zb.id].nom + ").", "debut", base);
  return G;
}
/** Quartiers où une faction a 30 % ou plus (§5.2). */
function territoireFaction(f) {
  return VILLE.ordre.filter(function (id) { return (G.q[id].adv[f] || 0) >= 30; });
}
