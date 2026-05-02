# Phase 11 Status -- Room of Requirement Utility Slice + Carry-Forward Cleanup

## Summary

Expanded the Room of Requirement from 4 modes to 5, wired the Training Room to spawn a Dueling Dummy, localized ALL Room content (EN + ZH), fixed Owl Post hardcoded English strings, and added 18 new verification checks. Total: 98/98 passing.

## What Changed

### Room of Requirement Expansion

**New mode: Restoration Room** (priority 1 when despair > 15%)
- Relieves despair at 0.005/frame
- +6 life regen, +6 defense, +6% endurance
- Extends Ward of Hope buff duration
- Emits calming blue-white light
- Connects Reparo/ward system to despair recovery

**Training Room enhancement**
- Now spawns a Dueling Dummy near the player when the buff activates
- Dummy appears within 100px of player, max 1 nearby
- Respects multiplayer netmode

**Updated Room priority order:**
1. Restoration (despair > 15%) -- anti-despair recovery
2. Recovery (HP < 50%) -- emergency healing
3. Training (holding wand) -- spell practice + dummy spawn
4. Vault (10+ gold) -- treasure hunting
5. Sanctuary (default) -- balanced protection

### Owl Post Localization Fix (carry-forward)

Replaced 3 hardcoded English strings in MadamRosmerta.cs with `Language.GetTextValue()` calls:
- `"Owl Post"` button -> `OwlPostButton` key
- `"Today's delivery..."` -> `OwlPostComplete` key
- `"Wonderful! The owl..."` -> `OwlPostSuccess` key

Added corresponding entries in both EN and ZH HJSON files.

### Full Localization Coverage

**EN localization added:**
- RoomOfRequirementKey (DisplayName + Tooltip)
- RoomRecoveryBuff, RoomTrainingBuff, RoomVaultBuff, RoomSanctuaryBuff, RoomRestorationBuff (DisplayName + Description)
- OwlPostComplete, OwlPostSuccess, OwlPostButton

**ZH localization added:**
- All matching Chinese translations for the above keys

## Gameplay Loop

1. Craft Room of Requirement Key (10 Gold + 25 Essence + 5 Soul Light + 5 Soul Night at Enchanting Table)
2. Use the key -- the Room detects your state:
   - High despair? Restoration Room soothes dark aftereffects
   - Low HP? Recovery Room provides emergency healing
   - Holding a wand? Training Room spawns a Dueling Dummy for practice
   - Carrying gold? Vault Room reveals treasure
   - Otherwise? Sanctuary Room gives balanced protection
3. 3-minute buff with cooldown (can't use while any Room buff active)
4. The Room adapts to what you need -- "I need a place to..."

## Files Created

| File | Type |
|------|------|
| Content/Buffs/RoomRestorationBuff.cs | Buff (despair relief + defense + ward extension) |
| Content/Buffs/RoomRestorationBuff.png | Placeholder sprite (32x32) |
| PHASE11_STATUS.md | This document |

## Files Modified

| File | Change |
|------|--------|
| Content/Items/Consumables/RoomOfRequirementKey.cs | +Restoration mode, +despair check, +updated tooltips |
| Content/Buffs/RoomTrainingBuff.cs | +Dueling Dummy spawn on buff activation |
| Content/NPCs/Town/MadamRosmerta.cs | Owl Post strings -> Language.GetTextValue() |
| Localization/en-US_Mods.WizardingWorld.hjson | +Room item/buffs + Owl Post keys |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +Room item/buffs + Owl Post keys (Chinese) |
| scripts/verify_guide.py | +18 Room/localization checks |
| scripts/guide_content.json | Updated Room description with 5 modes |
| scripts/content_manifest.json | Regenerated |
| WizardingWorld_Guide_EN.pdf | Regenerated |

## New Localization Keys

**EN:**
- `Items.RoomOfRequirementKey.DisplayName` / `.Tooltip`
- `Buffs.RoomRecoveryBuff.DisplayName` / `.Description`
- `Buffs.RoomTrainingBuff.DisplayName` / `.Description`
- `Buffs.RoomVaultBuff.DisplayName` / `.Description`
- `Buffs.RoomSanctuaryBuff.DisplayName` / `.Description`
- `Buffs.RoomRestorationBuff.DisplayName` / `.Description`
- `Dialogue.MadamRosmerta.OwlPostButton`
- `Dialogue.MadamRosmerta.OwlPostComplete`
- `Dialogue.MadamRosmerta.OwlPostSuccess`

**ZH (zh-Hans):** All matching keys with Chinese translations.

## New Verification Checks (18 added, 98 total)

- RoomOfRequirementKey.cs exists
- 5 Room buff sprite checks
- 6 EN localization checks (item + 5 buffs)
- 2 EN Owl Post localization checks
- 3 ZH localization checks (key + 2 buffs)
- 1 Owl Post hardcoded string removal check

## Verification Results

**98 passed, 0 failed, 0 warnings.**

## Carry-Forward Status

| Issue | Status |
|-------|--------|
| Owl Post hardcoded English in Rosmerta | **FIXED** -- replaced with Language.GetTextValue |
| Room of Requirement EN localization | **FIXED** -- all keys added |
| Room of Requirement ZH localization | **FIXED** -- all keys added |
| zh-Hant (Traditional Chinese) | Deferred -- not in scope for this phase |
| OwlPostSystem.PossibleRequests English | Deferred -- array descriptions are English-only, not player-facing UI |
| Placeholder sprites | Remain placeholder for all Phase 8-11 content |

## Is the Room of Requirement Slice Playable and Closed?

**Yes.** A player can:
1. Craft the Room of Requirement Key at the Enchanting Table
2. Use it to receive an adaptive 3-minute buff based on current state
3. The Restoration Room soothes despair after Azkaban content
4. The Training Room spawns a Dueling Dummy for spell practice
5. All modes are localized in EN + ZH
6. Owl Post chat is no longer hardcoded English
7. 98 verification checks protect all content
