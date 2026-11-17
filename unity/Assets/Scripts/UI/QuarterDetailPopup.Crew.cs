// MARSEILLE — UI/QuarterDetailPopup.Crew.cs
// Partie « hiérarchie » du détail de quartier : l'arrondissement et son chef, puis l'équipe de chaque charbon du joueur
// (2 guetteurs, vendeur, gérant, tête). Chaque bouton place le meilleur équipier libre ou retire l'occupant.
// Les règles sont dans CrewActions.

using Marseille.Config;
using Marseille.Data;
using Marseille.Systems;

namespace Marseille.UI
{
    public partial class QuarterDetailPopup
    {
        /// <summary>Arrondissement du quartier et son chef.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        private void AddArrondissement(QuarterData q, GameData data, GameConfig c)
        {
            int ai = CrewRules.IndexOfArrondissement(data, q.arrondissement);
            string chief = ai >= 0 && data.arrondissements[ai].chiefOwnerId != Owners.None
                ? "chef : " + CrewRules.ChiefName(data, data.arrondissements[ai].chiefOwnerId)
                : "personne ne tient l'arrondissement";
            Paragraph(CrewRules.ArrondissementLabel(q.arrondissement) + " arrondissement · " + chief, 1, c.uiTextDim);
        }

        /// <summary>Équipe d'un charbon du joueur : qui tient chaque poste, et les gestes pour placer ou retirer.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="poi">Charbon.</param>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        private void AddCrew(QuarterData q, PoiData poi, GameData data, GameConfig c)
        {
            UiCard card = UiCard.Create(Body, 3, c);
            card.Text.text = "Équipe de " + poi.name + " · rendement " + (CrewRules.Efficiency(poi, c) * 100f).ToString("0") + " %\n" + CrewText(poi, data);
            int pid = poi.id;
            for (int s = 0; s < CrewSlots.Count; s++)
            {
                int slot = s;
                int who = CrewRules.GetSlot(poi, s);
                if (who == CrewSlots.Empty || (who == CrewSlots.Player && s == CrewSlots.Head))
                {
                    int best = CrewActions.BestCandidate(data, s, pid, c);
                    if (best < 0) continue;
                    int oi = data.IndexOfOperator(best);
                    string verb = who == CrewSlots.Player ? "Nommer une tête : " : CrewSlots.Label(s) + " : placer ";
                    card.AddButton(verb + data.operators[oi].Label(), c.uiButton,
                                   () => Act((ref GameData d, GameConfig cfg) => CrewActions.Assign(ref d, pid, slot, best, cfg)));
                }
                else if (who >= 0)
                {
                    card.AddButton("Retirer : " + CrewSlots.Label(s), c.uiButton,
                                   () => Act((ref GameData d, GameConfig cfg) => CrewActions.Unassign(ref d, pid, slot)));
                }
            }
        }

        /// <summary>Qui tient chaque poste, en deux lignes.</summary>
        /// <param name="poi">Charbon.</param>
        /// <param name="data">Partie.</param>
        /// <returns>Texte sur deux lignes.</returns>
        private static string CrewText(PoiData poi, GameData data)
        {
            string[] parts = new string[CrewSlots.Count];
            for (int s = 0; s < CrewSlots.Count; s++)
            {
                int who = CrewRules.GetSlot(poi, s);
                string name = who == CrewSlots.Empty ? "vide" : who == CrewSlots.Player ? "toi" : OperatorLabel(data, who);
                parts[s] = CrewSlots.Label(s) + " " + name;
            }
            return parts[0] + "  ·  " + parts[1] + "  ·  " + parts[2] + "\n" + parts[3] + "  ·  " + parts[4];
        }

        /// <summary>Nom d'un équipier pour l'affichage.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="operatorId">Identifiant.</param>
        /// <returns>Surnom ou numéro.</returns>
        private static string OperatorLabel(GameData data, int operatorId)
        {
            int oi = data.IndexOfOperator(operatorId);
            return oi >= 0 ? ShadowRules.Name(data.operators[oi]) : "?";
        }
    }
}
