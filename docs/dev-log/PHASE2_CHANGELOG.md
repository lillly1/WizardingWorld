# Phase 2 Changelog

## Summary

Phase 2 transforms the Wizarding World mod from a large feature bundle into a more polished, canon-aligned experience with stronger progression coherence, richer late-game content, and zero documentation drift.

**Files changed/added: ~25 | New systems: 4 | New items: 8 | Canon fixes: 11**

---

## Phase A: Consistency Audit & Repair

### Documentation Fixes
- Fixed file counts: 428 -> 440 C# files, 922 -> 935 total files
- Fixed wand count: "28 + 3 Upgrades" -> "22 + 1 Upgrade" (2 upgrades were [Obsolete])
- Fixed accessories count: 50 -> 53
- Fixed boss progression order: Dementor King is post-Golem (penultimate), Voldemort is post-Cultist (TRUE final boss)
- Fixed Voldemort HP: 50,000 -> 60,000 (matching code)
- Fixed Dementor King HP: 60,000/75,000 -> 45,000 (matching code)
- Fixed Elder Wand stats: 120 dmg -> 150 dmg, 15 mana -> 40 mana, 12 use -> 18 use
- Fixed Fluffy Phase 3: removed false "fire breath" text, replaced with actual mechanics (boulder barrage + bark shockwaves)
- Fixed Voldemort drop table: removed phantom Invisibility Cloak/Resurrection Stone drops, added actual Gaunt's Ring drop
- Removed duplicate PotionsMaster NPC entry

### Critical Bug Fixes
- **BUG-1 FIXED**: Resurrection Stone was completely unobtainable (no recipe, no drops). Now obtained by purifying Gaunt's Ring after completing Horcrux Hunt.
- **BUG-2 FIXED**: Voldemort drop table in docs didn't match code. Now accurately reflects actual drops.
- **BUG-3 FIXED**: Boss progression order was swapped in guides. Corrected to match BossChecklistIntegration.

---

## Phase B: Canon Cleanup

### B1: Elder Wand / Wand of Destiny Refactor
- ShadowElderWand and WandOfDestiny were already marked `[Obsolete]` in code
- Replaced the upgrade crafting path with the **Wand Mastery System** (see Phase C)
- Updated all guide text to reflect mastery-based progression
- Removed obsolete wand entries from guide tables

### B2: Invisibility Cloak Split
- **NEW ITEM**: `DemiguiseCloak` — craftable stealth accessory (15 Demiguise Hair + 20 Silk + 10 Soul of Light + 20 Essence of Magic)
  - Grants invisibility + 200 aggro reduction (lesser version)
  - Rarity: Light Purple
- **CHANGED**: `InvisibilityCloak` — no longer craftable
  - Now obtained exclusively through the Hallows Attunement questline
  - Remains the true Deathly Hallow with -400 aggro + 5% damage
  - Still triggers Master of Death when combined with Elder Wand + Resurrection Stone

### B3: Resurrection Stone Acquisition
- **NEW ITEM**: `GauntsRing` — cursed Horcrux accessory dropped by Voldemort (100%)
  - +8% damage, -4 defense, slow life drain
  - Dynamic tooltips show Horcrux Hunt progress
  - Purifiable into Resurrection Stone after all 4 Horcruxes destroyed
- **CHANGED**: `ResurrectionStone` now has a recipe: 1 Gaunt's Ring + 10 Essence of Magic + 5 Soul of Light at Enchanting Table
- **CHANGED**: Voldemort now drops Gaunt's Ring (guaranteed) in both normal and expert mode

### B4: Spell Fidelity
- **VERIFIED**: Riddikulus already correctly targets Boggarts with 5x damage bonus (canon-faithful)
- **VERIFIED**: Reparo already replaced by Episkey for healing (canon-faithful: Episkey heals, Reparo repairs objects)
- No changes needed — spells were already correct

