# Phase 13 Status -- Quidditch Cup & Hogwarts Grounds Vertical Slice

## Summary

Built a complete inter-house Quidditch Cup season system with match flow, scoring phases, Snitch chase integration, seasonal standings, and a separate Quidditch Cup ceremony distinct from the House Cup. Wired into existing Snitch catch and Quaffle weapon systems. 123/123 verification checks pass.

## Gameplay Loop

1. Craft Quidditch Whistle (3 Gold + 5 Essence at Enchanting Table)
2. Equip house armor + mount a broom during daytime
3. Blow whistle to start a match against a rival house
4. **Scoring phase (2 min):** throw Quaffle at enemies for 10 pts each; opponent scores over time
5. **Snitch phase:** Golden Snitch appears -- catch it for +150 pts and match end
6. If timer runs out, opponent Seeker catches the Snitch
7. Winner determined by final score; standings updated
8. Play 3 matches (one vs each rival house) to complete the season
9. House with most wins lifts the Quidditch Cup -- 15-min victory buff
10. Whistle resets the season for next cycle

## Architecture

### QuidditchCupSystem (new ModSystem)
- Persistent seasonal standings (wins per house, matches played, cup winner)
- Match lifecycle: scoring phase -> Snitch phase -> resolution
- Opponent scoring simulated via timer-based pressure (0-20 pts per 10 seconds)
- Reuses existing `QuidditchEvent.SpawnSnitch()` for Snitch phase
- Reuses existing `QuaffleProjectile` for goal scoring (OnHitNPC hook)
- Awards Great Hall House Points on match completion (20 win / 5 loss)
- Awards 50 House Points to cup winner house
- Full world save/load + multiplayer net sync

### Separation from House Cup
- Quidditch Cup tracks **match wins** (per house, seasonal)
- House Cup tracks **House Points** (cumulative from diverse activities)
- Different standings, different thresholds, different ceremonies
- Guide content explicitly states "separate from the House Cup"
- Verification check ensures this distinction exists

## Files Created (5)

| File | Type |
|------|------|
| Common/Systems/QuidditchCupSystem.cs | ModSystem (season + match + standings) |
| Content/Items/Consumables/QuidditchWhistle.cs | Access item |
| Content/Items/Consumables/QuidditchWhistle.png | Placeholder sprite |
| Content/Buffs/QuidditchCupBuff.cs | Victory buff (+10% dmg, +10% speed, +luck) |
| Content/Buffs/QuidditchCupBuff.png | Placeholder sprite |
| PHASE13_STATUS.md | This document |

## Files Modified (6)

| File | Change |
|------|--------|
| Common/Systems/QuidditchEvent.cs | Made SpawnSnitch public; added match-aware catch hook |
| Content/Projectiles/QuaffleProjectile.cs | Added OnHitNPC goal scoring during matches |
| Localization/en-US_Mods.WizardingWorld.hjson | +item, +buff, +20 Quidditch system text keys |
| Localization/zh-Hans_Mods.WizardingWorld.hjson | +all matching Chinese keys |
| scripts/verify_guide.py | +10 Quidditch Cup checks (123 total) |
| scripts/guide_content.json | +Quidditch Cup system description (13 systems total) |

## Verification Results

**123 passed, 0 failed, 0 warnings.**

10 new Quidditch checks:
- QuidditchCupSystem.cs exists
- 2 sprite existence (whistle + cup buff)
- 3 EN localization (item + buff + 20-key system text section)
- 1 guide content separation check (Quidditch Cup vs House Cup)
- 2 ZH localization (item + system text)
- 1 Language.GetTextValue usage

## Is the Quidditch Cup Slice Playable and Closed?

**Yes.** A player can:
1. Craft the Quidditch Whistle
2. Start inter-house matches while on a broom in daytime
3. Score goals by throwing Quaffle at enemies (10 pts each)
4. Chase and catch the Golden Snitch (+150 pts, ends match)
5. Track seasonal standings across 3 matches
6. Win the Quidditch Cup with a 15-minute victory buff
7. See all text in EN + ZH localization
8. The Quidditch Cup is clearly separate from the House Cup

## Remaining Limitations

- All sprites are placeholder solid-color rectangles
- Match scoring uses "hit any NPC with Quaffle = goal" rather than actual hoop targets
- Opponent scoring is simulated timer-based, not visual NPCs playing
- No Bludger hazard system during matches (could enhance with enemy spawns)
- No visual pitch/hoops structure -- the match is spatial (player flies around fighting)
- No Madam Hooch NPC (whistle item serves as the lore anchor)
- Season is simple round-robin (3 matches), not a full league table
- zh-Hant not updated
