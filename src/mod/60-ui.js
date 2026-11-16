/* CHOUF v2 — 60-ui : écrans. CARTE par défaut ; OBJECTIFS, ÉQUIPE (opérateurs), BORD (indicateurs, prévisions,
   flux internationaux), FACTIONS, JOURNAL. Six indicateurs toujours visibles en haut, bouton unique
   « Valider le cycle » en bas. Notifications d'une ligne. Aucune règle de jeu ici. */

var TAB = "carte", UI = { selOp: null, selQ: null, taches: false, rapport: null, toast: null, sousBord: null };
var $ = function (s) { return document.querySelector(s); };
function esc(s) { return String(s == null ? "" : s).replace(/[&<>"]/g, function (c) { return { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;" }[c]; }); }
function eur(n) { return Math.round(n).toLocaleString("fr-FR") + " €"; }
function sgn(n, u) { n = Math.round(n * 10) / 10; return (n > 0 ? "+" : "") + n + (u || ""); }
function jauge(v, cls) { return '<div class="jauge ' + (cls || "") + '"><i style="width:' + clamp(v, 0, 100) + '%"></i></div>'; }

var ASSET = {
  u: function (k) { var a = window.ASSETS || {}; return a[k] || null; },
  img: function (k, cls) { var u = ASSET.u(k); return u ? '<img class="' + (cls || "") + '" src="' + u + '" alt="">' : '<div class="' + (cls || "") + ' ph"></div>'; }
};
function ico(k, repli) { var u = ASSET.u("icone_" + k); return u ? '<img class="ico" src="' + u + '" alt="">' : (repli || ""); }
function toast(t) { UI.toast = t; render(); setTimeout(function () { if (UI.toast === t) { UI.toast = null; render(); } }, 2400); }
function go(t) { TAB = t; UI.taches = false; UI.sousBord = null; render(); }

/* ---------------- indicateurs ---------------- */
function indicateurs() {
  var d = G.dernier || {}, ind = G.ind;
  return [
    { k: "territoire", n: "Territoire", v: Math.round(territoire()) + " %", d: d.d_territoire, i: "humeur", e: "Part de la ville que tu tiens. Victoire à " + B.fin.victoire_territoire + " %." },
    { k: "tresorerie", n: "Trésorerie", v: eur(G.tresorerie), d: d.solde, i: "propre", e: "À zéro " + B.economie.faillite_cycles + " cycles de suite : faillite." + (G.dette ? " Dette : " + eur(G.dette) + "." : "") },
    { k: "effectif", n: "Effectif", v: opsActifs().filter(function (o) { return o.dispo !== "indisponible"; }).length + "/" + opsActifs().length + " · " + G.capacite, i: "respect", e: "Opérateurs disponibles / total · capacité." },
    { k: "pression", n: "Pression", v: Math.round(ind.pression), d: d.d_pression, i: "police", alerte: ind.pression > B.indicateurs.pression_alerte, inv: true, e: "Surveillance. Au-dessus de " + B.indicateurs.pression_alerte + " : opérations. À 100 : chute." },
    { k: "stabilite", n: "Stabilité", v: Math.round(ind.stabilite), d: d.d_stabilite, i: "sangfroid", alerte: ind.stabilite < B.indicateurs.stabilite_basse, e: "Cohérence du réseau. Sous " + B.indicateurs.stabilite_basse + " : tout s'use plus vite." },
    { k: "exposition", n: "Exposition", v: Math.round(ind.exposition), d: d.d_exposition, i: "tension", alerte: ind.exposition > B.indicateurs.exposition_alerte, inv: true, e: "Risque cumulé. Au-dessus de " + B.indicateurs.exposition_alerte + " : la pression monte seule." }
  ];
}
function vBandeau() {
  return '<header class="bandeau"><div class="cyc"><b>Cycle ' + G.cycle + "</b><span>" + moisNom() + " " + annee() + "</span></div><div class=\"chips\">" +
    indicateurs().map(function (x) {
      var bon = x.d > 0 !== !!x.inv;
      return '<button class="chip' + (x.alerte ? " al" : "") + '" data-ind="' + x.k + '">' + ico(x.i) + "<span><small>" + x.n + "</small><b>" + esc(x.v) + "</b>" +
        (x.d ? '<em class="' + (bon ? "up" : "dn") + '">' + sgn(x.d) + "</em>" : "") + "</span></button>";
    }).join("") + "</div></header>";
}
function vPied() {
  var nq = quetesActives().filter(function (q) { return !q.fini; }).length;
  var o = [["carte", "Carte"], ["objectifs", "Objectifs"], ["equipe", "Équipe"], ["bord", "Bord"], ["factions", "Factions"], ["journal", "Journal"]];
  return '<div class="pied"><button class="btn valider" data-a="valider">Valider le cycle ' + G.cycle + '</button><nav class="onglets">' +
    o.map(function (x) { return '<button data-tab="' + x[0] + '" class="' + (TAB === x[0] ? "on" : "") + '">' + x[1] + (x[0] === "objectifs" && nq ? " <i class=\"nb\">" + nq + "</i>" : "") + "</button>"; }).join("") + "</nav></div>";
}

