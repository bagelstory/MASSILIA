/* CHOUF — 60-ui : écrans. Même principe que KICK : TAB + SUB, go(), render() par table d'écrans.
   Aucune règle de jeu ici : l'interface appelle les fonctions des modules 00 à 30. */

var TAB = "quartier", SUB = null, UI = { creation: null, toast: null, bilan: null };
var $ = function (s) { return document.querySelector(s); };
function esc(s) { return String(s == null ? "" : s).replace(/[&<>"]/g, function (c) { return { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;" }[c]; }); }

/** Images : ASSETS est injecté par build.py (index des planches découpées). Sinon, aplat. */
var ASSET = {
  u: function (k) { var a = window.ASSETS || {}; return a[k] || null; },
  img: function (k, cls) {
    var u = ASSET.u(k);
    return u ? '<img class="' + (cls || "") + '" src="' + u + '" alt="">' : '<div class="' + (cls || "") + ' ph ph-' + esc(k.split("_")[0]) + '"></div>';
  }
};

/** Explications contextuelles des indicateurs (GDD V7 §6 : chiffré, jamais sans explication). */
var EXPL = {
  tension: [[25, "Calme. Les gens se parlent dans le hall."], [50, "Quelques embrouilles, rien d'ouvert."], [75, "Ça se regarde de travers. Les mères rentrent les enfants tôt."], [101, "Ça peut éclater à tout moment."]],
  police: [[25, "On ne voit presque jamais de patrouille."], [50, "Une voiture passe de temps en temps."], [75, "Contrôles réguliers, les guetteurs sont sur les nerfs."], [101, "La police est là tous les jours."]],
  humeur: [[25, "Le quartier n'en peut plus des réseaux."], [50, "Les gens se taisent, mais ils n'en pensent pas moins."], [75, "On laisse faire, tant que ça reste discret."], [101, "Beaucoup comptent sur l'argent qui circule."]],
  stress: [[25, "Tu dors bien."], [50, "Ça tourne un peu dans ta tête le soir."], [75, "Tu sursautes au moindre bruit d'escalier."], [101, "Tu ne dors plus."]],
  confiance: [[25, "Ne te croit plus."], [50, "Se méfie."], [75, "Te fait plutôt confiance."], [101, "Compte sur toi."]]
};
function expl(k, v) { var t = EXPL[k]; for (var i = 0; i < t.length; i++) if (v < t[i][0]) return t[i][1]; return ""; }
function jauge(v, cls) { return '<div class="jauge ' + (cls || "") + '"><i style="width:' + clamp(v, 0, 100) + '%"></i></div>'; }
function euros(n) { return Math.round(n).toLocaleString("fr-FR") + " €"; }

/* ---------------- Navigation ---------------- */

function go(t) { TAB = t; SUB = null; render(); }
function setSub(s) { SUB = s; render(); }
function toast(t) {
  UI.toast = t; render();
  setTimeout(function () { if (UI.toast === t) { UI.toast = null; render(); } }, 2600);
}

/** Point d'entrée de l'affichage. */
function render() {
  var app = $("#app");
  if (!G) { app.innerHTML = vAccueil(); return; }
  syncTemps();
  if (finCampagne()) { app.innerHTML = vFin(); return; }
  var ecrans = { carte: vCarte, quartier: vQuartier, gens: vGens, journal: vJournal, moi: vMoi };
  var corps = SUB === "zone" ? vZoneDistante() : SUB && SUB.indexOf("pnj:") === 0 ? vPnj(SUB.slice(4)) : ecrans[TAB]();
  app.innerHTML = vEntete() + '<main class="corps">' + corps + "</main>" + vOnglets() +
    (G.carte ? vCarteEvenement() : "") + (UI.bilan && !G.carte ? vBilan() : "") + (G.absence && !G.carte ? vAbsence() : "") +
    (UI.toast ? '<div class="toast">' + esc(UI.toast) + "</div>" : "");
}

/* ---------------- Accueil et création ---------------- */

