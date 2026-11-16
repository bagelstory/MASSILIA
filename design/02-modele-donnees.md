# CHOUF — Étape 2 : modèle de données

Références : `MARSEILLE_GDD_V6_GITHUB.txt` (vision, règles), GDD V7 (progression),
`design/01-plan-v1.md` (architecture reprise de KICK). En cas de conflit, ce document suit les
décisions du 16 novembre 2026 :

| Sujet | Décision |
|---|---|
| Titre | **CHOUF** (titre de travail des GDD : MARSEILLE) |
| Période | **2021 → 2026**, 16 ans en 2021, 5 chapitres (GDD V7), puis **« L'après »** |
| Rythme | **Mélange** : énergie en temps réel (= points d'action du GDD) + semaine de campagne que le joueur clôture quand il veut |
| Carte | **10 zones** : les 8 du GDD V3 + La Plaine / Cours Julien + Prado / Castellane |
| Départ | Zone tirée au sort parmi les zones populaires |
| GDD V7 §12 | A : option 3 (signalé si important, sinon bilan) · B : option 3 (régression avec raison narrative) · C : option 3 (tout maximiser possible mais très coûteux) — **recommandations reprises, à confirmer** |

Tout ce qui est chiffré vit dans `data/*.json`. Le code ne contient aucune valeur d'équilibrage.

---

## 1. Les fichiers de données

| Fichier | Contenu |
|---|---|
| `data/equilibrage.json` | énergie, sang-froid, santé, semaine, seuils, coefficients de toutes les formules |
| `data/calendrier.json` | chapitres 2021-2026 + l'après, événements fixes de la ville (rentrée, OM, mistral, été…) |
| `data/competences.json` | 5 compétences × 4 paliers, textes affichés |
| `data/zones.json` | 10 zones, leurs quartiers officiels, valeurs de départ |
| `data/personnages.json` | personnages importants (tous inventés) : famille, quartier, organisation, institutions |
| `data/roles.json` | rôles de l'équipe |
| `data/equipes.json` | équipes rivales inventées |
| `data/economie.json` | niveaux de marchandise abstraits, façades légales, réseaux internationaux abstraits |
| `data/police.json` | pression, enquête, procédures, peines |
| `data/missions.json` | missions de zone (façon « jobs » de Mafia Wars) |
| `data/evenements.json` | événements à conditions, options, conséquences immédiates et différées, mémoire |
| `tests/lint/interdits.json` | noms réels interdits (personnes, équipes, familles du dossier de recherche) — jamais embarqué dans le jeu |

`tools/check_data.py` vérifie : JSON valide, identifiants uniques, toutes les références existent
(une zone citée par un personnage, un personnage cité par un événement…), aucune valeur hors bornes.

---

## 2. Le temps (rythme « mélange »)

Deux horloges, toutes deux injectées (`now()` pour le temps réel, `G.cal` pour la campagne) afin que le
simulateur puisse les accélérer.

**Temps réel** (comme Mafia Wars) :
- `energie` : 0..`energie.max`, +1 toutes les `energie.regen_min` minutes. Les actions coûtent de
  l'énergie. C'est l'équivalent des « points d'action » du GDD : on ne peut pas tout faire.
- `sangfroid` : même principe, pour les confrontations et les situations tendues.
- `chantiers` : chacun a une heure de fin réelle (ouvrir une façade, former quelqu'un, préparer un
  engagement).

**Temps de campagne** (comme le GDD) :
- `G.cal = {annee, semaine, jour}`. Le joueur appuie sur **« Clore la semaine »** quand il veut.
- La clôture résout la semaine : revenus des zones, police, rivaux, personnages qui agissent seuls,
  conséquences différées arrivées à échéance, humeur des quartiers, bilan (mensuel toutes les 4
  semaines, annuel à la dernière semaine).
- Clore une semaine sans agir est permis : l'équipe et les zones tournent seules, avec plus de risques
  (pas de décision du protagoniste, pas de ramassage).
- 2021-2026 = environ 300 semaines. Les périodes calmes peuvent s'enchaîner vite.

**« Pendant ton absence »** : au retour, l'écran résume ce qui a changé en temps réel (énergie remplie,
chantiers terminés, messages reçus). Le calendrier, lui, n'avance que par les clôtures.