function render() {
  var app = $("#app");
  if (!G) { app.innerHTML = vAccueil(); return; }
  if (G.fini) { app.innerHTML = vFin(); return; }
  var ecr = { carte: vCarte, objectifs: vObjectifs, equipe: vEquipe, bord: UI.sousBord === "flux" ? vFlux : vBord, factions: vFactions, journal: vJournal }[TAB]();
  app.innerHTML = vBandeau() + '<main class="corps ' + TAB + '">' + ecr + "</main>" + vPied() +
    (UI.taches ? vTaches() : "") + (UI.rapport ? vRapport() : "") + (UI.toast ? '<div class="toast">' + esc(UI.toast) + "</div>" : "");
  var pl = $("#plan");
  if (pl) planMonter(pl);
}

function vAccueil() {
  return '<div class="accueil">' + (ASSET.u("vue_centre") ? '<img class="hero" src="' + ASSET.u("vue_centre") + '" alt="">' : "") +
    '<h1 class="titre">CHOUF</h1><p class="sous">Marseille · 111 quartiers</p>' +
    '<p class="avert">Jeu de stratégie pour adultes. Vrais quartiers, factions inventées. Tu diriges un réseau depuis la carte : opérateurs, charbons, argent, pression.</p>' +
    (hasSave() ? '<button class="btn" data-a="continuer">Continuer</button>' : "") +
    '<button class="btn ' + (hasSave() ? "sec" : "") + '" data-a="nouvelle">Nouvelle partie</button>' +
    '<p class="source">Contours des quartiers : data.gouv.fr, « Quartiers de Marseille » (Licence ouverte).</p></div>';
}

