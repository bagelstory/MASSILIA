# Story 001: Carte de 111 quartiers qui compile et se charge

> **Epic**: CHOUF — MVP
> **Status**: Ready
> **Layer**: Foundation
> **Type**: Logic
> **Estimate**: L
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: —

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 1

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 2022.3 LTS | **Risk**: NOT ASSESSED (no VERSION.md risk rating)
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` (MVP 1, ordre de construction 1) :*

- [ ] Le projet s'ouvre dans Unity 2022.3 LTS avec 0 erreur de compilation dans la console.
- [ ] Le menu Marseille → 2. Créer la scène principale produit une scène qui se lance en mode Play sans exception.
- [ ] `Resources/Data/quarters.json` se charge : 111 quartiers, id = arrondissement × 100 + n, zoneId de 1 à 10.
- [ ] Le graphe des voisins est symétrique (si A voit B, B voit A) et connexe (tout quartier est atteignable).
- [ ] La carte affiche les 111 quartiers en portrait ; toucher un quartier ouvre sa fiche.
- [ ] Sans aucune image dans `Resources/Art`, la carte s'affiche quand même (repli sans sprite).

---

## Implementation Notes

- Code existant : `Map/`, `Systems/QuarterCatalog.cs`, `Systems/WorldBuilder.cs`, `Editor/MarseilleSetup.cs`.
- Corriger d'abord toutes les erreurs de compilation remontées par Unity : le code n'a jamais été compilé.

---

## Out of Scope

- Story 002 : opérateurs posés sur la carte.
- Story 006 : finition de l'interface et sauvegarde.

---

## Files

- `unity/Assets/Scripts/Systems/QuarterCatalog.cs`
- `unity/Assets/Scripts/Map/MapController.cs`
- `unity/Assets/Tests/EditMode/Map/QuarterCatalogTests.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `unity/Assets/Tests/EditMode/Map/QuarterCatalogTests.cs` (waived at `qa.level: minimal`, advisory) + capture d'écran de la carte gardée dans `production/qa/evidence/` (la carte se voit à l'écran).

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: None
- Unlocks: `production/epics/chouf/story-002-operateurs-taches.md`, `production/epics/chouf/story-003-economie-flux.md`, `production/epics/chouf/story-004-factions-ia.md`
