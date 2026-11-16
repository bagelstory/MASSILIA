# Story 002: Opérateurs et tâches

> **Epic**: CHOUF — MVP
> **Status**: Ready
> **Layer**: Core
> **Type**: Logic
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: —

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 2

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 2022.3 LTS | **Risk**: NOT ASSESSED (no VERSION.md risk rating)
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` (MVP 2, boucle étape 2) :*

- [ ] Le joueur peut recruter un opérateur ; son coût est lu dans `GameConfig`, jamais en dur.
- [ ] Un opérateur s'assigne à un quartier et à une tâche ; il ne peut tenir qu'une affectation à la fois.
- [ ] L'efficacité d'un opérateur suit la formule de `OperatorRules.Efficiency` ; même entrée, même résultat (déterministe avec la graine).
- [ ] À la fin de la semaine, la tâche produit son effet sur le quartier (contrôle, chaleur ou renseignement).
- [ ] Un opérateur perdu disparaît de la liste, et la perte est écrite dans le journal.
- [ ] Le panneau Opérateurs liste chaque opérateur avec portrait, type, quartier et tâche.

---

## Implementation Notes

- Code existant : `Systems/OperatorRules.cs`, `Systems/OperatorSystem.cs`, `UI/OperatorsPanel.cs`, `UI/OperatorDetailPopup.cs`.
- Les effets sont montrés comme des chiffres et des conséquences, jamais comme des méthodes.

---

## Out of Scope

- Story 003 : ce que rapportent et coûtent les tâches.
- Story 004 : réactions des factions aux opérateurs.

---

## Files

- `unity/Assets/Scripts/Systems/OperatorRules.cs`
- `unity/Assets/Scripts/Systems/OperatorSystem.cs`
- `unity/Assets/Tests/EditMode/Operators/OperatorRulesTests.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `unity/Assets/Tests/EditMode/Operators/OperatorRulesTests.cs` (waived at `qa.level: minimal`, advisory) + capture du panneau Opérateurs gardée dans `production/qa/evidence/`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/chouf/story-001-carte-111-quartiers.md`
- Unlocks: `production/epics/chouf/story-003-economie-flux.md`