/* ---------------- CARTE ---------------- */
function vCarte() {
  var h = '<div class="vue-ville"><canvas id="plan"></canvas><div class="hud-zoom"><button data-a="zplus">+</button><button data-a="zmoins">−</button></div>';
  var op = UI.selOp ? opParId(UI.selOp) : null;
  if (op && !UI.taches) {
    h += '<div class="hud-bas"><div class="hb-l1"><b>' + esc(nomOp(op)) + "</b><span>" + esc(TYPES()[op.type].nom) + " · " + esc(VILLE.q[lieuOp(op)].nom) + "</span></div>" +
      '<p class="hb-txt">Touche un quartier entouré de vert pour lui donner une tâche.</p><div class="hb-btns"><button class="btn sec" data-a="annuler">Annuler</button></div></div>';
  } else if (UI.selQ && !UI.taches) h += vFicheQ(UI.selQ);
  else if (!UI.taches) h += '<div class="hud-bas petit-hb">Touche un opérateur (rond de couleur), puis un quartier. Touche un quartier pour sa fiche. Zoome pour voir les charbons.</div>';
  return h + "</div>";
}
var ETAT_CH = function (c) { return c.saisi ? "saisi (" + c.saisi + ")" : c.proprio === "joueur" ? (c.ouvert ? "à toi" : "fermé") : c.proprio ? IDX.equipes[c.proprio].nom : "libre"; };
function vFicheQ(id) {
  var q = VILLE.q[id], s = G.q[id], st = statut(id), z = IDX.zones[q.zone], ouv = zoneOuverte(q.zone);
  var h = '<div class="hud-bas fiche-q">' + ASSET.img("banniere_" + q.zone, "fq-ban") + '<div class="hb-l1"><b>' + esc(q.nom) + "</b><span>" + q.arr + "e arr. · " + esc(z.court) + "</span></div>";
  if (!ouv) h += '<p class="hb-txt verrou">🔒 Zone verrouillée. Va dans Objectifs pour l\'ouvrir.</p>';
  if (s.conn === 0 && s.ctl <= 0) return h + '<p class="hb-txt">Inconnu. Un opérateur Repérage dans un quartier voisin le fera apparaître.</p><div class="hb-btns"><button class="btn sec" data-a="fermer">Fermer</button></div></div>';
  var dom = factionDominante(s);
  h += '<div class="hb-ctrl"><b style="background:' + (couleurQuartier(id) || "#555") + '"></b>' + STATUT_NOM[st] + (st === "faction" && dom ? " : " + esc(IDX.equipes[dom].nom) : "") + "</div>";
  h += '<div class="barre-ctl"><i class="moi" style="width:' + s.ctl + '%"></i>' + (s.conn >= 2 ? Object.keys(s.adv).filter(function (f) { return s.adv[f] > 0.5; }).map(function (f) {
    return '<i style="width:' + s.adv[f] + '%;background:' + IDX.equipes[f].couleur + '"></i>';
  }).join("") : '<i style="width:' + advTotal(s) + '%;background:#9a5050"></i>') + "</div>";
  h += '<div class="hb-stats"><span>Contrôle <b>' + Math.round(s.ctl) + " %</b></span><span>Adverse <b>" + (s.conn >= 1 ? Math.round(advTotal(s)) + " %" : "?") + "</b></span><span>Maintien <b>" + eur(s.maintien) + "</b></span></div>";
  if (s.conn >= 2 || s.ctl > 0) {
    h += '<div class="charbons">' + (s.ch.length ? s.ch.map(function (c, i) {
      var p = peutCharbon(id, i), act = c.proprio === "joueur" ? '<button class="mini" data-fermerch="' + i + '">' + (c.ouvert ? "Fermer" : "Rouvrir") + "</button>"
        : p.ok ? '<button class="mini or" data-ch="' + i + '">' + (p.reprise ? "Reprendre " : "Ouvrir ") + eur(p.cout) + "</button>" : '<span class="petit">' + esc(p.raison) + "</span>";
      return '<div class="ch"><span><b>' + esc(c.nom) + "</b><small>" + eur(c.rent) + "/cycle · risque " + "▲".repeat(c.risque) + " · " + esc(ETAT_CH(c)) + "</small></span>" + act + "</div>";
    }).join("") : '<p class="petit">Aucun charbon ici.</p>') + "</div>";
  } else h += '<p class="petit">Charbons : envoie un Repérage pour les voir (connaissance ' + s.conn + "/3).</p>";
  var ici = opsActifs().filter(function (o) { return lieuOp(o) === id; });
  if (ici.length) h += '<p class="petit">Sur place : ' + ici.map(function (o) { return esc(o.id) + " " + esc(B.taches[o.tache].nom.toLowerCase()); }).join(" · ") + "</p>";
  h += '<div class="hb-btns">' + (s.ctl > 0 ? '<button class="btn sec" data-a="investir">Investir ' + eur(coutInvestir(id)) + "</button>" : "") + '<button class="btn sec" data-a="fermer">Fermer</button></div></div>';
  return h;
}
function vTaches() {
  var o = opParId(UI.selOp), id = UI.selQ, tr = dureeTransit(o.pos, id);
  var l = Object.keys(B.taches).map(function (k) {
    var t = B.taches[k], e = efficacite(o, k), spe = t.type === o.type;
    return '<button class="ligne-action' + (spe ? " spe" : "") + '" data-tache="' + k + '"><span><b>' + esc(t.nom) + (spe ? " ★" : "") + "</b><small>" +
      (k === "repos" ? "fatigue −" + B.operateurs.fatigue_repos : "efficacité " + Math.round(e * 100) + " % · fatigue +" + t.fatigue) + "</small></span><em>" + (tr ? tr + " cycle" + (tr > 1 ? "s" : "") + " de trajet" : "sur place") + "</em></button>";
  }).join("");
  return '<div class="voile"><div class="sheet"><h2>' + esc(o.id) + " → " + esc(VILLE.q[id].nom) + '</h2><p class="petit">' + esc(TYPES()[o.type].nom) + " · compétence " + o.comp + " · fatigue " + Math.round(o.fatigue) +
    "</p>" + l + '<button class="btn sec" data-a="fermertaches">Annuler</button></div></div>';
}

