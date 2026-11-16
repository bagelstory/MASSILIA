/* CHOUF — 20-temps : énergie et sang-froid en temps réel (rythme « mélange »).
   La semaine de campagne n'avance que quand le joueur la clôture (30-semaine.js). */

/** Recharge l'énergie et le sang-froid selon le temps réel écoulé depuis G.temps.maj.
    Retourne le résumé « Pendant ton absence » si l'absence est assez longue. */
function syncTemps() {
  var T = B.temps, t = G.temps, now = CLOCK.now();
  var dt = Math.max(0, now - t.maj);
  var resume = null;
  if (dt >= T.absence_resume_min * 60000) resume = { minutes: Math.round(dt / 60000), energie: t.energie, sangfroid: t.sangfroid };
  var e = T.energie_regen_min * 60000, s = T.sangfroid_regen_min * 60000;
  // on avance par pas entiers pour garder le reste
  var gE = Math.floor(dt / e), gS = Math.floor(dt / s);
  var avantE = t.energie, avantS = t.sangfroid;
  t.energie = Math.min(T.energie_max, t.energie + gE);
  t.sangfroid = Math.min(T.sangfroid_max, t.sangfroid + gS);
  // le plus lent des deux fixe la nouvelle référence, sans perdre de recharge en cours
  t.maj = (t.energie >= T.energie_max && t.sangfroid >= T.sangfroid_max) ? now : now - (dt % e);
  if (resume) {
    resume.energie = t.energie - avantE; resume.sangfroid = t.sangfroid - avantS;
    if (resume.energie > 0 || resume.sangfroid > 0) G.absence = resume;
  }
  return resume;
}

/** Minutes avant le prochain point d'énergie (affichage). */
function prochainPoint() {
  var T = B.temps;
  if (G.temps.energie >= T.energie_max) return 0;
  var e = T.energie_regen_min * 60000;
  return Math.max(1, Math.ceil((e - ((CLOCK.now() - G.temps.maj) % e)) / 60000));
}

/** Dépense : retourne false si pas assez. */
function depenser(cout) {
  cout = cout || {};
  if ((cout.energie || 0) > G.temps.energie || (cout.sangfroid || 0) > G.temps.sangfroid) return false;
  G.temps.energie -= cout.energie || 0;
  G.temps.sangfroid -= cout.sangfroid || 0;
  return true;
}
