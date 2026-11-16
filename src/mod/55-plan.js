/* CHOUF — 55-plan : la ville en relief (vue stratégique façon jeu de gestion de territoires).
   Immeubles générés à partir de data/zones.json (bati, plan), teintés par l'équipe qui tient la zone,
   badges ronds au-dessus des quartiers, glisser pour se déplacer, pincer pour zoomer, toucher pour choisir.
   Le dessin des immeubles est fait une fois dans une image hors écran, puis seulement déplacé. */

var PLAN = { cache: null, sig: null, cam: null, el: null, ptr: {}, raf: 0 };

/* ---------- projection : plan (x, y, hauteur) → image hors écran ---------- */
function planProj() {
  var C = B.carte, a = C.rotation_deg * Math.PI / 180;
  return { c: Math.cos(a), s: Math.sin(a), k: C.ecrasement, hk: C.hauteur, R: C.resolution, minX: 0, minY: 0 };
}
function proj(P, x, y, h) {
  var dx = x - VILLE.taille[0] / 2, dy = y - VILLE.taille[1] / 2;
  var rx = dx * P.c - dy * P.s, ry = dx * P.s + dy * P.c;
  return [(rx - P.minX) * P.R, (ry * P.k - (h || 0) * P.hk - P.minY) * P.R];
}
function deproj(P, ox, oy) {
  var rx = ox / P.R + P.minX, ry = (oy / P.R + P.minY) / P.k;
  return [rx * P.c + ry * P.s + VILLE.taille[0] / 2, -rx * P.s + ry * P.c + VILLE.taille[1] / 2];
}
function hex(c) { return [parseInt(c.slice(1, 3), 16), parseInt(c.slice(3, 5), 16), parseInt(c.slice(5, 7), 16)]; }
function mix(a, b, t) { return [a[0] + (b[0] - a[0]) * t, a[1] + (b[1] - a[1]) * t, a[2] + (b[2] - a[2]) * t]; }
function rgb(c, f) { f = f == null ? 1 : f; return "rgb(" + Math.round(c[0] * f) + "," + Math.round(c[1] * f) + "," + Math.round(c[2] * f) + ")"; }

/** Couleur d'un quartier selon son statut (§12.2 : vert joueur, couleur de faction, gris libre, orange contesté). */
var COUL = { joueur: "#6ccf4f", securise: "#4fd06a", conteste: "#e8862c", libre: "#8d9096" };
function couleurQuartier(id) {
  var st = statut(id), s = G.q[id];
  if (s.conn === 0 && s.ctl <= 0) return null;                  // inconnu : brouillard
  if (st === "faction") { var f = factionDominante(s); return f ? IDX.equipes[f].couleur : COUL.libre; }
  return COUL[st];
}
function forceTeinte(id) {
  var s = G.q[id], st = statut(id);
  if (st === "faction") return 0.25 + Math.min(0.4, advTotal(s) / 200);
  if (st === "libre") return 0;
  return 0.3 + Math.min(0.35, s.ctl / 250);
}
/** Génère les immeubles d'une zone sur les cases de ses quartiers (déterministe). */
function immeubles(z) {
  var r = rngDecor(z.id), b = z.bati, s = b.pas, out = [], ids = VILLE.zones[z.id] || [];
  var dens = { centre: 0.92, aise: 0.8, tours: 0.42, mixte: 0.62, port: 0.55, maisons: 0.55 }[b.type] || 0.6;
  var xs = [], ys = [];
  ids.forEach(function (id) { VILLE.q[id].cells.forEach(function (c) { xs.push(c[0]); ys.push(c[1]); }); });
  if (!xs.length) return out;
  function dans(x, y) { var q = quartierEn(x, y); return q && VILLE.q[q].zone === z.id ? q : null; }
  for (var x = Math.min.apply(0, xs); x < Math.max.apply(0, xs); x += s) {
    for (var y = Math.min.apply(0, ys); y < Math.max.apply(0, ys); y += s) {
      if (r() > dens) continue;
      var cx = x + s / 2 + (r() - 0.5) * s * 0.25, cy = y + s / 2 + (r() - 0.5) * s * 0.25;
      var w = s * (b.type === "tours" ? 0.42 + r() * 0.2 : 0.55 + r() * 0.25), d = s * (b.type === "tours" ? 0.42 + r() * 0.5 : 0.55 + r() * 0.25);
      var pts = [[cx - w / 2, cy - d / 2], [cx + w / 2, cy - d / 2], [cx + w / 2, cy + d / 2], [cx - w / 2, cy + d / 2]];
      var q = dans(cx, cy);
      if (!q || !pts.every(function (p) { return dans(p[0], p[1]); })) continue;
      var h = b.hmin + Math.pow(r(), b.type === "tours" ? 0.7 : 1.6) * (b.hmax - b.hmin);
      out.push({ pts: pts, h: h, cx: cx, cy: cy, z: z.id, q: q });
    }
  }
  return out;
}