function vAccueil() {
  var c = UI.creation;
  if (c) {
    var f = IDX.familles[c.G.famille], z = IDX.zones[c.G.protagoniste.zone_origine];
    return '<div class="accueil"><h1 class="titre">CHOUF</h1>' +
      '<div class="carte-bloc"><p class="petit">Rentrée 2021. Tu as 16 ans.</p>' +
      "<h2>" + esc(c.G.protagoniste.prenom + " " + f.nom) + "</h2>" +
      "<p>Tu grandis à <b>" + esc(z.nom) + "</b> (" + esc(z.quartiers.slice(0, 3).join(", ")) + "…).</p>" +
      "<p>" + esc(f.note.charAt(0).toUpperCase() + f.note.slice(1)) + ". Ta mère, " + esc(f.mere.prenom) + ", est " + esc(f.mere.metier) +
      ". Ton petit frère " + esc(f.petit_frere.prenom) + " a 11 ans, ta grande sœur " + esc(f.grande_soeur.prenom) + " en a 20.</p></div>" +
      '<button class="btn" data-a="commencer">Commencer cette vie</button>' +
      '<button class="btn sec" data-a="retirer">Tirer une autre vie</button></div>';
  }
  return '<div class="accueil"><h1 class="titre">CHOUF</h1><p class="sous">Marseille, 2021 → 2026</p>' +
    '<p class="avert">Jeu de fiction pour adultes. Lieux réels, personnages et équipes inventés. Le jeu montre la prison, le deuil, la peur et les proches abîmés.</p>' +
    (hasSave() ? '<button class="btn" data-a="continuer">Continuer</button>' : "") +
    '<label class="champ">Ton prénom<input id="prenom" maxlength="18" autocomplete="off" placeholder="Ton prénom"></label>' +
    '<button class="btn ' + (hasSave() ? "sec" : "") + '" data-a="nouvelle">Nouvelle vie</button></div>';
}

/* ---------------- En-tête et onglets ---------------- */

function vEntete() {
  var t = G.temps, T = B.temps, ch = IDX.chapitres[G.chapitre];
  var pp = prochainPoint();
  return '<header class="entete"><div class="l1"><b>Sem. ' + G.cal.semaine + " · " + G.cal.annee + "</b><span>" + age() + " ans · " + esc(ch.nom) + "</span></div>" +
    '<div class="l2"><span class="res e" title="Énergie">⚡ ' + t.energie + "/" + T.energie_max + (pp ? ' <small>+1 dans ' + pp + " min</small>" : "") + "</span>" +
    '<span class="res s" title="Sang-froid">❄ ' + t.sangfroid + "/" + T.sangfroid_max + "</span>" +
    '<span class="res a">' + euros(G.argent.propre) + "</span></div></header>";
}
function vOnglets() {
  var o = [["carte", "Carte"], ["quartier", "Quartier"], ["gens", "Gens"], ["journal", "Journal"], ["moi", "Moi"]];
  return '<nav class="onglets">' + o.map(function (x) {
    return '<button data-tab="' + x[0] + '" class="' + (TAB === x[0] && !SUB ? "on" : "") + '">' + x[1] + "</button>";
  }).join("") + "</nav>";
}
function vCloreBouton() {
  return '<div class="clore"><button class="btn" data-a="clore">Clore la semaine</button><p class="petit">La ville avance d\'une semaine. Ton énergie remonte un peu.</p></div>';
}

/* ---------------- Carte de Marseille ---------------- */

