# Phase 3 Changelog

## Scope

Phase 3 focused on canon cleanup, Deathly Hallows progression refactoring, Horcrux/Voldemort narrative support, Azkaban survival depth, and a documentation consistency sweep.

## Key Gameplay Changes

- Added a dedicated `HallowsSystem` to store world Hallows progression.
- Reworked the true `Invisibility Cloak` into a one-time Dumbledore reward instead of a generic craft/drop path.
- Kept `Demiguise Weave Cloak` as ordinary stealth gear, clearly separated from the Hallow.
- Reworked `Gaunt's Ring` into the carrier for the `Resurrection Stone`; Dumbledore now purifies it after the Horcrux Hunt and Voldemort progression.
- Moved active Hallows resolution into `WizardPlayer` + `HallowsSystem` instead of cloak-side scanning.
- Expanded `HorcruxHuntSystem` with Nagini tracking and additional Voldemort fight scaling hooks.
- Voldemort now changes teleport pressure, shield behavior, minion pressure, and projectile density based on Horcrux/Hallows preparation.
- Expanded Azkaban content with a real despair meter loop, `Ward of Hope`, `Azkaban Ward Sigil`, stronger Patronus relevance, and better event identity.
- Reframed `Reparo` as ward/construct/support repair instead of flesh healing.
- Recentered `Riddikulus` on Boggarts, fear, and anti-illusion utility.
- Adjusted Fluffy toward guardian-dog flavor with a music weakness.

## Canon Corrections

- `Wand of Destiny` is no longer a separate legendary progression path.
- `Shadow Elder Wand` and `Wand of Destiny` now exist only as legacy migration stubs for old saves.
- Patronus is now the main anti-Dementor answer.
- The unique Hallow cloak is no longer treated like Demiguise-crafted gear.
- The Resurrection Stone is no longer an unrelated standalone loot artifact.

## Documentation Sweep

- Updated README, manifest, in-game books, English localization, and PDF generator text.
- Standardized counts and progression wording around bosses, accessories, potions, pets, Hallows, and Horcrux Hunt messaging.

## Remaining Intentional Deviations

- Voldemort remains beatable without a full Horcrux clear for gameplay accessibility, but the fight is materially harder.
- Fluffy's music weakness uses the existing flute summon item rather than a separate scripted lullaby system.
- Horcrux representation is inspired by canon structure without forcing a one-to-one book recreation.
