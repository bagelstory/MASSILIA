# Story 004: Factions pilotées par l'IA

> **Epic**: CHOUF — MVP
> **Status**: Ready
> **Layer**: Core
> **Type**: Logic
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: —

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 4

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 2022.3 LTS | **Risk**: NOT ASSESSED (no VERSION.md risk rating)
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` (MVP 4, boucle étape 3, échec par chute) :*

- [ ] À chaque semaine, chaque faction rivale joue son tour : elle tient, attaque ou cède un quartier voisin du sien.
- [ ] Avec la même graine, les factions jouent exactement les mêmes coups (déterministe).
- [ ] Une faction n'attaque qu'un quartier voisin d'un quartier qu'elle tient.
- [ ] La diplomatie (`FactionDiplomacy`) change l'attitude d'une faction, et ce changement est visible dans le panneau Factions.
- [ ] Perdre tous ses quartiers déclenche la fin Chute.
- [ ] Toutes les factions et leurs chefs sont fictifs ; leurs noms viennent des données, pas du code.

---

## Implementation Notes

- Code existant : `Systems/FactionRules.cs`, `FactionAISystem.cs`, `FactionDiplomacy.cs`, `UI/FactionsPanel.cs`.
- Les affrontements se montrent par leurs conséquences (quartiers perdus, pression), jamais par des scènes de violence détaillées.

---

## Out of Scope

- Story 005 : l'écran et le score de la fin Chute.
- Story 007 : les événements narratifs liés aux factions.

---

## Files

- `unity/Assets/Scripts/Systems/FactionRules.cs`
- `unity/Assets/Scripts/Systems/FactionAISystem.cs`
- `unity/Assets/Tests/EditMode/Factions/FactionRulesTests.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `unity/Assets/Tests/EditMode/Factions/FactionRulesTests.cs` (waived at `qa.level: minimal`, advisory) + capture du panneau Factions gardée dans `production/qa/evidence/`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/chouf/story-001-carte-111-quartiers.md`
- Unlocks: `production/epics/chouf/story-005-fins-score.md`
