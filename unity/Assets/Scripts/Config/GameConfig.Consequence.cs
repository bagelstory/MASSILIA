// MARSEILLE — Config/GameConfig.Consequence.cs
// Section « conséquences » de GameConfig (design/20-consequences.md) : tous les boutons de réglage.
// Valeurs de départ à calibrer par tests ; rien n'est en dur dans les systèmes.

using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        [Header("Conséquences : justice")]
        [Tooltip("Pression à partir de laquelle une garde à vue devient possible.")]
        [Range(40f, 90f)] public float justiceGavSeuilPression = 60f;
        [Tooltip("Chance par semaine d'une garde à vue quand la Pression dépasse le seuil (augmente avec le Casier).")]
        [Range(0f, 0.5f)] public float justiceGavChance = 0.05f;
        [Tooltip("Casier ajouté par une garde à vue.")]
        [Min(0f)] public float justiceCasierGav = 4f;
        [Tooltip("Casier ajouté par une condamnation avec sursis.")]
        [Min(0f)] public float justiceCasierSursis = 15f;
        [Tooltip("Casier ajouté par une condamnation ferme.")]
        [Min(0f)] public float justiceCasierFerme = 30f;
        [Tooltip("Chance qu'une garde à vue ouvre une affaire (procès).")]
        [Range(0f, 1f)] public float justiceChanceAffaire = 0.35f;
        [Tooltip("Semaines entre l'ouverture de l'affaire et le procès.")]
        [Min(1)] public int justiceDelaiProces = 26;
        [Tooltip("Chance de relaxe au procès.")]
        [Range(0f, 1f)] public float justiceChanceRelaxe = 0.2f;
        [Tooltip("Chance de sursis au procès (sinon : peine ferme).")]
        [Range(0f, 1f)] public float justiceChanceSursis = 0.5f;
        [Tooltip("Durée du sursis (semaines) ; un fait grave pendant le sursis le rend ferme.")]
        [Min(26)] public int justiceSursisDuree = 156;
        [Tooltip("Pression qui, pendant un sursis, peut le faire sauter.")]
        [Range(40f, 100f)] public float justiceSursisPression = 70f;
        [Tooltip("Chance par semaine que le sursis saute quand la Pression dépasse le seuil.")]
        [Range(0f, 0.5f)] public float justiceSursisChance = 0.05f;
        [Tooltip("Peine ferme minimale (semaines).")]
        [Min(1)] public int justicePeineMin = 26;
        [Tooltip("Peine ferme maximale (semaines) pour un Casier bas.")]
        [Min(1)] public int justicePeineCourteMax = 104;
        [Tooltip("Peine ferme maximale (semaines) pour un Casier élevé.")]
        [Min(1)] public int justicePeineLongueMax = 520;
        [Tooltip("Casier à partir duquel la peine peut être longue.")]
        [Range(0f, 100f)] public float justiceSeuilPeineLongue = 50f;
        [Tooltip("Part des revenus perdue pendant une détention.")]
        [Range(0f, 1f)] public float justicePerteDetention = 0.5f;
        [Tooltip("Frais d'avocat par affaire : minimum et maximum (€).")]
        public Vector2 justiceAvocat = new Vector2(1500f, 6000f);
        [Tooltip("Une visite au parloir toutes les N semaines.")]
        [Min(1)] public int justiceParloirCadence = 2;
        [Tooltip("Foyer gagné par visite au parloir.")]
        [Min(0f)] public float justiceParloirFoyer = 3f;
        [Tooltip("Usure retirée par visite au parloir.")]
        [Min(0f)] public float justiceParloirUsure = 2f;
        [Tooltip("Exposition des proches ajoutée par visite au parloir.")]
        [Min(0f)] public float justiceParloirExposition = 2f;
        [Tooltip("Usure ajoutée par an de détention.")]
        [Min(0f)] public float justiceDetentionUsureAn = 6f;

        [Header("Conséquences : santé et corps")]
        [Tooltip("Usure de base ajoutée par an, avant puis après l'âge de bascule.")]
        public Vector2 santeUsureAn = new Vector2(3f, 4f);
        [Tooltip("Âge à partir duquel l'usure annuelle augmente.")]
        [Min(18)] public int santeAgeBascule = 35;
        [Tooltip("Usure par semaine et par point de Pression au-dessus de la base.")]
        [Min(0f)] public float santeUsureTension = 0.05f;
        [Tooltip("Pression au-dessus de laquelle la tension use le corps.")]
        [Range(0f, 100f)] public float santeTensionBase = 40f;
        [Tooltip("Pression sous laquelle une semaine est calme (repos possible).")]
        [Range(0f, 100f)] public float santeCalmePression = 25f;
        [Tooltip("Usure retirée une semaine calme (hors détention).")]
        [Min(0f)] public float santeRepos = 2f;
        [Tooltip("Usure à partir de laquelle le rendement baisse.")]
        [Range(30f, 70f)] public float santeSeuilGene = 50f;
        [Tooltip("Part des revenus perdue au-dessus du seuil de gêne.")]
        [Range(0f, 0.5f)] public float santeMalusRendement = 0.1f;
        [Tooltip("Usure à partir de laquelle un événement médical devient possible.")]
        [Range(60f, 90f)] public float santeSeuilAlerte = 75f;
        [Tooltip("Chance par semaine de l'événement médical au-dessus du seuil d'alerte.")]
        [Range(0f, 0.3f)] public float santeAlerteChance = 0.04f;
        [Tooltip("Usure retirée par une consultation et du repos.")]
        [Min(0f)] public float santeSoinUsure = 8f;
        [Tooltip("Coût d'une consultation (€).")]
        [Min(0f)] public float santeSoinCout = 600f;
        [Tooltip("Foyer à partir duquel on dort mieux (l'usure monte moins).")]
        [Range(0f, 100f)] public float santeFoyerAbri = 70f;
        [Tooltip("Part de l'usure gagnée qui est évitée quand le foyer est solide.")]
        [Range(0f, 0.5f)] public float santeFoyerAbriPart = 0.1f;

        [Header("Conséquences : famille")]
        [Tooltip("Foyer au premier cycle.")]
        [Range(0f, 100f)] public float familleFoyerDepart = 60f;
        [Tooltip("Semaines sans temps privé avant que le lien s'use.")]
        [Min(1)] public int familleAbsenceSeuil = 4;
        [Tooltip("Foyer perdu par semaine quand le lien s'use.")]
        [Min(0f)] public float familleAbsence = 1f;
        [Tooltip("Foyer gagné par repas partagé (action « temps privé »).")]
        [Min(0f)] public float familleRepas = 3f;
        [Tooltip("Âge à partir duquel la mère a besoin de soins.")]
        [Min(18)] public int familleMereAge = 28;
        [Tooltip("Coût mensuel des soins de la mère (€).")]
        [Min(0f)] public float familleMereCoutMois = 300f;
        [Tooltip("Foyer sous lequel le lien est très fragile.")]
        [Range(0f, 100f)] public float familleFoyerFragile = 25f;
        [Tooltip("Semaines de foyer très fragile avant le départ du partenaire.")]
        [Min(1)] public int familleDepartSemaines = 26;
        [Tooltip("Stabilité retirée au départ du partenaire.")]
        [Min(0f)] public float familleDepartStabilite = 20f;
        [Tooltip("Foyer sous lequel la vie privée pèse sur le reste (Loyauté, envie de partir).")]
        [Range(0f, 100f)] public float familleFoyerBas = 30f;

        [Header("Conséquences : réputation et voisinage")]
        [Tooltip("Contrôle minimal pour qu'un quartier ait une réputation.")]
        [Range(0f, 100f)] public float repControlMin = 30f;
        [Tooltip("Retour de la réputation vers 0, par semaine.")]
        [Min(0f)] public float repDecay = 1f;
        [Tooltip("Peur du quartier à partir de laquelle la réputation baisse.")]
        [Range(0f, 100f)] public float repPeurSeuil = 60f;
        [Tooltip("Réputation perdue par semaine quand la peur dépasse le seuil.")]
        [Min(0f)] public float repPeurPerte = 0.5f;
        [Tooltip("Réputation au-dessus de laquelle les habitants se taisent pour toi.")]
        [Range(0f, 100f)] public float repSeuilTemoin = 40f;
        [Tooltip("Réputation sous laquelle les habitants parlent aux enquêteurs.")]
        [Range(-100f, 0f)] public float repSeuilDelation = -40f;
        [Tooltip("Part des revenus du quartier perdue sous le seuil de délation (boycott).")]
        [Range(0f, 0.5f)] public float repBoycott = 0.2f;
        [Tooltip("Voisinage ajouté par semaine et par charbon ouvert sans guetteur discret.")]
        [Min(0f)] public float voiBruit = 2f;
        [Tooltip("Retour du voisinage vers le calme, par semaine, quand rien ne se voit.")]
        [Min(0f)] public float voiCalme = 0.5f;
        [Tooltip("Voisinage au-dessus duquel une pétition éclate.")]
        [Range(0f, 100f)] public float voiPetition = 60f;

        [Header("Conséquences : dettes")]
        [Tooltip("Intérêt hebdomadaire de la dette personnelle (0,005 = 0,5 %).")]
        [Range(0f, 0.05f)] public float detteTaux = 0.005f;
        [Tooltip("Dette personnelle à partir de laquelle l'alerte est donnée (€).")]
        [Min(0f)] public float detteSeuilAlerte = 25000f;
        [Tooltip("Part de la trésorerie utilisée chaque semaine pour rembourser.")]
        [Range(0f, 0.5f)] public float detteRemboursementPart = 0.1f;
        [Tooltip("Coût hebdomadaire d'un proche en difficulté (€), quand l'exposition est haute.")]
        [Min(0f)] public float detteProche = 80f;

        [Header("Conséquences : double vie et proches")]
        [Tooltip("Revenu hebdomadaire (€) qui fait monter la double vie de 1 par semaine.")]
        [Min(100f)] public float duaRevenuParPoint = 2000f;
        [Tooltip("Double vie retirée par semaine et par façade ouverte.")]
        [Min(0f)] public float duaFacadeBaisse = 1f;
        [Tooltip("Double vie à partir de laquelle une enquête financière s'ouvre.")]
        [Range(40f, 85f)] public float duaSeuilEnquete = 60f;
        [Tooltip("Pression ajoutée par semaine pendant une enquête financière.")]
        [Min(0f)] public float duaEnquetePression = 1f;
        [Tooltip("Double vie à partir de laquelle la vie normale s'effondre.")]
        [Range(60f, 100f)] public float duaSeuilRuine = 90f;
        [Tooltip("Exposition des proches ajoutée par semaine quand la double vie dépasse 50.")]
        [Min(0f)] public float expMontee = 2f;
        [Tooltip("Exposition retirée par semaine quand la double vie est basse.")]
        [Min(0f)] public float expRetombee = 0.5f;
        [Tooltip("Exposition des proches à partir de laquelle ils prennent leurs distances.")]
        [Range(40f, 80f)] public float expSeuilEloignement = 60f;
        [Tooltip("Foyer retiré par semaine au-dessus du seuil d'éloignement.")]
        [Min(0f)] public float expPerteFoyer = 0.5f;
    }
}
