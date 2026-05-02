# Phase 14 Status -- Quidditch Authenticity Pass

## Summary

Replaced the abstract "hit any NPC" Quaffle scoring with real hoop-based goal targets, added Bludger hazards that pursue mounted players, created Madam Hooch as a Town NPC referee/shop anchor, and wired pitch setup/cleanup into the match lifecycle. 135/135 verification checks pass.

## What Changed

### Hoop-Based Goal Scoring (replaces generic NPC hits)
- **QuidditchHoop NPC** -- 3 stationary golden ring targets spawn 400px ahead of the player at match start
- Stacked vertically at 100px intervals (low/mid/high)
- Semi-transparent, indestructible, no health bar
- Quaffle ONLY scores when hitting a QuidditchHoop (not any NPC)
- Gold dust celebration burst on each valid goal
- Auto-despawns when match ends

### Bludger Hazard
- **Bludger NPC** -- 2 autonomous iron balls spawn at match start
- Pursues mounted players at speed 6 with lerp tracking
- 25 contact damage + 2-second Slow debuff
- Can be knocked away by Beater's Bat (5x knockback multiplier)
- Iron dust trail visual
- Auto-despawns when match ends

### Madam Hooch (Town NPC #9)
- Flying instructor and Quidditch referee
- Moves in after Basilisk defeated
- **Shop**: Quidditch Whistle, Quaffle, Beater's Bat, Flying Carpet (Hardmode)
- **Standings button**: shows current Quidditch Cup standings
- 6 dialogue lines (standard + seasonal + cup-won variants)
- Bestiary entry

### Pitch Setup/Cleanup
- `SpawnPitchElements()` called at match start: 3 hoops + 2 Bludgers
- `CleanupPitchElements()` called at match end: removes all hoops + Bludgers
- Both hoops and Bludgers self-despawn if match becomes inactive

### Updated Match Flow
```
1. Blow Quidditch Whistle (or buy from Madam Hooch)
2. 3 golden hoops spawn ahead + 2 Bludgers flank you
3. SCORING PHASE (2 min):
   - Throw Quaffle at hoops for 10 pts each
   - Dodge/bat away Bludgers
   - Opponent scores over time (simulated)
4. SNITCH PHASE (1 min):
   - Golden Snitch spawns
   - Catch it for +150 pts
   - Miss it and opponent Seeker catches it
5. Final score determines winner
6. Hoops and Bludgers despawn
7. Season standings updated
```

## Files Created (7)

| File | Type |
|------|------|
| Content/NPCs/Enemies/QuidditchHoop.cs | Goal target NPC |
| Content/NPCs/Enemies/QuidditchHoop.png | Placeholder sprite (32x48) |
| Content/NPCs/Enemies/Bludger.cs | Hazard NPC |
| Content/NPCs/Enemies/Bludger.png | Placeholder sprite (20x20) |
| Content/NPCs/Town/MadamHooch.cs | Town NPC |
| Content/NPCs/Town/MadamHooch.png | Placeholder body sprite (40x56) |
| Content/NPCs/Town/MadamHooch_Head.png | Placeholder head sprite (36x38) |
| PHASE14_STATUS.md | This document |

## Files Modified (8)

| File | Change |
|------|--------|
| Common/Systems/QuidditchCupSystem.cs | +SpawnPitchElements, +CleanupPitchElements in StartMatch/EndMatch |
| Content/Projectiles/QuaffleProjectile.cs | OnHitNPC now checks for QuidditchHoop type (not any NPC) |
| Localization/en-US_Mods.WizardingWorld.hjson | +MadamHooch NPC + dialogue + bestiary, +Bludger/Hoop names, +StandingsButton |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +all matching Chinese entries |
| scripts/mechanical_data/shops.json | +MadamHooch shop (4 items) |
| scripts/guide_content.json | +MadamHooch in NPC table, intro "Nine" NPCs |
| scripts/verify_guide.py | +12 Phase 14 authenticity checks (135 total) |
| scripts/content_manifest.json | Regenerated (468 C#, 55 enemies, 9 NPCs) |

## Verification Results

**135 passed, 0 failed, 0 warnings.**

12 new checks:
- QuidditchHoop.cs, Bludger.cs, MadamHooch.cs existence
- 4 sprite existence (hoop, Bludger, Hooch body + head)
- 2 EN localization (MadamHooch, Bludger)
- 1 ZH localization (MadamHooch)
- 1 Quaffle hoop-based scoring check
- 1 MadamHooch shop export check

## Canon Assessment

| Element | Canon Status |
|---------|-------------|
| Quaffle through hoops | Canon-faithful (goals scored through ring targets) |
| Bludger pursuit | Canon-faithful (enchanted iron balls that unseat riders) |
| Beater's Bat knockback | Canon-faithful (Beaters hit Bludgers away) |
| Madam Hooch as referee | Canon-faithful (flying instructor / match official) |
| 3 hoops per side | Canon-faithful (three goal posts at each end) |
| Golden Snitch +150 | Canon-faithful (150 points, ends match) |
| Opponent scoring simulated | **Honest abstraction** -- guide describes this as "simulated pressure" |
| No visual pitch structure | **Honest limitation** -- hoops represent the pitch spatially |

## Remaining Limitations

- All sprites are placeholder solid-color rectangles
- Opponent team is fully simulated (no visible Chasers/Keeper NPCs)
- No visual pitch boundary or stands
- Hoops spawn in a fixed pattern relative to player facing direction
- No match commentary system (Lee Jordan flavor deferred)
- zh-Hant not updated
- Hoop collision with Quaffle uses NPC hit detection (works but could benefit from custom hitbox)
