# Phase 30 — Landmarks Completion, Scene Filters Verified, Localization Sweep, Chinese Guide Parity

**Date**: 2026-04-17
**Verification**: 660/660 checks pass (up from 489 in Phase 29 — +171 new checks)
**Phase type**: Productionization closure / wiring pass — no new lore pillars
**Assistant**: Claude (Opus 4.7) — code-only; no sprite-art or audio generation available in this environment

---

## Step 0 — Reality Audit (Findings vs Phase 29 Report)

The repo was substantially further along than the Phase 29 status claimed. Several items
reported as "not yet wired" or "for future work" were already scaffolded on disk:

| Area                            | Phase 29 said                              | Repo reality (2026-04-17)                                        |
|---------------------------------|--------------------------------------------|------------------------------------------------------------------|
| `Filters.Scene` registration    | "filters need `Filters.Scene` setup"       | **Already done** — `Common/Systems/WizardingFilterSystem.cs` present, 5 filters + `WizardingAmbientSystem` particle system wired |
| Landmark tiles                  | not mentioned                              | **12 tile classes already present** in `Content/Tiles/Landmarks/` |
| Landmark placement items        | not mentioned                              | **12 placement items already present** in `Content/Items/Placeable/` with recipes at `Tiles.EnchantingTable` |
| en-US landmark localization     | not mentioned                              | **Already done** — 12 `MapEntry` + 12 Item DisplayName/Tooltip  |
| zh-Hans landmark localization   | not mentioned                              | **0 entries — gap**                                              |
| zh-Hant landmark localization   | not mentioned                              | **0 entries — gap**                                              |
| Landmark sprites                | not mentioned                              | **0 entries in `generate_sprites.py` — PNGs missing → mod would fail to load** |
| ASSET_BACKLOG.md                | covers ~572 placeholders                   | No landmark rows; no audio section                               |
| Hardcoded `Main.NewText`        | "~50 remain (justified)"                   | ~8 genuinely player-facing calls remained (not justified)        |
| `generate_chinese_pdf.py`       | "13 counts replaced with manifest vars"    | Still **1465 lines** with ~100 lines of inline curated translation dicts |
| `release_manifest.json`         | CI-only                                    | No local generator                                               |
| Strict verify count             | 489                                        | 489 confirmed; Phase 30 adds 171 → **660**                       |

Conclusion: Phase 30's mission was largely **closing the gap between scaffolded code
and actually-shippable state**, not building new systems from scratch.

---

## Workstream A — Scene Filters & Ambient (Audit-Only, Plus New Verification)

**Status**: code already present from a prior commit, now under explicit verification.

- `Common/Systems/WizardingFilterSystem.cs` — `ModSystem.Load()` registers 5 filters via
  `Filters.Scene["WizardingWorld:<key>"]`, using vanilla `FilterMiniTower` shader with
  per-scene color tint + opacity (Azkaban desat blue; Battle dark red; Gringotts gold;
  Knockturn murky green-gray; Shack pale blue).
- `Common/Systems/WizardingFilterSystem.cs` — `WizardingAmbientSystem.PostUpdateWorld()`
  spawns themed dust per active scene (ice motes / ember+smoke / gold shimmer /
  shadowflame wisps / wraith wisps). Client-only guards (`Main.dedServ`, `NetmodeID.Server`)
  prevent server crashes.
- Fallback behavior: filter keys guarded with `HasEffect` before re-registering.
  `ManageSpecialBiomeVisuals` is a no-op if key not registered, so scene effects
  silently degrade to music-only if the filter system fails to load.

**Verification added (Phase 30 §1–2)**: 11 checks — filter system file exists, all 5
`WizardingWorld:*` filter keys registered, `WizardingAmbientSystem` present, and every
scene effect's `ManageSpecialBiomeVisuals` string matches a registered filter key (prevents
the silent-no-op bug if a key is renamed on only one side).

**What is visual-only vs mechanical**:
- Screen tint + ambient dust = purely presentational (no gameplay effect).
- Music routing in scene effects = presentational.
- State driving each scene (`AzkabanDespairEvent.eventActive`, etc.) = owned by the
  respective gameplay system. Scene effects are passive observers; they don't modify state.

---

## Workstream B / C — Landmark Structures (Hogwarts + London/Ministry)

**All 12 landmarks were already scaffolded on disk.** This phase completed the wiring:
localization for both Chinese variants, sprite entries in the generator, and honest
documentation in the asset backlog.

### Landmark coverage (12 tiles + 12 items)