/* ---------------- OBJECTIFS ---------------- */
function vObjectifs() {
  var l = quetesActives();
  function bloc(q) {
    var r = q.recompense || {}, rec = [];
    if (q.zone) rec.push("ouvre la zone");
    if (r.argent) rec.push("+" + eur(r.argent));
    if (r.capacite) rec.push("capacité +" + r.capacite);
    if (r.capacite_toutes_les) rec.push("capacité +1 toutes les " + r.capacite_toutes_les + " zones");
    return '<div class="carte-bloc quete' + (q.zone ? " zq" : "") + '">' + (q.zone ? ASSET.img("banniere_" + q.zone, "q-ban") : "") + '<div class="ligne"><b>' + esc(q.nom) + "</b></div>" +
      q.avance.map(function (a) {
        return '<div class="obj' + (a.ok ? " ok" : "") + '"><span>' + (a.ok ? "✔ " : "○ ") + esc(a.txt) + "</span><b>" + Math.min(a.v, a.but).toLocaleString("fr-FR") + " / " + a.but.toLocaleString("fr-FR") + "</b></div>" +
          jauge(a.but ? Math.min(100, a.v / a.but * 100) : 0, "q");
      }).join("") + '<p class="petit">Récompense : ' + esc(rec.join(" · ")) + "</p></div>";
  }
  var zq = l.filter(function (q) { return q.zone; }), aut = l.filter(function (q) { return !q.zone; });
  return '<h2 class="h">Objectifs</h2><p class="petit">Zones ouvertes : ' + G.zonesOuvertes.map(function (z) { return esc(IDX.zones[z].court); }).join(", ") + ".</p>" +
    '<h3 class="h3">Ouvrir une zone</h3>' + (zq.length ? zq.map(bloc).join("") : '<p class="petit">Toute la ville est ouverte.</p>') +
    '<h3 class="h3">Réseau</h3>' + (aut.length ? aut.map(bloc).join("") : '<p class="petit">Tout est fait.</p>');
}

/* ---------------- ÉQUIPE ---------------- */
function vEquipe() {
  var l = opsActifs();
  var h = '<h2 class="h">Opérateurs <small>' + l.length + "/" + G.capacite + "</small></h2>" + l.map(function (o) {
    var t = TYPES()[o.type], v = o.visage ? "medaillon_" + o.visage : null;
    return '<button class="op" data-op="' + o.id + '">' + (v && ASSET.u(v) ? ASSET.img(v, "med") : '<span class="med lettre" style="border-color:' + t.couleur + '">' + esc(t.lettre) + "</span>") +
      "<span><b>" + esc(nomOp(o)) + "</b><small>" + esc(t.nom) + " · " + "★".repeat(o.comp) + "☆".repeat(5 - o.comp) + "</small><small>" +
      esc({ disponible: "disponible", transit: "en trajet (" + o.transit + ")", occupe: "occupé", indisponible: "épuisé" }[o.dispo] || o.dispo) + " · " + esc(VILLE.q[lieuOp(o)].nom) + " · " + esc(B.taches[o.tache].nom) + "</small>" +
      jauge(o.fatigue, "fat") + "</span></button>";
  }).join("");
  h += '<h3 class="h3">Recruter</h3>' + Object.keys(TYPES()).map(function (k) {
    var t = TYPES()[k], r = peutRecruter(k);
    return '<button class="ligne-action' + (r.ok ? "" : " off") + '" data-recruter="' + k + '"><span><b><i class="pt" style="background:' + t.couleur + '"></i> ' + esc(t.nom) + "</b><small>" +
      esc(B.taches[t.tache].nom) + " · entretien " + eur(t.entretien) + "/cycle" + (r.ok ? "" : " · " + r.raison) + "</small></span><em>" + eur(t.recrutement) + "</em></button>";
  }).join("");
  return h;
}

