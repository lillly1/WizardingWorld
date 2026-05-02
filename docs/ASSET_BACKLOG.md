# Wizarding World Mod -- Asset Backlog Manifest

> **Last updated**: 2026-04-17 (Phase 30)
> **Honest status**: ALL ~596 sprites in this mod are procedurally generated placeholders
> produced by `generate_sprites.py`. There is zero hand-drawn or AI-generated final art
> in the project at this time. The SPRITE_PROMPTS.md file describes intended art direction
> for each asset but none of those prompts have been executed yet.
> **No audio exists either.** Scene effects reference music slots that route to existing
> boss tracks (also procedural/placeholder); there are no dedicated landmark or ambient
> audio files. See "Audio Gap" section below.

---

## Summary

| Category              | Count | Placeholder | Final Art | Notes                                    |
|-----------------------|-------|-------------|-----------|------------------------------------------|
| Town NPCs (sheets)    | 16    | 16          | 0         | 5 were single-frame; now all 25-frame    |
| Town NPC heads        | 16    | 16          | 0         | 36x36 map icons                          |
| Boss spritesheets     | 12    | 12          | 0         | 6-frame sheets, 3 shape types only       |
| Boss head icons       | 12    | 12          | 0         | 32x32, use generic round shape           |
| Boss loot (Bag/Trophy/Expert) | 36 | 36    | 0         | 3 per boss, generic shapes               |
| Enemy spritesheets    | 51+   | 51+         | 0         | 81 PNGs; many reuse same shape templates |
| Wands                 | 26    | 26          | 0         | Good procedural detail with wood grain   |
| Accessories           | 53+   | 53+         | 0         | 69 PNGs; simple geometric shapes         |
| Armor sets (items+equip) | 8 sets | 48 PNGs | 0        | 6 PNGs per set (item + equip)            |
| Potions               | 19    | 19          | 0         | Decent bottle silhouettes                |
| Consumables           | 88    | 88          | 0         | Mostly round/rect generic shapes         |
| Weapons (non-wand)    | 27    | 27          | 0         | Swords, brooms, staffs                   |
| Projectiles/Spells    | 26    | 26          | 0         | Orbs, beams, shields, flames             |
| Projectiles (other)   | 9     | 9           | 0         | Misc non-spell projectiles               |
| Mounts                | 5     | 5           | 0         | 4-frame sheets; broom + hippogriff types |
| Pets                  | 18    | 18          | 0         | 6 pets x 3 files each                    |
| Buffs/Debuffs         | 59    | 59          | 0         | 32x32 icons, simple symbol shapes        |
| Tiles                 | 13    | 13          | 0         | EnchantingTable + 12 Phase 30 landmarks  |
| Placeable items       | 15    | 15          | 0         | 3 Phase 22 + 12 Phase 30 landmarks       |
| Mod icon              | 1     | 1           | 0         | 80x80 icon.png                           |
| **TOTAL**             | **~596** | **~596** | **0**     |                                          |

---

## Priority Tiers for Artist Attention

### TIER 1 -- Critical (most player-facing, most immersion-breaking)

These are the assets players see constantly. The procedural placeholders are most jarring here.

1. **Town NPCs (16 spritesheets + 16 heads)** -- Players interact with these every session.
   The current sprites are colored rectangles with dot eyes on a stick-figure body.
   Every NPC uses the same body template with only color swaps. Dumbledore, Hagrid, Dobby,
   and Ollivander are particularly important since they appear early and often.

2. **Boss spritesheets (12)** -- Boss encounters are the mod's marquee content.
   Currently only 3 visual archetypes exist: `serpent`, `dragon`, and `wizard`. This means
   the Troll boss looks like a wizard, Aragog looks like a snake, and Fluffy looks like a
   dragon. These are the most immersion-breaking placeholders in the entire mod.

3. **Mod icon (icon.png)** -- First thing players see in the mod browser. Currently a
   procedural circle with lines.

