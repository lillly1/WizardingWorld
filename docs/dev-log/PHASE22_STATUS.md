# Phase 22 Status -- St Mungo's Hospital for Magical Maladies and Injuries

## Summary

Built a complete St Mungo's magical triage system with 3 rotating ward missions (Spell Damage, Creature Injuries, Potion Accidents), a Healer town NPC anchor, Healer's Satchel supportive accessory, and lore-shaped hospital framing. NOT a resurrection service -- focused on triage and stabilization. 269/269 verification checks pass.

## Gameplay Loop

1. **Unlock**: Defeat Hungarian Horntail, then use St Mungo's Visitor Pass
2. **Entrance framing**: "You approach what looks like a condemned department store... the glass shimmers and admits you"
3. **Mission**: 3-minute triage -- stabilize 3 ward nodes by destroying them
4. **Ward rotation** (cycles automatically):
   - **Spell Damage Ward**: Hex Residue Nodes (purple, ManaSickness hazard)
   - **Creature Injuries Ward**: Venom Wound Nodes (green, Venom hazard)
   - **Potion Accidents Ward**: Cauldron Spill Nodes (orange, Confused/burn hazard)
5. **Completion**: Healer's Salts (scales) + Essence + Triage Resolved buff (10 min: +8 regen, +5 def, venom immunity) + gold + 15 House Renown
6. **Crafting**: Healer's Salts -> Healer's Satchel accessory

## Content Added

### New NPC: Healer (Town NPC #13)
- St Mungo's staff member (names: Healer Miriam, Augustus, Hippocrates)
- Moves in after hospital unlocked
- **Shop**: St Mungo's Pass, Pepperup Potion, Skele-Gro, Mandrake Restorative, Healing Potion, Restoration Potion
- **Triage Status button**: shows current ward and mission state

### New Items

| Item | Type | Effect |
|------|------|--------|
| St Mungo's Visitor Pass | Reusable access | Unlocks + starts triage missions |
| Healer's Salts | Material | Scales with completions |
| Healer's Satchel | Accessory | +6 regen, +5% endurance, Venom/Confused immunity, reduced potion cooldown, despair relief |
| Triage Resolved | Buff (10 min) | +8 regen, +5 defense, venom immunity |

### Ward Objectives (3 types)

| Node | Ward | Hazard | HP |
|------|------|--------|-----|
| Hex Residue Node | Spell Damage | ManaSickness aura | 250 |
| Venom Wound Node | Creature Injuries | Venom aura | 280 |
| Cauldron Spill Node | Potion Accidents | Confused/burn aura | 220 |

## Canon Assessment

| Element | Status |
|---------|--------|
| St Mungo's as hidden hospital | Canon-faithful |
| Concealed entrance (department store) | Canon-faithful |
| Healer as hospital staff | Canon-faithful |
| Pepperup / Skele-Gro / Mandrake Restorative in shop | Canon-faithful |
| Spell damage / creature injuries / potion accidents as ward types | Canon-faithful |
| Triage / stabilization framing | Canon-faithful |
| NOT resurrection or miracle cures | **Verified by check** |
| Ward node objectives | Mod-original |
| Healer's Satchel accessory | Mod-original |
| Healer's Salts material | Mod-original |

## Files Created (15)

| File | Type |
|------|------|
| Common/Systems/StMungosTriageSystem.cs | ModSystem (3 wards + rotation) |
| Content/Items/Consumables/StMungosPass.cs + .png | Access item |
| Content/Items/Consumables/HealersSalts.cs + .png | Material |
| Content/Items/Accessories/HealersSatchel.cs + .png | Supportive accessory |
| Content/Buffs/TriageResolvedBuff.cs + .png | Reward buff |
| Content/NPCs/Enemies/HexResidueNode.cs + .png | Spell Damage ward node |
| Content/NPCs/Enemies/VenomWoundNode.cs + .png | Creature ward node |
| Content/NPCs/Enemies/CauldronSpillNode.cs + .png | Potion ward node |
| Content/NPCs/Town/Healer.cs + .png + _Head.png | Town NPC |
| PHASE22_STATUS.md | This document |

## Files Modified (6)

| File | Change |
|------|--------|
| Localization/en-US_Mods.WizardingWorld.hjson | +5 items/NPCs, +1 buff, +3 nodes, +dialogue, +bestiary, +17 system text keys |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +all matching Chinese entries |
| scripts/mechanical_data/shops.json | +Healer shop (6 items) |
| scripts/guide_content.json | +Healer NPC, "Thirteen NPCs", +St Mungo's system (fixed "House Renown") |
| scripts/verify_guide.py | +38 St Mungo's checks incl. resurrection safety (269 total) |
| scripts/content_manifest.json | Regenerated (516 C#, 65 enemies, 13 NPCs, 60 accessories) |

## Verification Results

**269 passed, 0 failed, 0 warnings.**

38 new St Mungo's checks:
- System + NPC + node .cs existence (5)
- Sprite checks: 3 nodes + material + accessory + buff + pass + Healer body/head (9)
- EN localization: items + NPCs + nodes + buff + dialogue + system text (12)
- ZH localization: items + nodes + Healer + system text (6)
- Guide content: St Mungo's mention + Healer mention (2)
- Canon safety: no resurrection claims (1)
- Shop export: Healer shop (1)
- Hardcoded English check (1)
- Guide framing check (1)

## Is the St Mungo's Slice Playable and Closed?

**Yes.** A player can:
1. Defeat the Horntail to unlock St Mungo's
2. Craft/buy a Visitor Pass from the Healer NPC
3. Enter through the hidden hospital entrance
4. Complete 3 rotating ward missions (Spell/Creature/Potion)
5. Earn Healer's Salts + Triage Resolved buff
6. Craft the Healer's Satchel for permanent support utility
7. All text localized EN + ZH
8. Framed as triage/stabilization, verified not resurrection
9. 269 checks protect the full content stack

## Remaining Limitations

- Placeholder sprites throughout
- No visual hospital environment (text-based ward identity)
- No Janus Thickey Ward or long-term spell damage content (intentionally deferred)
- Ward hazards are periodic debuff pulses, not visual environmental effects
- zh-Hant not updated
