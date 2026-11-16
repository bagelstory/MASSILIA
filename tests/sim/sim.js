// CHOUF — simulation de vies complètes sans navigateur (même principe que tests/sim/sim.js de KICK).
// Usage : node tests/sim/sim.js [nombre_de_vies=200] [style=hasard|prudent|hall]
// Sortie : statistiques + erreurs. Code de sortie 1 si une vie plante ou viole un invariant.
const fs = require("fs"), vm = require("vm"), path = require("path");
const ROOT = path.join(__dirname, "..", "..");
const html = fs.readFileSync(path.join(ROOT, "dist", "chouf.html"), "utf8");
const script = html.slice(html.indexOf("<script>") + 8, html.lastIndexOf("</script>"));
const N = +(process.argv[2] || 200), STYLE = process.argv[3] || "hasard";

function nouveauMonde() {
  const store = {};
  const ctx = { window: {}, console, Math, JSON, Date, setTimeout() {}, setInterval() {} };
  ctx.window = ctx;
  ctx.window.__NOBOOT__ = true;
  ctx.window.localStorage = { getItem: k => store[k] || null, setItem: (k, v) => { store[k] = String(v); }, removeItem: k => { delete store[k]; } };
  ctx.document = { querySelector: () => null, addEventListener() {} };
  vm.createContext(ctx);
  vm.runInContext(script + "\n;this.__api={get G(){return G},newGame,cloreSemaine,faireMission,missionsDispo,choisir,optionsCarte,tenirEngagement,engagementsOuverts,finCampagne,age,CLOCK,syncTemps,IDX,B,save,loadSave,palier};", ctx);
  return ctx.__api;
}

const stats = { vies: 0, semaines: 0, erreurs: [], cartes: {}, promessesTenues: 0, promessesRompues: 0, paliers: {}, argent: [], stress: [], faits: {} };
const borne = (v, lo, hi) => typeof v === "number" && !isNaN(v) && v >= lo && v <= hi;

for (let v = 0; v < N; v++) {
  const A = nouveauMonde();
  let t = 1763300000000;
  A.CLOCK.now = () => t;
  try {
    A.newGame("Test", 1000 + v);
    let garde = 0, r = (() => { let s = 77 + v; return () => ((s = (s * 1103515245 + 12345) & 0x7fffffff) / 0x7fffffff); })();
    while (!A.finCampagne() && garde++ < 400) {
      const G = A.G;
      // décisions en attente
      let k = 0;
      while (G.carte && k++ < 10) {
        stats.cartes[G.carte.id] = (stats.cartes[G.carte.id] || 0) + 1;
        const opts = A.optionsCarte(G.carte);
        if (!opts.length) throw new Error("carte sans option visible : " + G.carte.id);
        let choix = opts[Math.floor(r() * opts.length)];
        if (STYLE === "prudent") choix = opts[0];
        if (STYLE === "hall") choix = opts.find(o => /Tof|Zépek|service|Prévenir/.test(o.o.t)) || opts[opts.length - 1];
        A.choisir(choix.i);
      }
      // promesses
      A.engagementsOuverts().forEach((e, i) => { if (STYLE !== "hall" && r() < 0.6) A.tenirEngagement(0); });
      // missions tant qu'il y a de l'énergie
      let essais = 0;
      while (essais++ < 8) {
        const l = A.missionsDispo().filter(m => (m.cout.energie || 0) <= G.temps.energie && (m.cout.sangfroid || 0) <= G.temps.sangfroid);
        if (!l.length || r() < 0.25) break;
        let m = l[Math.floor(r() * l.length)];
        if (STYLE === "hall") m = l.find(x => x.id === "service_tof") || m;
        if (STYLE === "prudent") m = l.find(x => x.id === "reviser") || l.find(x => x.id !== "service_tof") || m;
        A.faireMission(m.id);
      }
      t += 20 * 60000;                          // vingt minutes réelles entre deux semaines
      A.syncTemps();
      A.cloreSemaine();
      stats.semaines++;
      // invariants
      const P = G.protagoniste;
      for (const c in P.competences) if (!borne(P.competences[c], 0, 100)) throw new Error("compétence hors bornes " + c + "=" + P.competences[c]);
      for (const e in P.etat) if (!borne(P.etat[e], 0, 100)) throw new Error("état hors bornes " + e);
      for (const id in G.relations) for (const f of ["confiance", "proximite", "tension"]) if (!borne(G.relations[id][f], 0, 100)) throw new Error("relation hors bornes " + id + "." + f);
      if (!borne(G.temps.energie, 0, A.B.temps.energie_max)) throw new Error("énergie hors bornes");
      if (!A.IDX.chapitres[G.chapitre]) throw new Error("chapitre inconnu");
    }
    if (garde >= 400) throw new Error("la vie ne se termine pas");
    const G = A.G;
    stats.vies++;
    G.protagoniste.engagements.forEach(e => e.tenu ? stats.promessesTenues++ : e.tenu === false && stats.promessesRompues++);
    for (const c in G.protagoniste.competences) {
      const p = A.palier(G.protagoniste.competences[c]);
      (stats.paliers[c] = stats.paliers[c] || [0, 0, 0, 0])[p - 1]++;
    }
    for (const f in G.evenements.faits) stats.faits[f] = (stats.faits[f] || 0) + 1;
    stats.argent.push(G.argent.propre); stats.stress.push(G.protagoniste.etat.stress);
    // sauvegarde relisible
    A.save(); if (!A.loadSave()) throw new Error("sauvegarde illisible");
  } catch (e) {
    stats.erreurs.push("vie " + v + " : " + (e.stack || e).toString().split("\n").slice(0, 3).join(" | "));
  }
}

const moy = a => a.length ? Math.round(a.reduce((x, y) => x + y, 0) / a.length) : 0;
console.log(`CHOUF sim — ${stats.vies}/${N} vies terminées (style ${STYLE}), ${Math.round(stats.semaines / Math.max(1, N))} semaines par vie`);
console.log(`argent propre moyen à la fin : ${moy(stats.argent)} € · stress moyen : ${moy(stats.stress)}/100`);
console.log(`promesses tenues ${stats.promessesTenues} · rompues ${stats.promessesRompues}`);
console.log("paliers de compétence en fin de vie (P1/P2/P3/P4) :");
for (const c in stats.paliers) console.log("  " + c.padEnd(14) + stats.paliers[c].join(" / "));
console.log("cartes vues (par vie) : " + Object.entries(stats.cartes).sort((a, b) => b[1] - a[1]).map(([k, n]) => k + " " + (n / N).toFixed(1)).join(", "));
const jamais = Object.keys(require(path.join(ROOT, "data", "evenements.json")).liste.reduce((o, e) => (o[e.id] = 1, o), {})).filter(id => !stats.cartes[id]);
if (jamais.length) console.log("jamais vues : " + jamais.join(", "));
if (stats.erreurs.length) { console.log("ERREURS (" + stats.erreurs.length + ") :\n" + stats.erreurs.slice(0, 10).join("\n")); process.exit(1); }
console.log("OK");
