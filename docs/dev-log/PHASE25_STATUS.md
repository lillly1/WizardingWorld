# Phase 25 Status -- Whomping Willow & Shrieking Shack Vertical Slice

## Summary

Built a 3-loop secret passage system: Willow Suppression, Tunnel Stabilization, and Shack Containment. Gated behind Fenrir Greyback (werewolf-themed progression), nighttime-only. Protective/stabilizing rewards. Softened UnicornBlood legacy tooltip. 287/287 verification checks pass.

## UnicornBlood Legacy Framing

**Action taken**: Softened the EN tooltip from "Rare drop -- only found in the Forbidden Forest at night" to "Found as tainted remnants in the Forbidden Forest -- never from harming unicorns". This explicitly removes any implication of unicorn killing while preserving the item's functionality and all existing recipes.

**Added verification check**: "UnicornBlood: no positive hunting framing" rejects "hunt unicorn" or "kill unicorn" language in EN localization.

**Status**: Legacy item preserved for backward compatibility. No recipes broken. No new unicorn-killing loops. Framing now explicitly safe.

## Gameplay Loop

1. **Unlock**: Defeat Fenrir Greyback, then use Willow Passage Token (craft or buy from Hagrid)
2. **Requirement**: Nighttime only
3. **Entry framing**: "Hagrid slips you a small wooden token... 'Know how to freeze the Willow?'"
4. **Missions (3 min each, rotate):**
   - **Willow Suppression**: Disable 3 Whomping Willow Knots. Hazard: periodic knockback pulses.
   - **Tunnel Stabilization**: Secure 4 Tunnel Wards. Hazard: periodic Darkness + Slow.
   - **Shack Containment**: Seal 3 Moon Ward Fractures. Hazard: periodic Jinxed + despair buildup.
5. **Rewards**: Moonsilver Thread (scales) + Essence + Moonward Protection buff (10 min: +6 def, +4 regen, Darkness/Jinxed immunity, +5% speed) + gold + 15 House Renown
6. **Crafting**: Moonsilver Thread -> Moonward Pendant (+5 def, +4% endurance, Darkness/Jinxed immunity, faster night movement)

## Canon Assessment

| Element | Status |
|---------|--------|
| Whomping Willow guards a passage | Canon-faithful |
| Tunnel to the Shrieking Shack | Canon-faithful |
| Shack built for werewolf containment | Canon-faithful |
| Post-Fenrir progression gate | Canon-appropriate (werewolf-adjacent) |
| NOT a tourist destination or public space | **Verified: no "tourist" framing** |
| NOT lycanthropy cure or werewolf reward | Rewards are protective/stabilizing |
| Moon ward / containment themes | Canon-inspired mod-original |
| Moonsilver Thread / Moonward Pendant | Mod-original |

## Files Created (17)

| File | Type |
|------|------|
| Common/Systems/ShriekingShackSystem.cs | ModSystem (3 loops + rotation) |
| Content/Items/Consumables/WillowPassageToken.cs + .png | Access item |
| Content/Items/Consumables/MoonsilverThread.cs + .png | Material |
| Content/Items/Accessories/MoonwardPendant.cs + .png | Protective accessory |
| Content/Buffs/MoonwardBuff.cs + .png | Reward buff |
| Content/NPCs/Enemies/WillowKnot.cs + .png | Willow Suppression objective |
| Content/NPCs/Enemies/TunnelWard.cs + .png | Tunnel Stabilization objective |
| Content/NPCs/Enemies/MoonWardFracture.cs + .png | Shack Containment objective |
| PHASE25_STATUS.md | This document |

## Files Modified (8)

| File | Change |
|------|--------|
| Content/NPCs/Town/Hagrid.cs | +WillowPassageToken in shop (DownedFenrir condition) |
| Common/Systems/DownedBossSystem.cs | +DownedFenrirCondition class |
| Localization/en-US_Mods.WizardingWorld.hjson | +items, +buff, +3 NPC names, +16 Shack system text keys, +UnicornBlood tooltip fix |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +all matching Chinese entries |
| Localization/zh-Hant_Mods.WizardingWorld.hjson | +all matching Traditional Chinese entries |
| scripts/mechanical_data/shops.json | +WillowPassageToken in Hagrid shop |
| scripts/guide_content.json | +Whomping Willow & Shrieking Shack system entry |
| scripts/verify_guide.py | +18 Phase 25 checks incl. UnicornBlood safety + Shack framing |

## Verification Results

**287 passed, 0 failed, 0 warnings.**

18 new checks:
- ShriekingShackSystem.cs existence
- 7 sprite checks (3 items + 1 buff + 3 objectives)
- 4 EN localization (items + buff + system text)
- 2 ZH-Hans localization
- 1 Shack not-tourist-destination check
- 1 UnicornBlood no-hunting-framing check
- 1 guide content (Whomping Willow + Shrieking Shack mentioned)

## Is the Whomping Willow / Shrieking Shack Slice Playable and Closed?

**Yes.** A player can:
1. Defeat Fenrir Greyback to unlock
2. Craft/buy Willow Passage Token from Hagrid
3. Use at night for secret-entry framing
4. Complete 3 distinct rotating missions (Willow/Tunnel/Shack)
5. Experience different hazards per loop (knockback / darkness / moon-ward despair)
6. Earn Moonsilver Thread + Moonward Protection buff
7. Craft Moonward Pendant for permanent night defense
8. All text localized EN + ZH-Hans + ZH-Hant
9. UnicornBlood tooltip explicitly says "never from harming unicorns"
10. 287 verification checks protect all framing and content

## Remaining Limitations

- Placeholder sprites throughout
- No visual Whomping Willow structure (text-based framing)
- No actual tunnel traversal mechanic (narrative transition)
- No werewolf enemy spawns during Shack Containment (hazard is ward-based, not feral combat)
- zh-Hant coverage may be less comprehensive than EN/ZH-Hans for older phases
