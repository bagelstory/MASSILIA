# Story 006: Interface portrait et sauvegarde JSON lisible

> **Epic**: CHOUF — MVP
> **Status**: Ready
> **Layer**: Presentation
> **Type**: Integration
> **Estimate**: L
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: —

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 7

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 2022.3 LTS | **Risk**: NOT ASSESSED (no VERSION.md risk rating)
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` (MVP 7, ordre de construction 2) :*

- [ ] Chaque écran tient en portrait et se pilote au pouce : aucune action importante hors de la moitié basse de l'écran.
- [ ] Les onglets (Carte, Tableau de bord, Opérateurs, Flux, Factions, Journal) s'ouvrent tous sans erreur.
- [ ] Sauvegarder puis recharger rend un état identique (même semaine, argent, quartiers, opérateurs, graine).
- [ ] Le fichier de sauvegarde est un JSON indenté, lisible à la main.
- [ ] Une sauvegarde abîmée ou d'une autre version de config ne plante pas le jeu : il propose une nouvelle partie.
- [ ] Sur un téléphone Snapdragon 720, la carte tient 60 fps, et le build fait moins de 50 Mo.

---

## Implementation Notes

- Code existant : `UI/` (UIManager, UiKit, panneaux), `Core/GameManager*.cs`, `GameConfig.ConfigVersion`.
- Pas de TextMeshPro, pas de coroutine, pas de package externe (règles du portage).

---

## Out of Scope

- Story 007 : l'affichage des événements et de l'arc.
- Le son (hors périmètre du brief).

---

## Files

- `unity/Assets/Scripts/Core/GameManager.cs`
- `unity/Assets/Scripts/UI/UIManager.cs`
- `unity/Assets/Tests/EditMode/Save/SaveRoundTripTests.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Required evidence**: `unity/Assets/Tests/EditMode/Save/SaveRoundTripTests.cs` ou un compte rendu de test sur téléphone (waived at `qa.level: minimal`, advisory) + une capture gardée de chaque onglet dans `production/qa/evidence/`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/chouf/story-005-fins-score.md`
- Unlocks: `production/epics/chouf/story-007-evenements-arc.md`
