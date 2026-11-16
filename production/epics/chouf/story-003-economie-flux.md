# Story 003: Économie et flux

> **Epic**: CHOUF — MVP
> **Status**: Ready
> **Layer**: Core
> **Type**: Logic
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: —

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 3

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 2022.3 LTS | **Risk**: NOT ASSESSED (no VERSION.md risk rating)
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` (MVP 3, pitch « chaque gain se paie ailleurs », échec par faillite) :*

- [ ] Chaque semaine, les revenus et les dépenses sont calculés par `EconomyRules` et notés dans le `CycleLedger`.
- [ ] La prévision de la semaine suivante (`EconomyActions.Forecast`) correspond au résultat réel quand rien ne change.
- [ ] Investir dans un quartier coûte de l'argent tout de suite et rapporte plus tard.
- [ ] Chaque gain fait monter un coût ailleurs (chaleur ou pression), visible dans les indicateurs.
- [ ] Une trésorerie négative plus longtemps que le délai de `GameConfig` déclenche la fin Faillite.
- [ ] Les flux nord et sud s'affichent dans le panneau Flux avec leur solde.
- [ ] Toutes les valeurs (prix, taux, délais) sont lues dans `GameConfig`.

---

## Implementation Notes

- Code existant : `Systems/EconomyRules.cs`, `EconomySystem.cs`, `EconomyActions.cs`, `FluxSystem.cs`, `FluxActions.cs`, `SeasonRules.cs`.
- Le blanchiment se montre par ses effets (risque, enquête), sans aucun procédé concret.

---

## Out of Scope

- Story 005 : l'écran et le score de la fin Faillite.

---

## Files

- `unity/Assets/Scripts/Systems/EconomyRules.cs`
- `unity/Assets/Scripts/Systems/FluxSystem.cs`
- `unity/Assets/Tests/EditMode/Economy/EconomyRulesTests.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `unity/Assets/Tests/EditMode/Economy/EconomyRulesTests.cs` (waived at `qa.level: minimal`, advisory) + capture du panneau Flux gardée dans `production/qa/evidence/`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/chouf/story-002-operateurs-taches.md`
- Unlocks: `production/epics/chouf/story-005-fins-score.md`