/** Dessine toute la ville dans une image hors écran. Refait seulement si le contrôle des zones change. */
function planCache() {
  var sig = VILLE.ordre.map(function (id) { var s = G.q[id]; return statut(id) + (factionDominante(s) || "") + (s.conn > 0 ? 1 : 0) + Math.floor(s.ctl / 20) + (zoneOuverte(VILLE.q[id].zone) ? "o" : "v"); }).join("|");
  if (PLAN.cache && PLAN.sig === sig) return PLAN.cache;
  var P = planProj(), R = D.quartiers.repere, TX = VILLE.taille[0], TY = VILLE.taille[1];
  // bornes
  var bx = [1e9, -1e9, 1e9, -1e9];
  [[0, 0], [TX, 0], [TX, TY], [0, TY]].forEach(function (p) {
    var dx = p[0] - TX / 2, dy = p[1] - TY / 2, rx = dx * P.c - dy * P.s, ry = (dx * P.s + dy * P.c) * P.k;
    bx[0] = Math.min(bx[0], rx); bx[1] = Math.max(bx[1], rx); bx[2] = Math.min(bx[2], ry - 30); bx[3] = Math.max(bx[3], ry);
  });
  P.minX = bx[0] - 10; P.minY = bx[2] - 10;
  var W = Math.ceil((bx[1] - bx[0] + 20) * P.R), H = Math.ceil((bx[3] - bx[2] + 20) * P.R);
  var cv = document.createElement("canvas"); cv.width = W; cv.height = H;
  var g = cv.getContext("2d");
  function poly(pts, h, fill, stroke, lw) {
    g.beginPath();
    pts.forEach(function (p, i) { var q = proj(P, p[0], p[1], h); i ? g.lineTo(q[0], q[1]) : g.moveTo(q[0], q[1]); });
    g.closePath();
    if (fill) { g.fillStyle = fill; g.fill(); }
    if (stroke) { g.strokeStyle = stroke; g.lineWidth = lw || 1; g.stroke(); }
  }
  // mer
  var gr = g.createLinearGradient(0, 0, 0, H); gr.addColorStop(0, "#1c2a33"); gr.addColorStop(1, "#0d151b");
  g.fillStyle = gr; g.fillRect(0, 0, W, H);
  // terre et sol des zones
  // sol : une case par point du découpage, teintée selon le quartier
  var PAS = VILLE.PAS, couleurs = {};
  VILLE.ordre.forEach(function (id) {
    var c = couleurQuartier(id);
    var ouv = zoneOuverte(VILLE.q[id].zone);
    couleurs[id] = c ? rgb(mix([70, 72, 76], hex(c), forceTeinte(id)), ouv ? 1 : 0.62) : (ouv ? "rgb(52,54,58)" : "rgb(36,38,41)");
  });
  Object.keys(VILLE.cell).forEach(function (k) {
    var p = k.split(",").map(Number);
    poly([[p[0], p[1]], [p[0] + PAS, p[1]], [p[0] + PAS, p[1] + PAS], [p[0], p[1] + PAS]], 0, couleurs[VILLE.cell[k]], couleurs[VILLE.cell[k]], 0.6);
  });
  // limites : fines entre quartiers, épaisses entre zones
  g.lineCap = "round";
  Object.keys(VILLE.cell).forEach(function (k) {
    var p = k.split(",").map(Number), a = VILLE.cell[k];
    [[PAS, 0, [p[0] + PAS, p[1]], [p[0] + PAS, p[1] + PAS]], [0, PAS, [p[0], p[1] + PAS], [p[0] + PAS, p[1] + PAS]]].forEach(function (d) {
      var b = VILLE.cell[(p[0] + d[0]) + "," + (p[1] + d[1])];
      if (!b || b === a) return;
      var zz = VILLE.q[a].zone !== VILLE.q[b].zone;
      var q1 = proj(P, d[2][0], d[2][1], 0), q2 = proj(P, d[3][0], d[3][1], 0);
      g.beginPath(); g.moveTo(q1[0], q1[1]); g.lineTo(q2[0], q2[1]);
      g.strokeStyle = zz ? "#b9bdc4" : "rgba(160,165,172,.45)"; g.lineWidth = (zz ? 1.4 : 0.6) * P.R; g.stroke();
    });
  });
  poly(R.vieux_port, 0, "#1c2a33", "#4a4e55", 1);
  // immeubles, triés de l'arrière vers l'avant
  var all = [];
  D.zones.liste.forEach(function (z) { all = all.concat(immeubles(z)); });
  all.forEach(function (b) { b.prof = (b.cx - TX / 2) * P.s + (b.cy - TY / 2) * P.c; });
  all.sort(function (a, b) { return a.prof - b.prof; });
  var lum = [0.62, 0.48];
  all.forEach(function (b) {
    var cq = couleurQuartier(b.q), inconnu = !cq;
    var c = inconnu ? [70, 72, 76] : mix([196, 198, 202], hex(cq), forceTeinte(b.q) * 1.3);
    if (!zoneOuverte(b.z)) c = mix(c, [40, 42, 46], 0.45);
    for (var i = 0; i < 4; i++) {
      var p1 = b.pts[i], p2 = b.pts[(i + 1) % 4];
      var nx = p2[1] - p1[1], ny = -(p2[0] - p1[0]);           // normale extérieure
      var nry = nx * P.s + ny * P.c, nrx = nx * P.c - ny * P.s;
      if (nry <= 0) continue;                                  // face cachée
      g.beginPath();
      [proj(P, p1[0], p1[1], 0), proj(P, p2[0], p2[1], 0), proj(P, p2[0], p2[1], b.h), proj(P, p1[0], p1[1], b.h)].forEach(function (q, k) { k ? g.lineTo(q[0], q[1]) : g.moveTo(q[0], q[1]); });
      g.closePath(); g.fillStyle = rgb(c, nrx < 0 ? lum[0] : lum[1]); g.fill();
    }
    poly(b.pts, b.h, rgb(c, 0.92), rgb(c, 0.55), 0.6);
    // fenêtres allumées sur les hautes tours
    if (b.h > 12 && !inconnu) {
      var q = proj(P, b.pts[3][0], b.pts[3][1], 0), q2 = proj(P, b.pts[2][0], b.pts[2][1], 0);
      g.fillStyle = "rgba(255,190,110,.55)";
      for (var f = 3; f < b.h - 1; f += 3) for (var t = 0.25; t < 0.9; t += 0.3) {
        var x = q[0] + (q2[0] - q[0]) * t, y = q[1] + (q2[1] - q[1]) * t - f * P.hk * P.R;
        if (rngDecor(b.cx + ":" + f + ":" + t)() < 0.35) g.fillRect(x, y, P.R * 0.7, P.R * 0.7);
      }
    }
  });
  // repères : Vélodrome, Notre-Dame de la Garde, grues, calanques
  var v = proj(P, R.velodrome[0], R.velodrome[1], 2);
  g.beginPath(); g.ellipse(v[0], v[1], 9 * P.R, 5 * P.R, P.c * 0.4, 0, Math.PI * 2); g.fillStyle = "#d9dbde"; g.fill();
  g.beginPath(); g.ellipse(v[0], v[1], 6 * P.R, 3.2 * P.R, P.c * 0.4, 0, Math.PI * 2); g.fillStyle = "#4b7d4a"; g.fill();
  var n = R.notre_dame, nb = proj(P, n[0], n[1], 0), nt = proj(P, n[0], n[1], 20);
  g.beginPath(); g.ellipse(nb[0], nb[1], 14 * P.R, 7 * P.R, 0, Math.PI, 0); g.fillStyle = "#55584f"; g.fill();
  g.fillStyle = "#e9e2cf"; g.fillRect(nt[0] - 2 * P.R, nt[1], 4 * P.R, (nb[1] - nt[1]) - 4 * P.R);
  g.beginPath(); g.arc(nt[0], nt[1], 2 * P.R, 0, Math.PI * 2); g.fillStyle = "#f2c94c"; g.fill();
  g.strokeStyle = "#c9a54a"; g.lineWidth = 1.2 * P.R;
  R.grues.forEach(function (p) {
    var a = proj(P, p[0], p[1], 0), b2 = proj(P, p[0], p[1], 16), c2 = proj(P, p[0] - 8, p[1] + 4, 16);
    g.beginPath(); g.moveTo(a[0], a[1]); g.lineTo(b2[0], b2[1]); g.lineTo(c2[0], c2[1]); g.stroke();
  });
  R.calanques.forEach(function (p, i) {
    var a = proj(P, p[0], p[1], 0), t2 = proj(P, p[0] + 6, p[1] + 2, 9 + (i % 2) * 4);
    g.beginPath(); g.moveTo(a[0] - 10 * P.R, a[1]); g.lineTo(t2[0], t2[1]); g.lineTo(a[0] + 12 * P.R, a[1] + 2 * P.R); g.closePath();
    g.fillStyle = "#d8d4c8"; g.fill();
  });
  PLAN.cache = { cv: cv, P: P, W: W, H: H };
  PLAN.sig = sig;
  return PLAN.cache;
}