4. **Wands (26)** -- The mod's signature items. Procedural versions have decent diagonal
   lines with wood grain and glowing tips, but they all look nearly identical at inventory
   scale. The Elder Wand, Holly Wand, and Yew Wand need distinct silhouettes.

### TIER 2 -- High (frequently visible, shape distinctness matters)

5. **Armor sets (8 sets, 48 PNGs)** -- Equipment visuals. Currently colored rectangles.
   House robes (Gryffindor, Slytherin, Ravenclaw, Hufflepuff) are the mod's identity.

6. **Key accessories** -- Deathly Hallows trio (Invisibility Cloak, Resurrection Stone,
   Elder Wand), Horcruxes (Diary, Locket, Cup, Diadem), Marauder's Map, Time-Turner.
   These are iconic HP items reduced to colored circles/squares.

7. **Spell projectiles (26)** -- Players see these constantly in combat. The orb/beam/shield
   templates work as placeholders but lack the distinctive colors and shapes described in
   SPRITE_PROMPTS.md (e.g., Avada Kedavra's green bolt vs Stupefy's red bolt are currently
   hard to distinguish).

8. **Mount spritesheets (5)** -- Brooms and Hippogriff. The broom mounts use the same
   horizontal line template. Hippogriff/Thestral mounts reuse the same body shape.

### TIER 3 -- Medium (visible but less critical)

9. **Common enemies (51+ sheets)** -- Many enemy types share the same base shape
   (`humanoid`, `ghost`, `spider`, `bat`, `blob`, `serpent`). Dementors, Acromantulas,
   and Death Eaters are most important to differentiate.

10. **Potions (19)** -- Bottle shapes are adequate but all look the same except for
    liquid color. Polyjuice, Felix Felicis, and Butterbeer need distinct bottle shapes.

11. **Boss head icons (12)** -- Map/health bar icons. Currently all use `draw_generic_item`
    with `shape='round'` -- literally identical circles in different colors.

12. **Pets (6 types, 18 PNGs)** -- Golden Snitch, Hedwig, Niffler, Pygmy Puff, Baby Dragon,
    Crumple-Horned Snorkack. These follow players around constantly.

### TIER 4 -- Low (can remain placeholder longest)

13. **Consumable items (88)** -- Crafting materials, summon items, misc drops. Generic
    round/rect shapes are acceptable for inventory filler.

14. **Buff/debuff icons (59)** -- Small UI icons. The procedural versions are functional
    if not pretty.

15. **Boss loot items (36)** -- Bags, trophies, expert items. Bags and trophies can
    remain generic longer since they're transient.

