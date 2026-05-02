# Phase 21 Status -- Knockturn Alley & Borgin and Burkes Vertical Slice

## Summary

Built a complete Knockturn Alley cursed-object containment loop with Mr Borgin as NPC anchor, Cursed Opal Necklace and Vanishing Cabinet Echo as iconic hazard objectives, Hand of Glory as the signature reward accessory, and dark commerce framing. 230/230 verification checks pass.

## Gameplay Loop

1. **Unlock**: Visit Diagon Alley + defeat Bellatrix, then use Knockturn Pass
2. **Street framing**: "A narrow side-alley catches your eye... Knockturn Alley"
3. **Contract**: Talk to Mr Borgin or use Pass to start containment contract
4. **Mission (3 min)**: Contain 3 dark objects:
   - **Cursed Opal Necklaces** (x2): Apply DarkCurse + despair on proximity. Destroy to contain.
   - **Vanishing Cabinet Echo** (x1): Teleports nearby players randomly + Confused. Destroy to seal.
5. **Hazards**: Periodic darkness pulses, despair buildup, slight Dark Arts corruption on mission start
6. **Reward**: Cursed Residue (scales) + Essence + Dark Appraiser buff (8 min: night vision, +6% dmg, danger sense) + gold + 10 House Points
7. **Crafting**: Cursed Residue -> **Hand of Glory** (night vision, spelunker, stealth, darkness bonuses)

## Iconic Dark Object Implementation

| Object | Canon Reference | Gameplay Role |
|--------|----------------|--------------|
| Cursed Opal Necklace | Nearly killed Katie Bell (HBP) | Containment objective -- DarkCurse + despair aura |
| Vanishing Cabinet | Draco's infiltration plot (HBP) | Containment objective -- teleport displacement + confusion |
| Hand of Glory | "Light only to the holder" (HBP) | Reward accessory -- night vision, stealth, darkness bonus |

## Canon Assessment

| Element | Status |
|---------|--------|
| Knockturn Alley as dark side-street | Canon-faithful |
| Borgin and Burkes as dark-object shop | Canon-faithful |
| Mr Borgin: suspicious, transactional | Canon-faithful |
| Cursed Opal Necklace as dangerous | Canon-faithful |
| Vanishing Cabinet as unstable transport | Canon-faithful |
| Hand of Glory: holder-only light | Canon-faithful (verified by check) |
| Containment contract framing | **Mod-original** (sanctioned work, not theft) |
| Dark Arts corruption risk | **Mod-original** (slight corruption on mission start) |

## Files Created (13)

| File | Type |
|------|------|
| Common/Systems/KnockturnAlleySystem.cs | ModSystem (containment missions) |
| Content/Items/Consumables/KnockturnPass.cs + .png | Access item |
| Content/Items/Consumables/CursedResidue.cs + .png | Crafting material |
| Content/Items/Accessories/HandOfGlory.cs + .png | Signature reward accessory |
| Content/Buffs/DarkAppraiserBuff.cs + .png | Mission reward buff |
| Content/NPCs/Enemies/CursedOpalDisplay.cs + .png | Mission objective |
| Content/NPCs/Enemies/VanishingCabinetEcho.cs + .png | Mission objective |
| Content/NPCs/Town/MrBorgin.cs + .png + _Head.png | Town NPC |
| PHASE21_STATUS.md | This document |

## Files Modified (6)

| File | Change |
|------|--------|
| Localization/en-US_Mods.WizardingWorld.hjson | +5 items/NPCs, +1 buff, +2 enemies, +dialogue, +bestiary, +16 system text keys |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +all matching Chinese entries |
| scripts/mechanical_data/shops.json | +MrBorgin shop (3 items) |
| scripts/guide_content.json | +MrBorgin NPC, "Twelve NPCs", +Knockturn system |
| scripts/verify_guide.py | +19 Phase 21 checks incl. Hand of Glory framing (230 total) |
| scripts/content_manifest.json | Regenerated (507 C#, 62 enemies, 12 NPCs, 59 accessories) |

## Verification Results

**230 passed, 0 failed, 0 warnings.**

19 new checks:
- KnockturnAlleySystem.cs + MrBorgin + HandOfGlory existence
- 5 sprite checks
- 5 EN localization + 1 system text + 1 Hand of Glory framing
- 2 ZH localization
- 1 shop export
- 1 guide content

## Is the Knockturn Alley & Borgin and Burkes Slice Playable and Closed?

**Yes.** A player can:
1. Visit Diagon Alley and defeat Bellatrix to unlock Knockturn
2. Craft/buy Knockturn Pass from Mr Borgin
3. Start timed containment contracts with 3 iconic dark objects
4. Experience distinct hazards: curse auras, cabinet teleportation, darkness pulses
5. Earn Cursed Residue + gold + Dark Appraiser buff
6. Craft the Hand of Glory for permanent night vision + stealth + darkness power
7. All text localized EN + ZH, framed as containment work
8. 230 verification checks protect all content and framing

## Remaining Limitations

- Placeholder sprites throughout
- No visual Knockturn Alley street (text-based identity)
- Dark Arts corruption is a small fixed amount (0.03) per mission start, not deeply integrated
- No additional Borgin stock items beyond KnockturnPass + DarkArtsTome + SoulofNight
- No extended dark-object catalog beyond opal necklace + cabinet
- zh-Hant not updated