### B5: Creature/Category Cleanup
- **RENAMED**: `CorruptedPatronus` -> `PhantomStag`
  - "A spectral mockery of a true Patronus, twisted by the Hallow's wild magic"
  - Labeled as "Fully original mod content"
  - All behavior unchanged
- **FIXED**: `Peeves` — removed overworld night spawn (0.02f), now dungeon-only
  - "Peeves is Hogwarts-specific — dungeon only (represents castle ruins)"
- **FIXED**: `Dementor` — no longer generic Hardmode night flyer
  - Now spawns only during: Blood Moon, Death Eater Invasion, or Forbidden Forest at night
  - Labeled as "Canon-faithful: appear during times of great darkness"
- **FIXED**: `Obscurus` — reclassified from generic beast to "dark parasitic force / magical phenomenon"
  - All behavior unchanged, only classification and documentation updated

### B6: Fluffy
- **VERIFIED**: Fluffy code has no fire breath — uses bark shockwaves, boulder projectiles, and pushback
- Guide text was the only issue (fixed in Phase A)

### B7: House Points -> House Renown
- **RENAMED**: `HousePointsSystem` -> `HouseRenownSystem`
  - All references updated (ClassQuestSystem, QuidditchSeasonSystem, WizardsAlmanac)
  - "House Cup" milestone renamed to "House Champion"
  - Combat text changed from "+N HP" to "+N HR"
  - Distinguished from canonical House Points (awarded by professors)

---

## Phase C: Expansion Content

### C1: Wand Mastery System (NEW)
Three new files implementing wand attunement through use:

- `Common/Systems/WandMasteryGlobalItem.cs` — applies mastery bonuses to wands
- `Common/Systems/WandMasteryGlobalProjectile.cs` — tracks spell hits for XP
- WizardPlayer additions: `wandMasteryXP` dictionary, save/load support

**Mastery Levels:**
| Level | Name | XP Required | Bonus |
|-------|------|------------|-------|
| 0 | New | 0 | None |
| 1 | Familiar | 100 | -5% mana cost |
| 2 | Attuned | 400 | -10% mana cost, +5% damage |
| 3 | Mastered | 1000 | -15% mana cost, +10% damage, golden visual |

- XP gained: 1 per enemy hit with spell projectile
- XP cap: 1500 per wand
- Color-coded tooltips show mastery level and progress
- Mastered wands produce golden sparkle particles on hit
- Persisted to player save data

### C2: Horcrux Hunt Questline Expansion
Enhanced `HorcruxHuntSystem` with full questline mechanics:

- **New method**: `AttemptDestroyHorcrux()` — requires Basilisk Fang or Sword of Gryffindor in inventory
- **New method**: `CheckHallowsAttunement()` — detects when all 3 Deathly Hallows are assembled
- **New field**: `hallowsAttuned` — tracks permanent Hallows mastery (saved to world data)
- **Horcrux destruction**: Right-click any Horcrux accessory to attempt destruction
  - Each Horcrux shows unique dramatic flavor text on destruction
  - All 4 destroyed: "Gaunt's Ring can now be purified!" message
- **Hallows Attunement**: Assembling all 3 Hallows triggers golden visual + corruption cleanse
- All 4 Horcrux accessories updated with `CanRightClick()` and `RightClick()` handlers

**Full Questline Flow:**
1. Obtain Horcrux accessories (crafting/drops)
2. Get a Basilisk Fang or Sword of Gryffindor
3. Right-click each Horcrux to destroy it (4 total)
4. Defeat Voldemort to get Gaunt's Ring + Elder Wand
5. Purify Gaunt's Ring into Resurrection Stone (crafting)
6. Craft/obtain Invisibility Cloak (or get DemiguiseCloak as alternative)
7. Assemble all 3 Hallows for Master of Death status

### C3: Azkaban's Despair Event (NEW)
Three new files implementing a late-game Dementor escape event:

- `Common/Systems/AzkabanDespairEvent.cs` — ModSystem managing event state
- `Content/Items/Consumables/AzkabanBreachStone.cs` — trigger item
- `Content/Items/Accessories/AzkabanWardensKey.cs` — event reward accessory