16. **Tiles and placeable items (28)** -- Enchanting Table, Goblet of Fire, Felix Cauldron,
    plus 12 Phase 30 landmark tiles with matching placement items (Hourglass, Goalpost,
    Ward Stone, Willow Stump, Shack Sign, Grimmauld Doorway, Leaky Cauldron Sign,
    Gringotts Facade, Borgin Storefront, St Mungo's Mannequin, Prophecy Shelf, Veil Arch).
    Currently rendered as colored rectangles via `draw_tile_sprite` / `draw_generic_item`.
    The 12 landmark items are the most important to upgrade because they visibly define
    in-world locations; the current placeholders are recognizable-as-landmarks-in-theory
    only via their map entry names.

### Phase 30 Landmark Placeholder Dimensions

| Landmark               | Tile Dim | Item Icon | Placeholder Color     | Intended Art Note                           |
|------------------------|---------:|----------:|-----------------------|---------------------------------------------|
| HousePointHourglass    | 2x3      | 20x30     | Brass/amber           | Four stacked sand columns with house gems   |
| QuidditchGoalpost      | 1x4      | 18x32     | Gold                  | Single tall post, ring at top               |
| CastleWardStone        | 2x2      | 24x24     | Stone gray            | Runestone with glowing sigil                |
| WhompingWillowStump    | 3x2      | 30x22     | Dark bark             | Exposed roots + visible knothole            |
| ShriekingShackSign     | 2x2      | 24x24     | Weathered wood        | Hanging signpost, peeling paint             |
| GrimmauldDoorway       | 2x3      | 22x30     | Black iron            | 12 Grimmauld Place, brass numbers           |
| LeakyCauldronSign      | 2x2      | 24x24     | Dark brown            | Iron bracket, cauldron silhouette           |
| GringottsFacade        | 3x3      | 30x30     | White marble          | Columns, goblin-script lintel               |
| BorginStorefront       | 2x2      | 24x24     | Grimy green-brown     | Crooked window display, cursed wares        |
| StMungosMannequin      | 2x2      | 24x24     | Faded pastels         | Pre-war department store mannequin          |
| ProphecyShelf          | 2x3      | 24x30     | Blue-silver glow      | Dusty shelving + glowing orbs               |
| VeilArch               | 3x4      | 28x36     | Weathered stone       | Stone archway + fluttering grey veil        |

All 24 placeholder PNGs are regenerated by `python generate_sprites.py`.

---

## Known Inconsistencies (Fixed or Flagged)

### Fixed in this pass

- **5 single-frame Town NPCs**: GoblinTeller, Healer, MadamHooch, MadamRosmerta, and
  MrBorgin had 40x56 single-frame sprites (no walk animation). These were not in
  `generate_sprites.py` at all. Added to generator with proper 25-frame sheets.

- **9 missing Town NPCs in generator**: Aberforth, GoblinTeller, Healer, Kingsley, Lupin,
  MadamHooch, MadamRosmerta, MrBorgin, and Neville were missing from `generate_sprites.py`.
  All 9 now have proper `draw_npc_sheet(40, 56, 25, ...)` calls plus `draw_head_icon` calls.

### Remaining inconsistencies

- **4 NPCs had 24 frames instead of 25**: Aberforth, Kingsley, Lupin, Neville were
  generated by an external process with 24 frames (40x1400 instead of 40x1450). The
  generator now produces correct 25-frame sheets for all, but the old files on disk
  need regeneration by running `python generate_sprites.py`.

- **Boss head icons use wrong function**: All 12 boss `_Head_Boss.png` files use
  `draw_generic_item(32, 32, color, shape='round')` -- a featureless colored circle.
  Town NPC heads use the proper `draw_head_icon()` which renders a face. Boss heads
  should use a dedicated function.

- **Boss shape mismatch**: Only 3 boss_type shapes exist (`serpent`, `dragon`, `wizard`).
  This creates misleading visuals:
  - Troll uses `wizard` (should be a hulking humanoid)
  - Aragog uses `serpent` (should be a giant spider)
  - Fluffy uses `dragon` (should be a three-headed dog)
  - Fenrir uses `dragon` (should be a werewolf)

- **Enemy shape reuse**: 7 shape templates (`humanoid`, `ghost`, `spider`, `bat`, `blob`,
  `serpent`, `fang`) serve 51+ enemies. Many visually identical creatures only differ in
  color palette.

---

## Sprite Dimension Standards

These are the conventions established in `generate_sprites.py` and `SPRITE_PROMPTS.md`:

| Asset Type           | Width | Frame Height | Frame Count | Sheet Height         |
|----------------------|-------|-------------|-------------|----------------------|
| Town NPC sheet       | 40    | 56          | 25          | 1450 (25 x 58)      |
| Town NPC head        | 36    | 36          | 1           | 36                   |
| Boss sheet           | 36-80 | 50-80       | 6           | varies               |
| Boss head icon       | 32    | 32          | 1           | 32                   |
| Enemy sheet          | 14-60 | 8-56        | 2-6         | varies               |
| Wand item            | 36-44 | 36-44       | 1           | same                 |
| Accessory item       | 18-30 | 18-30       | 1           | same                 |
| Potion item          | 20    | 26          | 1           | 26                   |
| Buff icon            | 32    | 32          | 1           | 32                   |
| Mount sheet          | 80    | 50          | 4           | 208                  |
| Pet sheet            | 16-20 | 16-20       | 4           | varies               |
| Armor item icon      | 18    | 18          | 1           | 18                   |
| Armor equip (head)   | 40    | 36          | 1           | 36                   |
| Armor equip (body)   | 40    | 54          | 1           | 54                   |
| Armor equip (legs)   | 40    | 44          | 1           | 44                   |
| Projectile           | 10-80 | 10-80       | 1           | same                 |
| Mod icon             | 80    | 80          | 1           | 80                   |

Inter-frame padding: 2 pixels (vertical gap between stacked frames in sheets).

---

## What "Placeholder" Means Here

All current sprites are generated by `generate_sprites.py` using PIL/Pillow. The generator:

- Draws geometric primitives (ellipses, rectangles, lines, polygons)
- Uses color palettes to distinguish items (no texture, no detail)
- Applies basic shading (darker/lighter variants of base color)
- Creates animation by offsetting limb positions by +/- 1-2 pixels per frame
- Reuses a small set of body templates across dozens of distinct creatures

**What the placeholders get right:**
- Correct dimensions for tModLoader
- Correct frame counts for animation
- Correct file paths and naming conventions
- Roughly correct color associations (green for Slytherin, red for Gryffindor, etc.)
- Functional walk cycles (arms/legs alternate)

**What the placeholders lack:**
- Character identity (all NPCs are the same stick figure in different colors)
- Creature anatomy (a spider, snake, and dragon share body templates)
- Iconic visual details (no lightning scar, no half-moon spectacles, no nose-slits)
- Texture and material quality (no fabric folds, no wood grain at proper scale)
- Distinct silhouettes (most items of the same type are identical shapes)
- Expressive animation (walk cycle is 1-2 pixel offset, no personality)

---

## Art Direction Reference

See `SPRITE_PROMPTS.md` for detailed per-asset AI image generation prompts that describe
the intended final art for every sprite. The prompts follow Terraria's pixel art style:
- 16-bit retro aesthetic
- Clean pixel edges, no anti-aliasing
- Limited palette with 2-3 shading levels per color
- Top-down angled perspective
- Transparent PNG backgrounds

---

## Audio Gap (NOT YET ADDRESSED)

There is **no custom audio** in this mod. The scene effects in `WizardingSceneEffects.cs`
route `Music` to existing boss-track slots (`Assets/Music/DementorKingBoss`,
`VoldemortBoss`, `FenrirBoss`), which themselves were never authored as distinct tracks.
No landmark/ambient audio, no custom SFX for spell cast, no Quidditch whistle sound,
no St Mungo's chime, no Whomping Willow impact.

This requires either a composer/sound designer or an audio-gen workflow. No automated
placeholder audio generation is performed because noise-synthesized placeholders would
be worse than silence.

### Audio Backlog (intended future work)

| Slot                             | Purpose                                     | Needed   |
|----------------------------------|---------------------------------------------|----------|
| Assets/Music/GreatHallAmbient    | Feast / ceremony background                 | ~2 min loop |
| Assets/Music/QuidditchMatch      | Match pitch energy                          | ~2 min loop |
| Assets/Music/KnockturnAlley      | Low, oppressive ambience                    | ~2 min loop |
| Assets/Music/DiagonAlley         | Lively, busy shopping street                | ~2 min loop |
| Assets/Sounds/OwlHoot            | Letter delivery cue                         | <2s      |
| Assets/Sounds/PortkeyActivate    | Transit passage cue                         | <2s      |
| Assets/Sounds/WillowImpact       | Whomping Willow swing                       | <2s      |
| Assets/Sounds/PensiveSubmerge    | Memory entry                                | <3s      |

Until filled, the mod uses vanilla Terraria sounds (`SoundID.*`) as approximations in code.

---

## Regeneration

To regenerate all placeholder sprites after modifying `generate_sprites.py`:

```bash
cd WizardingWorld
python generate_sprites.py
```

This overwrites all existing PNGs. Runtime is ~10-15 seconds for all ~596 sprites.