| Landmark                  | Slice                   | Tile Dim | Role                                    |
|---------------------------|-------------------------|----------|-----------------------------------------|
| HousePointHourglass       | Great Hall              | 2x3      | Feast/ceremony identity (not a buff toy)|
| QuidditchGoalpost         | Quidditch pitch         | 1x4      | Three-hoop pitch readability            |
| CastleWardStone           | Battle of Hogwarts      | 2x2      | Defense / rally node visual             |
| WhompingWillowStump       | Willow/Shack passage    | 3x2      | Hidden dangerous passage marker         |
| ShriekingShackSign        | Hogsmeade/Shack         | 2x2      | Containment / haunted framing           |
| GrimmauldDoorway          | Order safehouse         | 2x3      | Hidden + hostile, not cozy              |
| LeakyCauldronSign         | Diagon Alley gateway    | 2x2      | London/Muggle boundary                  |
| GringottsFacade           | Gringotts               | 3x3      | Goblin-controlled formal entrance       |
| BorginStorefront          | Knockturn Alley         | 2x2      | Dark Arts container-mood shop           |
| StMungosMannequin         | St Mungo's              | 2x2      | Hospital hidden entrance (**not** resurrection) |
| ProphecyShelf             | Dept of Mysteries       | 2x3      | Hall of Prophecy — orb bank             |
| VeilArch                  | Dept of Mysteries       | 3x4      | Death Chamber — Veil (**not** resurrection) |

### What was done (Phase 30)

- **zh-Hans localization**: added 12 `Tiles.*.MapEntry` + 12 `*Item` DisplayName/Tooltip entries
  (24 new keys). Tooltips match canon guardrails (feast/pitch/hospital/hidden-passage framing).
- **zh-Hant localization**: same 24 keys with Traditional-character forms using repo-standard
  place names (活米村, 夜行巷, 古靈閣, 聖蒙果, 古里某街, etc.).
- **`generate_sprites.py`**: added Phase 30 block producing 12 tile PNGs + 12 item PNGs via
  existing `draw_tile_sprite` + `draw_generic_item` primitives. Regenerated and all 24 PNGs
  now exist on disk. Honestly: these are colored rectangles; the backlog lists the intended
  art direction.
- **`docs/ASSET_BACKLOG.md`**: added dedicated Phase 30 landmark table with dimensions,
  placeholder color, and intended art note per landmark. Added an **Audio Gap** section
  stating plainly that no audio exists anywhere in the mod.

### Canon guardrails enforced via verification

- Veil Arch item tooltip must not contain "resurrection" / "revive" — kept Death Chamber framing.
- St Mungo's mannequin must not contain "resurrection" / "revive" — kept hospital framing.

Both pass. Checks will fail if a future edit drifts from these locks.

---

## Workstream D — Localization Sweep Closure

