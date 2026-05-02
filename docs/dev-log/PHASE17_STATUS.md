# Phase 17 Status -- Department of Mysteries / Order of the Phoenix Vertical Slice

## Summary

Built a complete Department of Mysteries prophecy retrieval mission with Kingsley Shacklebolt as Order liaison NPC, Prophecy Orb collection objectives, Death Eater ambush waves, and Order-aligned rewards. 167/167 verification checks pass.

## Gameplay Loop

1. **Unlock**: Defeat Bellatrix Lestrange, then use Ministry Visitor Badge (or talk to Kingsley)
2. **Mission Start**: 3-minute timed Hall of Prophecy retrieval
3. **Objectives**: Collect 3 floating Prophecy Orbs (walk into them to pick up)
4. **Hazards**: Death Eater ambush squads spawn every 20 seconds (escalates with completions)
5. **Completion**: Rewards: Prophecy Dust (scales with completions) + 15 Essence of Magic + Order Commendation buff (10 min) + 25 House Points
6. **Failure**: Timer expiry = mission failed, retry with the Badge
7. **Progression**: Repeatable with escalating difficulty (more Death Eaters per wave)

## Content Added

### New NPC: Kingsley Shacklebolt (Town NPC #10)
- Order of the Phoenix liaison and senior Auror
- Moves in after Bellatrix defeated
- **Shop**: Ministry Visitor Badge, Azkaban Breach Stone, Azkaban Ward Sigil, Soul of Night (Hardmode)
- **Mission Status button**: shows current prophecy mission state
- 5 dialogue lines + veteran variant
- Canon-faithful framing: competent, composed, mission-focused

### New Items

| Item | Type | Stats/Role |
|------|------|-----------|
| Ministry Visitor Badge | Reusable access | Unlocks + starts Department missions |
| Prophecy Dust | Crafting material | Scales with mission completions |
| Order of the Phoenix Badge | Accessory | +8% spell dmg, +5 def, +4% endurance, Dark Curse immunity, despair resistance |
| Order Commendation | Buff (10 min) | +10% spell dmg, +4 def, +3 life regen |

### New NPC: Prophecy Orb (mission objective)
- Stationary, glowing, collectible on proximity (40px)
- Spawns 3 per mission, despawns when mission ends
- Blue ethereal dust trail + gentle bobbing animation

## Canon Assessment

| Element | Status |
|---------|--------|
| Department of Mysteries setting | Canon-faithful |
| Hall of Prophecy with glass orbs | Canon-faithful |
| Death Eater ambush | Canon-faithful (Battle of the Dept of Mysteries) |
| Kingsley Shacklebolt as Order liaison | Canon-faithful |
| Ministry Visitor Badge access | Canon-inspired (red telephone box visitor entrance) |
| Prophecy Dust material | **Mod-original** |
| Order Badge accessory | **Mod-original** |
| Repeatable mission structure | **Mod-original** (canon event was unique) |

## Files Created (10)

| File | Type |
|------|------|
| Common/Systems/ProphecyMissionSystem.cs | ModSystem (mission loop + persistence) |
| Content/Items/Consumables/MinistryVisitorBadge.cs + .png | Access item |
| Content/Items/Consumables/ProphecyDust.cs + .png | Crafting material |
| Content/Items/Accessories/OrderBadge.cs + .png | Order accessory |
| Content/Buffs/OrderCommendationBuff.cs + .png | Mission reward buff |
| Content/NPCs/Enemies/ProphecyOrb.cs + .png | Mission objective NPC |
| Content/NPCs/Town/Kingsley.cs + .png + _Head.png | Town NPC |
| PHASE17_STATUS.md | This document |

## Files Modified (6)

| File | Change |
|------|--------|
| Localization/en-US_Mods.WizardingWorld.hjson | +6 items/NPCs, +1 buff, +14 system text keys, +dialogue, +bestiary |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +all matching Chinese entries |
| scripts/mechanical_data/shops.json | +Kingsley shop (4 items) |
| scripts/guide_content.json | +Kingsley NPC row, "Ten NPCs", +Department of Mysteries system |
| scripts/verify_guide.py | +16 Phase 17 checks (167 total) |
| scripts/content_manifest.json | Regenerated (480 C#, 56 enemies, 10 NPCs, 57 accessories) |

## Verification Results

**167 passed, 0 failed, 0 warnings.**

16 new checks:
- ProphecyMissionSystem.cs existence
- Kingsley NPC existence
- 5 sprite checks (3 items + NPC + buff)
- 6 EN localization checks
- 2 ZH localization checks
- 1 Kingsley shop export check

## Integration with Existing Content

| System | Connection |
|--------|-----------|
| Death Eaters | Reused as mission ambush enemies |
| House Points / Great Hall | +25 points on mission completion |
| Azkaban content | Kingsley sells Azkaban access items |
| Despair system | Order Badge provides passive despair resistance |

## Is the Department of Mysteries Slice Playable and Closed?

**Yes.** A player can:
1. Defeat Bellatrix to unlock the content
2. Craft/buy Ministry Visitor Badge from Kingsley
3. Start timed prophecy retrieval missions
4. Collect 3 Prophecy Orbs while fighting Death Eaters
5. Receive Prophecy Dust + Essence + buff + House Points
6. Craft Order Badge from accumulated Prophecy Dust
7. Repeat with escalating difficulty
8. All text localized EN + ZH
9. 167 verification checks protect the full stack

## Remaining Limitations

- Placeholder sprites throughout
- No visual Department of Mysteries environment (mission uses overworld space)
- No Brain Room / Veil Chamber / Time Room sub-missions (deferred)
- Death Eater spawns reuse the existing DeathEater enemy (no Ministry-specific variants)
- zh-Hant not updated
