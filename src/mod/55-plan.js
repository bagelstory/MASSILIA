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
  var dx = x - 180, dy = y - 200;
  var rx = dx * P.c - dy * P.s, ry = dx * P.s + dy * P.c;
  return [(rx - P.minX) * P.R, (ry * P.k - (h || 0) * P.hk - P.minY) * P.R];
}
function deproj(P, ox, oy) {
  var rx = ox / P.R + P.minX, ry = (oy / P.R + P.minY) / P.k;
  return [rx * P.c + ry * P.s + 180, -rx * P.s + ry * P.c + 200];
}
function dansPoly(pt, poly) {
  var x = pt[0], y = pt[1], ins = false;
  for (var i = 0, j = poly.length - 1; i < poly.length; j = i++) {
    var xi = poly[i][0], yi = poly[i][1], xj = poly[j][0], yj = poly[j][1];
    if (((yi > y) !== (yj > y)) && x < (xj - xi) * (y - yi) / (yj - yi) + xi) ins = !ins;
  }
  return ins;
}
/** Hasard du décor, indépendant de G.seed (le décor ne doit pas changer la partie). */
function rngDecor(txt) {
  var h = 2166136261;
  for (var i = 0; i < txt.length; i++) h = Math.imul(h ^ txt.charCodeAt(i), 16777619);
  return function () { h = (h + 0x6D2B79F5) | 0; var t = Math.imul(h ^ (h >>> 15), h | 1); t ^= t + Math.imul(t ^ (t >>> 7), t | 61); return ((t ^ (t >>> 14)) >>> 0) / 4294967296; };
}
function hex(c) { return [parseInt(c.slice(1, 3), 16), parseInt(c.slice(3, 5), 16), parseInt(c.slice(5, 7), 16)]; }
function mix(a, b, t) { return [a[0] + (b[0] - a[0]) * t, a[1] + (b[1] - a[1]) * t, a[2] + (b[2] - a[2]) * t]; }
function rgb(c, f) { f = f == null ? 1 : f; return "rgb(" + Math.round(c[0] * f) + "," + Math.round(c[1] * f) + "," + Math.round(c[2] * f) + ")"; }

/** Couleur de l'équipe qui tient une zone (ou neutre / joueur). */
function couleurZone(id) {
  var c = G.zones[id].controle;
  if (c === "joueur") return B.carte.couleur_joueur;
  return c && IDX.equipes[c] ? IDX.equipes[c].couleur : B.carte.couleur_neutre;
}

/** Génère les immeubles d'une zone (déterministe). */
function immeubles(z) {
  var r = rngDecor(z.id), b = z.bati, s = b.pas, out = [];
  var xs = z.plan.map(function (p) { return p[0]; }), ys = z.plan.map(function (p) { return p[1]; });
  var dens = { centre: 0.92, aise: 0.8, tours: 0.42, mixte: 0.62, port: 0.55, maisons: 0.6 }[b.type] || 0.6;
  for (var x = Math.min.apply(0, xs); x < Math.max.apply(0, xs); x += s) {
    for (var y = Math.min.apply(0, ys); y < Math.max.apply(0, ys); y += s) {
      if (r() > dens) continue;
      var cx = x + s / 2 + (r() - 0.5) * s * 0.25, cy = y + s / 2 + (r() - 0.5) * s * 0.25;
      var w = s * (b.type === "tours" ? 0.42 + r() * 0.2 : 0.55 + r() * 0.25), d = s * (b.type === "tours" ? 0.42 + r() * 0.5 : 0.55 + r() * 0.25);
      var pts = [[cx - w / 2, cy - d / 2], [cx + w / 2, cy - d / 2], [cx + w / 2, cy + d / 2], [cx - w / 2, cy + d / 2]];
      if (!pts.every(function (p) { return dansPoly(p, z.plan); })) continue;
      var h = b.hmin + Math.pow(r(), b.type === "tours" ? 0.7 : 1.6) * (b.hmax - b.hmin);
      out.push({ pts: pts, h: h, cx: cx, cy: cy, z: z.id });
    }
  }
  return out;
}

