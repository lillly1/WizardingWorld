# Release Readiness

## Current Status

The repository is ready for local tModLoader packaging outside the Codex sandbox.

- In-game PNG assets are present, readable, manually registered, and free of exact green/magenta key-color residue.
- Buff icons are normalized to `32x32`.
- Pet `Buff` and `Item` icons have been replaced with small transparent sprites matching their code dimensions.
- Strict guide/content verification passes.
- Pure .NET compilation passes when the tModLoader server packager is skipped.

## Verified Commands

Run these from the repository root:

```bash
python scripts/verify_assets.py
python scripts/verify_guide.py --strict
dotnet build --no-restore /p:BuildMod=false /p:TargetFramework=net8.0 /p:LangVersion=12.0
```

## Full Packaging

Full packaging still needs a normal local tModLoader environment:

```powershell
cd C:\Users\sam\Desktop\Terraria\WizardingWorld
dotnet build --no-restore
```

The Codex sandbox blocks socket creation during `dotnet tModLoader.dll -server -build ...`, so that final packaging step cannot be completed inside the sandbox even though the mod DLL compiles.

## Release Assets

Refresh the release snapshot before publishing:

```bash
python scripts/scan_content.py --pretty
python scripts/generate_release_manifest.py --phase "asset-ready" --commit "local-asset-ready"
python scripts/package_release_source.py
```

The source release asset is written to `dist/WizardingWorld_asset-ready_source.zip`. It is not a replacement for the final `.tmod`; it is a clean handoff package for local packaging or GitHub release attachment.
