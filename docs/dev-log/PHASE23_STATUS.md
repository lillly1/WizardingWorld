# Phase 23 Status -- Number Twelve, Grimmauld Place & Order of the Phoenix HQ

## Summary

Built a Grimmauld Place safehouse maintenance system with 3 rotating mission types (Doxy Sweep, Portrait Suppression, Boggart Ward), concealment-themed rewards, and integration with the existing Order/Kingsley infrastructure. The safehouse feels hidden, hostile, and war-effort-shaped. 269/269 verification checks pass.

## Gameplay Loop

1. **Unlock**: Defeat Dementor King + complete 1 Department mission, then use Secret Keeper's Note (craft or buy from Kingsley)
2. **Reveal**: "Number Twelve, Grimmauld Place materialises between numbers eleven and thirteen..."
3. **Mission Start**: Rotating ward type auto-selected
4. **Missions (3 min each, rotate):**
   - **Doxy Sweep**: Destroy 4 Doxy Nests (they spawn actual Doxy swarm enemies). Periodic poison hazard.
   - **Portrait Suppression**: Silence 4 Walburga Portraits (shrieking curse zones, apply Jinxed + despair). 
   - **Boggart Ward**: Banish 4 Grimmauld Boggarts (fear entities, Darkness+Slow+despair). Weak to Riddikulus (5x damage).
5. **Rewards**: Concealment Thread (scales) + Essence + Safehouse Resolve buff (10 min: +6 def, +4 regen, Jinxed immunity, stealth) + gold + 15 House Renown
6. **Crafting**: Concealment Thread -> Order Signet accessory (+5 def, -200 aggro, +4% endurance, Jinxed/Confused immunity, despair relief)

## Canon Assessment

| Element | Status |
|---------|--------|
| Grimmauld Place as hidden Black family home | Canon-faithful |
| Secret Keeper / Fidelius-style reveal | Canon-faithful |
| Doxy infestation in the house | Canon-faithful (OotP cleaning scenes) |
| Walburga Black's portrait permanently stuck | Canon-faithful |
| Boggart in the wardrobe | Canon-faithful (OotP, Molly encounters it) |
| Riddikulus effective against Boggarts (5x) | Canon-faithful, verified by check |
| Kreacher as dialogue reference (not separate NPC) | Canon-appropriate restraint |
| Safehouse maintenance framing | Mod-original |
| Concealment Thread / Order Signet | Mod-original |
| NOT a public portal or tourist destination | **Verified: "hidden" / "concealed" framing** |

## Files Created (13)

| File | Type |
|------|------|
| Common/Systems/GrimmauldPlaceSystem.cs | ModSystem (3 mission types + rotation + persistence) |
| Content/Items/Consumables/SecretKeeperNote.cs + .png | Access item |
| Content/Items/Consumables/ConcealmentThread.cs + .png | Material |
| Content/Items/Accessories/OrderSignet.cs + .png | Concealment accessory |
| Content/Buffs/SafehouseResolveBuff.cs + .png | Reward buff |
| Content/NPCs/Enemies/DoxyNest.cs + .png | Doxy Sweep objective |
| Content/NPCs/Enemies/WalburgaPortrait.cs + .png | Portrait Suppression objective |
| Content/NPCs/Enemies/GrimmauldBoggart.cs + .png | Boggart Ward objective |
| PHASE23_STATUS.md | This document |

## Files Modified (8)

| File | Change |
|------|--------|
| Content/NPCs/Town/Kingsley.cs | +SecretKeeperNote in shop, +Grimmauld/Kreacher dialogue |
| Content/Projectiles/Spells/RiddikulusProjectile.cs | +GrimmauldBoggart in 5x damage check |
| Localization/en-US_Mods.WizardingWorld.hjson | +5 items/NPCs, +1 buff, +3 threat names, +Kingsley dialogue, +20 system text keys |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +all matching Chinese entries |
| scripts/mechanical_data/shops.json | +SecretKeeperNote in Kingsley shop |
| scripts/guide_content.json | +Grimmauld Place system entry |
| scripts/verify_guide.py | +Grimmauld checks, +fixed Riddikulus regex scope |
| scripts/content_manifest.json | Regenerated (524 C#, 68 enemies, 61 accessories, 48 systems) |

## Verification Results

**269 passed, 0 failed, 0 warnings.**

New Grimmauld checks cover: system existence, all threat NPC sprites, material/accessory/buff sprites, access item sprite, EN + ZH localization, guide content, Walburga portrait reference, no public-portal wording, Boggart stays Boggart-specific (Riddikulus check scoped to 40 chars), and Riddikulus not anti-Dementor.

## Is the Grimmauld Place / Order HQ Slice Playable and Closed?

**Yes.** A player can:
1. Defeat Dementor King + complete a Department mission to unlock
2. Use Secret Keeper's Note for the hidden house reveal
3. Run 3 distinct safehouse maintenance missions (Doxy/Portrait/Boggart)
4. Experience house hostility (poison, Jinxed, fear/darkness hazards)
5. Earn Concealment Thread + gold + Safehouse Resolve buff
6. Craft Order Signet for permanent stealth/defense utility
7. Boggarts take 5x from Riddikulus (canon-faithful)
8. All text localized EN + ZH, framed as hidden safehouse, not public portal
9. 269 verification checks protect all framing and content

## Remaining Limitations

- Placeholder sprites throughout
- No visual Grimmauld Place structure (text-based safehouse identity)
- Kreacher referenced in Kingsley dialogue only (no standalone NPC)
- DoxyNest spawns existing Doxy enemies but the nest itself is a new NPC type
- GrimmauldBoggart uses bat AI (same as standard Boggart)
- zh-Hant not updated