/* Schéma stylisé (pas une carte exacte) : la mer à l'ouest et au sud. */
var POLY = {
  littoral_nord: "30,40 105,30 115,100 45,115", nord_ouest: "105,30 185,22 195,100 115,100",
  nord_central: "185,22 255,35 250,112 195,100", nord_est: "255,35 335,55 330,140 250,112",
  centre_nord: "115,100 195,100 250,112 200,160 125,165", centre_hist: "70,120 115,100 125,165 130,200 80,205 60,165",
  plaine: "130,200 125,165 200,160 205,210 150,225", est: "200,160 250,112 330,140 325,270 245,255 205,210",
  prado: "80,205 130,200 150,225 205,210 245,255 215,280 120,300 90,260", sud: "90,260 120,300 215,280 245,255 325,270 310,360 200,385 110,360 70,310"
};
var COURT = { centre_hist: "Centre", centre_nord: "Centre nord", plaine: "La Plaine", prado: "Prado", littoral_nord: "Littoral nord" };
function centre(pts) {
  var p = pts.split(" ").map(function (x) { return x.split(",").map(Number); }), sx = 0, sy = 0;
  p.forEach(function (q) { sx += q[0]; sy += q[1]; });
  return [sx / p.length, sy / p.length];
}
function vCarte() {
  var s = '<svg class="plan" viewBox="0 0 360 400"><rect width="360" height="400" class="mer"/>' +
    '<path class="terre" d="M30,40 L105,30 185,22 255,35 335,55 360,60 360,400 300,400 310,360 200,385 110,360 70,310 90,260 80,205 60,165 70,120 45,115 Z"/>';
  D.zones.liste.forEach(function (z) {
    var g = G.zones[z.id], c = centre(POLY[z.id]);
    var cls = "zone c" + g.connu + (z.id === G.protagoniste.zone_origine ? " chez" : "") + (z.id === G.zone ? " ici" : "");
    s += '<g data-zone="' + z.id + '"><polygon class="' + cls + '" points="' + POLY[z.id] + '"/>' +
      '<text x="' + c[0] + '" y="' + c[1] + '" class="lbl">' + esc(COURT[z.id] || z.nom) + "</text></g>";
  });
  s += '<text x="22" y="250" class="lbl mer-lbl">Méditerranée</text></svg>';
  return '<h2 class="h">Marseille</h2>' + s +
    '<p class="petit legende"><span class="pt c3"></span> chez toi · <span class="pt c1"></span> connu · <span class="pt c0"></span> inconnu — touche une zone</p>' +
    vCloreBouton();
}
/** Fiche d'une zone vue de la carte. */
function vZoneDistante() {
  var z = IDX.zones[UI.zoneVue], g = G.zones[z.id];
  var h = '<button class="retour" data-a="retour">‹ Carte</button>' + ASSET.img("banniere_" + z.id, "banniere") +
    '<h2 class="h">' + esc(z.nom) + '</h2><p class="petit">' + esc(z.arrondissements.join(", ")) + " arrondissement</p>";
  if (g.connu === 0) return h + '<p>Tu n\'y as jamais mis les pieds. Tu en connais le nom, c\'est tout.</p>';
  h += "<p>" + esc(z.quartiers.join(" · ")) + '</p><p class="petit">' + esc(z.lieux.join(" · ")) + "</p>";
  if (g.connu >= 1) h += vIndic(g);
  if (g.connu < 3) h += '<p class="petit">Tu n\'y as encore personne. Il faudra une raison, ou quelqu\'un, pour y aller.</p>';
  return h;
}

/* ---------------- Quartier (zone de vie) ---------------- */

function vIndic(g) {
  return '<div class="indics">' + ["tension", "police", "humeur"].map(function (k) {
    var nom = { tension: "Tension", police: "Présence policière", humeur: "Humeur envers les réseaux" }[k];
    return '<div class="indic"><div class="ligne"><span>' + nom + "</span><b>" + Math.round(g[k]) + "/100</b></div>" + jauge(g[k], k) + '<p class="petit">' + expl(k, g[k]) + "</p></div>";
  }).join("") + "</div>";
}
function vQuartier() {
  var z = IDX.zones[G.zone], g = G.zones[G.zone];
  var h = ASSET.img("banniere_" + z.id, "banniere") + '<h2 class="h">' + esc(z.nom) + "</h2>" + vIndic(g);
  var eng = engagementsOuverts();
  if (eng.length) {
    h += '<h3 class="h3">Tes promesses</h3>' + eng.map(function (e, i) {
      var reste = e.echeance - nowWk();
      return '<button class="ligne-action" data-eng="' + i + '"><span><b>Tenir ta promesse à ' + esc(pnjNom(e.envers)) + "</b><small>" +
        (reste <= 0 ? "c'est cette semaine" : "encore " + reste + " semaine" + (reste > 1 ? "s" : "")) + '</small></span><em>⚡ ' + B.engagements.cout_tenir_energie + "</em></button>";
    }).join("");
  }
  h += '<h3 class="h3">Ta semaine</h3>' + missionsDispo().map(function (m) {
    var c = m.cout || {}, ok = (c.energie || 0) <= G.temps.energie && (c.sangfroid || 0) <= G.temps.sangfroid;
    var mt = maitrise(m.id);
    return '<button class="ligne-action ' + (ok ? "" : "off") + '" data-mission="' + m.id + '"><span><b>' + esc(fillTexte(m.nom)) + "</b><small>" +
      (mt ? "★".repeat(mt) + "☆".repeat(3 - mt) : "jamais fait") + "</small></span><em>⚡ " + (c.energie || 0) + (c.sangfroid ? " ❄ " + c.sangfroid : "") + "</em></button>";
  }).join("");
  return h + vCloreBouton();
}

/* ---------------- Gens ---------------- */