/* ---------- badges ---------- */
function glyphe(g, type, x, y, r) {
  g.strokeStyle = "#fff"; g.fillStyle = "#fff"; g.lineWidth = r * 0.16; g.lineJoin = "round"; g.lineCap = "round";
  var u = r * 0.5;
  g.beginPath();
  switch (type) {
    case "maison": g.moveTo(x - u, y); g.lineTo(x, y - u); g.lineTo(x + u, y); g.moveTo(x - u * 0.7, y - u * 0.2); g.lineTo(x - u * 0.7, y + u * 0.8); g.lineTo(x + u * 0.7, y + u * 0.8); g.lineTo(x + u * 0.7, y - u * 0.2); g.stroke(); break;
    case "police": g.arc(x, y + u * 0.2, u * 0.6, Math.PI, 0); g.lineTo(x + u * 0.6, y + u * 0.7); g.lineTo(x - u * 0.6, y + u * 0.7); g.closePath(); g.fill();
      g.beginPath(); g.moveTo(x, y - u * 0.9); g.lineTo(x, y - u * 0.6); g.moveTo(x - u, y - u * 0.4); g.lineTo(x - u * 0.75, y - u * 0.25); g.moveTo(x + u, y - u * 0.4); g.lineTo(x + u * 0.75, y - u * 0.25); g.stroke(); break;
    case "tension": g.moveTo(x, y - u); g.quadraticCurveTo(x + u, y, x + u * 0.4, y + u * 0.8); g.lineTo(x - u * 0.4, y + u * 0.8); g.quadraticCurveTo(x - u, y, x, y - u); g.fill(); break;
    case "tour": g.rect(x - u * 0.55, y - u * 0.3, u * 1.1, u * 1.1); g.rect(x - u * 0.7, y - u * 0.8, u * 0.3, u * 0.5); g.rect(x - u * 0.15, y - u * 0.8, u * 0.3, u * 0.5); g.rect(x + u * 0.4, y - u * 0.8, u * 0.3, u * 0.5); g.fill(); break;
    case "oiseau": g.moveTo(x - u, y - u * 0.2); g.quadraticCurveTo(x - u * 0.4, y - u * 0.7, x, y + u * 0.1); g.quadraticCurveTo(x + u * 0.4, y - u * 0.7, x + u, y - u * 0.2); g.stroke(); break;
    case "vague": g.moveTo(x - u, y); g.bezierCurveTo(x - u * 0.5, y - u * 0.7, x, y + u * 0.7, x + u, y); g.stroke(); break;
    case "vent": g.arc(x, y, u * 0.7, 0.3, Math.PI * 1.7); g.stroke(); g.beginPath(); g.moveTo(x - u, y + u * 0.5); g.lineTo(x + u * 0.3, y + u * 0.5); g.stroke(); break;
    case "falaise": g.moveTo(x - u, y + u * 0.7); g.lineTo(x - u * 0.2, y - u * 0.8); g.lineTo(x + u * 0.3, y); g.lineTo(x + u, y + u * 0.7); g.closePath(); g.fill(); break;
    case "brouillard": g.font = "bold " + Math.round(r * 1.1) + "px system-ui"; g.textAlign = "center"; g.textBaseline = "middle"; g.fillText("?", x, y + 1); break;
  }
}
/** Images des badges (série I) quand elles existent ; sinon le badge est dessiné. */
var BADGE_IMG = { police: "icone_police", tension: "icone_tension" }, BADGE_CACHE = {};
function badgeImage(type) {
  var k = BADGE_IMG[type], u = k && ASSET.u(k);
  if (!u) return null;
  if (!BADGE_CACHE[k]) { var im = new Image(); im.onload = function () { planDessin(); }; im.src = u; BADGE_CACHE[k] = im; }
  return BADGE_CACHE[k].complete && BADGE_CACHE[k].naturalWidth ? BADGE_CACHE[k] : null;
}
function badge(g, x, y, r, anneau, type, fond) {
  var im = badgeImage(type);
  if (im) { g.drawImage(im, x - r * 1.15, y - r * 1.15, r * 2.3, r * 2.3); return; }
  g.beginPath(); g.arc(x, y + 1.5, r + 2, 0, Math.PI * 2); g.fillStyle = "rgba(0,0,0,.45)"; g.fill();
  g.beginPath(); g.arc(x, y, r, 0, Math.PI * 2); g.fillStyle = fond || "#1b1c1f"; g.fill();
  g.lineWidth = r * 0.2; g.strokeStyle = anneau; g.stroke();
  glyphe(g, type, x, y, r);
}
/* ---------- contours des quartiers (pour la sélection) ---------- */
var BORDS = null;
function bords(id) {
  if (!BORDS) {
    BORDS = {};
    var PAS = VILLE.PAS;
    Object.keys(VILLE.cell).forEach(function (k) {
      var p = k.split(",").map(Number), a = VILLE.cell[k];
      [[PAS, 0, p[0] + PAS, p[1], p[0] + PAS, p[1] + PAS], [-PAS, 0, p[0], p[1], p[0], p[1] + PAS],
       [0, PAS, p[0], p[1] + PAS, p[0] + PAS, p[1] + PAS], [0, -PAS, p[0], p[1], p[0] + PAS, p[1]]].forEach(function (d) {
        if (VILLE.cell[(p[0] + d[0]) + "," + (p[1] + d[1])] !== a) (BORDS[a] = BORDS[a] || []).push([d[2], d[3], d[4], d[5]]);
      });
    });
  }
  return BORDS[id] || [];
}