/** Dessine toute la ville dans une image hors écran. Refait seulement si le contrôle des zones change. */
function planCache() {
  var sig = D.zones.liste.map(function (z) { return z.id + G.zones[z.id].controle + G.zones[z.id].connu; }).join("|");
  if (PLAN.cache && PLAN.sig === sig) return PLAN.cache;
  var P = planProj(), R = D.zones.repere;
  // bornes
  var bx = [1e9, -1e9, 1e9, -1e9];
  [[0, 0], [360, 0], [360, 400], [0, 400]].forEach(function (p) {
    var dx = p[0] - 180, dy = p[1] - 200, rx = dx * P.c - dy * P.s, ry = (dx * P.s + dy * P.c) * P.k;
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
  poly(R.terre, 0, "#2f3134");
  D.zones.liste.forEach(function (z) {
    var gz = G.zones[z.id], col = hex(couleurZone(z.id));
    var base = gz.connu === 0 ? [44, 46, 49] : mix([70, 72, 76], col, gz.controle ? 0.38 : 0);
    poly(z.plan, 0, rgb(base), "#9a9ea5", 1.6 * P.R);
  });
  poly(R.vieux_port, 0, "#1c2a33", "#4a4e55", 1);
  // immeubles, triés de l'arrière vers l'avant
  var all = [];
  D.zones.liste.forEach(function (z) { all = all.concat(immeubles(z)); });
  all.forEach(function (b) { b.prof = (b.cx - 180) * P.s + (b.cy - 200) * P.c; });
  all.sort(function (a, b) { return a.prof - b.prof; });
  var lum = [0.62, 0.48];
  all.forEach(function (b) {
    var gz = G.zones[b.z], col = hex(couleurZone(b.z));
    var c = gz.connu === 0 ? [70, 72, 76] : mix([196, 198, 202], col, gz.controle ? 0.45 : 0);
    if (gz.controle === "joueur") c = mix(c, col, 0.3);
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
    if (b.h > 12 && gz.connu > 0) {
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
function badge(g, x, y, r, anneau, type, fond) {
  g.beginPath(); g.arc(x, y + 1.5, r + 2, 0, Math.PI * 2); g.fillStyle = "rgba(0,0,0,.45)"; g.fill();
  g.beginPath(); g.arc(x, y, r, 0, Math.PI * 2); g.fillStyle = fond || "#1b1c1f"; g.fill();
  g.lineWidth = r * 0.2; g.strokeStyle = anneau; g.stroke();
  glyphe(g, type, x, y, r);
}
/** Badges d'une zone : équipe qui la tient, chez toi, police forte, tension forte. */
function badgesZone(id) {
  var gz = G.zones[id], l = [];
  if (gz.connu === 0) return [{ t: "brouillard", c: "#6b6e73" }];
  if (id === G.protagoniste.zone_origine) l.push({ t: "maison", c: B.carte.couleur_joueur });
  if (gz.controle && IDX.equipes[gz.controle]) l.push({ t: IDX.equipes[gz.controle].embleme, c: IDX.equipes[gz.controle].couleur, f: IDX.equipes[gz.controle].couleur });
  if (gz.police >= 60) l.push({ t: "police", c: "#3d7fd9" });
  if (gz.tension >= 60) l.push({ t: "tension", c: "#e5484d" });
  return l;
}

/* ---------- affichage et gestes ---------- */
function planMonter(el) {
  PLAN.el = el;
  var C = planCache(), dpr = window.devicePixelRatio || 1;
  var W = el.clientWidth, H = el.clientHeight;
  el.width = W * dpr; el.height = H * dpr;
  if (!PLAN.cam) {
    var z0 = D.zones.liste.filter(function (z) { return z.id === G.protagoniste.zone_origine; })[0];
    var c = centrePlan(z0), q = proj(C.P, c[0], c[1], 0);
    PLAN.cam = { x: q[0], y: q[1] + 40, z: Math.max(W / C.W * 2.3, 0.3) };
  }
  if (!el.dataset.ok) {
    el.dataset.ok = "1";
    el.addEventListener("pointerdown", planDown);
    el.addEventListener("pointermove", planMove);
    el.addEventListener("pointerup", planUp);
    el.addEventListener("pointercancel", planUp);
    el.addEventListener("wheel", function (e) { e.preventDefault(); zoomer(e.deltaY < 0 ? 1.12 : 1 / 1.12); }, { passive: false });
  }
  planDessin();
}
function centrePlan(z) {
  var sx = 0, sy = 0;
  z.plan.forEach(function (p) { sx += p[0]; sy += p[1]; });
  return [sx / z.plan.length, sy / z.plan.length];
}
function versEcran(ox, oy) {
  var el = PLAN.el, cam = PLAN.cam;
  return [(ox - cam.x) * cam.z + el.clientWidth / 2, (oy - cam.y) * cam.z + el.clientHeight / 2];
}
function zoomer(f) {
  var C = PLAN.cache;
  PLAN.cam.z = clamp(PLAN.cam.z * f, PLAN.el.clientWidth / C.W * 0.9, 1.6);
  planDessin();
}
function planDessin() {
  var el = PLAN.el;
  if (!el || !el.isConnected) return;
  var g = el.getContext("2d"), dpr = window.devicePixelRatio || 1, C = PLAN.cache, cam = PLAN.cam;
  var W = el.clientWidth, H = el.clientHeight;
  // bornes de la caméra
  cam.x = clamp(cam.x, 0, C.W); cam.y = clamp(cam.y, 0, C.H);
  g.setTransform(dpr, 0, 0, dpr, 0, 0);
  g.fillStyle = "#0d151b"; g.fillRect(0, 0, W, H);
  g.imageSmoothingEnabled = true;
  g.drawImage(C.cv, W / 2 - cam.x * cam.z, H / 2 - cam.y * cam.z, C.W * cam.z, C.H * cam.z);
  // zone choisie : contour lumineux
  var sel = UI.sel ? IDX.zones[UI.sel] : null;
  if (sel) {
    g.beginPath();
    sel.plan.forEach(function (p, i) { var q = proj(C.P, p[0], p[1], 0), s = versEcran(q[0], q[1]); i ? g.lineTo(s[0], s[1]) : g.moveTo(s[0], s[1]); });
    g.closePath(); g.lineWidth = 3; g.strokeStyle = "#f2c94c"; g.shadowColor = "#f2c94c"; g.shadowBlur = 12; g.stroke(); g.shadowBlur = 0;
  }
  // badges et noms
  PLAN.hits = [];
  var r = clamp(11 * Math.sqrt(cam.z / 0.5), 9, 16);
  D.zones.liste.forEach(function (z) {
    var c = centrePlan(z), q = proj(C.P, c[0], c[1], 0), s = versEcran(q[0], q[1]);
    var l = badgesZone(z.id), w = l.length * (r * 2 + 4) - 4, x0 = s[0] - w / 2 + r, y = s[1] - r * 2.2;
    l.forEach(function (b, i) { badge(g, x0 + i * (r * 2 + 4), y, r, b.c, b.t, b.f); PLAN.hits.push({ x: x0 + i * (r * 2 + 4), y: y, r: r + 4, z: z.id }); });
    g.font = "600 " + Math.round(r * 1.05) + "px system-ui"; g.textAlign = "center"; g.textBaseline = "top";
    g.lineWidth = 3; g.strokeStyle = "rgba(0,0,0,.8)"; g.strokeText(z.court, s[0], s[1] - r * 0.6);
    g.fillStyle = G.zones[z.id].connu ? "#f4f1ea" : "#8d9096"; g.fillText(z.court, s[0], s[1] - r * 0.6);
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
    var a = PLAN.ptr[ids[0]], b = PLAN.ptr[ids[1]];
    var d0 = Math.hypot(a.x - b.x, a.y - b.y);
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
  if (!p || Math.hypot(e.clientX - p.x0, e.clientY - p.y0) > 8 || Date.now() - p.t > 450) return;
  var rc = PLAN.el.getBoundingClientRect(), sx = e.clientX - rc.left, sy = e.clientY - rc.top;
  var id = zoneSous(sx, sy);
  UI.sel = id || null;
  render();
}
/** Zone sous un point de l'écran (badges d'abord, puis sol). */
function zoneSous(sx, sy) {
  for (var i = 0; i < (PLAN.hits || []).length; i++) { var h = PLAN.hits[i]; if (Math.hypot(sx - h.x, sy - h.y) <= h.r) return h.z; }
  var cam = PLAN.cam, el = PLAN.el, ox = (sx - el.clientWidth / 2) / cam.z + cam.x, oy = (sy - el.clientHeight / 2) / cam.z + cam.y;
  var w = deproj(PLAN.cache.P, ox, oy);
  var z = D.zones.liste.filter(function (z) { return dansPoly(w, z.plan); })[0];
  return z ? z.id : null;
}
/** Part de la ville tenue par chaque équipe (pondérée par la population). */
function partsVille() {
  var t = {}, tot = 0;
  D.zones.liste.forEach(function (z) { var c = G.zones[z.id].controle || "aucun"; t[c] = (t[c] || 0) + z.population; tot += z.population; });
  return Object.keys(t).map(function (k) { return { id: k, p: t[k] / tot }; }).sort(function (a, b) { return b.p - a.p; });
}