---

## 3. L'état de partie `G` (sauvegardé tel quel)

Structure du GDD V7, complétée par la gestion (zones, équipe, police, argent).

```js
G = {
  v: 1,                         // version de sauvegarde (migrations par numéro)
  seed: 12345,                  // hasard reproductible (tests, rejouer un événement)
  cal: { annee: 2021, semaine: 37, jour: 1 },   // rentrée 2021
  chapitre: "le_debut",         // id dans calendrier.json
  temps: { energie: 20, sangfroid: 10, maj: 1763290000000 },   // dernière mise à jour réelle (ms)

  protagoniste: {
    prenom: "…", naissance: { annee: 2005, semaine: 12 },      // âge calculé, jamais stocké
    zone_origine: "nord_est",
    competences: { communication: 20, organisation: 15, sang_froid: 10, leadership: 5, autonomie: 15 },
    etat: { stabilite: 70, pression: 20, sante: 90, stress: 20 },
    scolarite: { statut: "lycee", assiduite: 70 },          // lycée, apprentissage, décrochage, diplôme
    positionnement: { respect: 5, influence: 0, crainte: 0 },   // GDD V6 §5 : quatre valeurs séparées
    responsabilites: [],        // ids débloqués (voir §6)
    engagements: []             // promesses en cours : { id, envers, echeance, tenu: null }
  },

  argent: { propre: 120, sale: 0, dettes: [] },            // en euros entiers

  relations: {                  // une entrée par personnage connu
    "mere": { confiance: 75, proximite: 80, tension: 10, memoire: ["a_promis_bac"], etat: "present" }
  },

  zones: {                      // copie vivante de zones.json
    "nord_est": { connu: 3, acces: 2, influence: 0, tension: 40, police: 45, humeur: 50,
                  plans: [], stock: 0, coffre: 0, controle: "compagnie" }
  },

  organisation: {               // inexistante au début : se crée au chapitre 3 ou plus tôt
    existe: false, nom: null, stabilite: 50, confiance_interne: 50, resilience: 40, ancrage: 50,
    membres: [],                // { pnj, role, zone, loyaute, fatigue, peur, statut }
    facades: [],                // { type, zone, niveau, chantier_fin }
    reseaux: []                 // engagements internationaux abstraits (economie.json)
  },

  police: { pression_ville: 30, enquetes: [], gav: 0, proces: null, peine: null },

  rivaux: { "compagnie": { force: 90, zones: ["nord_ouest", "nord_est"], relation: "domine", dette: 0 } },

  progression: {
    drapeaux: [],               // faits de campagne : "premiere_gav", "frere_au_courant"…
    irreversibles: [],          // faits qui ne s'effacent jamais (deuil, condamnation…)
    options: [],                // options de dialogue débloquées
    signaux: {}                 // trajectoires : { direction: 3, reconstruction: 1, … } (GDD V7 §8)
  },

  evenements: {
    faits: { "le_quotidien": { semaine: 3, option: 1 } },
    programmes: [ { id: "promesse_oubliee", a: { annee: 2021, semaine: 49 }, ctx: { pnj: "sofiane" } } ],
    vus: {}                     // dernier tirage, pour ne pas répéter
  },

  journal: [],                  // { quand, texte, type, cause } : affiché, sert aussi à l'explication des indicateurs
  messages: [],                 // téléphone
  carte: null                   // événement ouvert (même rôle que G.card dans KICK)
}
```

Règles techniques (GDD V7 §9) : changer de chapitre n'efface rien ; les irréversibles sont listés à part ;
toute variation d'indicateur passe par `change(chemin, delta, cause)` qui écrit la cause dans le journal
(règle UX « expliquer la cause connue », ou « cause incertaine » si le protagoniste ne la connaît pas).

---

## 4. Zones (`zones.json`)

Chaque zone : `id, nom, arrondissements, quartiers[]` (quartiers officiels réels), `lieux[]` (lieux réels :
places, stations, plages, stades), `transports[]`, `services[]` (commerçants, associations, mères de
famille, jeunes, anciens), et les valeurs de départ :