var GROUPES = [["famille", "Famille"], ["quartier", "Quartier"], ["organisation", "Le hall"], ["institution", "Institutions"], ["ville", "La ville"]];
function vGens() {
  var ids = Object.keys(G.relations);
  return '<h2 class="h">Les gens</h2>' + GROUPES.map(function (gr) {
    var l = ids.filter(function (id) { return IDX.personnages[id].groupe === gr[0]; });
    if (!l.length) return "";
    return '<h3 class="h3">' + gr[1] + "</h3>" + l.map(function (id) {
      var r = G.relations[id];
      return '<button class="pnj" data-pnj="' + id + '">' + ASSET.img("medaillon_" + id, "med") + "<span><b>" + esc(pnjNom(id)) + "</b><small>" +
        esc(expl("confiance", r.confiance)) + "</small>" + jauge(r.confiance, "conf") + "</span></button>";
    }).join("");
  }).join("");
}
function vPnj(id) {
  var p = IDX.personnages[id], r = G.relations[id], a = pnjAge(id);
  var mem = (r.memoire || []).map(function (m) { return MEMOIRE[m] || null; }).filter(Boolean);
  return '<button class="retour" data-a="retour">‹ Les gens</button><div class="fiche">' + ASSET.img("medaillon_" + id, "med grand") +
    "<h2>" + esc(pnjNomComplet(id)) + '</h2><p class="petit">' + (a ? a + " ans · " : "") + esc((p.traits || []).join(", ")) + "</p>" +
    (p.note ? "<p>" + esc(p.note.charAt(0).toUpperCase() + p.note.slice(1)) + ".</p>" : "") +
    "<p><i>Ce qu'il ou elle veut :</i> " + esc(p.objectif) + ".</p></div>" +
    ["confiance", "proximite", "tension"].map(function (k) {
      return '<div class="indic"><div class="ligne"><span>' + { confiance: "Confiance", proximite: "Proximité", tension: "Tension" }[k] + "</span><b>" + Math.round(r[k]) + "/100</b></div>" + jauge(r[k], k) + "</div>";
    }).join("") + (mem.length ? '<h3 class="h3">Se souvient que</h3><ul class="mem">' + mem.map(function (m) { return "<li>" + esc(m) + "</li>"; }).join("") + "</ul>" : "");
}
var MEMOIRE = {
  promesse_tenue: "tu as tenu ta promesse", promesse_rompue: "tu n'as pas tenu ta promesse", a_reconnu: "tu as reconnu ton erreur",
  a_fui: "tu as évité la conversation", est_venu: "tu es venu quand il t'a appelé", a_refuse: "tu as refusé son « truc simple »",
  a_promis: "tu lui as promis ton samedi", a_promis_rdv: "tu as promis d'être au rendez-vous", est_passe: "tu es passé sans t'arrêter",
  essai_fait: "tu es venu à l'essai", essai_refuse: "tu as refusé l'essai", a_vu_le_tel: "tu as sorti ton téléphone pendant la descente",
  a_vu_avec_flic: "on t'a vu parler avec un policier"
};

/* ---------------- Journal et Moi ---------------- */

function vJournal() {
  return '<h2 class="h">Journal</h2><ul class="journal">' + G.journal.slice(0, 80).map(function (j) {
    return '<li class="' + j.type + '"><small>S' + j.s + " " + j.a + "</small> " + esc(j.t) + (j.cause ? ' <em>— ' + esc(j.cause) + "</em>" : "") + "</li>";
  }).join("") + "</ul>";
}
function vMoi() {
  var p = G.protagoniste, f = IDX.familles[G.famille];
  return '<h2 class="h">' + esc(p.prenom + " " + f.nom) + '</h2><p class="petit">' + age() + " ans · " + esc(IDX.zones[p.zone_origine].nom) + " · " +
    (p.scolarite.statut === "lycee" ? "au lycée" : esc(p.scolarite.statut)) + "</p>" +
    '<h3 class="h3">Compétences</h3>' + D.competences.liste.map(function (c) {
      var v = p.competences[c.id], pl = palier(v), t = c.paliers[pl - 1];
      return '<div class="indic"><div class="ligne"><span>' + esc(c.nom) + "</span><b>Palier " + pl + " — " + esc(t[0]) + "</b></div>" + jauge(v, "comp") + '<p class="petit">' + esc(t[1]) + "</p></div>";
    }).join("") +
    '<h3 class="h3">Toi</h3>' + [["Stabilité", p.etat.stabilite, ""], ["Stress", p.etat.stress, expl("stress", p.etat.stress)], ["Santé", p.etat.sante, ""], ["Assiduité au lycée", p.scolarite.assiduite, ""]].map(function (x) {
      return '<div class="indic"><div class="ligne"><span>' + x[0] + "</span><b>" + Math.round(x[1]) + "/100</b></div>" + jauge(x[1]) + (x[2] ? '<p class="petit">' + x[2] + "</p>" : "") + "</div>";
    }).join("") +
    '<h3 class="h3">Ce qu\'on pense de toi</h3><p>Respect ' + p.positionnement.respect + " · Influence " + p.positionnement.influence + " · Crainte " + p.positionnement.crainte + "</p>" +
    '<button class="btn sec" data-a="effacer">Effacer cette vie</button>';
}

