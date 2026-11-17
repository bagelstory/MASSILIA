// MARSEILLE — Systems/CrewRules.cs
// Règles pures de la hiérarchie (design/15-hierarchie.md) : lecture et écriture des postes d'un charbon,
// rendement selon l'équipe, recherche d'un charbon, arrondissements. Aucune technique : des postes et des chiffres.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Calculs sur les postes, les arrondissements et leurs chefs.</summary>
    public static class CrewRules
    {
        /// <summary>Contenu d'un poste.</summary>
        /// <param name="poi">Charbon.</param>
        /// <param name="slot">Poste (0-4).</param>
        /// <returns>Id d'opérateur, <see cref="CrewSlots.Player"/> ou <see cref="CrewSlots.Empty"/>.</returns>
        public static int GetSlot(PoiData poi, int slot)
        {
            switch (slot)
            {
                case CrewSlots.Guetteur1: return poi.slotGuetteur1;
                case CrewSlots.Guetteur2: return poi.slotGuetteur2;
                case CrewSlots.Vendeur: return poi.slotVendeur;
                case CrewSlots.Gerant: return poi.slotGerant;
                default: return poi.slotHead;
            }
        }

        /// <summary>Copie du charbon avec un poste modifié.</summary>
        /// <param name="poi">Charbon.</param>
        /// <param name="slot">Poste (0-4).</param>
        /// <param name="value">Nouvelle valeur.</param>
        /// <returns>Le charbon modifié.</returns>
        public static PoiData SetSlot(PoiData poi, int slot, int value)
        {
            switch (slot)
            {
                case CrewSlots.Guetteur1: poi.slotGuetteur1 = value; break;
                case CrewSlots.Guetteur2: poi.slotGuetteur2 = value; break;
                case CrewSlots.Vendeur: poi.slotVendeur = value; break;
                case CrewSlots.Gerant: poi.slotGerant = value; break;
                default: poi.slotHead = value; break;
            }
            return poi;
        }

        /// <summary>Poids d'un poste dans le rendement.</summary>
        /// <param name="slot">Poste.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Poids.</returns>
        public static float SlotWeight(int slot, GameConfig config)
        {
            switch (slot)
            {
                case CrewSlots.Guetteur1:
                case CrewSlots.Guetteur2: return config.crewWeightGuetteur;
                case CrewSlots.Vendeur: return config.crewWeightVendeur;
                case CrewSlots.Gerant: return config.crewWeightGerant;
                default: return config.crewWeightHead;
            }
        }

        /// <summary>Rendement d'un charbon selon son équipe (1 = équipe complète, moins si des postes sont vides).</summary>
        /// <param name="poi">Charbon.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Multiplicateur de revenu (0-1).</returns>
        public static float Efficiency(PoiData poi, GameConfig config)
        {
            float total = 0f, missing = 0f;
            for (int s = 0; s < CrewSlots.Count; s++)
            {
                float w = SlotWeight(s, config);
                total += w;
                if (GetSlot(poi, s) == CrewSlots.Empty) missing += w;
            }
            if (total <= 0f) return 1f;
            return System.Math.Max(0f, 1f - config.crewMissingPenalty * missing / total);
        }

        /// <summary>Nombre de guetteurs manquants.</summary>
        /// <param name="poi">Charbon.</param>
        /// <returns>0, 1 ou 2.</returns>
        public static int MissingGuetteurs(PoiData poi)
        {
            return (poi.slotGuetteur1 == CrewSlots.Empty ? 1 : 0) + (poi.slotGuetteur2 == CrewSlots.Empty ? 1 : 0);
        }

        /// <summary>Cherche un charbon dans tous les quartiers.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="poiId">Identifiant du charbon.</param>
        /// <param name="qi">Index du quartier, -1 si introuvable.</param>
        /// <param name="pi">Index du charbon dans le quartier, -1 si introuvable.</param>
        public static void FindPoi(GameData data, int poiId, out int qi, out int pi)
        {
            for (int i = 0; i < data.quarters.Count; i++)
            {
                List<PoiData> pois = data.quarters[i].pois;
                for (int p = 0; p < pois.Count; p++)
                    if (pois[p].id == poiId) { qi = i; pi = p; return; }
            }
            qi = -1;
            pi = -1;
        }

        /// <summary>Identifiant du quartier d'un charbon.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="poiId">Charbon.</param>
        /// <returns>Identifiant de quartier, ou -1.</returns>
        public static int QuarterOfPoi(GameData data, int poiId)
        {
            int qi, pi;
            FindPoi(data, poiId, out qi, out pi);
            return qi >= 0 ? data.quarters[qi].id : Owners.None;
        }

        /// <summary>Nombre de charbons qu'une tête tient.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="headId">Id d'opérateur, ou <see cref="CrewSlots.Player"/>.</param>
        /// <returns>Nombre de charbons.</returns>
        public static int HeadCount(GameData data, int headId)
        {
            int n = 0;
            for (int i = 0; i < data.quarters.Count; i++)
                for (int p = 0; p < data.quarters[i].pois.Count; p++)
                    if (data.quarters[i].pois[p].slotHead == headId) n++;
            return n;
        }

        /// <summary>Les arrondissements présents dans les quartiers, chefs vides.</summary>
        /// <param name="data">Partie modifiée.</param>
        public static void BuildArrondissements(ref GameData data)
        {
            if (data.arrondissements == null) data.arrondissements = new List<ArrondissementData>();
            for (int i = 0; i < data.quarters.Count; i++)
            {
                int id = data.quarters[i].arrondissement;
                if (IndexOfArrondissement(data, id) >= 0) continue;
                data.arrondissements.Add(new ArrondissementData { id = id, chiefOwnerId = Owners.None, chiefWeight = 0f });
            }
            data.arrondissements.Sort((a, b) => a.id.CompareTo(b.id));
        }

        /// <summary>Index d'un arrondissement dans la liste.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="id">Numéro d'arrondissement.</param>
        /// <returns>Index, ou -1.</returns>
        public static int IndexOfArrondissement(GameData data, int id)
        {
            if (data.arrondissements == null) return -1;
            for (int i = 0; i < data.arrondissements.Count; i++) if (data.arrondissements[i].id == id) return i;
            return -1;
        }

        /// <summary>Écriture d'un numéro d'arrondissement : « 1er », « 9e ».</summary>
        /// <param name="n">Numéro (1-16).</param>
        /// <returns>Texte.</returns>
        public static string ArrondissementLabel(int n)
        {
            return n == 1 ? "1er" : n + "e";
        }

        /// <summary>Nom d'un chef pour l'affichage.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="ownerId">Propriétaire.</param>
        /// <returns>Texte : « toi », nom de la bande, ou « personne ».</returns>
        public static string ChiefName(GameData data, int ownerId)
        {
            if (ownerId == Owners.Player) return "toi";
            int fi = data.IndexOfFaction(ownerId);
            return fi >= 0 ? data.factions[fi].name : "personne";
        }
    }
}