/* ---------------- BORD ---------------- */
function vBord() {
  var p = previsions(), al = [];
  if (G.ind.pression > B.indicateurs.pression_alerte) al.push("Pression critique : opérations et arrestations possibles.");
  if (G.ind.exposition > B.indicateurs.exposition_alerte) al.push("Exposition trop haute.");
  if (G.tresorerie + p.solde < 0) al.push("Le prochain cycle te met à zéro.");
  if (G.cycles_zero) al.push("Trésorerie à zéro depuis " + G.cycles_zero + " cycle(s).");
  opsActifs().forEach(function (o) { if (o.fatigue >= 80) al.push(o.id + " presque épuisé."); });
  Object.keys(G.alertes || {}).forEach(function (id) { al.push("Attaque subie : " + VILLE.q[id].nom + "."); });
  return '<h2 class="h">Tableau de bord</h2>' + indicateurs().map(function (x) {
    return '<div class="indic"><div class="ligne"><span>' + ico(x.i) + " " + x.n + "</span><b>" + esc(x.v) + "</b></div>" + '<p class="petit">' + esc(x.e) + "</p></div>";
  }).join("") +
    '<h3 class="h3">Prévision du cycle ' + G.cycle + "</h3>" + [["Charbons (" + charbonsJoueur() + ")", p.revenus], ["Flux internationaux (attendu)", p.flux_revenus - p.flux_couts], ["Maintien des quartiers", -p.maintien], ["Entretien des opérateurs", -p.entretien], ["Intérêts de la dette", -p.interets]]
      .map(function (x) { return '<div class="ligne-chiffre"><span>' + esc(x[0]) + "</span><b class=\"" + (x[1] < 0 ? "neg" : "") + '">' + sgn(x[1]) + " €</b></div>"; }).join("") +
    '<div class="ligne-chiffre tot"><span>Solde prévu</span><b>' + sgn(p.solde) + " €</b></div>" +
    '<button class="ligne-action" data-a="voirflux"><span><b>Flux internationaux</b><small>' + fluxActifs().length + " actif(s) sur " + B.flux.offres.length + "</small></span><em>›</em></button>" +
    '<h3 class="h3">Alertes</h3>' + (al.length ? '<ul class="alertes">' + al.map(function (a) { return "<li>" + esc(a) + "</li>"; }).join("") + "</ul>" : '<p class="petit">Rien d\'urgent.</p>') +
    '<button class="btn sec" data-a="abandon">Abandonner la partie</button>';
}
function vFlux() {
  return '<button class="retour" data-a="retourbord">‹ Bord</button><h2 class="h">Flux internationaux</h2><p class="petit">Engagements abstraits avec des partenaires extérieurs. Plus de volume : plus de revenus, plus d\'exposition.</p>' +
    B.flux.offres.map(function (o) {
      var f = G.flux.filter(function (x) { return x.id === o.id && x.actif; })[0], v = f || o;
      return '<div class="carte-bloc"><div class="ligne"><b>' + esc(o.nom) + "</b><span>" + (f ? '<i class="ok">actif · ' + f.cycles + " cycles</i>" : "inactif") + "</span></div>" +
        '<div class="grille">' + [["Volume", o.volume + "/5"], ["Revenu", eur(o.revenu) + "/cycle"], ["Coût", eur(o.cout) + "/cycle"], ["Engagement", eur(f ? f.engage : o.engagement)],
          ["Fiabilité", Math.round(v.fiabilite) + " %"], ["Exposition", Math.round(v.exposition)], ["Instabilité", Math.round(v.instabilite)], ["Activation", eur(o.activation)]]
          .map(function (x) { return "<span><small>" + x[0] + "</small><b>" + x[1] + "</b></span>"; }).join("") + "</div>" +
        (f ? '<button class="btn sec" data-couper="' + o.id + '">Couper (perd ' + eur(f.engage) + ")</button>" : '<button class="btn" data-activer="' + o.id + '">Activer · ' + eur(o.activation + o.engagement) + "</button>") + "</div>";
    }).join("") + '<p class="petit">Un opérateur Logistique sur « Gérer les flux » améliore la fiabilité. Plus de ' + B.flux.trop_de_flux + " flux à la fois : la stabilité baisse.</p>";
}

