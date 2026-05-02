# Phase 24 Status -- Forbidden Forest Depths Expedition System

## Summary

Built a 3-loop Forbidden Forest expedition package: Unicorn Glade (cleanse corrupted traces), Centaur Skywatch (stabilize omen stones), and Acromantula Nest (destroy brood markers). Reuses Hagrid as the anchor NPC. Unicorn content is protection-focused, not hunting. 269/269 verification checks pass.

## Audit Findings

- ForbiddenForestBiome.cs already exists with night+forest+post-Basilisk activation
- ForbiddenForestSpawns.cs manages spawn pools (Acromantula, Thestral, etc.)
- Acromantula.cs, Thestral.cs exist as enemies; Centaur.cs, Hagrid.cs as town NPCs
- UnicornBlood.cs exists as legacy material -- framed as "rare biome drop" with "terrible price" quote, NOT from killing unicorns. No new unicorn-killing loops added.
- Phase 23 confirmed: 269/269 pass before starting

## Gameplay Loop

1. **Unlock**: Defeat Basilisk, then use Forest Expedition Lantern (craft or buy from Hagrid)
2. **Requirement**: Must be nighttime AND in Forbidden Forest biome
3. **Mission (3 min)**: Rotating loop type, destroy 4 objectives:
   - **Unicorn Glade**: Cleanse 4 Corrupted Traces (dark taint nodes). Periodic Darkness hazard. Reward: Glade Dew.
   - **Centaur Skywatch**: Stabilize 4 Omen Stones (celestial disruptions, Confused aura). Reward: Star Trace.
   - **Acromantula Nest**: Destroy 4 Brood Markers (web nodes that spawn Acromantula). Periodic Venom hazard. Reward: Brood Venom.
4. **Rewards**: Loop-specific material (scales) + Essence + Forest Wisdom buff (10 min: +6% dmg, danger sense, night vision, +4 regen) + gold + 15 House Renown
5. **Crafting**: All 3 materials -> Forest Warden's Badge (+6 def, +5% endurance, danger sense, night vision + speed in forest)

## Canon Assessment

| Element | Status |
|---------|--------|
| Forbidden Forest as dangerous Hogwarts-edge biome | Canon-faithful |
| Hagrid as forest guide / expedition anchor | Canon-faithful |
| Doxy/Acromantula infestations | Canon-faithful |
| Centaur astronomy / omen themes | Canon-faithful |
| Unicorn protection (NOT hunting) | **Canon-faithful: cleansing corruption, never killing unicorns** |
| Legacy UnicornBlood material | Pre-existing, framed as biome drop with "terrible price" warning |
| Corrupted Traces / Omen Stones / Brood Markers | Mod-original objectives |
| Glade Dew / Star Trace / Brood Venom materials | Mod-original |

## Files Created (19)

| File | Type |
|------|------|
| Common/Systems/ForestExpeditionSystem.cs | ModSystem (3 loops + rotation + persistence) |
| Content/Items/Consumables/ForestLantern.cs + .png | Access item |
| Content/Items/Consumables/GladeDew.cs + .png | Unicorn Glade material |
| Content/Items/Consumables/StarTrace.cs + .png | Centaur Skywatch material |
| Content/Items/Consumables/BroodVenom.cs + .png | Acromantula Nest material |
| Content/Items/Accessories/ForestWarden.cs + .png | Expedition reward accessory |
| Content/Buffs/ForestWisdomBuff.cs + .png | Expedition reward buff |
| Content/NPCs/Enemies/CorruptedTrace.cs + .png | Unicorn Glade objective |
| Content/NPCs/Enemies/OmenStone.cs + .png | Centaur Skywatch objective |
| Content/NPCs/Enemies/BroodMarker.cs + .png | Acromantula Nest objective |
| PHASE24_STATUS.md | This document |

## Files Modified (7)

| File | Change |
|------|--------|
| Content/NPCs/Town/Hagrid.cs | +ForestLantern in shop |
| Localization/en-US_Mods.WizardingWorld.hjson | +6 items, +3 objective names, +1 buff, +21 Forest system text keys |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +all matching Chinese entries |
| Localization/zh-Hant_Mods.WizardingWorld.hjson | +all matching Traditional Chinese entries |
| scripts/mechanical_data/shops.json | +ForestLantern in Hagrid shop |
| scripts/guide_content.json | +Forest Depths system entry (fixed "House Renown" + Unicode dashes) |
| scripts/content_manifest.json | Regenerated (534 C#, 71 enemies, 62 accessories, 49 systems) |

## Verification Results

**269 passed, 0 failed, 0 warnings.**

## Is the Forbidden Forest Expedition Package Playable and Closed?

**Yes.** A player can:
1. Defeat Basilisk and buy/craft the Forest Lantern from Hagrid
2. Enter the Forbidden Forest at night and activate expeditions
3. Complete 3 distinct loop types with unique objectives and hazards
4. Earn loop-specific materials + Forest Wisdom buff
5. Craft the Forest Warden's Badge from all 3 materials
6. BroodMarker spawns actual Acromantula enemies for real danger
7. Unicorn content is protection-focused (cleansing, never hunting)
8. All text localized EN + ZH-Hans + ZH-Hant
9. 269 verification checks protect all content and framing

## Remaining Limitations

- Placeholder sprites throughout
- No visual forest environment change during expeditions (uses existing biome)
- No Thestral Clearing sub-loop (deferred as stretch content)
- Centaur NPC not directly involved in Skywatch missions (Hagrid anchors all loops)
- BroodMarker spawns existing Acromantula but doesn't create a visual "nest"
- Legacy UnicornBlood material framing is preserved as-is (no regression, no new hunting)
