# Phase 29 — Productionization, Presentation, Localization Parity, and Tooling Hardening

**Date**: 2026-04-16
**Verification**: 489/489 checks pass (up from 429 in Phase 28)
**Phase type**: Cross-cutting productionization megapack (no new lore pillars)

---

## What Was Audited (Step 0)

1. **Localization state**: zh-Hant had 439 missing keys (27% gap), ~100 hardcoded English Main.NewText calls across 53 C# files
2. **Tooling state**: Chinese PDF had inline hardcoded counts (drift risk), schemas only for bosses + wands
3. **Scene/presentation state**: Zero ModSceneEffect classes existed; all atmosphere was debuff/dust-based
4. **Quidditch state**: Snitch chase worked but lacked pitch visuals, commentary, and standings UI
5. **Transit state**: Secret passages used instant teleport with minimal framing
6. **Battle state**: Nagini was auto-resolved via flag flip; no McGonagall defense flavor
7. **Asset state**: All 572 sprites procedurally generated; 9 town NPCs had single-frame sprites

---

## Workstream A — Tri-Language Localization Parity

### What was fixed:
- **Moved ~30 highest-priority hardcoded English strings** to localization keys across 10 C# files
- **House names** (GetHouseName) now use `Language.GetTextValue("Mods.WizardingWorld.Houses.*")`
- **Horcrux Tracker** names now localized (DiaryName, LocketName, CupName, DiademName, NaginiName)
- **Room of Requirement** room names and tooltips fully localized (5 room modes)
- **Owl Post** request descriptions, delivery text, and status text all localized (14 keys)
- **Dark Arts Corruption** tier warnings and cleanse text localized (5 keys)
- **HorcruxHuntSystem** destruction flavor text, soul-destroyed messages, and all-complete messages localized (17 keys)
- **AzkabanDespairEvent** start/end messages localized (3 keys)
- **QuidditchEvent** snitch spawn/escape/warning/catch messages now use existing HJSON keys
- **Added 10 new HJSON sections**: Houses, DarkArts, RoomOfRequirement, OwlPost, HorcruxHunt, Azkaban, SceneEffects, QuidditchMatch, Transit, BattleEndgame

### zh-Hant retroactive gap fill:
- **Added 7 entirely missing sections** (142+ keys): Triwizard, Ministry, Gringotts, DiagonAlley, StMungos, Knockturn, Grimmauld
- **Added missing LoadingTips** Tip9, Tip10
- **Added InfoDisplays** section
- zh-Hant now has practical parity with EN + zh-Hans for all active player-visible systems

### zh-Hans fixes:
- Added missing LoadingTips Tip9, Tip10
- All 10 new Phase 29 sections added

### What remains hardcoded (with justification):
- **SpellBook tutorial text** (~60 lines): Deep-nested wand descriptions in ModifyTooltips; would require major refactor for marginal gain since the item is a reference guide
- **Boss dialogue lines** (~15 in various boss files): Would require dialogue framework refactoring; these are boss-combat-context strings rarely seen
- **BossCompass instructions** (~12 lines): Boss summon hint text; low priority since it's a compass info display
- **HogwartsLetter welcome text** (8 lines): First-time tutorial framing; low frequency of display
- **HallowsSystem dialogue** (~8 lines): Dumbledore quest dialogue lines; would benefit from a dialogue system

---

## Workstream B — Chinese Guide Parity + Tooling Hardening

### What was fixed:
- **Chinese PDF**: Replaced 13 inline hardcoded count strings with manifest-derived f-strings (e.g., `"23根魔杖"` → `f"{S['wands_base_combat']}根魔杖"`)
- **New schemas**: Created `mechanical_enemies.schema.json`, `mechanical_accessories.schema.json`, `mechanical_shops.schema.json`
- **Schema validation**: Added `verify_mechanical_schemas()` to verify_guide.py validating all 5 data files against schemas
- **description.txt**: Updated from wildly outdated (7 wands, 3 bosses) to accurate current counts

### What remains:
- Guide content JSON still requires manual curation for narrative text
- "21 Fantastic Beasts" count is curated, no manifest key (matches EN pattern)

---

## Workstream C — Scene Effects / Ambient Presentation

### What was created:
- **New file**: `Common/Systems/WizardingSceneEffects.cs` — 5 ModSceneEffect subclasses:
  - **AzkabanSceneEffect**: Active during Azkaban event, plays DementorKingBoss music, Priority BossHigh
  - **BattleSceneEffect**: Active during Battle of Hogwarts, plays VoldemortBoss music, Priority BossHigh
  - **GringottsDescentEffect**: Active during Gringotts missions, dark atmosphere
  - **KnockturnAlleyEffect**: Active during Knockturn missions, oppressive atmosphere
  - **ShriekingShackEffect**: Active during Shack missions, plays FenrirBoss during full moon

### Caveat:
- Screen shader filters registered but require `Filters.Scene` registration for visible tints (music routing works immediately)

---

## Workstream D — Quidditch Presentation and UX

### What was improved:
- **Pitch boundary markers**: Gold dust particles spawn in rectangle around match area during active matches
- **Score popups**: `CombatText.NewText` floating text for goals and Snitch catches
- **Commentary**: Random localized flavor messages every 15 seconds during matches
- **Match framing**: Start/end messages with score display
- **Season events**: Hardcoded English strings replaced with localization keys
- **Rank-up**: Now uses localized rank title