**Event Mechanics:**
- Triggers: Post-Golem, night only, via Azkaban Breach Stone
- Light suppression: 40% reduced lighting
- Despair: -4 life regen unless Patronus active, periodic Darkness debuff
- Kill tracking: Dementor +2, AzkabanGuard +5, vanilla dark enemies +1
- Progress: 100 + 25 per active player kills to complete
- Ends at dawn or when progress reaches max
- Dementors spawn at 0.15f chance (enhanced rate)
- AzkabanGuards spawn at 0.12f chance during event

**Azkaban Warden's Key** (1/20 drop from AzkabanGuard):
- +6 defense, +5% damage reduction
- Darkness/Blackout immunity
- Patronus synergy: +12% damage, +4 life regen when Patronus active

### C4: Room of Requirement (NEW)
Five new files implementing a magical adaptive room:

- `Content/Items/Consumables/RoomOfRequirementKey.cs` — reusable key item
- `Content/Buffs/RoomRecoveryBuff.cs`
- `Content/Buffs/RoomTrainingBuff.cs`
- `Content/Buffs/RoomVaultBuff.cs`
- `Content/Buffs/RoomSanctuaryBuff.cs`

**Room Modes** (auto-detected from player state):
| Mode | Trigger | Buff (3 min) |
|------|---------|-------------|
| Recovery | HP < 50% | +12 life regen, +15 def, +10% DR |
| Training | Holding wand | +20% spell dmg, +10 spell crit, +40 max mana |
| Vault | 10+ gold coins | +0.3 luck, treasure sense, coin attraction |
| Sanctuary | Default | +8 def, +4 life regen, +50 mana regen, +5% DR |

- Reusable with cooldown (can't use while any Room buff active)
- 30-particle MagicMirror visual effect on activation
- Crafted: 10 Gold Bars + 25 Essence of Magic + 5 Soul of Light + 5 Soul of Night

---

## Intentionally Deferred

- **Triwizard Tournament**: Too complex for this pass, needs arena/challenge framework
- **Full creature ecosystem**: Needs UI framework for field guide, capture mechanics
- **Hogsmeade village expansion**: Worldgen exists but needs proper NPC shops
- **Machine-readable ContentManifest.json**: Deferred to future tooling pass
- **McGonagall / Lupin NPC**: Would need full shop + dialogue implementation
- **Localization updates**: PhantomStag rename needs localization file updates for all languages
- **Sprite assets**: New items (DemiguiseCloak, GauntsRing, etc.) need PNG sprites

## Canon Compromises Remaining

1. **DemiguiseCloak grants full invisibility**: In canon, Demiguise cloaks fade over time. Kept for gameplay fun.
2. **Patronus as combat summon**: Canon Patronus is purely defensive. Kept as summon weapon for gameplay variety.
3. **Horcrux destruction via right-click**: Simplified from canon complexity. Requires lore-correct tools (Basilisk Fang/Gryffindor Sword).
4. **PhantomStag in Hallow biome**: Original content, not canon. Clearly labeled as mod-original.
5. **Dementor spawning during Blood Moon**: Minor canon stretch (Dementors aren't tied to blood). Justified as "time of darkness."

## Future Expansion Hooks

1. **WandloreSystem**: Full taxonomy of woods, cores, and schools ready for wand ceremony system
2. **ClassQuestSystem**: 4 school subjects with mastery tracking ready for Hogwarts campus content
3. **HogsmeadeGen**: Village structures already generating, ready for NPC population
4. **QuidditchSeasonSystem**: Multi-snitch seasonal events ready for Triwizard integration
5. **AzkabanDespairEvent**: Event framework ready for boss escalation (Azkaban Warden boss)
6. **HorcruxHuntSystem**: `hallowsAttuned` flag ready to gate exclusive content
7. **WandMasteryGlobalItem**: Ready for per-wood mastery bonuses using WandWood enum
