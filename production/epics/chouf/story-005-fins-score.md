# Story 005: Fins et score de A à E

> **Epic**: CHOUF — MVP
> **Status**: Ready
> **Layer**: Feature
> **Type**: Logic
> **Estimate**: S
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: —

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 6

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 2022.3 LTS | **Risk**: NOT ASSESSED (no VERSION.md risk rating)
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` (But et échec, MVP 6) :*

- [ ] Tenir au moins le seuil de quartiers de `GameConfig` à la fin du temps donne la fin Victoire (seuil 60 %, décidé le 17 nov.).
- [ ] Chaque fin (Victoire, Faillite, Chute, Abandon, Temps) se déclenche une seule fois et arrête la partie.
- [ ] L'Abandon demande une double confirmation avant de finir la partie.
- [ ] Le score donne une note de A à E selon `EndRules` ; mêmes chiffres, même note.
- [ ] L'écran de fin affiche le logo, l'image de la fin, la note et le bilan.

---

## Implementation Notes

- Code existant : `Systems/EndRules.cs`, `EndConditionSystem.cs`, `UI/GameOverPanel.cs`, `UI/DashboardPanel.cs` (Abandon).

---

## Out of Scope

- Story 006 : la sauvegarde de la partie terminée.

---

## Files

- `unity/Assets/Scripts/Systems/EndRules.cs`
- `unity/Assets/Scripts/Systems/EndConditionSystem.cs`
- `unity/Assets/Tests/EditMode/End/EndRulesTests.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `unity/Assets/Tests/EditMode/End/EndRulesTests.cs` (waived at `qa.level: minimal`, advisory) + capture de chaque écran de fin gardée dans `production/qa/evidence/`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/chouf/story-003-economie-flux.md`, `production/epics/chouf/story-004-factions-ia.md`
- Unlocks: `production/epics/chouf/story-006-ui-portrait-sauvegarde.md`