/* ---------------- Fenêtres ---------------- */

function vCarteEvenement() {
  var c = G.carte, ev = IDX.evenements[c.id];
  return '<div class="voile"><div class="sheet">' + ASSET.img("scene_" + ev.id, "scene") + "<h2>" + esc(ev.titre) + "</h2><p>" + esc(fillTexte(ev.texte, c.ctx)) + "</p>" +
    optionsCarte(c).map(function (x) { return '<button class="btn opt" data-opt="' + x.i + '">' + esc(fillTexte(x.o.t, c.ctx)) + "</button>"; }).join("") + "</div></div>";
}
function vBilan() {
  var b = UI.bilan;
  return '<div class="voile"><div class="sheet"><h2>Semaine ' + G.cal.semaine + " · " + G.cal.annee + "</h2>" +
    (b.lignes.length ? "<ul>" + b.lignes.map(function (l) { return "<li>" + esc(l) + "</li>"; }).join("") + "</ul>" : "<p>Une semaine comme les autres.</p>") +
    (b.mensuel ? '<p class="petit">Fin du mois. Argent : ' + euros(G.argent.propre) + ". Stress : " + Math.round(G.protagoniste.etat.stress) + "/100.</p>" : "") +
    '<button class="btn" data-a="okbilan">Continuer</button></div></div>';
}
function vAbsence() {
  var a = G.absence;
  return '<div class="voile"><div class="sheet"><h2>Pendant ton absence</h2><p>' + (a.minutes >= 120 ? Math.round(a.minutes / 60) + " heures" : a.minutes + " minutes") +
    " ont passé.</p><ul>" + (a.energie ? "<li>Énergie +" + a.energie + "</li>" : "") + (a.sangfroid ? "<li>Sang-froid +" + a.sangfroid + "</li>" : "") +
    '</ul><p class="petit">La semaine, elle, n\'avance que quand tu la clôtures.</p><button class="btn" data-a="okabsence">Continuer</button></div></div>';
}
function vFin() {
  return '<div class="accueil"><h1 class="titre">2026</h1><p>Fin de la campagne du prototype. L\'après arrive dans une prochaine version.</p>' +
    '<button class="btn sec" data-a="effacer">Recommencer une vie</button></div>';
}

/* ---------------- Événements d'interface ---------------- */

function onClick(ev) {
  var t = ev.target.closest("[data-a],[data-tab],[data-zone],[data-mission],[data-opt],[data-pnj],[data-eng]");
  if (!t) return;
  var d = t.dataset;
  if (d.tab) return go(d.tab);
  if (d.zone) { UI.zoneVue = d.zone; return setSub("zone"); }
  if (d.pnj) return setSub("pnj:" + d.pnj);
  if (d.opt != null) { choisir(+d.opt); return render(); }
  if (d.eng != null) { if (!tenirEngagement(+d.eng)) toast("Pas assez d'énergie."); return render(); }
  if (d.mission) { var r = faireMission(d.mission); toast(r.texte); return; }
  switch (d.a) {
    case "nouvelle":
      var pr = ($("#prenom") || {}).value || "";
      UI.creation = { prenom: pr, G: newGame(pr) }; G = null; return render();
    case "retirer": UI.creation = { prenom: UI.creation.prenom, G: newGame(UI.creation.prenom) }; G = null; return render();
    case "commencer": G = UI.creation.G; UI.creation = null; TAB = "quartier"; save(); return render();
    case "continuer": if (loadSave()) { TAB = "quartier"; } return render();
    case "clore": UI.bilan = cloreSemaine(); return render();
    case "okbilan": UI.bilan = null; return render();
    case "okabsence": G.absence = null; save(); return render();
    case "retour": SUB = null; return render();
    case "effacer":
      if (!UI.confirmeEffacer) { UI.confirmeEffacer = true; toast("Touche encore une fois pour effacer cette vie."); setTimeout(function () { UI.confirmeEffacer = false; }, 4000); return; }
      UI.confirmeEffacer = false; eraseSave(); UI.creation = null; return render();
  }
}

/** Démarrage : écouteurs et premier affichage. */
function boot() {
  document.addEventListener("click", onClick);
  document.addEventListener("visibilitychange", function () { if (!document.hidden && G) render(); });
  setInterval(function () { if (G && !G.carte && !UI.bilan && document.activeElement.tagName !== "INPUT") render(); }, 30000);
  render();
}
