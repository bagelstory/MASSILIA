// CHOUF v2 — simulation de parties complètes sans navigateur, avec un joueur automatique.
// Usage : node tests/sim/sim.js [parties=100] [style=equilibre|prudent|agressif|passif]   (TRACE=1 : détail de la 1re partie)
// Vérifie : aucune erreur, indicateurs bornés, contrôle cohérent, sauvegarde relisible ; affiche la répartition des fins.
const fs = require("fs"), vm = require("vm"), path = require("path");
const ROOT = path.join(__dirname, "..", "..");
const html = fs.readFileSync(path.join(ROOT, "dist", "chouf.html"), "utf8");
const script = html.slice(html.indexOf("<script>") + 8, html.lastIndexOf("</script>"));
const N = +(process.argv[2] || 100), STYLE = process.argv[3] || "equilibre", TRACE = process.env.TRACE;

function monde() {
  const mem = {};
  const c = { window: {}, console, Math, JSON, Date, setTimeout() {}, setInterval() {} };
  c.window = c; c.window.__NOBOOT__ = true;
  c.window.localStorage = { getItem: k => mem[k] || null, setItem: (k, v) => { mem[k] = String(v); }, removeItem: k => { delete mem[k]; } };
  c.document = { querySelector: () => null, addEventListener() {}, createElement() { return {}; } };
  vm.createContext(c);
  vm.runInContext(script + ";this.A={VILLE,B,IDX,newGame,get G(){return G},validerCycle,previsions,affecter,recruter,peutRecruter,prendreCharbon,peutCharbon,accessible,opsActifs,lieuOp,activerFlux,fluxActifs,territoire,statut,advTotal,quetesActives,save,loadSave,charbonsJoueur,zoneOuverte};", c);
  return c.A;
}

/** Joueur automatique : simple et lisible, pas optimal. */
function jouer(A) {
  const G = A.G, V = A.VILLE, ops = A.opsActifs();
  const tenus = V.ordre.filter(id => G.q[id].ctl > 0);
  const front = V.ordre.filter(id => G.q[id].ctl < 85 && A.accessible(id) && id !== G.base);
  const reserve = STYLE === "prudent" ? 4000 : 1500;
  for (const id of tenus) G.q[id].ch.forEach((c, i) => { const p = A.peutCharbon(id, i); if (p.ok && G.tresorerie - p.cout > reserve) A.prendreCharbon(id, i); });
  const compte = t => ops.filter(o => o.type === t).length;
  const voulu = compte("pression") <= compte("force") ? "pression" : compte("reperage") < 1 ? "reperage" : compte("force") < 3 || ops.length < 6 ? "force" : compte("finances") < 1 ? "finances" : ops.length % 4 === 0 ? "intelligence" : "pression";
  const solde = A.previsions().solde;
  if (STYLE !== "passif" && A.peutRecruter(voulu).ok && G.tresorerie > A.B.operateurs.types[voulu].recrutement + reserve * 1.5 && solde > A.B.operateurs.types[voulu].entretien * 1.5) A.recruter(voulu);
  if (STYLE === "agressif" && G.tresorerie > 25000 && !A.fluxActifs().length) A.activerFlux("route_sud");
  const libres = A.opsActifs().filter(o => o.dispo !== "transit" && o.dispo !== "indisponible");
  const base = G.base, consolide = G.q[base].ctl >= 80;
  const score = id => G.q[id].ch.reduce((a, c) => a + c.rent, 0) / (1 + A.advTotal(G.q[id]) / 40);
  const cibles = front.sort((a, b) => score(b) - score(a));
  let pBase = false, np = 0, nf = 0;
  libres.forEach(o => {
    if (o.fatigue > 85) return A.affecter(o.id, o.pos, "repos");
    const chaud = G.ind.exposition > 80 || G.ind.pression > 65;     // profil bas
    if (chaud && STYLE !== "agressif" && (o.type === "force" || (o.type === "pression" && o.pos !== base))) return A.affecter(o.id, o.pos, "repos");
    let q = o.pos;
    const tache = A.B.operateurs.types[o.type].tache;
    // binômes : la n-ième Pression et la n-ième Force vont sur la n-ième meilleure cible
    if (o.type === "pression") { if (!consolide && !pBase) { q = base; pBase = true; } else q = cibles[np++ % Math.max(1, cibles.length)] || base; }
    else if (o.type === "force") q = !consolide ? base : (cibles[nf++ % Math.max(1, cibles.length)] || base);
    else if (o.type === "reperage") {
      const bord = V.ordre.filter(id => G.q[id].conn < 1 && A.accessible(id, "reperer"));
      q = bord[0] || tenus.filter(id => V.adj[id].some(v => G.q[v].conn < 2))[0] || o.pos;
      if (q !== o.pos && !A.accessible(q) && !A.accessible(q, "reperer")) q = o.pos;
      return A.affecter(o.id, q, "reperer");
    }
    else q = tenus.sort((a, b) => score(b) - score(a))[0] || o.pos;
    if (q !== o.pos && !A.accessible(q)) q = o.pos;
    A.affecter(o.id, q, tache);
  });
}