| Champ | Sens | Bornes |
|---|---|---|
| `revenu` | potentiel de revenu abstrait par semaine | €, 0..∞ |
| `population` | taille relative | 1..5 |
| `tension` | conflits en cours | 0..100 |
| `police` | présence policière | 0..100 |
| `humeur` | humeur du quartier envers les réseaux | 0..100 |
| `controle` | équipe qui tient la zone au départ | id d'équipe ou `null` |
| `depart` | peut être tirée au sort comme zone de départ | bool |

Pour le joueur, chaque zone a aussi `connu` (0 visible → 4 pleinement exploitable, GDD V6 §2.2) et
`acces` (actions possibles). Le déblocage se fait par événements et personnages, pas par score.

## 5. Personnages (`personnages.json`)

Tous **inventés**, composites. Champs : `id, prenom, nom (facultatif), age_2021, zone, groupe`
(`famille`, `quartier`, `organisation`, `institution`, `ville`), `traits[]`, `objectif`, `limites[]`,
`relation_depart {confiance, proximite, tension}`, `liens {autre_id: nature}`, `portrait` (planche Gemini),
`autonome` (règles simples : ce qu'il fait seul à la clôture de semaine selon son état).

Le prologue en contient 10 : la mère, le petit frère, la grande sœur, l'ami d'enfance, le grand du hall,
l'éducatrice du centre social, le patron du snack, la voisine, la professeure principale, le commandant de
police du secteur (fictif). La famille est générée à partir de ces gabarits selon la zone de départ.

## 6. Compétences et responsabilités

`competences.json` : 5 compétences, valeur interne 0..100, 4 paliers (`seuils: [0,25,50,75]`), texte par palier.
Gain : par expérience vécue (option choisie dans un événement), avec rendement décroissant au-delà du
palier 3 (règle « tout maximiser possible mais coûteux »). Perte : seulement par un événement qui la
justifie (prison, deuil, épuisement).

Responsabilités (dans `calendrier.json`, par chapitre) : 4 catégories (personnelle, relationnelle,
collective, organisationnelle). Chaque responsabilité a `conditions_narratives[]` **et**
`conditions_systemiques[]`, plus au moins une `alternative` (GDD V7 §5 : un joueur faible en leadership
n'est jamais bloqué).

## 7. Équipe (`roles.json`)

Rôles abstraits : guetteur, vendeur, nourrice, chauffeur, bras droit, avocat, comptable. Champs : `id,
nom, chapitre_min, salaire_semaine, effet` (ce que le rôle change dans les formules), `risque` (exposition
judiciaire), `cout_humain` (texte : ce que ce rôle fait à la personne qui l'occupe). Un membre est un PNJ
avec loyauté, fatigue, peur, statut (`actif`, `garde_a_vue`, `incarcere`, `parti`, `mort`).

Note de ton : un guetteur ou une nourrice sont presque toujours des gens contraints ou très jeunes. Leurs
cartes le montrent.

## 8. Économie (`economie.json`)

- **Marchandise** : 3 niveaux abstraits (`petite`, `moyenne`, `lourde`) avec `marge`, `bruit`
  (attire la police), `peine` (gravité en cas de procès). Aucun nom de produit, aucune méthode.
- **Revenu de zone par semaine** :
  `revenu_sem = zone.revenu × plans × niveau.marge × (0.5 + humeur/200) × (1 − police/200) × equipe_eff`
  L'argent sale va au `coffre` de la zone ; au-delà de `coffre_max`, chaque semaine non ramassée
  ajoute `risque_coffre` (saisie, vol interne, visite d'un rival).
- **Façades** (snack, salle de boxe, restaurant, appartements) : convertissent une part de l'argent
  sale en propre chaque semaine, plafonnée par le niveau de la façade ; elles augmentent
  l'enquête financière. Abstrait : un taux, un plafond, un risque. Aucun procédé.
- **Réseaux internationaux** (GDD V6 §11) : engagements abstraits avec une origine narrative,
  `volume`, `argent_expose`, `fiabilite`, `exposition`, `instabilite`, et 5 familles de risques.
- **Richesse / liquidité / solidité** calculées pour le tableau de bord.

## 9. Police (`police.json`)

- `pression_ville` (0..100) : monte avec les violences dans la ville (pas seulement les tiennes) ; au-delà
  de `seuil_operation`, une **opération massive** (nom inventé) se déclenche sur plusieurs semaines.
- `enquetes[]` : une par cible (toi, un membre, une façade). Chacune avance chaque semaine selon le
  bruit fait ; invisible au joueur, sauf indices (rumeurs, voiture banalisée, avocat qui prévient).
- Procédures dans l'ordre : contrôle → convocation → garde à vue → mise en examen → détention
  provisoire ou contrôle judiciaire → procès → peine. Chaque étape est une carte avec ses choix (se taire,
  parler, appeler l'avocat, couvrir quelqu'un) et ses conséquences humaines (la mère au parloir, le petit
  frère qui raconte à l'école).
- Mineur (avant 18 ans) : justice des mineurs, éducateurs, famille convoquée ; peines différentes.
- Corruption : un contact acheté réduit une enquête mais crée une dépendance et une enquête interne.

## 10. Équipes rivales (`equipes.json`)

5 équipes **inventées** : une super-structure qui tient la majorité de la ville en 2021, deux équipes
moyennes en guerre, une équipe de quartier indépendante, une jeune équipe qui monte. Champs : `id, nom,
chef (personnage inventé), origine (zone), style` (`expansion`, `defense`, `negociation`,
`violence`), `force`, `argent`, `zones[]`. Chaque semaine elles jouent un coup simple (avancer, tenir,
négocier, frapper), qui suit la chronologie de la ville (montée de la guerre en 2022-2023, opérations
massives en 2024, hégémonie en 2025-2026), sans jamais reproduire une affaire réelle.

## 11. Missions (`missions.json`)

Façon « jobs » de Mafia Wars : `id, zone, chapitre_min, cout {energie, sangfroid}, gains, risques,
maitrise` (3 niveaux, chaque niveau rejoué améliore les gains et la compétence liée), `voix` (une phrase
d'un habitant qui montre le coût). Les missions de 16 ans sont surtout de la vie : livrer pour le snack,
réviser, garder le petit frère, rendre un service au grand du hall (le premier glissement).

## 12. Événements (`evenements.json`)

Format unique (GDD V7 §7) :

```json
{
  "id": "promesse_oubliee",
  "titre": "Une promesse oubliée",
  "declencheur": "programme",            // "tirage" | "programme" | "calendrier" | "seuil"
  "conditions": ["engagement_rompu:{pnj}", "relation:{pnj}.etat!=rompu"],
  "poids": 1, "once": false, "delai_retour": 12,
  "texte": "{pnj.prenom} t'attend en bas du bâtiment. « Tu m'avais dit que tu viendrais. »",
  "options": [
    { "t": "Reconnaître ton erreur",
      "effets": ["relation:{pnj}.confiance+8", "competence:communication+3"],
      "memoire": "{pnj}:a_reconnu" },
    { "t": "Éviter la conversation",
      "effets": ["relation:{pnj}.confiance-12", "relation:{pnj}.tension+10"],
      "programme": [{ "id": "rancune", "dans": 8 }],
      "memoire": "{pnj}:a_fui", "irreversible": false }
  ]
}
```

Conditions et effets sont des chaînes courtes lues par un seul interpréteur (`cond()`, `effet()`), testé
à part. Chaque événement est rejouable en test avec une graine et un état donnés.

---

## 13. Ce qui reste à décider (étape 3 ou plus tard)

- Création du personnage : prénom choisi, reste tiré au sort (proposition).
- Valeurs fines d'équilibrage : posées dans `equilibrage.json`, réglées par la simulation à l'étape 6.
- Liste complète des 111 quartiers : les quartiers des 10 zones sont dans `zones.json` ; la carte de
  niveau 2 complète viendra avec les assets.
- La Castellane (cité) : les sources la placent dans le 15e ou le 16e ; noté « 15e-16e, à vérifier ».