/* ---------- affichage et gestes ---------- */
function planMonter(el) {
  PLAN.el = el;
  var C = planCache(), dpr = window.devicePixelRatio || 1;
  var W = el.clientWidth, H = el.clientHeight;
  el.width = W * dpr; el.height = H * dpr;
  if (!PLAN.cam) {
    var b = VILLE.q[G.base], q = proj(C.P, b.cx, b.cy, 0);
    PLAN.cam = { x: q[0], y: q[1] + 20, z: Math.max(W / C.W * 2.6, 0.32) };
  }
  if (!el.dataset.ok) {
    el.dataset.ok = "1";
    el.addEventListener("pointerdown", planDown);
    el.addEventListener("pointermove", planMove);
    el.addEventListener("pointerup", planUp);
    el.addEventListener("pointercancel", function (e) { delete PLAN.ptr[e.pointerId]; });
    el.addEventListener("wheel", function (e) { e.preventDefault(); zoomer(e.deltaY < 0 ? 1.12 : 1 / 1.12); }, { passive: false });
  }
  planDessin();
}
function versEcran(ox, oy) {
  var el = PLAN.el, cam = PLAN.cam;
  return [(ox - cam.x) * cam.z + el.clientWidth / 2, (oy - cam.y) * cam.z + el.clientHeight / 2];
}
function ecranQ(id, h) { var q = VILLE.q[id], p = proj(PLAN.cache.P, q.cx, q.cy, h || 0); return versEcran(p[0], p[1]); }
function zoomer(f) {
  var C = PLAN.cache;
  PLAN.cam.z = clamp(PLAN.cam.z * f, PLAN.el.clientWidth / C.W * 0.9, 2.2);
  planDessin();
}
function contour(g, id, couleur, largeur, glow) {
  var P = PLAN.cache.P;
  g.beginPath();
  bords(id).forEach(function (s) {
    var a = proj(P, s[0], s[1], 0), b = proj(P, s[2], s[3], 0), A = versEcran(a[0], a[1]), Bq = versEcran(b[0], b[1]);
    g.moveTo(A[0], A[1]); g.lineTo(Bq[0], Bq[1]);
  });
  g.lineWidth = largeur; g.strokeStyle = couleur; g.lineCap = "round";
  if (glow) { g.shadowColor = couleur; g.shadowBlur = glow; }
  g.stroke(); g.shadowBlur = 0;
}
function pastille(g, x, y, r, anneau, lettre, fond, plein) {
  g.beginPath(); g.arc(x, y + 1.5, r + 2, 0, Math.PI * 2); g.fillStyle = "rgba(0,0,0,.5)"; g.fill();
  g.beginPath(); g.arc(x, y, r, 0, Math.PI * 2); g.fillStyle = plein ? anneau : (fond || "#1b1c1f"); g.fill();
  g.lineWidth = Math.max(2, r * 0.22); g.strokeStyle = plein ? "#fff" : anneau; g.stroke();
  g.fillStyle = plein ? "#111" : "#fff"; g.font = "700 " + Math.round(r * 1.05) + "px system-ui"; g.textAlign = "center"; g.textBaseline = "middle";
  g.fillText(lettre, x, y + 0.5);
}
function planDessin() {
  var el = PLAN.el;
  if (!el || !el.isConnected) return;
  var g = el.getContext("2d"), dpr = window.devicePixelRatio || 1, C = PLAN.cache, cam = PLAN.cam;
  var W = el.clientWidth, H = el.clientHeight;
  cam.x = clamp(cam.x, 0, C.W); cam.y = clamp(cam.y, 0, C.H);
  g.setTransform(dpr, 0, 0, dpr, 0, 0);
  g.fillStyle = "#0d151b"; g.fillRect(0, 0, W, H);
  g.drawImage(C.cv, W / 2 - cam.x * cam.z, H / 2 - cam.y * cam.z, C.W * cam.z, C.H * cam.z);
  var op = UI.selOp ? opParId(UI.selOp) : null;
  // accès possibles pour l'opérateur choisi
  if (op) VILLE.ordre.forEach(function (id) {
    if (accessible(id)) contour(g, id, "rgba(108,207,79,.55)", 1.4);
    else if (op.type === "reperage" && accessible(id, "reperer")) contour(g, id, "rgba(143,198,255,.6)", 1.4);
  });
  if (UI.selQ) contour(g, UI.selQ, "#f2c94c", 3, 12);
  PLAN.hits = [];
  var r = clamp(9 * Math.sqrt(cam.z / 0.5), 8, 15);
  // noms : zones de loin, quartiers de près
  g.textAlign = "center"; g.textBaseline = "middle"; g.lineWidth = 3; g.strokeStyle = "rgba(0,0,0,.8)";
  if (cam.z < 0.9) {
    D.zones.liste.forEach(function (z) {
      var ids = VILLE.ordre.filter(function (id) { return VILLE.q[id].zone === z.id; }), sx = 0, sy = 0;
      ids.forEach(function (id) { sx += VILLE.q[id].cx; sy += VILLE.q[id].cy; });
      var p = proj(C.P, sx / ids.length, sy / ids.length, 0), s = versEcran(p[0], p[1]);
      g.font = "700 12px system-ui"; g.strokeText(z.court, s[0], s[1]); g.fillStyle = "#f4f1ea"; g.fillText(z.court, s[0], s[1]);
    });
  } else {
    VILLE.ordre.forEach(function (id) {
      var s = ecranQ(id), conn = G.q[id].conn > 0 || G.q[id].ctl > 0;
      if (!conn && cam.z < 1.3) return;
      if (s[0] < -40 || s[0] > W + 40 || s[1] < -20 || s[1] > H + 20) return;
      g.font = "600 " + Math.round(clamp(10 * cam.z, 9, 13)) + "px system-ui";
      g.strokeText(VILLE.q[id].nom, s[0], s[1] + r * 1.6); g.fillStyle = conn ? "#f4f1ea" : "#8d9096"; g.fillText(VILLE.q[id].nom, s[0], s[1] + r * 1.6);
    });
  }
  // alertes et intentions repérées
  VILLE.ordre.forEach(function (id) {
    var s = G.q[id], p = ecranQ(id);
    if (G.alertes && G.alertes[id]) pastille(g, p[0] + r * 1.3, p[1] - r * 2.4, r * 0.75, "#e5484d", "!", null, true);
    if (s.vu >= G.cycle) {
      Object.keys(G.factions).forEach(function (fid) {
        var f = G.factions[fid];
        if (f.vivante && f.intention && f.intention.q === id && f.intention.objectif !== "maintenir")
          pastille(g, p[0] - r * 1.3, p[1] - r * 2.4, r * 0.75, IDX.equipes[fid].couleur, "⚔", null, false);
      });
    }
  });
  // zones verrouillées : un cadenas au centre de la zone
  D.zones.liste.forEach(function (z) {
    if (zoneOuverte(z.id)) return;
    var ids = VILLE.zones[z.id], sx = 0, sy = 0;
    ids.forEach(function (id) { sx += VILLE.q[id].cx; sy += VILLE.q[id].cy; });
    var p = proj(C.P, sx / ids.length, sy / ids.length, 0), e = versEcran(p[0], p[1]);
    pastille(g, e[0], e[1] - r * 2.2, r * 0.95, "#8d9096", "🔒", "#26282c", false);
  });
  // charbons (de près) : or = à toi, couleur = faction, gris = libre
  if (cam.z >= 0.62) VILLE.ordre.forEach(function (id) {
    var s = G.q[id];
    if (s.conn < 2 && s.ctl <= 0) return;
    s.ch.forEach(function (c) {
      var p = proj(C.P, c.x, c.y, 1), e = versEcran(p[0], p[1]);
      if (e[0] < -10 || e[0] > W + 10 || e[1] < -10 || e[1] > H + 10) return;
      var col = c.proprio === "joueur" ? (c.saisi ? "#7a5a20" : "#f2c94c") : c.proprio ? IDX.equipes[c.proprio].couleur : "#8d9096";
      g.beginPath(); g.arc(e[0], e[1], r * 0.42, 0, Math.PI * 2); g.fillStyle = col; g.fill();
      g.lineWidth = 1.5; g.strokeStyle = "#111"; g.stroke();
    });
  });
  // opérateurs, groupés par quartier
  var parQ = {};
  opsActifs().forEach(function (o) { var q = lieuOp(o); (parQ[q] = parQ[q] || []).push(o); });
  Object.keys(parQ).forEach(function (id) {
    var l = parQ[id], p = ecranQ(id), w = l.length * (r * 2 + 3) - 3, x0 = p[0] - w / 2 + r;
    l.forEach(function (o, i) {
      var t = TYPES()[o.type], x = x0 + i * (r * 2 + 3), y = p[1] - r * 0.4;
      if (UI.selOp === o.id) { g.beginPath(); g.arc(x, y, r + 5, 0, Math.PI * 2); g.fillStyle = "rgba(242,201,76,.45)"; g.fill(); }
      pastille(g, x, y, r, t.couleur, t.lettre, o.dispo === "indisponible" ? "#4a1c1c" : null, false);
      if (o.dispo === "transit") { g.setLineDash([3, 3]); g.beginPath(); g.arc(x, y, r + 3, 0, Math.PI * 2); g.strokeStyle = "#fff"; g.lineWidth = 1.5; g.stroke(); g.setLineDash([]); }
      PLAN.hits.push({ x: x, y: y, r: r + 4, op: o.id });
    });
  });
}
function planDown(e) {
  PLAN.el.setPointerCapture(e.pointerId);
  PLAN.ptr[e.pointerId] = { x: e.clientX, y: e.clientY, x0: e.clientX, y0: e.clientY, t: Date.now() };
}
function planMove(e) {
  var p = PLAN.ptr[e.pointerId];
  if (!p) return;
  var ids = Object.keys(PLAN.ptr);
  if (ids.length === 2) {
    var a = PLAN.ptr[ids[0]], b = PLAN.ptr[ids[1]], d0 = Math.hypot(a.x - b.x, a.y - b.y);
    p.x = e.clientX; p.y = e.clientY;
    var d1 = Math.hypot(a.x - b.x, a.y - b.y);
    if (d0 > 0) zoomer(d1 / d0);
    return;
  }
  PLAN.cam.x -= (e.clientX - p.x) / PLAN.cam.z; PLAN.cam.y -= (e.clientY - p.y) / PLAN.cam.z;
  p.x = e.clientX; p.y = e.clientY;
  cancelAnimationFrame(PLAN.raf); PLAN.raf = requestAnimationFrame(planDessin);
}
function planUp(e) {
  var p = PLAN.ptr[e.pointerId];
  delete PLAN.ptr[e.pointerId];
  if (!p || Math.hypot(e.clientX - p.x0, e.clientY - p.y0) > 8 || Date.now() - p.t > 500) return;
  var rc = PLAN.el.getBoundingClientRect();
  toucherPlan(e.clientX - rc.left, e.clientY - rc.top);
}
/** Ce qui est sous le doigt : un opérateur (badge) ou un quartier (sol). */
function sousDoigt(sx, sy) {
  for (var i = 0; i < (PLAN.hits || []).length; i++) { var h = PLAN.hits[i]; if (Math.hypot(sx - h.x, sy - h.y) <= h.r) return { op: h.op }; }
  var cam = PLAN.cam, el = PLAN.el, ox = (sx - el.clientWidth / 2) / cam.z + cam.x, oy = (sy - el.clientHeight / 2) / cam.z + cam.y;
  var w = deproj(PLAN.cache.P, ox, oy);
  return { q: quartierEn(w[0], w[1]) };
}
