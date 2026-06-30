# Release Readiness

## Current Status

The repository is in release-candidate shape for local tModLoader packaging.

- In-game PNG assets are present, readable, manually registered, and free of exact green/magenta key-color residue.
- Buff icons are normalized to `32x32`.
- Pet `Buff` and `Item` icons have been replaced with small transparent sprites matching their code dimensions.
- Strict guide/content verification passes.
- Primary single-player route QA has passed from early progression through Voldemort and the Deathly Hallows ending.
- Debug/QA command code is compiled only in `Debug`; public `Release` builds do not register `/wwdebug`.
- Local tModLoader packaging passes on the maintainer machine.

## Verified Commands

Run these from the repository root:

```bash
python scripts/verify_assets.py
python scripts/verify_guide.py --strict
dotnet build --no-restore /p:BuildMod=false /p:TargetFramework=net8.0 /p:LangVersion=12.0
```

## Full Packaging

Use a normal local tModLoader environment. For public builds, prefer `Release` so developer commands are omitted:

```powershell
cd "$env:USERPROFILE\Documents\My Games\Terraria\tModLoader\ModSources\WizardingWorld"
dotnet build -c Release --no-restore
```

For QA builds that need `/wwdebug`, use the default Debug build:

```powershell
dotnet build --no-restore
```

## Release Assets

Refresh the release snapshot before publishing:

```bash
python scripts/scan_content.py --pretty
python scripts/generate_release_manifest.py --phase "release-candidate" --commit "local-release-candidate"
python scripts/package_release_source.py
```

The source release asset is written to `dist/WizardingWorld_release-candidate_source.zip`. It is not a replacement for the final `.tmod`; it is a clean handoff package for local packaging or GitHub release attachment.

## Publishing Materials

- `description.txt` is the short Mod Browser description.
- `docs/WORKSHOP_PAGE.md` contains ready-to-paste Steam Workshop copy, version notes, screenshot guidance, and public disclaimer text.
- `docs/RELEASE_CHECKLIST.md` is the final upload checklist for the maintainer.
