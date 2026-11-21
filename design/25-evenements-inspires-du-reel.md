# CHOUF : 30 événements de ville inspirés de faits réels marseillais

*21 novembre 2026. Suite de `10` §14, `17` §1-5, `20` (conséquences) et `22`/`24` (scènes à choix). **Idées de conception : rien n'est codé.** Ces événements sont le « temps qu'il fait » de la ville : ils arrivent au joueur, il n'en est jamais l'auteur. Ils s'inspirent de **phénomènes réels et récurrents** (le mistral, la grève, le soir de match, l'ouverture d'un chantier), jamais d'un épisode précis.*

## 0. Les lignes qui ne bougent pas
1. **Phénomènes, pas épisodes.** Chaque événement décrit ce qui revient chaque année ou presque (un incendie de colline en été, une grève, une tempête). Aucune date, aucune affaire, aucun nom d'affaire reconnaissable. Les années restent « Année N ».
2. **Institutions et lieux : oui, comme décor.** La mairie, la RTM, les marins-pompiers, le port, les hôpitaux publics, l'OM et le Vélodrome peuvent être cités tels quels (règle 5 de `00` : l'OM est un décor de quartier, **jamais lié à un acte ni à un personnage**).
3. **Aucune personne, aucune victime, aucune bande réelle.** Les morts, les blessés, les coupables et les témoins sont **inventés** (personnages de `10`, `18`, `21`). Une victime n'a ni nom ni âge dans le journal ; un deuil se montre par ses traces (une bougie, un rideau baissé).
4. **Aucune accusation contre une institution réelle.** Quand un service manque (une classe fermée, un ascenseur en panne), le texte dit **ce qui manque**, jamais « qui est coupable ». Les abus, les fautes et les trahisons restent attribués à des **personnages fictifs**.
5. **Jamais ces sujets :** la rue d'Aubagne et tout ce qui en évoque l'affaire (effondrement d'immeuble, évacuations, arrêtés de péril) ; toute fusillade ou affaire nommée ; tout nom d'opération de police ; tout média, élu ou club (hors OM) réel.
6. **On montre le coût, jamais la méthode.** Aucun événement ne détaille un procédé (trafic, blanchiment, armes, évitement de la police). Quand le crime en profite, le texte dit **ce que la ville perd** à côté.
7. **Mineurs hors champ.** Les écoles, les crèches et les rentrées se racontent par les adultes (mères, enseignants, agents) et par des chaises vides, jamais par un enfant à l'écran, jamais comme ressource.

## 1. Légende
`Rev×` multiplicateur de revenu des charbons (`SeasonSettings.incomeMultiplier`, `GameConfig.Events`) · `Pr` Pression policière de la zone · `Év` Éveil des façades (`13`) · `Loy` Loyauté de l'équipe · `Rép` Réputation du quartier · `Voi` Voisinage · `Foy` Foyer · `Usu` Usure · `Cas` Casier · `Exp` Exposition des proches · `Dua` Double vie (`20`) · `Pe` peur du quartier. **Semaine** = un cycle (`GameConfig.weeksPerYear` = 52). Les valeurs sont des **points de départ à régler par tests** (`20` §11). Les noms `en code` sont des propositions de `GameConfig`.

**Mise en œuvre (idée) :** chaque ligne se traduit en une entrée proche de `SeasonSettings` (`weeks`, `chance`, `zoneIds`, `incomeMultiplier`) plus une liste d'effets de jauges et une ligne de journal ; les événements à choix renvoient à une scène de `22`/`24`.

## 2. Vue d'ensemble
| N° | Événement | Saison | Durée | Zones (arr.) | Personnage lié |
|---|---|---|---|---|---|
| R01 | Mistral et départ de feu sur la colline | été | 2 sem. | 15e, 16e, 9e | Capitaine Coste |
| R02 | Les massifs fermés | été | 3 sem. | 8e, 9e | (aucun) |
| R03 | Canicule | été | 3 sem. | toute la ville | Soraya Benkhelifa |
| R04 | Restrictions d'eau | été-automne | 6 sem. | toute la ville | Jean-Mi Roustan |
| R05 | Orage méditerranéen, rues inondées | automne | 1 sem. | 1er, 2e, 3e, 4e | Marcel Gianni |
| R06 | Tempête : la Corniche fermée | hiver | 1 sem. | 7e, 8e | Jean-Mi Roustan |
| R07 | Grève des éboueurs | variable | 2 sem. | 1er, 3e, 14e | Madame Zohra |
| R08 | Grève des transports | variable | 2 sem. | toute la ville | Brigitte Alessi |
| R09 | Soir de match au Vélodrome | automne-printemps | 1 nuit | 8e, 9e, 10e, 6e | (aucun) |
| R10 | Fête de la musique | été | 1 nuit | 1er, 6e, 2e | Djibril « Basse » Camara |
| R11 | Feux d'artifice sur le Vieux-Port | été | 1 nuit | 1er, 2e, 7e | Marcel Gianni |
| R12 | Chandeleur : la procession et les navettes | hiver | 1 sem. | 7e, 1er | Père Anselme |
| R13 | Foire aux santons et Noël | hiver | 3 sem. | 1er, 2e, 5e | Madeleine Pagès |
| R14 | Les nuits de Ramadan | variable | 4 sem. | 1er, 3e, 14e, 15e | Hadj Ismaël Taleb |
| R15 | La rentrée : les chaises vides | automne | 2 sem. | 3e, 13e, 14e, 15e | Claire Aubanel |
| R16 | Les ferries d'été | été | 3 sem. | 2e, 15e | Rosalie Vidal |
| R17 | Les paquebots de croisière | printemps-été | continu | 2e, 1er | Madame Zohra |
| R18 | Le chantier de la Joliette | permanent | 20 sem. | 2e, 3e | Madeleine Pagès |
| R19 | Les meublés touristiques du Panier | permanent | 12 sem. | 2e, 1er | Gérard « le Syndic » Faure |
| R20 | Une classe ferme | automne | permanent | 3e, 13e, 14e | Hélène Caron |
| R21 | Le centre social perd sa subvention | année | permanent | 14e, 15e | Barral |
| R22 | Une nuit de tirs : le lendemain | tout | 3 sem. | une zone | Soraya Benkhelifa |
| R23 | Le rassemblement silencieux | tout | 1 sem. | une zone | Père Anselme |
| R24 | Une grande opération de police | tout | 3 sem. | une zone | Commandant Orsini |
| R25 | La ville à la une | tout | 4 sem. | toute la ville | La Pigiste, Hugo Tavernier |
| R26 | La campagne locale | année électorale | 8 sem. | tous | Le Ruban |
| R27 | Un grand événement sportif | variable | 3 sem. | 8e, 9e, 1er | Marianne Léotard |
| R28 | L'ascenseur en panne depuis des mois | permanent | continu | 14e, 13e | Fatma Oulad, La Vigie |
| R29 | Les urgences saturées | hiver | 3 sem. | 15e, 14e, 5e | Docteure Inès Marrou |
| R30 | Jours d'alerte à l'air autour du port | été | 2 sem. | 2e, 15e, 16e | Roger Mazet |

## 3. Les événements

### Le feu, l'eau et le vent

**R01 · Mistral et départ de feu sur la colline** *(inspiré des incendies de colline de fin d'été)*
- **Déclencheur :** été (semaines 27 à 36), mistral actif (`chance` 0,10 par cycle) ; deux cycles de mistral de suite relèvent la chance à 0,25.
- **Quartiers :** L'Estaque, Séon, Saint-Louis, Mourepiane (15e, 16e), Luminy (9e).
- **Effets (2 sem.) :** `Rev×` 0,70 ; `Pr` −6 (les forces de l'ordre sont ailleurs) ; `Rép` +4 pour qui aide à l'évacuation ; `Usu` +3 ; `Dua` −2 ; un charbon de la zone est fermé 1 semaine (`evtFeuFermetureSem` = 1).
- **Journal :** *« Le mistral souffle depuis trois jours, la colline brûle au nord. Les marins-pompiers montent toute la nuit. Les charbons ferment, les nerfs aussi. »*
- **Lien :** Capitaine Coste (`21` n° 12, marin-pompier) ; scène possible : E45 (`24`).

**R02 · Les massifs fermés** *(les accès aux calanques et aux massifs fermés par risque d'incendie)*
- **Déclencheur :** été, vent fort et sécheresse ; suit R01 ou R03 (`chance` 0,15).
- **Quartiers :** Prado, Pointe-Rouge, Goudes, Mazargues, Luminy (8e, 9e).
- **Effets (3 sem.) :** `Rev×` 0,85 ; la foule se reporte sur les plages et le Vieux-Port : `Voi` +4 en bas des tours ; `Rép` −2 (les riverains ruminent).
- **Journal :** *« Les calanques sont fermées. Tout le monde descend sur la plage, et les voisins comptent les serviettes. »*
- **Lien :** aucun.

**R03 · Canicule**
- **Déclencheur :** été, `chance` 0,12 par cycle (juin à août) ; ne se répète pas deux fois en moins de 6 semaines.
- **Quartiers :** toute la ville, plus durement les immeubles sans balcon (1er, 3e, 14e).
- **Effets (3 sem.) :** `Usu` +5 par semaine pour tout le monde (joueur, équipe, proches), plafonné par le repos ; `Rev×` 0,90 le jour et 1,10 la nuit (pas d'effet net sur les charbons de rue) ; `Voi` +3 (nuits dehors, bruit) ; `Foy` −2 si un proche âgé vit seul ; l'action « Se soigner » (`20` §2) coûte 15 % de moins pendant l'épisode.
- **Journal :** *« Trente-sept à l'ombre. Les cages d'escalier se remplissent de chaises. Les vieux ne dorment plus. »*
- **Lien :** Soraya Benkhelifa (n° 1 de `21`) fait sa tournée dans la chaleur.

**R04 · Restrictions d'eau**
- **Déclencheur :** été-automne après une saison sans pluie (`chance` 0,06) ; durée longue.
- **Quartiers :** toute la ville.
- **Effets (6 sem.) :** `Rev×` 0,95 (les commerces d'eau et de glace d'abord) ; `Voi` +2 ; façades alimentaires et chichas : `Év` +2 (contrôles d'hygiène) ; un charbon au pied d'un immeuble sans eau : `Rép` −3.
- **Journal :** *« Interdit de laver les voitures, interdit d'arroser. Les terrasses mettent des cartons à la place des verres d'eau. »*
- **Lien :** Jean-Mi Roustan (n° 5 de `21`), pêcheur : la rade manque de poissons autant que la ville d'eau.

**R05 · Orage méditerranéen, rues inondées**
- **Déclencheur :** automne (septembre à novembre), `chance` 0,10.
- **Quartiers :** Canebière, Belsunce, Joliette, Chutes-Lavie, Belle de Mai (1er à 4e).
- **Effets (1 sem.) :** `Rev×` 0,60 ; les parkings et les caves des charbons sont inondés : −400 € de remise en état par charbon touché ; `Pr` −3 ; `Rép` +3 pour qui prête un coffre ou une pelle.
- **Journal :** *« Quarante millimètres en deux heures. La rue de la Palud est un canal. Un taxi nage devant le Vieux-Port. »*
- **Lien :** Marcel Gianni (n° 2 de `21`), taxi, coincé dans l'eau.

**R06 · Tempête : la Corniche fermée**
- **Déclencheur :** hiver, `chance` 0,06 ; les vagues passent par-dessus la route.
- **Quartiers :** Endoume, Vallon des Auffes, Roucas-Blanc, Prado (7e, 8e).
- **Effets (1 sem.) :** `Rev×` 0,80 dans la zone ; l'accès aux cabanons est coupé ; `Voi` −2 (le calme revient en bas) ; les pointus sont tirés à terre : revenus de pêche de Jean-Mi −100 %, `Foy` de sa famille −2.
- **Journal :** *« La Corniche est fermée : la mer passe par-dessus les barrières. Les pointus attendent, à sec. »*
- **Lien :** Jean-Mi Roustan, Rosalie Vidal.

### La grève, la poubelle, le bus

**R07 · Grève des éboueurs**
- **Déclencheur :** variable, `chance` 0,05 par cycle ; maximum une fois par an ; jamais en été.
- **Quartiers :** Belsunce, Noailles, Belle de Mai, Saint-Mauront, Saint-Just (1er, 3e, 14e).
- **Effets (2 sem.) :** `Voi` +6 en bas des cages d'escalier ; `Rev×` 0,90 (les clients évitent les rues sales) ; `Rép` −2 par semaine pour le quartier qui n'est pas aidé ; une action « Faire nettoyer devant sa porte » (−300 €) apporte `Rép` +5 mais `Dua` +2 (on se demande qui paie).
- **Journal :** *« Les poubelles débordent depuis dix jours. Les rats se promènent, les riverains ruminent. »*
- **Lien :** Madame Zohra (n° 15), qui connaît chaque escalier.

**R08 · Grève des transports**
- **Déclencheur :** variable, `chance` 0,05 ; une fois par an maximum.
- **Quartiers :** toute la ville, plus durement les 13e, 14e, 15e.
- **Effets (2 sem.) :** déplacement des équipiers : −1 tâche par équipier et par semaine ; `Rev×` 0,85 dans les quartiers éloignés ; `Pr` +2 (plus de voitures, plus de contrôles) ; `Usu` +2 pour les équipiers qui marchent ; les taxis de nuit sont cherchés : le revenu de Marcel Gianni double, il en sait davantage (`Exp` du joueur +2).
- **Journal :** *« Pas de métro, pas de bus, pas de tram. La ville marche, ou paie un taxi. Les retards se comptent en heures. »*
- **Lien :** Brigitte Alessi (n° 22), conductrice : elle ne conduit plus, elle compte.

### La ville en fête

**R09 · Soir de match au Vélodrome** *(décor de quartier uniquement : aucun lien avec un acte, un personnage ou un gain)*
- **Déclencheur :** automne à printemps, toutes les deux ou trois semaines ; `chance` 0,35 par cycle de la saison de championnat.
- **Quartiers :** Prado, Rond-Point, Michelet, Capelette, Plaine (8e, 9e, 10e, 6e).
- **Effets (1 nuit) :** circulation bloquée : −1 déplacement d'équipier dans le 8e et le 9e ; les rues sont pleines : `Rev×` 1,10 pour tout commerce de bouche, 0,90 pour les charbons de rue (trop de regards) ; `Pr` +3 (présence policière visible) ; aucun effet sur `Cas`, `Exp` ni `Dua`.
- **Journal :** *« Le Vélodrome chante jusqu'à minuit. Les rues autour sont bloquées, les cafés doublent leurs tarifs. »*
- **Lien :** aucun (par décision de `00` règle 5).

**R10 · Fête de la musique**
- **Déclencheur :** une fois par an, semaine 25 ; fixe.
- **Quartiers :** Cours Julien, Plaine, Vieux-Port, Panier (1er, 2e, 6e).
- **Effets (1 nuit) :** `Rev×` 1,20 pour les façades bar et club (`13`) mais `Év` +3 (la foule regarde) ; `Voi` +5 (bruit) ; `Rép` +3 pour une façade qui paie un vrai groupe ; les équipiers de nuit : `Usu` +2.
- **Journal :** *« Des scènes dans chaque rue, des voisins aux fenêtres. Le bruit couvre tout, et les regards aussi. »*
- **Lien :** Djibril « Basse » Camara (n° 10) ; scène : E43 (`24`).

**R11 · Feux d'artifice sur le Vieux-Port**
- **Déclencheur :** une fois par an, mi-juillet ; fixe.
- **Quartiers :** Vieux-Port, Panier, Endoume (1er, 2e, 7e).
- **Effets (1 nuit) :** foule dense : `Rev×` 1,15 pour les cafés ; `Pr` +4 (présence visible) ; les téléphones sont saturés : un cycle de **Fil** (`12`) à 50 % de fiabilité ; `Rép` +1 pour qui offre un verre aux voisins.
- **Journal :** *« Le ciel du Vieux-Port est en couleurs. Dans le noir, les téléphones ne répondent plus. »*
- **Lien :** Marcel Gianni, coincé dans la foule avec son taxi.

**R12 · Chandeleur : la procession et les navettes**
- **Déclencheur :** une fois par an, semaine 5 ; fixe.
- **Quartiers :** Saint-Victor, Vieux-Port (7e, 1er).
- **Effets (1 sem.) :** `Rép` +4 pour qui tient la file (distribution de navettes à la porte du four) ; `Foy` +2 (famille dans la rue) ; `Rev×` 1,05.
- **Journal :** *« La procession descend vers la mer. Le four des navettes ne s'éteint plus. »*
- **Lien :** Père Anselme (n° 24).

**R13 · Foire aux santons et Noël**
- **Déclencheur :** décembre ; fixe.
- **Quartiers :** centre-ville (1er, 2e, 5e).
- **Effets (3 sem.) :** `Rev×` 1,15 (cadeaux, repas de fête) ; `Foy` +3 pour qui rentre dîner ; `Usu` −2 au repos ; `Dua` +2 (écart plus visible entre l'argent et les cadeaux).
- **Journal :** *« Les santons s'alignent sous les parasols. Une crèche de plus dans la vitrine. Madeleine a mis un couvert de plus. »*
- **Lien :** Madeleine Pagès (n° 14).

**R14 · Les nuits de Ramadan** *(mois mobile ; traité avec respect : vie sociale, entraide)*
- **Déclencheur :** un mois de 4 semaines qui se décale chaque année (`ramadanStartWeek` avance de ~1 semaine par an).
- **Quartiers :** Belsunce, Noailles, Belle de Mai, Saint-Just, L'Estaque (1er, 3e, 14e, 15e).
- **Effets (4 sem.) :** `Rev×` 1,15 la nuit (les rues vivent) et 0,85 le jour ; `Rép` +4 pour qui participe aux iftars solidaires ; la tension baisse : `Voi` −2 ; les familles se retrouvent : `Foy` +3. Aucun effet moralisateur : l'événement montre une communauté, pas un « problème ».
- **Journal :** *« À la tombée de la nuit, les rues se remplissent. Les tables se dressent. Quelqu'un frappe à une porte, un plat à la main. »*
- **Lien :** Hadj Ismaël Taleb (n° 16 de `21`, aumônier).

### La ville qui travaille

**R15 · La rentrée : les chaises vides**
- **Déclencheur :** une fois par an, début septembre ; fixe.
- **Quartiers :** Belle de Mai, Malpassé, Saint-Jérôme, Saint-Just, Saint-Louis (3e, 13e, 14e, 15e).
- **Effets (2 sem.) :** `Rép` +2 pour qui paie les fournitures (−200 €) ; une chaise vide se lit : si un équipier est tombé l'an passé, `Foy` −4 et `Rép` −3 dans son quartier ; `Pe` −1 (la rentrée apaise).
- **Journal :** *« C'est la rentrée. Devant le portail, les mères se serrent. On compte les enfants, et ceux qui manquent. »* (aucun enfant à l'écran)
- **Lien :** Claire Aubanel (n° 11), Hélène Caron (n° 3 de `21`).

**R16 · Les ferries d'été**
- **Déclencheur :** été, départs vers l'autre rive de la Méditerranée ; `chance` 1 en semaines 28-34.
- **Quartiers :** port de la Joliette (2e), Mourepiane (15e), Gare maritime.
- **Effets (3 sem.) :** départ et retours de familles entières : `Rev×` 0,85 dans les quartiers vidés ; l'équipe prend des congés (`Loy` +3 si accordés, −3 sinon) ; les proches qui partent loin du joueur : `Exp` −5 pendant l'absence ; `Pr` +2 au port (contrôles d'été).
- **Journal :** *« Les voitures chargées jusqu'au toit font la queue devant le ferry. La moitié du quartier est partie. Le reste compte les jours. »*
- **Lien :** Rosalie Vidal (n° 7), qui voit partir les barques et les bateaux.

**R17 · Les paquebots de croisière**
- **Déclencheur :** printemps-été, `chance` 0,50 par cycle.
- **Quartiers :** Joliette, Panier, Vieux-Port (2e, 1er).
- **Effets (continu) :** `Rev×` 1,10 pour les cafés et les boutiques du centre ; `Voi` +2 (rues bondées) ; les pickpockets et la police s'y déplacent : `Pr` +2 à la Joliette ; `Dua` +1.
- **Journal :** *« Un paquebot amarré : cinq mille visiteurs descendent sur le quai. Les commerçants relèvent les prix, les habitants changent de trottoir. »*
- **Lien :** Madame Zohra, la « sentinelle » du Panier.

### La ville qui change

**R18 · Le chantier de la Joliette**
- **Déclencheur :** une fois, sans date (Acte I ou II) ; dure.
- **Quartiers :** Joliette, Docks, Arenc (2e, 3e).
- **Effets (20 sem.) :** `Rev×` 0,80 (bruit, accès coupés) puis 1,10 (nouveaux bureaux) ; loyers en hausse : `Voi` +3 ; les habitants partent : `Rép` −4 ; `Dua` +4 pour qui profite des travaux (une façade immo, `13`) ; `Év` +2 sur Immo.
- **Journal :** *« Une grue de plus. Un immeuble de verre de plus. Les vieux voisins déménagent, les cafés changent de nom. »*
- **Lien :** Madeleine Pagès, qui voit son quartier changer de visage.

**R19 · Les meublés touristiques du Panier**
- **Déclencheur :** Acte II ; `chance` 0,05.
- **Quartiers :** Panier, Accoules, Belsunce (2e, 1er).
- **Effets (12 sem.) :** `Rép` −3 pour les habitants qui perdent un logement ; `Voi` +4 ; une façade Immo (`13`) y gagne +20 % de revenu mais `Év` +5 ; la police surveille plus (`Pr` +2).
- **Journal :** *« Les boîtes à clés se multiplient sur les portes du Panier. Des valises roulent sur les pavés à six heures du matin. »*
- **Lien :** Gérard « le Syndic » Faure (n° 21), scène : E50 (`24`).

**R20 · Une classe ferme** *(un manque de moyens, jamais une faute désignée)*
- **Déclencheur :** rentrée, `chance` 0,08 ; une fois par quartier.
- **Quartiers :** Belle de Mai, Saint-Just, Malpassé (3e, 13e, 14e).
- **Effets (permanent) :** `Rép` −3 ; `Pe` +2 ; les mères discutent plus longtemps au portail ; *indicateur de coût pour la ville* : `Of` +1 (« enfance perdue », `11`), affiché comme une perte, jamais comme un gain ni une ressource ; `Foy` −2 pour les équipiers parents.
- **Journal :** *« L'école perd une classe à la rentrée. Les parents font un rassemblement devant le portail. Les enseignants comptent leurs heures. »*
- **Lien :** Hélène Caron (n° 3 de `21`).

**R21 · Le centre social perd sa subvention** *(un fait administratif, non un reproche)*
- **Déclencheur :** fin d'année budgétaire, `chance` 0,05.
- **Quartiers :** Saint-Louis, Saint-Just, Frais-Vallon (14e, 15e).
- **Effets (permanent) :** `Rép` −2 ; la salle de boxe de Barral ferme ses créneaux du soir : `Pe` +2, `Of` +1 ; **option :** financer la salle (−1 800 €, `Dua` +3, `Rép` +8, Barral refuse l'argent sale avec une réplique à écrire).
- **Journal :** *« La salle de boxe n'ouvre plus le soir. Barral a rangé les gants, et mis un papier sur la porte. »*
- **Lien :** Barral (`10`).

### La peur et ses suites

**R22 · Une nuit de tirs : le lendemain** *(on ne montre que ce qui reste)*
- **Déclencheur :** `Pr` élevée dans une zone en conflit de bandes (`11`) ; `chance` 0,10 par cycle quand `guerre` actif.
- **Quartiers :** la zone où un conflit est ouvert.
- **Effets (3 sem.) :** `Pe` +8 ; `Rev×` 0,50 la première semaine ; `Rép` −4 pour le joueur si le conflit lui est attribué ; l'école ferme un jour ; `Foy` −3 pour les équipiers de la zone ; un charbon est « gelé » 1 semaine ; `Pr` +10 ; une dette de famille (`20` §5) s'ouvre si un équipier est touché (adulte, hors écran).
- **Journal :** *« Cette nuit, des tirs dans la cité. Ce matin, un scotch sur une vitre, une bougie par terre, et le café qui n'ouvre pas. »*
- **Lien :** Soraya Benkhelifa (soigne, sans commentaire).

**R23 · Le rassemblement silencieux** *(suite possible de R22 ; victime **inventée**, adulte, jamais nommée)*
- **Déclencheur :** 1 à 2 semaines après la mort d'un personnage adulte inventé du quartier.
- **Quartiers :** la zone concernée.
- **Effets (1 sem.) :** `Rép` +4 pour qui s'y rend sans bruit, −8 pour qui s'y affiche ; `Foy` +2 ; `Pr` +3 (observation) ; l'**écran de deuil** : trois lignes, sans musique triomphante.
- **Journal :** *« Une centaine de personnes sur le trottoir, sans un mot. Les bougies tiennent malgré le vent. »*
- **Lien :** Père Anselme (n° 24), Dolorès Marquès (n° 13) si le défunt est proche.

**R24 · Une grande opération de police** *(sans nom d'opération, sans reproche : elle déplace sans résoudre)*
- **Déclencheur :** `Pr` ≥ 70 dans une zone depuis 4 semaines, `chance` 0,15.
- **Quartiers :** une zone, au choix du moteur.
- **Effets (3 sem.) :** `Rev×` 0,55 dans la zone ; `Cas` +4 par équipier interpellé (`20` §1) ; `Pr` zone −20 après l'opération mais +8 dans les zones voisines (déplacement) ; `Loy` −3 pour l'équipe ; la presse parle : R25 possible.
- **Journal :** *« Cent fonctionnaires, des fourgons alignés. À midi, tout est fini. Le soir, la rue d'à côté s'est remplie. »*
- **Lien :** Commandant Orsini (`10`), La Tortue (`10`).

**R25 · La ville à la une** *(médias : fictifs uniquement ; aucun titre réel)*
- **Déclencheur :** après R22 ou R24 (`chance` 0,50), ou un cycle de forte `Pe`.
- **Quartiers :** toute la ville.
- **Effets (4 sem.) :** `Pr` +6 en ville ; `Rép` −2 à l'extérieur, +2 pour les habitants que la presse défend ; `Év` +3 sur les façades visibles ; une scène possible : la Pigiste (`10`) vend une enquête, Hugo Tavernier (n° 8 de `21`) la place en une.
- **Journal :** *« Les caméras s'installent sur la Canebière. Un reporter dit "Marseille" trois fois en une minute. »*
- **Lien :** La Pigiste (`10`), Hugo Tavernier.

### La politique, le sport, l'ascenseur

**R26 · La campagne locale** *(élu fictif : le Ruban, `10`)*
- **Déclencheur :** une année sur six, 8 semaines.
- **Quartiers :** tous ; surtout 1er, 3e, 14e.
- **Effets (8 sem.) :** `Pr` +4 puis −4 après le scrutin ; promesses : `Rép` +2 pour le Ruban (à écrire comme un personnage fictif) ; les tracts disent « sécurité » ; `Dua` −2 (les regards se tournent ailleurs) ; une rencontre possible : un conseiller propose un *service* contre un « coup de pouce » (scène à écrire, **sans détail**).
- **Journal :** *« Des affiches sur les palissades. Le Ruban parle d'ordre à la radio. Les ascenseurs restent en panne. »*
- **Lien :** Le Ruban (`10`).

**R27 · Un grand événement sportif** *(générique : championnat, régate, tournoi)*
- **Déclencheur :** une ou deux fois sur la vie ; planifié.
- **Quartiers :** Prado, Vélodrome, Vieux-Port, Roucas-Blanc (8e, 9e, 1er, 7e).
- **Effets (3 sem.) :** `Pr` +8 en centre-ville ; `Rev×` 1,20 pour les cafés ; transport saturé (voir R08 sans la grève) ; `Voi` +3 ; un charbon visible est gelé (`Év` +4) ; `Rép` +2 pour qui laisse la place aux familles.
- **Journal :** *« Des tribunes, des écrans, des drapeaux. Les agents sont partout, pour trois semaines. »*
- **Lien :** Marianne Léotard (n° 17 de `21`), qui voit les familles se déplacer pour les visites.

**R28 · L'ascenseur en panne depuis des mois** *(l'état d'un service, pas un reproche)*
- **Déclencheur :** `chance` 0,10 par an dans une tour ; durée : continue jusqu'à réparation (`evtAscenseurSem` = 30).
- **Quartiers :** Saint-Just, Malpassé, La Rose (14e, 13e).
- **Effets (continu) :** `Rép` +1 par semaine pour qui monte les courses (E44) ou paie un dépannage (−600 €) ; `Usu` +1 pour les vieux ; `Pe` −1 ; le dépannage par le jeu ne remplace pas la réparation (indicateur : `Dua` +2) ; une panne résolue par l'office public (hors écran) donne `Rép` +2 à la Vigie, pas au joueur.
- **Journal :** *« Quatorzième mois de panne. La Vigie a photographié chaque palier. Elle monte à pied. »*
- **Lien :** Fatma Oulad (n° 13 de `21`), La Vigie (`10`).

**R29 · Les urgences saturées** *(un fait hospitalier hivernal, jamais un reproche)*
- **Déclencheur :** hiver, épidémie de grippe ; `chance` 0,15.
- **Quartiers :** autour des hôpitaux publics du 15e, du 14e et du 5e.
- **Effets (3 sem.) :** attente aux urgences : l'action « Se soigner » (`20` §2) coûte +2 jours de jeu ; `Usu` +3 par semaine pour tous ; `Foy` −2 (les proches s'inquiètent) ; `Rép` +3 pour qui paie un médecin à domicile (−300 €).
- **Journal :** *« Aux urgences, la salle d'attente déborde. On y reste six heures, sans manger. »*
- **Lien :** Docteure Inès Marrou (n° 18), Soraya Benkhelifa.

**R30 · Jours d'alerte à l'air autour du port**
- **Déclencheur :** été, vent faible, `chance` 0,08.
- **Quartiers :** Joliette, Arenc, L'Estaque, Mourepiane (2e, 15e, 16e).
- **Effets (2 sem.) :** `Usu` +2 pour les asthmatiques (proches et équipiers, sans diagnostic montré) ; `Voi` +2 ; `Rev×` 0,95 ; la Joliette : `Rép` −2.
- **Journal :** *« L'air est lourd sur les quais. Les paquebots tournent leurs moteurs, les volets restent fermés. »*
- **Lien :** Roger Mazet (n° 11 de `21`), ancien docker.

## 4. Ce qu'on ne fait pas
- Aucun de ces événements ne **nomme** un auteur réel, une victime réelle, une bande réelle, une affaire, un journal, un élu, une opération, ou une date.
- Aucune fusillade n'est **montrée** : R22 ne commence que le matin d'après.
- Aucune institution réelle n'est **accusée** : un service qui manque (R20, R21, R28, R29) est un **fait de ville**, exprimé par une chaise vide, un papier sur une porte, une file d'attente.
- Aucun événement ne dit **comment** profiter du chaos (R07, R18, R19, R26). Le jeu montre l'écart (`Dua`, `Év`), jamais le procédé.

## 5. À régler et à vérifier
1. **Fréquence :** au plus **un événement de ville majeur** par semaine, au plus **deux** par mois ; R09 à R14 sont du calendrier (fixes), R01 à R08 sont tirés au hasard avec `chance`.
2. **Équilibre :** les événements négatifs ne doivent pas se cumuler plus de 3 en même temps (R01 + R03 + R07, par exemple) : prévoir un plafond (`evtCumulMax` = 3).
3. **Relecture externe** (comme `12` §10) pour R14 (religion), R22 et R23 (deuil et violence) et R20 (école).
4. **Vérifier hors du jeu :** les périodes réelles (semaines d'incendie, mois de mistral, calendrier de Ramadan) et les noms des institutions avant toute publication : aucune recherche web n'a été faite pour ce document.
5. **Rien n'est codé :** pour l'implémentation, partir de `SeasonSettings` (`GameConfig.Events`) et de `GameEventSystem`.
