# Phase 4 Status — Source-of-Truth Rebuild

## What Changed

### Deliverable A: Source-of-Truth Content Manifest
**File:** `scripts/scan_content.py`
**Output:** `scripts/content_manifest.json`

A Python script that scans the actual repository filesystem and produces a
machine-readable JSON manifest with verified counts and item lists for every
content category.

**Auto-derived data** (from filesystem scan):
- File counts (C#, PNG, total)
- Wand lists with classification (combat/utility/upgrade/obsolete)
- Boss, enemy, town NPC, accessory, armor, pet, mount, potion lists
- Buff, projectile, system, consumable, boss loot, placeable lists
- Crafting material list (heuristic + curated name list)

**Manually maintained data** (clearly separated):
- Crafting material names (the list of which consumables are materials is curated)
- "Fantastic Beasts" count (21, a subset classification of enemies)
- Custom systems count (10 player-facing, vs 35 total system files)
- Minion/summon count (3, specific items)

### Deliverable B: PDF Generator Rebuild
**Files:** `generate_english_pdf.py`, `generate_chinese_pdf.py`

Both generators now:
- **Read `scripts/content_manifest.json`** for all counts
- Title page, back cover, TOC, and section titles all pull from `S = MANIFEST["summary"]`
- No more hardcoded `443` / `458` / `958` count strings
- Output path uses `os.path.dirname(__file__)` (portable), overridable via `--output`
- The "Auto-generated from source code" claim is replaced with "Counts from source scan | Narrative curated"
- Curated content (boss phase descriptions, spell tables, narrative text) remains inline but is clearly not claimed as auto-generated

### Deliverable C: Verification Script
**File:** `scripts/verify_guide.py`

19 automated checks covering:
- Manifest self-consistency (wand math, armor math)
- Stale reference detection (WandOfDestiny, ShadowElderWand, CorruptedPatronus, HousePointsSystem)
- Lore state verification (InvisibilityCloak not craftable, DemiguiseCloak craftable, ResurrectionStone references Gaunt, HallowsSystem exists, HorcruxHuntSystem exists, legacy stubs are [Obsolete])
- Riddikulus targets Boggarts (no Dementor bonus code)
- Peeves dungeon-only, Dementor contextual spawning
- PDF generator: no hardcoded desktop paths, no misleading auto-generated claim

**Result: 19 passed, 0 failed, 0 warnings.**

### Deliverable D: Lore Cleanup Verification

All lore guardrails verified against actual code:

| Rule | Status | Evidence |
|------|--------|----------|
| Elder Wand / Wand of Destiny not separate items | PASS | WandOfDestiny.cs is [Obsolete] migration stub, auto-converts to ElderWand |
| True Invisibility Cloak not craftable | PASS | No AddRecipes method in InvisibilityCloak.cs |
| DemiguiseCloak is the craftable stealth option | PASS | Has AddRecipes with Demiguise Hair recipe |
| Resurrection Stone tied to Gaunt's Ring | PASS | Comment references Gaunt, obtained via HallowsSystem purification |
| Riddikulus primarily anti-Boggart | PASS | 5x damage vs Boggart, no Dementor bonus code |
| Reparo is ward/object repair, not healing | PASS | Extends buff timers, repairs Protego shields, relieves despair |
| Dementors contextual spawning | PASS | Requires Blood Moon / Invasion / Azkaban Event / Forbidden Forest |
| Peeves dungeon-only | PASS | Only ZoneDungeon check |
| HorcruxHuntSystem is real | PASS | 350+ line system with 4 destroyable Horcruxes, Voldemort scaling, world persistence |
| HallowsSystem is real | PASS | Full questline with Dumbledore guidance, cloak claiming, ring purification |

### Deliverable E: Horcrux Hunt Status

The Horcrux Hunt is **fully implemented** as a first-class system:

**World state** (persisted):
- `diaryDestroyed`, `locketDestroyed`, `cupDestroyed`, `diademDestroyed`, `naginiDefeated`
- `horcruxesDestroyed` (0-4 counter)

**Mechanics:**
- Right-click Horcrux accessories to attempt destruction
- Requires Basilisk Fang or Sword of Gryffindor in inventory
- Each destruction weakens Voldemort: 100% → 85% → 70% → 55% → 40% power
- Affects HP, damage, defense, teleport frequency, minion count, bolt count
- Dramatic per-Horcrux flavor text and visual effects
- Corruption cleansing (15% per destruction)

**Integration:**
- Connects to HallowsSystem (all Horcruxes → claim Cloak → Voldemort → purify Ring → Master of Death)
- Dumbledore NPC provides context-sensitive guidance text
- BossChecklistIntegration shows Voldemort as "Weakened by Horcrux Hunt"

No dangling references. The system is real and documented.

## Verified Counts (from manifest)

| Category | Count | Source |
|----------|-------|--------|
| C# source files | 443 | auto-scan |
| PNG sprites | 535 | auto-scan |
| Total files | 1057 | auto-scan |
| Active wands | 24 (19 combat + 1 upgrade + 4 utility) | auto-scan |
| Legacy wand stubs | 2 | auto-scan |
| Bosses | 12 | auto-scan |
| Enemies | 51 | auto-scan |
| Town NPCs | 7 | auto-scan |
| Accessories | 53 | auto-scan |
| Armor sets | 8 (24 pieces) | auto-scan |
| Pets | 6 | auto-scan |
| Mounts | 5 | auto-scan |
| Potions | 19 | auto-scan |
| Crafting materials | 20 | curated list |
| Player-facing systems | 10 | curated count |
| Fantastic Beasts | 21 | curated subset |
| Minion weapons | 3 | curated count |

## Canon Deviations (Intentional)

| Deviation | Reason | Labeled? |
|-----------|--------|----------|
| Patronus as combat summon weapon | Fun gameplay, gives Summoner class a wizard option | Yes, in tooltips |
| DemiguiseCloak grants full invisibility | Simplification; canon Demiguise cloaks fade over time | Yes, "lesser cloak" framing |
| PhantomStag enemy in Hallow biome | Original content; wild-magic corrupted echo | Yes, "Fully original mod content" |
| Dementor spawning during Blood Moon | Minor stretch; Blood Moon = "time of darkness" | Yes, in code comments |
| Reparo extends buff timers | Creative reinterpretation of "repair" for gameplay | Yes, in summary comment |
| House Renown kill-based system | Renamed from "House Points" to distinguish from canon | Yes, explicitly framed |

## Workflow

To maintain consistency going forward:

```bash
# 1. Make code changes
# 2. Refresh the manifest
python scripts/scan_content.py

# 3. Regenerate the guide
python generate_english_pdf.py
python generate_chinese_pdf.py

# 4. Verify everything matches
python scripts/verify_guide.py --strict
```

If step 4 fails, fix the issue before shipping.
