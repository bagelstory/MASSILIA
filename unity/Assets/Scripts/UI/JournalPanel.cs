// MARSEILLE — UI/JournalPanel.cs
// Écran « Journal » (spec §12.4) : dernières lignes, filtrées par catégorie ; toucher une ligne liée à un
// quartier le montre sur la carte.

using Marseille.Config;
using Marseille.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Marseille.UI
{
    /// <summary>Écran du journal.</summary>
    public class JournalPanel : ListPanel
    {
        private enum Filter { All, Losses, Gains, Police, Factions }

        private static readonly string[] FilterNames = { "Tout", "Pertes", "Gains", "Police", "Rivaux" };
        [Tooltip("Nombre maximal de lignes affichées.")]
        [SerializeField, Min(10)] private int maxLines = 80;
        private Filter filter = Filter.All;

        /// <inheritdoc />
        public override string TabLabel { get { return "Journal"; } }

        /// <inheritdoc />
        public override string TabIcon { get { return "tab_journal"; } }

        /// <summary>Remplit les filtres et les lignes.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        protected override void Fill(GameData data, GameConfig config)
        {
            RectTransform row = UiKit.Row("Filters", Column, config.uiButtonHeight * 0.75f, config);
            for (int i = 0; i < FilterNames.Length; i++)
            {
                Filter f = (Filter)i;
                Button b = UiKit.MakeButton(row, FilterNames[i], f == filter ? config.uiAccent : config.uiButton, () => { filter = f; Refresh(); }, config);
                b.GetComponentInChildren<Text>().fontSize = config.uiFontSizes.z;
            }
            int shown = 0;
            for (int i = 0; i < data.journal.Count && shown < maxLines; i++)
            {
                JournalEntry e = data.journal[i];
                if (!Matches(e.type)) continue;
                AddLine(e, config);
                shown++;
            }
            if (shown == 0) UiKit.Label(Column, "Rien pour l'instant.", config.uiFontSizes.z, config.uiTextDim);
        }

        /// <summary>Une ligne du journal (bouton si elle concerne un quartier).</summary>
        /// <param name="e">Entrée.</param>
        /// <param name="config">Configuration.</param>
        private void AddLine(JournalEntry e, GameConfig config)
        {
            Text t = UiKit.Label(Column, "C" + e.cycle + " · " + e.text, config.uiFontSizes.z, ColorOf(e.type, config));
            t.alignment = TextAnchor.UpperLeft;
            UiKit.SetHeight(t.rectTransform, config.uiFontSizes.z * (e.text.Length > 48 ? 2.6f : 1.4f));
            if (e.quarterId == Owners.None) return;
            t.raycastTarget = true;
            Button b = t.gameObject.AddComponent<Button>();
            b.transition = Selectable.Transition.None;
            int quarterId = e.quarterId;
            b.onClick.AddListener(() => ShowOnMap(quarterId));
        }

        /// <summary>La catégorie passe-t-elle le filtre ?</summary>
        /// <param name="type">Catégorie.</param>
        /// <returns>Vrai si affichée.</returns>
        private bool Matches(JournalType type)
        {
            switch (filter)
            {
                case Filter.Losses: return type == JournalType.Loss || type == JournalType.OperatorLost || type == JournalType.Attack || type == JournalType.Shadow;
                case Filter.Gains: return type == JournalType.Gain || type == JournalType.Quest || type == JournalType.Economy;
                case Filter.Police: return type == JournalType.Police;
                case Filter.Factions: return type == JournalType.Faction || type == JournalType.Attack;
            }
            return true;
        }

        /// <summary>Couleur d'une catégorie.</summary>
        /// <param name="type">Catégorie.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Couleur.</returns>
        private static Color ColorOf(JournalType type, GameConfig config)
        {
            switch (type)
            {
                case JournalType.Loss: case JournalType.OperatorLost: case JournalType.Attack: return config.uiNegative;
                case JournalType.Shadow: return config.uiTextDim;
                case JournalType.Gain: case JournalType.Quest: return config.uiPositive;
                case JournalType.Police: case JournalType.Flux: return config.uiWarning;
                case JournalType.Info: case JournalType.Season: return config.uiTextDim;
            }
            return config.uiText;
        }

        /// <summary>Montre un quartier sur la carte.</summary>
        /// <param name="quarterId">Quartier.</param>
        private void ShowOnMap(int quarterId)
        {
            if (Manager == null) return;
            MapPanel map = Manager.ShowPanel<MapPanel>();
            if (map != null && map.Map != null) { map.Map.HandleQuarterClicked(quarterId); map.Map.FocusQuarter(quarterId); }
        }
    }
}
