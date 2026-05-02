# Phase 26 Status -- Marauders & Moonlight Expansion Pack

## Summary

Built a connected 5-workstream expansion: Remus Lupin Town NPC (#14), upgraded Marauder's Map with passage synergy, Secret Passage Network (3 discoverable routes), Thestral Clearing sub-loop (4th Forbidden Forest expedition), and Shrieking Shack full-moon authenticity pass. All interconnected through school-grounds / nocturnal / Marauders theming. 331/331 verification checks pass.

## Workstream A: Remus Lupin NPC

- Town NPC #14, spawns post-Fenrir
- **Shop**: Willow Passage Token, Wolfsbane Potion, Marauder's Map, Pepperup Potion, Soul of Light (Hardmode)
- **Passages button**: shows Secret Passage Network status
- 6 dialogue lines (4 standard + full moon + Shack)
- Framing: teacher, Patronus mentor, Marauder, moon-aware advisor
- Wolfsbane in shop: "manages symptoms, does NOT cure" (verified)

## Workstream B: Marauder's Map Upgrade

- Enhanced existing MaraudersMap.cs with passage synergy
- Now grants: detectCreature + findTreasure + dangerSense + nightVision
- Passage cooldown reduction: 5 min -> 3 min with Map equipped
- Tooltip updated with creator credit ("Moony, Wormtail, Padfoot, Prongs")

## Workstream C: Secret Passage Network

- **SecretPassageSystem.cs**: tracks 3 discoverable passages
- Passage 1 (Whomping Willow): discovered when Shack is revealed
- Passage 2 (One-Eyed Witch): discovered when entering Diagon Alley
- Network unlocks at 2 passages discovered
- Cooldown-based covert transit to spawn (5 min base, 3 min with Map)
- Lupin's "Passages" chat button shows network status

## Workstream D: Thestral Clearing Sub-Loop

- 4th loop added to ForestExpeditionSystem (now rotates 4 ways)
- **ThestralBeacon** objective NPCs: moonlit guidance markers, collected on proximity (not destroyed)
- Non-hostile loop: hazard is Slow + NightOwl (eerie, not aggressive)
- Reward: **Spectral Essence** material
- Thestrals framed as eerie/loyal/intelligent, not generic monsters

## Workstream E: Shrieking Shack Full-Moon Pass

- Full-moon detection (`Main.moonPhase == 0`) in ShriekingShackSystem
- **Full Moon Warning** text on mission start during full moon
- Doubled hazard debuff durations during full moon missions
- +2 bonus MoonsilverThread on full moon completions
- Containment framing preserved, not tourist framing

## Files Created (6)

| File | Type |
|------|------|
| Content/NPCs/Town/Lupin.cs + .png + _Head.png | Town NPC |
| Common/Systems/SecretPassageSystem.cs | ModSystem (passage network) |
| Content/NPCs/Enemies/ThestralBeacon.cs + .png | Forest loop 4 objective |
| Content/Items/Consumables/SpectralEssence.cs + .png | Thestral material |
| PHASE26_STATUS.md | This document |

## Files Modified (12)

| File | Change |
|------|--------|
| Content/Items/Accessories/MaraudersMap.cs | +enhanced tooltip, passage synergy documented |
| Common/Systems/ForestExpeditionSystem.cs | +Thestral Clearing (loop 3), now 4-loop rotation |
| Common/Systems/ShriekingShackSystem.cs | +full-moon detection, doubled hazards, bonus material, +passage discovery |
| Common/Systems/DiagonAlleySystem.cs | +One-Eyed Witch passage discovery |
| Localization/en-US_Mods.WizardingWorld.hjson | +Lupin (NPC, dialogue, bestiary), +Passages section, +Thestral/SpectralEssence, +FullMoonWarning, +Map tooltip |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +all matching ZH-Hans entries |
| Localization/zh-Hant_Mods.WizardingWorld.hjson | +all matching ZH-Hant entries |
| scripts/mechanical_data/shops.json | +Lupin shop (5 items) |
| scripts/guide_content.json | +Lupin NPC ("Fourteen"), +Thestral Clearing in Forest text, +Shack full-moon text |
| scripts/verify_guide.py | +42 Phase 26 checks (331 total) |
| scripts/content_manifest.json | Regenerated (546 C#, 75 enemies, 14 NPCs, 51 systems) |

## Canon Assessment

| Element | Status |
|---------|--------|
| Remus Lupin as teacher/Marauder/Patronus mentor | Canon-faithful |
| Wolfsbane as symptom relief, NOT cure | **Verified by check** |
| Marauder's Map reveals hidden things | Canon-faithful |
| Map "not fooled" passage synergy | Canon-inspired |
| Secret passages from Hogwarts to Hogsmeade | Canon-faithful |
| One-Eyed Witch statue passage | Canon-faithful |
| Whomping Willow -> Shrieking Shack passage | Canon-faithful |
| Thestrals as eerie, intelligent, loyal | Canon-faithful (non-hostile loop) |
| Full moon danger at the Shack | Canon-faithful |
| Shack NOT tourist destination | **Verified by check** |
| UnicornBlood NOT from hunting | **Verified by check** |
| Passage Network as system | Mod-original |
| Spectral Essence material | Mod-original |

## Verification Results

**331 passed, 0 failed, 0 warnings.**

42 new Phase 26 checks covering:
- Lupin NPC, sprites, shop export
- Thestral loop, beacon, SpectralEssence
- 4-loop rotation in Forest system
- Full-moon moonPhase check + FullMoonWarning + bonus MoonsilverThread
- SecretPassageSystem existence
- Passage discovery wiring
- MaraudersMap existence
- Wolfsbane "not cure" safety
- UnicornBlood "not hunting" safety
- Shack "not tourism" safety
- Tri-language localization (EN + ZH-Hans + ZH-Hant) for all new keys

## Is the Marauders & Moonlight Package Playable and Closed?

**Yes.** All 5 workstreams are implemented and interconnected:
1. Lupin sells passage/moon/Patronus support
2. Map now interacts with the passage network
3. Passages discovered through existing Shack + Diagon flows
4. Thestral Clearing adds a 4th forest loop
5. Shack missions scale with the actual moon phase
6. All text in 3 languages
7. 331 checks protect the full package

## Remaining Limitations

- Placeholder sprites throughout
- Passage transit is spawn-teleport (no visual tunnel traversal)
- No Marauder's Map UI overlay (enhanced tooltips/buffs only)
- No Peter/Sirius/James NPC implementation
- Only 3 passages discoverable (expandable)
- Thestral loop is non-combat (eerie collection, not battle)
- zh-Hant may still have gaps from pre-Phase 24 content
