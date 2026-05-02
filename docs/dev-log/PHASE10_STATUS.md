# Phase 10 Status -- Azkaban Vertical Slice

## Repo Inspection

Phase 9 verified: 71/71 checks pass, Hogsmeade fully closed. Existing Azkaban infrastructure confirmed:
- AzkabanDespairEvent.cs (158 lines, functional event system)
- AzkabanBreachStone.cs (trigger item)
- AzkabanWardSigil.cs + WardOfHopeBuff.cs (consumable anti-despair)
- AzkabanWardensKey.cs (accessory)
- Dementor.cs + AzkabanGuard.cs (2 existing enemies)
- Full despair meter in WizardPlayer
- Patronus counterplay integrated

Gap identified: event was kill-farming only with no objectives. Two enemy types. No intermediate rewards.

## What Was Added

### 2 New Azkaban Enemies

| Enemy | HP | Damage | Defense | Role |
|-------|------|--------|---------|------|
| Prison Wraith | 250 | 40 | 12 | Group spawner, applies Darkness+Slow+despair (0.08) |
| Despair Anchor | 500 | 0 | 20 | Stationary ward objective, radiates despair aura, +10 event progress when destroyed |

**Prison Wraith**: Spectral bat-AI enemy, spawns only during Azkaban event (12% chance). Drops SoulAsh + Soul of Night. Mod-original content.

**Despair Anchor**: Stationary destructible objective. Max 3 per world. Applies 0.05 despair/sec to players within 400px. Destroying one grants 10 event progress (vs 2 for Dementors), relieves 0.10 despair to nearby players, and shows encouraging text. Drops SoulAsh + Essence of Magic.

### 1 New Crafting Material

**Soul Ash** -- residue from broken Azkaban wards. Drops from Prison Wraiths (50%) and Despair Anchors (guaranteed 2-4). Used to craft Patronus Focus.

### 1 New Accessory

**Patronus Focus** -- crafted from 15 Soul Ash + 10 Dementor's Shroud + 10 Soul of Light + 20 Essence of Magic. Effects:
- Passive: -0.003 despair/frame, +4 defense, +4% endurance
- With Patronus active: +8 defense, +3 life regen, +8% damage, slows Patronus timer decay

### Event System Expansion

Added to AzkabanDespairEvent:
- `anchorsDestroyed` counter tracking objective completion
- `OnAnchorDestroyed()` method granting 10 progress per anchor
- Victory message includes anchor count
- Despair Anchors spawn during event as destructible objectives

### Localization Fix

Added missing WardOfHopeBuff localization (was a Phase 3 gap, now closed):
- EN: "Ward of Hope" / "A silver ward against despair"
- ZH: "希望守护" / description

## Progression Placement

| Content | Tier | Access |
|---------|------|--------|
| Azkaban Despair Event | Post-Golem (late Hardmode) | Azkaban Breach Stone (crafted) |
| Prison Wraith | Event-only spawn | During Azkaban event |
| Despair Anchor | Event-only spawn | During Azkaban event (max 3) |
| Soul Ash | Event reward material | Dropped during Azkaban event |
| Patronus Focus | Post-event crafting | Soul Ash + Shroud + Souls + Essence |

The Azkaban slice sits between Golem and Dementor King, strengthening the road to the endgame.

## Gameplay Loop

1. Craft Azkaban Breach Stone (10 Dementor's Shroud + 15 Essence + 5 Soul of Night)
2. Use at night, post-Golem to start Azkaban's Despair event
3. Survive Dementor swarms + Prison Wraith groups
4. Find and destroy Despair Anchors (bonus progress + despair relief)
5. Manage despair meter using Patronus, Ward Sigils, or Warden's Key
6. Complete event (100+ kills/anchors worth of progress)
7. Collect Soul Ash and Dementor's Shroud from event enemies
8. Craft Patronus Focus for future events and Dementor King fight

## Truth Pipeline Updates

| Layer | Change |
|-------|--------|
| content_manifest.json | 455 C# files, 53 enemies (was 51), 54 accessories (was 53) |
| mechanical_data/enemies.json | +PrisonWraith, +DespairAnchor with spawn contexts |
| mechanical_data/accessories.json | +PatronusFocus |
| guide_content.json | Updated Azkaban system description, added enemies to Dark Events group |
| verify_guide.py | +9 Azkaban checks (sprites, localization, WardOfHopeBuff) = 80 total |
| EN HJSON | +SoulAsh, +PatronusFocus, +PrisonWraith, +DespairAnchor, +WardOfHopeBuff, +bestiary |
| ZH HJSON | +all Azkaban item/enemy entries + WardOfHopeBuff |

## Verification: 80/80 passed, 0 failed, 0 warnings

## Files Created

| File | Type |
|------|------|
| Content/NPCs/Enemies/PrisonWraith.cs | Enemy NPC |
| Content/NPCs/Enemies/PrisonWraith.png | Placeholder sprite (24x160, 4 frames) |
| Content/NPCs/Enemies/DespairAnchor.cs | Objective NPC |
| Content/NPCs/Enemies/DespairAnchor.png | Placeholder sprite (24x32) |
| Content/Items/Consumables/SoulAsh.cs | Crafting material |
| Content/Items/Consumables/SoulAsh.png | Placeholder sprite (16x16) |
| Content/Items/Accessories/PatronusFocus.cs | Accessory |
| Content/Items/Accessories/PatronusFocus.png | Placeholder sprite (24x24) |
| PHASE10_STATUS.md | This document |

## Files Modified

| File | Change |
|------|--------|
| Common/Systems/AzkabanDespairEvent.cs | +anchorsDestroyed field, +OnAnchorDestroyed(), +anchor count in victory msg |
| Localization/en-US_Mods.WizardingWorld.hjson | +SoulAsh, +PatronusFocus, +PrisonWraith, +DespairAnchor, +WardOfHopeBuff, +bestiary |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +all Azkaban entries in ZH |
| scripts/verify_guide.py | +9 Azkaban closure checks |
| scripts/guide_content.json | Updated Azkaban system description + enemy entries |
| scripts/content_manifest.json | Regenerated |
| scripts/mechanical_data/*.json | Regenerated |
| WizardingWorld_Guide_EN.pdf | Regenerated (24 pages) |

## Is the Azkaban Slice Playable and Truth-Pipeline Complete?

**Yes.** A player can:
1. Craft an Azkaban Breach Stone after defeating Golem
2. Use it at night to trigger Azkaban's Despair event
3. Fight Dementors, Prison Wraiths, and AzkabanGuards
4. Destroy Despair Anchors as objectives (bonus progress + despair relief)
5. Manage despair pressure using Patronus, Ward of Hope, or Warden's Key
6. Collect Soul Ash drops to craft Patronus Focus
7. The event has a clear completion condition and victory message

All content is localized (EN + ZH), has placeholder sprites, flows through the truth pipeline, and is protected by 80 verification checks.

## Remaining Limitations

- Sprites are placeholder solid-color rectangles
- Prison Wraith uses bat AI -- could benefit from custom movement
- DespairAnchor doesn't have a visual distinction from enemies in the bestiary (marked Hide)
- No Azkaban-specific music or ambient effects
- Owl Post strings in OnChatButtonClicked still hardcoded English (carry-forward from Phase 9)
