// MARSEILLE — Data/Enums.cs
// Énumérations partagées par toutes les données du jeu.
// JsonUtility sérialise les enums en entiers : ne jamais réordonner les valeurs existantes,
// ajouter les nouvelles valeurs à la fin pour garder les sauvegardes compatibles.

namespace Marseille.Data
{
    /// <summary>Spécialité d'un opérateur (spec §4.2).</summary>
    [System.Serializable]
    public enum OperatorType
    {
        /// <summary>Repérage : augmente la connaissance d'un quartier.</summary>
        Recon = 0,
        /// <summary>Pression : augmente le contrôle et les revenus.</summary>
        Pressure = 1,
        /// <summary>Logistique : réduit les coûts, alimente les flux internationaux.</summary>
        Logistics = 2,
        /// <summary>Finances : réduit les coûts de maintien et l'exposition.</summary>
        Finance = 3,
        /// <summary>Intelligence : révèle les intentions des factions.</summary>
        Intel = 4,
        /// <summary>Force : réduit la présence adverse.</summary>
        Force = 5
    }

    /// <summary>Disponibilité d'un opérateur (spec §4.3).</summary>
    [System.Serializable]
    public enum OperatorStatus
    {
        /// <summary>Sur place, sans tâche.</summary>
        Available = 0,
        /// <summary>En déplacement vers un autre quartier.</summary>
        InTransit = 1,
        /// <summary>Sur place, exécute une tâche.</summary>
        Occupied = 2,
        /// <summary>Épuisé (fatigue à 100) : ne peut rien faire ce cycle.</summary>
        Unavailable = 3,
        /// <summary>Perdu (arrêté, écarté, en fuite) : ne revient pas.</summary>
        Lost = 4,
        /// <summary>Incarcéré aux Baumettes : agit seulement par le parloir (ordres en retard).</summary>
        Jailed = 5
    }

    /// <summary>Tâche qu'un opérateur exécute dans un quartier (spec §4.5).</summary>
    [System.Serializable]
    public enum OperatorTask
    {
        /// <summary>Aucune tâche : l'opérateur récupère de la fatigue.</summary>
        Rest = 0,
        /// <summary>Sécuriser (Force).</summary>
        Secure = 1,
        /// <summary>Presser (Pression).</summary>
        Press = 2,
        /// <summary>Repérer (Repérage).</summary>
        Scout = 3,
        /// <summary>Surveiller (Intelligence).</summary>
        Watch = 4,
        /// <summary>Gérer les flux (Logistique).</summary>
        ManageFlux = 5,
        /// <summary>Optimiser (Finances).</summary>
        Optimize = 6
    }

    /// <summary>Objectif courant de l'IA d'une faction (spec §5.2).</summary>
    [System.Serializable]
    public enum FactionObjective
    {
        /// <summary>Gagner un quartier voisin faible.</summary>
        Expand = 0,
        /// <summary>Conserver ses effectifs et ses revenus.</summary>
        Maintain = 1,
        /// <summary>Reprendre un quartier attaqué.</summary>
        CounterAttack = 2,
        /// <summary>Se replier sur ses quartiers les plus sûrs.</summary>
        Retract = 3
    }

    /// <summary>Nature d'un point d'intérêt de quartier.</summary>
    [System.Serializable]
    public enum PoiKind
    {
        /// <summary>Commerce (revenu faible, risque faible).</summary>
        Shop = 0,
        /// <summary>Dépôt (logistique).</summary>
        Depot = 1,
        /// <summary>Lieu stratégique (sans revenu direct).</summary>
        Strategic = 2,
        /// <summary>Charbon : point de vente abstrait, rentabilité et risque variables.</summary>
        Charbon = 3
    }

    /// <summary>Catégorie d'une ligne de journal (sert à la couleur dans l'UI).</summary>
    [System.Serializable]
    public enum JournalType
    {
        Info = 0, Gain = 1, Loss = 2, Attack = 3, OperatorLost = 4, Police = 5,
        Flux = 6, Economy = 7, Quest = 8, Faction = 9, Season = 10, End = 11, Shadow = 12
    }

    /// <summary>Type d'objectif à atteindre dans une quête chiffrée.</summary>
    [System.Serializable]
    public enum QuestObjectiveType
    {
        /// <summary>Nombre de quartiers avec un contrôle au moins égal à <c>threshold</c>.</summary>
        QuartersHeld = 0,
        /// <summary>Nombre de quartiers d'une zone avec une connaissance au moins égale à <c>threshold</c>.</summary>
        ZoneKnowledge = 1,
        /// <summary>Nombre de quartiers (toute la ville) avec une connaissance au moins égale à <c>threshold</c>.</summary>
        Knowledge = 2,
        /// <summary>Nombre de charbons ouverts au joueur.</summary>
        OpenCharbons = 3,
        /// <summary>Trésorerie minimale (€).</summary>
        Treasury = 4,
        /// <summary>Nombre de quartiers sécurisés.</summary>
        SecuredQuarters = 5,
        /// <summary>Nombre d'opérateurs actifs.</summary>
        Operators = 6,
        /// <summary>Territoire minimal (%).</summary>
        Territory = 7,
        /// <summary>Nombre de flux internationaux actifs.</summary>
        ActiveFluxes = 8
    }

    /// <summary>Manière dont la partie s'est terminée (spec §11).</summary>
    [System.Serializable]
    public enum EndType
    {
        /// <summary>La partie continue.</summary>
        None = 0,
        Victory = 1,
        Bankruptcy = 2,
        Collapse = 3,
        Abandon = 4,
        TimeOut = 5,
        /// <summary>Retraite choisie par le joueur après la durée minimale (ajouté en dernier : enum sérialisée en entier).</summary>
        Retirement = 6
    }
}
