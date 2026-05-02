# Phase 5 Status -- Source-of-Truth Rebuild + Semantic Verification

## Source-of-Truth Audit Results

### Canonical Files (active pipeline)
| File | Role | Type |
|------|------|------|
| `scripts/scan_content.py` | Filesystem scanner -> content_manifest.json | Auto-derived |
| `scripts/export_guide_data.py` | Curated content -> guide_content.json | Manually maintained |
| `scripts/content_manifest.json` | Authoritative counts and item lists | Auto-generated |
| `scripts/guide_content.json` | Authoritative guide tables and prose | Manually maintained |
| `scripts/verify_guide.py` | 37-check verification with semantic drift | Validation |
| `generate_english_pdf.py` | EN guide PDF (reads both JSONs) | Consumer |
| `generate_chinese_pdf.py` | ZH guide PDF (reads manifest for counts) | Consumer |
| `.github/workflows/guide-ci.yml` | CI: scan -> export -> verify -> generate | Automation |

### Deprecated / Historical Files
| File | Status | Reason |
|------|--------|--------|
| `ContentManifest.md` | **DEPRECATED** | Superseded by auto-generated `scripts/content_manifest.json` |
| `PHASE2_AUDIT.md` | Historical | Phase 2 audit record, not active truth |
| `PHASE2_CHANGELOG.md` | Historical | Phase 2 changelog |
| `PHASE3_CHANGELOG.md` | Historical | Phase 3 changelog |
| `PHASE4_STATUS.md` | Historical | Superseded by this file |
| `generate_sprites.py` | Dormant | Replaced by Gemini sprite pipeline in `tools/gemini/` |

### Not Documentation Pipeline
| File | Purpose |
|------|---------|
| `tools/gemini/*` | Sprite generation tooling, separate concern |
| `SPRITE_PROMPTS.md` | AI sprite generation prompts |
| `SOUND_PROMPTS.md` | AI music generation prompts |
| `CLAUDE.md` | Developer architecture guide (kept current) |
| `README.md` | User-facing project documentation |

## What Is Now Auto-Derived

All counts in the guide come from `scripts/content_manifest.json`:
- C# file count, PNG count, total file count
- Wand count (with active/utility/upgrade/obsolete breakdown)
- Boss, enemy, town NPC, accessory, armor, pet, mount, potion counts
- Crafting material, system, projectile, buff counts

The EN PDF generator reads these at runtime via `S = MANIFEST["summary"]`.

## What Remains Manually Curated

All narrative content is in `scripts/guide_content.json`:
- Wand tier tables (stat rows, spell descriptions, notes)
- Boss data (HP, phases, drops, expert items)
- Enemy tables (grouped by biome, with stats and notes)
- Town NPC roster and descriptions
- Custom system descriptions
- Deathly Hallows acquisition text
- Boss progression table and endgame goals
- Canon disclosure appendix

**Why manual?** These contain game-design prose, lore interpretations, and balance descriptions that cannot be reliably auto-extracted from C# code. The verification script catches semantic drift in this curated content.

## Strict Verification Checks (37 total)

### Manifest Self-Consistency (3 checks)
- Wand math: active = base + upgrade + utility
- Wand files: total = active + obsolete
- Armor math: pieces >= sets * 2

### Stale Code References (4 checks)
- No `WandOfDestiny` references (outside migration stubs)
- No `ShadowElderWand` references (outside migration stubs)
- No `CorruptedPatronus` references (renamed to PhantomStag)
- No `HousePointsSystem` references (renamed to HouseRenownSystem)

### Lore State from C# Code (11 checks)
- InvisibilityCloak NOT craftable
- DemiguiseCloak IS craftable
- ResurrectionStone references Gaunt's Ring
- HallowsSystem.cs exists
- HorcruxHuntSystem.cs exists
- ShadowElderWand is [Obsolete] stub
- WandOfDestiny is [Obsolete] stub
- Riddikulus targets Boggarts (5x bonus)
- Riddikulus has NO Dementor bonus code
- Peeves is dungeon-only
- Dementor spawns require narrative context

### Semantic Drift in Guide Content (18 checks)
**Forbidden patterns** (must NOT appear):
- "Wand of Destiny" as separate weapon
- "Shadow Elder Wand" as current item
- Invisibility Cloak as 33% drop
- Resurrection Stone as 33% drop
- Riddikulus as anti-Dementor
- Reparo as healing aura
- Dementor as generic night enemy
- CorruptedPatronus name
- "House Points" as system name
- "House Cup Victory" as milestone
- Voldemort as Post-Golem
- Dementor King as Post-Cultist
- Fluffy with fire breath

**Required patterns** (must appear):
- Gaunt's Ring in Hallows acquisition
- Patronus as anti-Dementor defense
- Wand Mastery level names (Familiar/Attuned/Mastered)
- Voldemort as True Final Boss
- Canon disclosure section

### PDF Generator (1 check)
- No hardcoded Windows desktop paths

## Lore Corrections Applied in Guide Content

| Issue | Before | After |
|-------|--------|-------|
| Dementor notes | "Hardmode night" | "Blood Moon / Invasions / Forbidden Forest / Azkaban event" |
| CorruptedPatronus | Listed as "CorruptedPatronus" | "Phantom Stag" with "Mod-original" label |
| House Points system | "House Points" / "House Cup Victory" | "House Renown" / "House Champion" |
| Wand Mastery levels | 4 levels (Novice/Apprentice/Adept/Master) | 3 levels (Familiar/Attuned/Mastered) matching code |
| Room of Requirement | Described as placeable tile | Described as reusable buff key with 4 modes |
| Reparo | Described as "healing aura" | Described as "repairs Protego shields, Room buffs, and wards" |
| Riddikulus notes | "3x damage vs Boggarts/Dementors" | "Anti-Boggart charm; use Patronus for Dementors" |
| Voldemort drops | "Invisibility Cloak (33%), Resurrection Stone (33%)" | "Elder Wand (100%), Gaunt's Ring (100%), Soul Fragment (Expert)" |
| Fluffy Phase 3 | "fire breath from all 3 heads" | "boulder barrage + pushback shockwaves" |

## Commands to Regenerate and Verify

```bash
# Step 1: Refresh auto-derived counts
python scripts/scan_content.py --pretty

# Step 2: Export curated content (run after editing guide_content data)
python scripts/export_guide_data.py

# Step 3: Verify everything (strict mode fails on any issue)
python scripts/verify_guide.py --strict

# Step 4: Generate PDFs
python generate_english_pdf.py
python generate_chinese_pdf.py

# One-liner for CI:
python scripts/scan_content.py && python scripts/export_guide_data.py && python scripts/verify_guide.py --strict && python generate_english_pdf.py
```

## Architecture Diagram

```
  C# Source Code                    guide_content.json
  (443 .cs files)                   (curated tables/prose)
        |                                  |
        v                                  v
  scan_content.py              export_guide_data.py
        |                                  |
        v                                  v
  content_manifest.json         guide_content.json
  (auto: counts + lists)        (manual: narrative)
        |                                  |
        +------ verify_guide.py -----------+
        |       (37 strict checks)         |
        v                                  v
  generate_english_pdf.py ---- reads both JSONs
        |
        v
  WizardingWorld_Guide_EN.pdf
  (24 pages, counts + content from JSON sources)
```

Two distinct truth layers:
- **Count truth**: auto-derived, always fresh
- **Content truth**: manually maintained, verified for drift
