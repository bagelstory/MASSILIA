# Story 007: Événements à choix et campagne de 5 ans

> **Epic**: CHOUF — MVP
> **Status**: Ready
> **Layer**: Feature
> **Type**: Logic
> **Estimate**: L (campagne : plutôt XL)
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: —

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 5

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 2022.3 LTS | **Risk**: NOT ASSESSED (no VERSION.md risk rating)
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` (MVP 5, boucle étapes 3-4, ordre de construction 3) :*

- [ ] Une semaine peut déclencher un événement à 2 ou 3 choix ; chaque choix montre son coût avant d'être validé.
- [ ] Un choix change au moins un chiffre du jeu (argent, chaleur, contrôle ou attitude d'une faction).
- [ ] Avec la même graine, les mêmes événements arrivent dans le même ordre.
- [ ] La campagne de 5 ans (`design/09-campagne-marseille.md`) se lit depuis `Resources/Data/campagne.json` : 5 années, un carrefour par an, chaque choix pose un drapeau dans la sauvegarde qui change une règle jusqu'à la fin.
- [ ] La fin « Sortie » existe, peut obtenir la note A et a son écran.
- [ ] Chaque événement et chaque choix est écrit dans le journal.
- [ ] Les textes viennent des données (pas en dur), avec un argot marseillais dosé et sans caricature ; personnages 100 % fictifs.

---

## Implementation Notes

- Code existant : `Systems/GameEventSystem.cs`, `QuestRules.cs`, `QuestSystem.cs`, `UI/JournalPanel.cs`, `UI/ResultOverlay.cs`.
- Les conséquences sont montrées ; aucun texte ne décrit une méthode concrète.

---

## Out of Scope

- Saisons et événements datés (hors périmètre du brief, après le MVP).

---

## Files

- `unity/Assets/Scripts/Systems/QuestRules.cs`
- `unity/Assets/Scripts/Systems/GameEventSystem.cs`
- `unity/Assets/Tests/EditMode/Events/QuestRulesTests.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `unity/Assets/Tests/EditMode/Events/QuestRulesTests.cs` (waived at `qa.level: minimal`, advisory) + capture d'un événement à choix gardée dans `production/qa/evidence/`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/chouf/story-006-ui-portrait-sauvegarde.md`
- Unlocks: None
