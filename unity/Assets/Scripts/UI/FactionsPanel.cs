// MARSEILLE — UI/FactionsPanel.cs
// Écran « Factions » (spec §5) : équipes rivales fictives, territoire, effectifs, agressivité, trêves.
// Le budget n'est donné qu'en ordre de grandeur, l'objectif seulement s'il a été repéré (surveillance).

using Marseille.Art;
using Marseille.Config;
using Marseille.Data;
using Marseille.Map;
using UnityEngine;

namespace Marseille.UI
{
    /// <summary>Écran des factions.</summary>
    public class FactionsPanel : ListPanel
    {
        /// <inheritdoc />
        public override string TabLabel { get { return "Rivaux"; } }

        /// <inheritdoc />
        public override string TabIcon { get { return "tab_rivaux"; } }

        /// <summary>Remplit la liste des factions.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        protected override void Fill(GameData data, GameConfig config)
        {
            Title("Équipes rivales", config);
            for (int i = 0; i < data.factions.Count; i++) AddFactionCard(data.factions[i], data, config);
        }

        /// <summary>Carte d'une faction.</summary>
        /// <param name="f">Faction.</param>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        private void AddFactionCard(FactionData f, GameData data, GameConfig config)
        {
            UiCard card = UiCard.Create(Column, 4, config);
            card.SetIcon(ArtLibrary.FactionEmblem(f.id), config.uiFontSizes.z * 4f);
            card.Text.color = f.isAlive ? MapStyle.FromHex(f.colorHex) : config.uiTextDim;
            if (!f.isAlive) { card.Text.text = f.name + " · a disparu de la ville."; return; }
            string truce = f.allianceCyclesLeft > 0
                ? "Trêve avec " + FactionName(data, f.allyFactionId) + " (" + f.allianceCyclesLeft + " cycle(s))"
                : "Pas de trêve";
            card.Text.text = f.name + (f.isFragmented ? " · divisée" : "") + "\n"
                + "Territoire " + f.territory.Count + " quartier(s) · effectifs " + f.effectifs + " · agressivité " + f.aggressiveness + "/5\n"
                + "Budget : " + BudgetRange(f.budget) + " · " + truce + "\n"
                + "Intention : " + Intention(f, data);
            int target = f.targetQuarter;
            if (target != Owners.None && IsRevealed(data, target))
                card.AddButton("Voir la cible", config.uiButton, () => ShowOnMap(target));
        }

        /// <summary>Budget en ordre de grandeur.</summary>
        /// <param name="budget">Budget réel.</param>
        /// <returns>Texte.</returns>
        private static string BudgetRange(float budget)
        {
            if (budget < 5000f) return "faible";
            if (budget < 15000f) return "moyen";
            return budget < 30000f ? "élevé" : "très élevé";
        }

        /// <summary>Objectif courant, s'il a été repéré par une surveillance.</summary>
        /// <param name="f">Faction.</param>
        /// <param name="data">Partie.</param>
        /// <returns>Texte.</returns>
        private static string Intention(FactionData f, GameData data)
        {
            if (f.targetQuarter == Owners.None || !IsRevealed(data, f.targetQuarter)) return "inconnue (surveille ses voisins)";
            string what = f.objective == FactionObjective.CounterAttack ? "contre-attaque" : f.objective == FactionObjective.Expand ? "s'étend" : "agit";
            return what + " vers " + UiText.QuarterName(data, f.targetQuarter);
        }

        /// <summary>Le quartier est-il sous surveillance ce cycle ?</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <returns>Vrai si révélé.</returns>
        private static bool IsRevealed(GameData data, int quarterId)
        {
            int i = data.IndexOfQuarter(quarterId);
            return i >= 0 && data.quarters[i].revealedUntilCycle >= data.currentCycle;
        }

        /// <summary>Nom d'une faction.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="factionId">Faction.</param>
        /// <returns>Nom.</returns>
        private static string FactionName(GameData data, int factionId)
        {
            int i = data.IndexOfFaction(factionId);
            return i >= 0 ? data.factions[i].name : "?";
        }

        /// <summary>Affiche un quartier sur la carte.</summary>
        /// <param name="quarterId">Quartier.</param>
        private void ShowOnMap(int quarterId)
        {
            if (Manager == null) return;
            MapPanel map = Manager.ShowPanel<MapPanel>();
            if (map != null && map.Map != null) { map.Map.HandleQuarterClicked(quarterId); map.Map.FocusQuarter(quarterId); }
        }
    }
}
