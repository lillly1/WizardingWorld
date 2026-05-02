# Phase 6 Status -- Mechanical Truth + Three-Layer Pipeline

## Summary

Phase 6 adds a mechanical data extraction layer between C# source code and the
guide PDF, creating a provable three-layer pipeline where guide numbers can be
traced back to actual gameplay code.

## Three-Layer Architecture

```
Layer A: AUTO-DERIVED TRUTH
  C# source files
      |
      +--> scan_content.py     --> content_manifest.json  (file counts, item lists)
      +--> export_mechanical_data.py --> mechanical_data/*.json  (item/boss/enemy stats)

Layer B: CURATED NARRATIVE
  export_guide_data.py --> guide_content.json  (prose, group ordering, disclosure tags)

Layer C: VERIFICATION + GENERATION
  verify_guide.py  (46 strict checks: counts + lore + semantics + mechanical consistency)
      |
      v
  generate_english_pdf.py  (reads manifest + guide_content, verified by mechanical data)
      |
      v
  WizardingWorld_Guide_EN.pdf  (24 pages)
```

## Files Created

| File | Purpose | Type |
|------|---------|------|
| `scripts/export_mechanical_data.py` | Extracts stats from C# via regex | Auto-derived |
| `scripts/mechanical_data/wands.json` | Wand damage/mana/useTime/rarity | Auto-derived |
| `scripts/mechanical_data/bosses.json` | Boss HP/damage/defense/drops | Auto-derived |
| `scripts/mechanical_data/enemies.json` | Enemy stats + spawn contexts | Auto-derived |
| `scripts/mechanical_data/accessories.json` | Accessory stats | Auto-derived |
| `scripts/mechanical_data/potions.json` | Potion buff type/duration | Auto-derived |
| `scripts/mechanical_data/armor.json` | Armor defense + set names | Auto-derived |
| `scripts/mechanical_data/other_weapons.json` | Non-wand weapon stats | Auto-derived |
| `scripts/mechanical_data/town_npcs.json` | Town NPC base stats | Auto-derived |
| `scripts/mechanical_data/all.json` | Combined mechanical data | Auto-derived |
| `schemas/mechanical_wands.schema.json` | Wand data schema | Validation |
| `schemas/mechanical_bosses.schema.json` | Boss data schema | Validation |
| `PHASE6_STATUS.md` | This document | Documentation |

## Files Modified

| File | Change |
|------|--------|
| `scripts/verify_guide.py` | Added 9 mechanical consistency checks (wand stats, boss HP, drop tables, spawn contexts) |
| `.github/workflows/guide-ci.yml` | Added mechanical data export step + artifacts |

## Verification: 46 Checks

### Manifest Self-Consistency (3)
Wand math, file counts, armor piece counts.

### Stale Code References (4)
WandOfDestiny, ShadowElderWand, CorruptedPatronus, HousePointsSystem.

### Lore State from C# (11)
InvisibilityCloak, DemiguiseCloak, ResurrectionStone, HallowsSystem, HorcruxHuntSystem,
legacy stubs, Riddikulus, Peeves, Dementor spawning.

### Semantic Drift in guide_content.json (18)
13 forbidden patterns + 5 required patterns.

### Mechanical Data Consistency (9) -- NEW
- Guide wand damage/mana match extracted C# stats
- Guide boss HP matches extracted C# base values
- Voldemort drops Elder Wand + Gaunt's Ring (confirmed from code)
- Voldemort does NOT drop InvisibilityCloak or ResurrectionStone
- Dementor spawn contexts include blood_moon + azkaban_event
- Peeves spawn context is dungeon-only

### PDF Generator (1)
No hardcoded Windows paths.

## What Is Auto-Derived vs Curated

| Data | Source | Type |
|------|--------|------|
| File counts (C#, PNG, total) | scan_content.py | Auto |
| Item lists by category | scan_content.py | Auto |
| Wand base stats (damage, mana, useTime, rarity) | export_mechanical_data.py | Auto |
| Boss base stats (HP, damage, defense) | export_mechanical_data.py | Auto |
| Boss drop tables | export_mechanical_data.py | Auto |
| Enemy stats + spawn contexts | export_mechanical_data.py | Auto |
| Accessory/potion/armor base stats | export_mechanical_data.py | Auto |
| Wand tier grouping and notes | guide_content.json | Curated |
| Boss phase descriptions | guide_content.json | Curated |
| Enemy biome grouping and flavor text | guide_content.json | Curated |
| System descriptions | guide_content.json | Curated |
| Hallows acquisition narrative | guide_content.json | Curated |
| Canon disclosure categories | guide_content.json | Curated |
| Dynamic scaling behavior (Voldemort) | Marked `_curated: true` in mechanical data | Curated |
| Buff effect descriptions | Not auto-extracted (complex UpdateAccessory logic) | Curated |
| Fantastic Beasts count (21) | Curated subset classification | Curated |

## Pipeline Commands

```bash
# Full pipeline (run in repo root):
python scripts/scan_content.py --pretty
python scripts/export_mechanical_data.py
python scripts/export_guide_data.py
python scripts/verify_guide.py --strict
python generate_english_pdf.py

# Quick check (no generation):
python scripts/export_mechanical_data.py --check
python scripts/verify_guide.py --strict
```

## Known Limitations

1. **Buff effects not auto-extracted**: UpdateAccessory() logic is too varied for reliable regex. Buff descriptions remain curated in guide_content.json.
2. **Dynamic boss scaling**: Voldemort's stats depend on HorcruxHuntSystem. The extractor captures base values and marks them `_curated`.
3. **Shop inventories**: NPC shops use fluent builder APIs that resist simple regex extraction. Shop descriptions remain curated.
4. **Chinese PDF**: The ZH generator reads manifest counts but does NOT yet read guide_content.json for table data. Its tables remain inline.
5. **Wand display name mapping**: Guide rows use display names ("Oak Wand") while mechanical data uses class names ("OakWand"). Matching is done via fuzzy name normalization, which could break for unusual names.

## Remaining Lore Deviations (Intentional)

All unchanged from Phase 5. Key intentional deviations:
- Patronus as combat summon weapon (canon is defensive only)
- DemiguiseCloak grants full invisibility (canon fades over time)
- Phantom Stag in Hallow biome (mod-original)
- Reparo extends buff timers (creative adaptation of "repair")
- House Renown is kill-based (canonical House Points are professor-awarded)

## Recommended Phase 7 Focus

1. **Content expansion** -- the metadata model now supports adding new items/bosses with less drift risk
2. **Chinese PDF refactor** -- migrate ZH generator to read guide_content.json like EN
3. **Auto-extract buff effects** -- AST-based extraction or explicit metadata annotations in C# code
4. **Shop inventory extraction** -- parse AddShops() fluent API or add metadata registry
5. **In-game dev diagnostics** -- debug commands for Hallows/Horcrux state, progression flags
6. **Balance audit** -- use extracted mechanical data to check progression curve sanity
