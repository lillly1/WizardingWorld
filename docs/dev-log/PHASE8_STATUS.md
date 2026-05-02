# Phase 8 Status -- Hogsmeade Expansion Pack

## Repo Inspection Summary

Phase 7 claims verified:
- 48/48 strict checks pass
- WizardDebugCommand.cs exists with 6 subcommands
- shops.json has 7 NPCs (65 items)
- docs/ exists with DEBUG_COMMANDS.md and RELEASE_PIPELINE.md
- ZH generator loads guide_content.json
- CI workflow includes mechanical export + release manifest

No gaps blocked the expansion. HogsmeadeGen.cs already generates village structures.

## Expansion Choice: Hogsmeade Pack

Chosen because the worldgen skeleton already exists but had no gameplay loop.
The village generated Three Broomsticks, Honeydukes, and Weasleys' structures
but offered no reason to visit, no unique items, and no repeatable engagement.

## Content Added

### New NPC: Madam Rosmerta (Town NPC #8)
- Three Broomsticks innkeeper
- Moves in after Basilisk is defeated
- Sells 8 Honeydukes/inn items
- Progression tier: post-Skeletron (pre-Hardmode midgame)

### 5 Honeydukes Consumables

| Item | Buff | Duration | Rarity | Price | Canon Note |
|------|------|----------|--------|-------|------------|
| Fizzing Whizbee | Featherfall | 5 min | Blue | 15s | Levitation sherbet balls |
| Pepper Imp | Inferno | 4 min | Orange | 25s | Fire-breathing sweets |
| Sugar Quill | Mana Regen | 6 min | Blue | 10s | Candy disguised as quill |
| Acid Pop | Rage | 4 min | Orange | 20s | Dangerous lollipop |
| Drooble's Best Blowing Gum | Swiftness | 6 min | Green | 12s | Dumbledore's favorite |

### Owl Post Delivery System
- Daily request board generating random vanilla-item requests
- 10 possible delivery types (Mushrooms, Daybloom, Stars, Cobweb, etc.)
- Complete by having items in inventory + talking to any town NPC
- Reward: 5-15 Essence of Magic + random 5-minute buff
- World-saved state with multiplayer sync
- Canon-inspired: Owl Post is the wizarding mail service

## Progression Placement

| Content | When Available | Why Here |
|---------|---------------|----------|
| Madam Rosmerta NPC | Post-Basilisk | Midgame -- players have reached Hogsmeade |
| Honeydukes consumables | From Rosmerta | Fills buff gap between early potions and Hardmode gear |
| Owl Post | From world creation | Simple, accessible, scales with progression |

## Truth Pipeline Updates

| Layer | Change |
|-------|--------|
| content_manifest.json | 451 C# files (was 444), 8 town NPCs (was 7), 37 systems (was 36) |
| mechanical_data/shops.json | Added MadamRosmerta with 8 items |
| guide_content.json | Added Rosmerta to NPC table, Owl Post to systems list |
| verify_guide.py | Shop count check now expects 8 NPCs |
| Generated PDF | 24 pages, counts updated from manifest |

## Verification Results

```
48 passed, 0 failed, 0 warnings
All checks passed.
```

All canon locks preserved. No lore regressions introduced.

## Files Created

| File | Type |
|------|------|
| Content/Items/Consumables/FizzyWhizbee.cs | Consumable item |
| Content/Items/Consumables/PepperImp.cs | Consumable item |
| Content/Items/Consumables/SugarQuill.cs | Consumable item |
| Content/Items/Consumables/AcidPop.cs | Consumable item |
| Content/Items/Consumables/DrooblesBestBlowingGum.cs | Consumable item |
| Content/NPCs/Town/MadamRosmerta.cs | Town NPC |
| Common/Systems/OwlPostSystem.cs | Gameplay system |
| PHASE8_STATUS.md | Documentation |

## Files Modified

| File | Change |
|------|--------|
| scripts/mechanical_data/shops.json | +MadamRosmerta (8 items) |
| scripts/guide_content.json | +Rosmerta NPC row, +Owl Post system |
| scripts/content_manifest.json | Regenerated (451 C#, 8 NPCs, 37 systems) |
| WizardingWorld_Guide_EN.pdf | Regenerated (24 pages) |

## Deferred Items

- Owl Post completion hook needs wiring into GlobalNPC chat (currently TryCompleteRequest is callable but not yet triggered by NPC interaction UI)
- Hogsmeade visit tracker (considered but deferred -- Owl Post provides the repeatable engagement loop)
- Honeydukes PNG sprites needed for all 5 new consumables
- Localization entries for MadamRosmerta and new items in HJSON files
- ZH generator inline tables not yet consuming guide_content.json for these new entries