const st = { fins: {}, cycles: [], terr: [], tres: [], err: [], zones: [], quetes: [], courbe: {} };
const borne = (v, a, b) => typeof v === "number" && !isNaN(v) && v >= a && v <= b;
for (let n = 0; n < N; n++) {
  const A = monde();
  try {
    A.newGame(1000 + n);
    let garde = 0;
    while (!A.G.fini && garde++ < 400) {
      if (STYLE !== "passif") jouer(A);
      A.validerCycle();
      const G = A.G;
      for (const k of ["pression", "stabilite", "exposition"]) if (!borne(G.ind[k], 0, 100)) throw new Error(k + " hors bornes : " + G.ind[k]);
      for (const id of A.VILLE.ordre) { const q = G.q[id]; if (!borne(q.ctl, 0, 100.01) || q.ctl + A.advTotal(q) > 100.5) throw new Error("contrôle incohérent " + id + " " + q.ctl + "+" + A.advTotal(q)); }
      if (!borne(G.tresorerie, 0, 1e9)) throw new Error("trésorerie " + G.tresorerie);
      if (G.cycle % 10 === 0) (st.courbe[G.cycle] = st.courbe[G.cycle] || []).push(A.territoire());
      if (TRACE && n === 0 && G.cycle % 10 === 0) console.log("C" + G.cycle, "trés.", G.tresorerie, "terr.", A.territoire().toFixed(1), "P", Math.round(G.ind.pression), "E", Math.round(G.ind.exposition), "ops", A.opsActifs().length + "/" + G.capacite, "charbons", A.charbonsJoueur(), "zones", G.zonesOuvertes.length);
    }
    if (!A.G.fini) throw new Error("la partie ne finit pas");
    const f = A.G.fini;
    st.fins[f.type] = (st.fins[f.type] || 0) + 1; st.cycles.push(f.cycles); st.terr.push(f.territoire); st.tres.push(f.tresorerie);
    st.zones.push(A.G.zonesOuvertes.length); st.quetes.push(Object.keys(A.G.quetesFaites).length);
    A.save(); if (!A.loadSave()) throw new Error("sauvegarde illisible");
  } catch (e) { st.err.push("partie " + n + " : " + String(e.stack || e).split("\n").slice(0, 3).join(" | ")); }
}
const med = a => { const b = a.slice().sort((x, y) => x - y); return b.length ? b[Math.floor(b.length / 2)] : 0; };
console.log(`CHOUF v2 sim — ${N} parties, joueur « ${STYLE} »`);
console.log("fins : " + Object.entries(st.fins).map(([k, v]) => k + " " + v).join(", "));
console.log(`cycles médians ${med(st.cycles)} · territoire final médian ${med(st.terr)} % · trésorerie finale médiane ${med(st.tres)} € · zones ouvertes médianes ${med(st.zones)} · objectifs faits médians ${med(st.quetes)}`);
console.log("territoire médian par cycle : " + Object.keys(st.courbe).sort((a, b) => a - b).filter((c, i) => i % 2 === 0).slice(0, 13).map(c => "C" + c + " " + med(st.courbe[c]).toFixed(1)).join(" · "));
if (st.err.length) { console.log("ERREURS (" + st.err.length + ") :\n" + st.err.slice(0, 8).join("\n")); process.exit(1); }
console.log("OK");