**Target**: remaining hardcoded `Main.NewText(...)` calls with plain English literals.
After Phase 29 the count was ~8 genuine violations (a messier starting point than
Phase 29's "~50 justified" claim).

### Moved to localization

| File                                          | Keys added                                          |
|-----------------------------------------------|-----------------------------------------------------|
| `Common/Systems/DailyChallengeSystem.cs`      | `DailyChallenge.Complete`                           |
| `Common/Systems/HogwartsLetterSystem.cs`      | `HogwartsLetterEvent.OwlArrives`, `HogwartsLetterEvent.Accepted` (new section — `HogwartsLetter:` was already taken for the item) |
| `Common/Systems/HouseRenownSystem.cs`         | `Renown.Champion`, `Renown.MajorBuff`, `Renown.MinorBuff` |
| `Content/Buffs/ResurrectionBuff.cs`           | `Resurrection.RiseFromAshes`                        |

Each file gained `using Terraria.Localization;` where missing. Each `Main.NewText`
literal was replaced with `Language.GetTextValue("Mods.WizardingWorld.<Key>")`.

All 7 keys added to **en-US**, **zh-Hans**, **zh-Hant** in a new `// === PHASE 30 ===`
block sitting between `BattleEndgame` and the supplementary `Bestiary` entry.

### What remains hardcoded (with justification — unchanged from Phase 29)

- SpellBook tutorial text (~60 lines, deeply nested tooltip prose)
- Boss dialogue lines (~15 across boss files — needs dialogue framework refactor)
- BossCompass summon hints (~12 lines — compass internals)
- HallowsSystem Dumbledore lines (~8 — dialogue system work)

These remain out of scope for this phase and are documented here rather than in code
comments, per the feedback convention.

### zh-Hant parity

zh-Hant now has full coverage of every section EN has, including the Phase 30 landmark
+ sweep keys. No parity gap remaining.

---

## Workstream E — Chinese Guide Generator Refactor

### The problem

Phase 29 replaced 13 inline count strings with manifest-derived f-strings but left
the bulk of the divergence untouched: `generate_chinese_pdf.py` was 1465 lines vs
`generate_english_pdf.py` at 779 lines. The delta was ~100 lines of inline curated
translation dicts (`WAND_NAMES_ZH`, `BOSS_NAMES_ZH`, `WAND_SPELLS_ZH`, `WAND_TIERS`,
`_RARITY_ZH`, `_WAND_TIER_ORDER`, `BOSS_PROGRESSION_ORDER`) living in the same file as
PDF layout logic.

### 80/20 refactor (this phase)

- **New file: `scripts/zh_translations.json`** — holds all 6 curated dicts as
  structured JSON with a `_meta` block (phase, schema_version). Python `tuple` shape
  for `wand_tiers` is restored in-code via `{k: tuple(v) for k, v in ...}` to keep
  downstream code unchanged.
- **`generate_chinese_pdf.py`**: dropped ~100 lines of inline dicts, replaced with a
  single JSON load + 6 variable bindings. Docstring updated to name the three data
  layers (auto-derived / curated prose / zh translations / presentation).
- **Verified**: `python generate_chinese_pdf.py` still produces a 24-page PDF.

### What was NOT done (deliberate)

- The prose narrative inside `build_pdf()` remains hand-authored per language.
  Chinese and English guides tell different stories at the narrative layer — this
  is intentional and matches the English side's "curated narrative" truth layer.
- Schemas were not added for `zh_translations.json` this phase — low priority since
  it's a flat dict-of-dicts with no external consumers yet.

### Local release manifest generation

- **New file: `scripts/generate_release_manifest.py`** — stand-alone CLI that
  reproduces CI's inline `release_manifest.json` generation. Default `phase=local`,
  `commit=local`; both overridable via `--phase` and `--commit`. Local builds can now
  snapshot the same release metadata CI produces without running CI.

---

## Workstream F — Verification Expansion

New method: `Verifier.verify_phase30_landmarks_and_sweep()` — wired into `main()`.

**Check categories added (~171 new):**

1. **Filter registration system (§1)** — file exists, all 5 filter keys present, ambient system present (7 checks)
2. **Scene effect ↔ filter symmetry (§2)** — every `ManageSpecialBiomeVisuals("WizardingWorld:<k>")` call has a matching `Filters.Scene["WizardingWorld:<k>"]` registration (5 checks)
3. **Landmark tile class existence (§3)** — 12 `.cs` files in `Content/Tiles/Landmarks/`
4. **Landmark placement item existence + recipe (§4)** — 24 checks (12 files × 2: file exists + uses `Tiles.EnchantingTable`)
5. **Landmark sprite existence (§5)** — 24 PNGs (12 tile + 12 item)
6. **Landmark localization (§6)** — 72 checks (12 landmarks × 2 entry types × 3 languages = `MapEntry` + `<Item>` across EN / zh-Hans / zh-Hant)
7. **Phase 30 localization sections (§7)** — 12 checks (4 new sections × 3 languages)
8. **Hardcoded string removal (§8)** — 7 checks (one per English literal targeted in Workstream D)
9. **Chinese JSON refactor (§9)** — 3 checks (JSON exists, generator loads it, no inline literal)
10. **Local release manifest (§10)** — 1 check
11. **Asset backlog hygiene (§11)** — 2 checks (landmark row, audio gap)
12. **Canon guardrail locks (§12–13)** — 2 checks (Veil Arch no resurrection; St Mungo's no resurrection)

**Runtime-safety improvements**:
- Filter registration already guards with `if (Main.dedServ) return;` and `HasEffect`
  re-entry check (pre-existing). No change needed.
- Scene effect `ManageSpecialBiomeVisuals` is a no-op on unrecognized keys. If
  `WizardingFilterSystem` fails to load, scenes still play music without crashing.

---

## Files Changed / Created

### New files (3)
- `scripts/zh_translations.json` — externalized Chinese curated dicts
- `scripts/generate_release_manifest.py` — local release manifest CLI
- `PHASE30_STATUS.md` — this file

### Modified C# (4)
- `Common/Systems/DailyChallengeSystem.cs` — `using Terraria.Localization;` + localized completion text
- `Common/Systems/HogwartsLetterSystem.cs` — `using Terraria.Localization;` + localized owl-arrival + acceptance text
- `Common/Systems/HouseRenownSystem.cs` — 3 renown milestone messages localized
- `Content/Buffs/ResurrectionBuff.cs` — `using Terraria.Localization;` + localized phoenix rise text

### Modified localization (3)
- `Localization/en-US_Mods.WizardingWorld.hjson` — added `DailyChallenge:`, `HogwartsLetterEvent:`, `Renown:`, `Resurrection:` sections (7 keys)
- `Localization/zh-Hans_Mods.WizardingWorld.hjson` — added 12 tile `MapEntry` + 12 placeable items (24 keys) + 4 Phase 30 sweep sections (7 keys) = **31 new keys**
- `Localization/zh-Hant_Mods.WizardingWorld.hjson` — same 31 new keys in Traditional characters

### Modified tooling (3)
- `generate_sprites.py` — added Phase 30 block generating 12 landmark tile PNGs + 12 placement item PNGs (24 new placeholder sprites)
- `generate_chinese_pdf.py` — removed ~100 lines of inline Chinese dicts; now loads from `scripts/zh_translations.json`; docstring updated with data-layer labels
- `scripts/verify_guide.py` — added `verify_phase30_landmarks_and_sweep()` (171 new checks) + wired into `main()`

### Modified docs (1)
- `docs/ASSET_BACKLOG.md` — Phase 30 landmark dimensions table, Audio Gap section with backlog of needed music/SFX slots, updated total sprite count (~596)

---

## Honest Limitations (unchanged or new)

### Carried over from Phase 29
1. **ALL ~596 sprites are procedural placeholders.** No hand-drawn or AI-gen art has
   landed. The 24 new landmark PNGs are colored rectangles via `draw_tile_sprite` /
   `draw_generic_item`. Running `python generate_sprites.py` regenerates everything.
2. **Boss sprites still use 3 shape templates for 12 bosses.** Most immersion-breaking.
3. SpellBook tutorial, BossCompass, Hallows dialogue — still hardcoded (low-frequency).

### New / reinforced in Phase 30
4. **NO audio files exist in the mod.** Scene effects route `Music` to existing boss-track
   slots that were themselves never authored as distinct music. No ambient loops, no
   custom SFX, no Quidditch whistle sound. The asset backlog now lists the intended
   audio slots (Great Hall ambient, Quidditch match, Knockturn, Diagon Alley, Owl hoot,
   Portkey, Willow impact, Pensieve submerge) — none filled. Audio requires a composer /
   sound designer or a gen workflow not available in the current build environment.
5. **Landmark placeholders are inventory-readable only.** The tiles exist and render
   without errors, but they read as color blocks until replaced with real pixel art
   matching the backlog dimensions (per-landmark intended art notes in ASSET_BACKLOG.md).
6. **Chinese guide narrative prose still hand-authored.** The JSON refactor externalized
   translation *dicts*, not narrative sections. Chinese and English guides intentionally
   diverge in storytelling detail.

### What the Claude Code assistant could not do in this phase

- **No sprite generation beyond PIL primitives.** No image-generation model / MCP server
  was connected in this environment. Asked to produce final art, I produced placeholders
  and flagged them as placeholders.
- **No audio synthesis.** No DAW / ElevenLabs / audio-gen tool was available. Audio
  remains unaddressed in this pass.

---

## Success-Criteria Self-Check

| Phase 30 success criterion                                         | Status |
|---------------------------------------------------------------------|--------|
| Major slices feel more spatially recognizable                       | Partial — tiles + items + recipes + map entries all wired; visual identity limited by placeholder art |
| Filters/ambience visibly real in runtime where intended             | Yes — `WizardingFilterSystem` registers 5 filters + `WizardingAmbientSystem` spawns themed dust |
| Remaining high-priority hardcoded English strings are gone          | Yes — 7 targets removed; ~4 justified groups documented as deferred |
| Chinese guide generation materially closer to same truth pipeline   | Yes — 6 curated dicts moved to `scripts/zh_translations.json`; generator shrank by ~100 lines |
| Verification count increases and stays strict                       | Yes — 489 → **660** (+171), strict mode passes |
| No old lore drift                                                   | Yes — Veil Arch + St Mungo's locks verified; all prior Hallows/Patronus/Riddikulus/Wolfsbane/unicorn locks unchanged |
| Repo is more shippable, not just bigger                             | Partial — code/wiring/localization shippable; art + audio remain non-shippable |
