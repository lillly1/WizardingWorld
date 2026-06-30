# Release Checklist

Use this checklist immediately before uploading Wizarding World 1.0.0.

## Current Release Candidate

- Version: `1.0.0`
- Phase: `release-candidate`
- Public package: `C:\Users\Lily\Documents\My Games\Terraria\tModLoader\Mods\WizardingWorld.tmod`
- Mod source used for packaging: `C:\Users\Lily\Documents\My Games\Terraria\tModLoader\ModSources\WizardingWorld`
- Workshop copy: `docs/WORKSHOP_PAGE.md`
- Mod Browser copy: `description.txt`
- Release manifest: `release_manifest.json`

## Required Before Upload

- [ ] Confirm `build.txt` has the correct public version.
- [ ] Fill `homepage =` in `build.txt` with the final GitHub or Workshop URL if you want the Mod Browser listing to show one.
- [ ] Build the public package in `Release`, not Debug:

```powershell
cd "$env:USERPROFILE\Documents\My Games\Terraria\tModLoader\ModSources\WizardingWorld"
dotnet build -c Release --no-restore
```

- [ ] Confirm the Release DLL does not include `/wwdebug`, `WizardDebugCommand`, or `QATestWand`.
- [ ] Confirm `WizardingWorld.tmod` timestamp updated after the final Release build.
- [ ] Launch tModLoader once and confirm `Mod Load Completed` appears in `tModLoader-Logs/client.log`.
- [ ] Open the mod in the Mods menu and confirm the icon, name, version, and description look normal.
- [ ] Confirm no Debug-only QA commands are advertised in player-facing docs.

## Verification Commands

Run from the repository root:

```powershell
python scripts\verify_assets.py
python scripts\verify_guide.py --strict
python scripts\scan_content.py --pretty
python scripts\generate_release_manifest.py --phase "release-candidate" --commit "local-release-candidate"
```

Expected current summary:

- `verify_assets.py`: passes
- `verify_guide.py --strict`: `665 passed, 0 failed, 0 warnings`
- Content snapshot: `593 C# files`, `606 PNGs`, `1509 total`
- Main counts: `12 bosses`, `24 active wands`, `78 enemies`, `16 town NPCs`, `65 accessories`

## Upload Materials

- [ ] `description.txt` for Mod Browser text.
- [ ] `docs/WORKSHOP_PAGE.md` for long-form Workshop description.
- [ ] `icon.png` as the icon.
- [ ] `release_assets/wizardingworld-cover-16x9.png` as primary cover.
- [ ] 5-8 screenshots, preferably real gameplay:
  - [ ] First wand / Hogwarts Letter / early inventory
  - [ ] Boss Compass or Wizard's Almanac guidance
  - [ ] Early boss fight
  - [ ] Late boss fight
  - [ ] Magical location or town NPC
  - [ ] Hallows/Horcrux route screen, optional

## Known Release Notes

- Best-tested path: single-player main progression from early game through Voldemort and Deathly Hallows.
- Wider community testing still needed for multiplayer sync, long balance runs, and unusual world states.
- Public Release builds omit `/wwdebug` and QA-only test weapons.
- This is an unofficial fan project and must keep the disclaimer visible on public pages.

## Do Not Ship

- A Debug build.
- A package that registers `/wwdebug`.
- A package containing `QATestWand`.
- New gameplay changes after final QA unless the full affected route is retested.
- Placeholder-only screenshots where real gameplay would be clearer.

## Final Manual Smoke

After upload, subscribe/download the public listing on the same machine or a second machine:

- [ ] tModLoader launches with Wizarding World enabled.
- [ ] Main menu loads without mod errors.
- [ ] New character + new world can enter.
- [ ] A wand item displays with proper localization.
- [ ] Boss Compass or Wizard's Almanac text appears correctly.
- [ ] Chinese localization can be switched and does not show missing keys in obvious UI.