/* ---------------- FACTIONS ---------------- */
var OBJ = { contre: "Contre-attaquer", retraite: "Se rétracter", etendre: "Étendre", maintenir: "Maintenir" };
function vFactions() {
  var renseigne = opsActifs().some(function (o) { return o.tache === "surveiller" && o.dispo === "occupe"; });
  return '<h2 class="h">Factions</h2>' + D.equipes.liste.map(function (e) {
    var f = G.factions[e.id], t = territoireFaction(e.id), al = G.alliances.filter(function (x) { return x.a === e.id || x.b === e.id; })[0];
    var nch = 0; VILLE.ordre.forEach(function (id) { G.q[id].ch.forEach(function (c) { if (c.proprio === e.id) nch++; }); });
    return '<div class="carte-bloc faction' + (f.vivante ? "" : " morte") + '" style="border-left-color:' + e.couleur + '"><div class="ligne">' +
      (ASSET.u("chef_" + e.id) ? ASSET.img("chef_" + e.id, "med") : "") + "<b>" + esc(e.nom) + "</b><span>" + (f.vivante ? t.length + " quartiers · " + nch + " charbons" : "disparue") + "</span></div>" +
      (f.vivante ? '<div class="grille">' + [["Effectif", renseigne ? f.effectif : "≈ " + Math.max(1, Math.round(f.effectif / 3) * 3)], ["Budget", renseigne ? eur(f.budget) : "?"], ["Agressivité", "●".repeat(f.agressivite) + "○".repeat(5 - f.agressivite)],
        ["Objectif", renseigne ? OBJ[f.objectif] : "?"], ["Cible", renseigne && f.cible ? VILLE.q[f.cible].nom : "?"], ["Trêve", al ? "avec " + IDX.equipes[al.a === e.id ? al.b : al.a].nom : "—"]]
        .map(function (x) { return "<span><small>" + x[0] + "</small><b>" + esc(x[1]) + "</b></span>"; }).join("") + "</div>" : "") + "</div>";
  }).join("") + (renseigne ? "" : '<p class="petit">Mets un opérateur Intelligence sur « Surveiller » pour connaître objectifs, cibles et budgets.</p>');
}

/* ---------------- JOURNAL ---------------- */
function vJournal() {
  return '<h2 class="h">Journal</h2><ul class="journal">' + G.journal.slice(0, 150).map(function (j) {
    return '<li class="' + j.type + '"><small>C' + j.c + "</small> " + esc(j.t) + "</li>";
  }).join("") + "</ul>";
}

/* ---------------- fenêtres ---------------- */
function vRapport() {
  var r = UI.rapport, d = r.d;
  return '<div class="voile"><div class="sheet"><h2>Cycle ' + (G.cycle - 1) + " résolu</h2>" +
    '<div class="grille">' + [["Solde", sgn(d.solde) + " €"], ["Territoire", sgn(d.d_territoire, " %")], ["Pression", sgn(d.d_pression)], ["Stabilité", sgn(d.d_stabilite)]]
      .map(function (x) { return "<span><small>" + x[0] + "</small><b>" + x[1] + "</b></span>"; }).join("") + "</div>" +
    '<ul class="rapport">' + r.lignes.slice(0, 16).map(function (l) { return '<li class="' + l.type + '">' + esc(l.t) + "</li>"; }).join("") + "</ul>" +
    '<button class="btn" data-a="okrapport">Planifier le cycle ' + G.cycle + "</button></div></div>";
}
var FIN = { victoire: "Victoire", faillite: "Faillite", chute: "Chute", abandon: "Abandon", fin: "Fin du temps" };
function vFin() {
  var f = G.fini;
  return '<div class="accueil"><h1 class="titre">' + FIN[f.type] + '</h1><p class="note">Note ' + f.note + "</p>" +
    '<div class="grille">' + [["Cycles joués", f.cycles], ["Territoire", f.territoire + " %"], ["Trésorerie", eur(f.tresorerie)], ["Dette", eur(f.dette)], ["Opérateurs", f.operateurs],
      ["Factions restantes", f.factions], ["Flux actifs", f.flux], ["Pression", f.pression], ["Stabilité", f.stabilite], ["Exposition", f.exposition]]
      .map(function (x) { return "<span><small>" + x[0] + "</small><b>" + esc(x[1]) + "</b></span>"; }).join("") + "</div>" +
    '<button class="btn" data-a="rejouer">Nouvelle partie</button></div>';
}

