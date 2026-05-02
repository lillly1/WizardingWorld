# Phase 19 Status -- Diagon Alley & Gringotts Vertical Slice

## Summary

Built a complete Gringotts vault retrieval mission loop with Goblin Teller NPC, Vault Relic collection objectives, Cursed Treasure hazards, anti-theft jinx bursts, and a treasure-themed reward economy (Galleon Dust -> Goblin Ledger accessory). Framed as sanctioned goblin recovery contracts, not robbery. 200/200 verification checks pass.

## Gameplay Loop

1. **Unlock**: Defeat Basilisk, then use Gringotts Vault Key (craft or buy from Goblin Teller)
2. **Mission Start**: 3-minute sanctioned vault recovery
3. **Objectives**: Collect 4 glowing Vault Relics (walk into them)
4. **Hazards**:
   - Anti-theft jinx bursts every 15 seconds (random Confused debuff)
   - Cursed Treasure decoys (Geminus Curse themed, burn players on proximity with OnFire)
5. **Completion**: Galleon Dust (scales with completions) + Essence of Magic + Vault Fortune buff (8 min: +luck, coin attraction, +5% dmg) + Gold Coins + 15 House Points
6. **Crafting**: Accumulated Galleon Dust -> Goblin Ledger accessory (+luck, coin attraction, treasure sense, +5% dmg)
7. **Repeatable**: Escalating gold rewards per run

## Lore Framing

The vault runs are explicitly framed as **goblin-authorized recovery contracts**, not theft:
- System comment: "NOT framed as robbery -- this is authorized bank work"
- Goblin Teller dialogue: "Your contract record is satisfactory. Proceed."
- Guide canon note: "sanctioned recovery contracts, not robbery"
- Verification check: confirms guide does NOT frame runs as robbery

## Content Added

### New NPC: Goblin Teller (Town NPC #11)
- Gringotts bank teller (names: Griphook, Bogrod, Ragnok)
- Moves in after Basilisk defeated
- **Shop**: Vault Key, Gold Coins (Hardmode), Platinum Coins (post-Moon Lord)
- **Vault Status button**: shows current mission state
- Attacks by throwing gold coins

### New Items

| Item | Type | Stats/Role |
|------|------|-----------|
| Gringotts Vault Key | Reusable access | Unlocks + starts vault missions |
| Galleon Dust | Crafting material | Scales with mission completions |
| Goblin Ledger | Accessory | +0.15 luck, coin attraction, treasure sense, +5% dmg |
| Vault Fortune | Buff (8 min) | +0.2 luck, coin attraction, +5% dmg |

### New Mission Entities
- **Vault Relic**: Glowing golden collectible (4 per mission)
- **Cursed Treasure**: Geminus Curse trap, burns on proximity (2 per mission)

## Files Created (14)

| File | Type |
|------|------|
| Common/Systems/GringottsVaultSystem.cs | ModSystem (vault mission + persistence) |
| Content/Items/Consumables/GringottsVaultKey.cs + .png | Access item |
| Content/Items/Consumables/GalleonDust.cs + .png | Crafting material |
| Content/Items/Accessories/GoblinLedger.cs + .png | Treasure accessory |
| Content/Buffs/VaultFortuneBuff.cs + .png | Mission reward buff |
| Content/NPCs/Enemies/VaultRelic.cs + .png | Mission objective |
| Content/NPCs/Enemies/CursedTreasure.cs + .png | Mission hazard |
| Content/NPCs/Town/GoblinTeller.cs + .png + _Head.png | Town NPC |
| PHASE19_STATUS.md | This document |

## Files Modified (6)

| File | Change |
|------|--------|
| Localization/en-US_Mods.WizardingWorld.hjson | +5 items/NPCs, +1 buff, +2 enemies, +dialogue, +bestiary, +15 system text keys |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +all matching Chinese entries |
| scripts/mechanical_data/shops.json | +GoblinTeller shop (3 items) |
| scripts/guide_content.json | +GoblinTeller NPC, "Eleven NPCs", +Diagon/Gringotts system |
| scripts/verify_guide.py | +17 Phase 19 checks incl. canon robbery safety (200 total) |
| scripts/content_manifest.json | Regenerated (496 C#, 60 enemies, 11 NPCs, 58 accessories) |

## Verification Results

**200 passed, 0 failed, 0 warnings.**

17 new checks:
- GringottsVaultSystem.cs + GoblinTeller NPC existence
- 5 sprite checks (3 items + NPC + buff)
- 5 EN localization + 1 system text
- 2 ZH localization
- 1 canon robbery safety check (allows "not robbery" negation)
- 1 GoblinTeller shop export

## Is the Diagon Alley & Gringotts Slice Playable and Closed?

**Yes.** A player can:
1. Defeat the Basilisk to unlock Gringotts access
2. Craft/buy a Vault Key from the Goblin Teller
3. Start timed vault retrieval missions
4. Collect Vault Relics while dodging jinxes and Cursed Treasure
5. Receive Galleon Dust + gold + Vault Fortune buff
6. Craft the Goblin Ledger for permanent treasure-finding utility
7. All text localized EN + ZH, framed as sanctioned bank work
8. 200 verification checks protect the full content stack

## Remaining Limitations

- Placeholder sprites throughout
- No visual Diagon Alley street or Gringotts building (missions use overworld space)
- No Leaky Cauldron / Tom the Barman NPC (deferred)
- No Knockturn Alley content
- No cart ride mechanic
- zh-Hant not updated