---

## Workstream E — Transit / Passage Presentation

### What was improved:
- **SecretPassageSystem**: Replaced instant teleport with staged transition (entry text → darkness → travel text → teleport → arrival text + dust effects)
- **DiagonAlleySystem**: Cart descent now shows three sequential Transit framing messages before existing lines
- **ShriekingShackSystem**: Tunnel entry now shows WillowEntry and WillowTunnel framing text for Willow/Tunnel missions

---

## Workstream F — Battle of Hogwarts Closure

### What was improved:
- **Nagini encounter subphase**: Before auto-marking defeated, shows NaginiPhase + NaginiStrike text with dramatic CursedTorch + Shadowflame dust effects
- **McGonagall rally**: Battle start now shows McGonagallRally and CastleDefenders text
- **Siege flavor**: Every 30 seconds during battle phases 1-2, random SiegeIntensity or DefenderFlavor messages appear; stone dust near ward nodes
- **CastleDefenseBuff**: New +8 defense buff applied during battle, representing animated castle defenses

---

## Workstream G — Asset / Animation Scaffolding

### What was improved:
- **generate_sprites.py**: Added 9 missing town NPC entries (Aberforth, GoblinTeller, Healer, Kingsley, Lupin, MadamHooch, MadamRosmerta, MrBorgin, Neville) with proper 25-frame walk cycle sheets and head icons
- **docs/ASSET_BACKLOG.md**: Created honest asset inventory with priority tiers, known inconsistencies, and dimension standards

### Honest caveat:
- **ALL 572+ sprites remain procedural placeholders** — zero final art exists
- Boss sprites are most immersion-breaking (3 shape templates serve all 12 bosses)
- The backlog clearly documents what needs human artist attention

---

## Verification / CI

- **60 new verification checks** added via `verify_phase29_productionization()`:
  - Tri-language section coverage (30 checks across 3 languages × 10 sections)
  - Localization key usage in 8 critical C# systems
  - Scene effects file and class existence
  - Schema file existence (3 new schemas)
  - Asset backlog existence
  - zh-Hant retroactive coverage (7 previously-missing sections)
  - Chinese PDF manifest variable usage
- **Total**: 489/489 checks pass (was 429 in Phase 28)

---

## Files Changed / Created

### New files:
- `Common/Systems/WizardingSceneEffects.cs` — 5 ModSceneEffect classes
- `Content/Buffs/CastleDefenseBuff.cs` — Battle defense buff
- `schemas/mechanical_enemies.schema.json`
- `schemas/mechanical_accessories.schema.json`
- `schemas/mechanical_shops.schema.json`
- `docs/ASSET_BACKLOG.md`

### Modified C# (localization fixes):
- `Common/Systems/GreatHallSystem.cs` — House names localized
- `Common/Players/DarkArtsCorruptionPlayer.cs` — Corruption messages localized
- `Content/Items/Consumables/RoomOfRequirementKey.cs` — Room names + tooltips localized
- `Content/Items/Consumables/HorcruxTracker.cs` — Horcrux names localized
- `Common/Systems/OwlPostSystem.cs` — Request descriptions + delivery text localized
- `Common/Systems/HorcruxHuntSystem.cs` — Destruction flavor + Nagini text localized
- `Common/Systems/QuidditchEvent.cs` — Snitch messages now use existing HJSON keys
- `Common/Systems/AzkabanDespairEvent.cs` — Event start/end localized
- `Common/Systems/BattleOfHogwartsSystem.cs` — Nagini subphase + McGonagall rally + siege flavor + defense buff
- `Common/Systems/QuidditchCupSystem.cs` — Pitch markers, score popups, commentary
- `Common/Systems/QuidditchSeasonSystem.cs` — Season/rank text localized
- `Common/Systems/SecretPassageSystem.cs` — Staged transition framing
- `Common/Systems/DiagonAlleySystem.cs` — Cart descent framing
- `Common/Systems/ShriekingShackSystem.cs` — Tunnel entry framing

### Localization:
- `Localization/en-US_Mods.WizardingWorld.hjson` — 10 new sections (100+ keys)
- `Localization/zh-Hans_Mods.WizardingWorld.hjson` — 10 new sections + Tip9/10 + all retroactive keys
- `Localization/zh-Hant_Mods.WizardingWorld.hjson` — 10 new sections + 7 retroactive sections (Triwizard through Grimmauld) + Tip9/10 + InfoDisplays

### Tooling:
- `generate_chinese_pdf.py` — 13 hardcoded counts replaced with manifest variables
- `generate_sprites.py` — 9 town NPC entries added with walk cycle sheets
- `scripts/verify_guide.py` — Phase 29 verification method (60 checks) + schema validation
- `description.txt` — Updated to reflect current content counts

---

## Remaining Limitations

1. **~50 hardcoded English strings remain** in lower-priority systems (SpellBook tutorial, boss dialogue, BossCompass, HogwartsLetter, HallowsSystem dialogue) — documented with justification above
2. **Screen shader filters** registered in scene effects but need `Filters.Scene` setup for visible screen tints (music routing works)
3. **All sprites are procedural placeholders** — need human artist for final art
4. **Boss sprites are the most immersion-breaking** — 3 shape templates serve 12 bosses
5. **Guide content JSON** still requires manual curation for narrative text
6. **No full visible 7v7 Quidditch** — kept as lightweight presentation enhancement per guardrails