/* ---------------- gestes ---------------- */
function toucherPlan(sx, sy) {
  var h = sousDoigt(sx, sy);
  if (h.op) { UI.selOp = UI.selOp === h.op ? null : h.op; UI.selQ = null; return render(); }
  if (!h.q) { UI.selQ = null; return render(); }
  if (UI.selOp) {
    var o = opParId(UI.selOp);
    var okR = o.type === "reperage" && accessible(h.q, "reperer");
    if (h.q !== o.pos && !accessible(h.q) && !okR) return toast(zoneOuverte(VILLE.q[h.q].zone) ? "Inaccessible : il faut un quartier voisin à " + B.controle.acces_voisin + " %." : "Zone verrouillée : seul un Repérage peut y entrer, depuis un quartier voisin.");
    UI.selQ = h.q; UI.taches = true; return render();
  }
  UI.selQ = h.q; render();
}
function onClick(ev) {
  var t = ev.target.closest("[data-a],[data-tab],[data-tache],[data-op],[data-recruter],[data-activer],[data-couper],[data-ind],[data-ch],[data-fermerch]");
  if (!t) return;
  var d = t.dataset;
  if (d.tab) return go(d.tab);
  if (d.ind) { var x = indicateurs().filter(function (i) { return i.k === d.ind; })[0]; return toast(x.n + " : " + x.e); }
  if (d.ch != null) { var rc = prendreCharbon(UI.selQ, +d.ch); return rc.ok ? render() : toast(rc.raison); }
  if (d.fermerch != null) { fermerCharbon(UI.selQ, +d.fermerch); return render(); }
  if (d.tache) {
    var r = affecter(UI.selOp, UI.selQ, d.tache);
    UI.taches = false;
    if (!r.ok) return toast(r.raison);
    var o = opParId(UI.selOp); UI.selOp = null; UI.selQ = null;
    return toast(o.id + " : " + B.taches[d.tache].nom + " à " + VILLE.q[lieuOp(o)].nom + ".");
  }
  if (d.op) {
    UI.selOp = d.op; UI.selQ = null; TAB = "carte";
    var q = VILLE.q[lieuOp(opParId(d.op))];
    if (PLAN.cam && PLAN.cache) { var p = proj(PLAN.cache.P, q.cx, q.cy, 0); PLAN.cam.x = p[0]; PLAN.cam.y = p[1]; }
    return render();
  }
  if (d.recruter) { var rr = recruter(d.recruter); return rr.ok ? toast(rr.op.id + " recruté.") : toast(rr.raison); }
  if (d.activer) { var ra = activerFlux(d.activer); verifierQuetes(); return ra.ok ? render() : toast(ra.raison); }
  if (d.couper) { couperFlux(d.couper); return render(); }
  switch (d.a) {
    case "nouvelle": case "rejouer": eraseSave(); newGame(); PLAN.cam = null; PLAN.cache = null; TAB = "carte"; UI.selOp = UI.selQ = null; save(); return render();
    case "continuer": loadSave(); PLAN.cam = null; PLAN.cache = null; return render();
    case "valider":
      UI.selOp = null; UI.selQ = null; UI.taches = false;
      var rap = validerCycle();
      if (!rap) return render();
      G.alertes = {};
      rap.lignes.forEach(function (l) { if ((l.type === "attaque" || l.type === "perte") && l.q) G.alertes[l.q] = 1; });
      UI.rapport = G.fini ? null : rap; save(); return render();
    case "okrapport": UI.rapport = null; return render();
    case "annuler": UI.selOp = null; return render();
    case "fermer": UI.selQ = null; return render();
    case "fermertaches": UI.taches = false; UI.selQ = null; return render();
    case "investir": var ri = investir(UI.selQ); return ri.ok ? render() : toast(ri.raison);
    case "voirflux": UI.sousBord = "flux"; return render();
    case "retourbord": UI.sousBord = null; return render();
    case "zplus": return zoomer(1.25);
    case "zmoins": return zoomer(0.8);
    case "abandon":
      if (!UI.confAb) { UI.confAb = true; setTimeout(function () { UI.confAb = false; }, 4000); return toast("Touche encore pour abandonner."); }
      abandonner(); return render();
  }
}
function boot() { document.addEventListener("click", onClick); render(); }
