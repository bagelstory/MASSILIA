// MARSEILLE — UI/JournalPanel.cs
// Écran « Journal » : fil d'activité en cartes groupées par semaine. Filtres fixes en haut (une ligne),
// carte à bande de couleur par catégorie, toucher une carte liée à un quartier l'ouvre (« Voir sur la carte »).

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Data;
using Marseille.Map;
using UnityEngine;
using UnityEngine.UI;

namespace Marseille.UI
{
    /// <summary>Écran du journal.</summary>
    public class JournalPanel : ListPanel
    {
        private enum Filter { All, Losses, Gains, Police, Factions }

        private static readonly string[] FilterNames = { "Tout", "Pertes", "Gains", "Police", "Rivaux" };
        [Tooltip("Nombre maximal de cartes affichées.")]
        [SerializeField, Min(10)] private int maxLines = 80;
        private Filter filter = Filter.All;
        private readonly List<Image> chips = new List<Image>();
        private readonly HashSet<int> opened = new HashSet<int>();

        /// <inheritdoc />
        public override string TabLabel { get { return "Journal"; } }

        /// <inheritdoc />
        public override string TabIcon { get { return "tab_journal"; } }

        /// <summary>Construit la liste puis l'en-tête fixe (titre et filtres).</summary>
        /// <param name="config">Configuration.</param>
        protected override void Build(GameConfig config)
        {
            base.Build(config);
            float head = config.uiFontSizes.x * 1.5f + config.uiChipHeight + config.uiPadding * 1.6f;
            RectTransform scroll = (RectTransform)Column.parent;
            scroll.offsetMax = new Vector2(0f, -head);
            Image bar = UiFactory.CreateImage("Header", transform, config.uiPanel, true);
            UiFactory.Stretch(bar.rectTransform, new Vector2(0f, 1f), Vector2.one);
            bar.rectTransform.offsetMin = new Vector2(0f, -head);
            Text title = UiFactory.CreateText("Title", bar.transform, UiKit.Font, config.uiFontSizes.x, config.uiText);
            title.alignment = TextAnchor.MiddleLeft;
            UiFactory.Stretch(title.rectTransform, new Vector2(0.04f, 0.5f), new Vector2(0.96f, 1f));
            title.text = "Journal";
            RectTransform row = UiFactory.CreateRect("Filters", bar.transform);
            UiFactory.Stretch(row, new Vector2(0f, 0f), new Vector2(1f, 0.5f));
            row.offsetMin = new Vector2(config.uiPadding, config.uiPadding * 0.5f);
            row.offsetMax = new Vector2(-config.uiPadding, 0f);
            HorizontalLayoutGroup h = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = config.uiSpacing * 0.6f;
            h.childControlWidth = h.childControlHeight = h.childForceExpandWidth = true;
            h.childForceExpandHeight = false;
            h.childAlignment = TextAnchor.MiddleCenter;
            for (int i = 0; i < FilterNames.Length; i++)
            {
                Filter f = (Filter)i;
                chips.Add(UiKit.Chip(row, FilterNames[i], config.uiButton, config.uiTextDim, () => { filter = f; Refresh(); }, config));
            }
        }

        /// <summary>Remplit le fil.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        protected override void Fill(GameData data, GameConfig config)
        {
            for (int i = 0; i < chips.Count; i++)
            {
                bool on = i == (int)filter;
                chips[i].color = on ? config.uiAccent : config.uiButton;
                chips[i].GetComponentInChildren<Text>().color = on ? config.uiBackground : config.uiTextDim;
            }
            int shown = 0, lastCycle = -1;
            for (int i = 0; i < data.journal.Count && shown < maxLines; i++)
            {
                JournalEntry e = data.journal[i];
                if (!Matches(e.type)) continue;
                if (e.cycle != lastCycle)
                {
                    lastCycle = e.cycle;
                    Text day = UiKit.Label(Column, "Semaine " + e.cycle, config.uiFontSizes.z - 3, config.uiTextDim);
                    day.alignment = TextAnchor.LowerLeft;
                }
                AddCard(data, e, i, config);
                shown++;
            }
            if (shown == 0) UiKit.Label(Column, "Rien pour l'instant.", config.uiFontSizes.z, config.uiTextDim);
        }

        /// <summary>Une carte d'événement : catégorie et lieu, texte, détail repliable.</summary>
        private void AddCard(GameData data, JournalEntry e, int index, GameConfig config)
        {
            Color tone = ColorOf(e.type, config);
            Image card = UiKit.BandCard(Column, tone, config);
            string place = string.Empty;
            int qi = e.quarterId == Owners.None ? -1 : data.IndexOfQuarter(e.quarterId);
            if (qi >= 0) place = "  ·  " + data.quarters[qi].name;
            UiKit.Flow(card.transform, NameOf(e.type) + place, config.uiFontSizes.z - 4, tone);
            UiKit.Flow(card.transform, e.text, config.uiFontSizes.z + 2, config.uiText);
            if (qi < 0) return;
            Button b = card.gameObject.AddComponent<Button>();
            b.targetGraphic = card;
            b.transition = Selectable.Transition.None;
            int key = index;
            int quarterId = e.quarterId;
            b.onClick.AddListener(() => { if (!opened.Remove(key)) opened.Add(key); Refresh(); });
            if (!opened.Contains(index)) return;
            UiKit.Chip(card.transform, "Voir sur la carte", config.uiAccent, config.uiBackground, () => ShowOnMap(quarterId), config);
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

        private static string NameOf(JournalType type)
        {
            switch (type)
            {
                case JournalType.Gain: return "Gain";
                case JournalType.Loss: return "Perte";
                case JournalType.Attack: return "Attaque";
                case JournalType.OperatorLost: return "Équipe";
                case JournalType.Police: return "Police";
                case JournalType.Flux: return "Flux";
                case JournalType.Economy: return "Économie";
                case JournalType.Quest: return "Objectif";
                case JournalType.Faction: return "Rivaux";
                case JournalType.Season: return "Saison";
                case JournalType.End: return "Fin";
                case JournalType.Shadow: return "Ombre";
            }
            return "Info";
        }

        private static Color ColorOf(JournalType type, GameConfig config)
        {
            switch (type)
            {
                case JournalType.Loss: case JournalType.OperatorLost: return config.uiNegative;
                case JournalType.Attack: case JournalType.Faction: return config.uiWarning;
                case JournalType.Shadow: case JournalType.Info: case JournalType.Season: return config.uiInfo;
                case JournalType.Gain: case JournalType.Quest: case JournalType.Economy: return config.uiPositive;
                case JournalType.Police: case JournalType.Flux: return config.uiAccent;
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
