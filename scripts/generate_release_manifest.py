#!/usr/bin/env python3
"""
Wizarding World -- Local Release Manifest Generator (Phase 30)

Reproduces the release_manifest.json that CI produces, but runnable locally
so a dev can snapshot a build the same way.

Output: release_manifest.json at repo root.

Usage:
    python scripts/generate_release_manifest.py
    python scripts/generate_release_manifest.py --phase "Phase 30"
    python scripts/generate_release_manifest.py --commit "$(git rev-parse HEAD)"

The defaults are: phase = "local", commit = "local" if not passed.
This is intentional -- locally we may not have git and don't know the phase.
"""

import argparse
import datetime
import json
import sys
from pathlib import Path


def find_repo_root() -> Path:
    here = Path(__file__).resolve().parent.parent
    if (here / "WizardingWorld.csproj").exists():
        return here
    for p in Path(__file__).resolve().parents:
        if (p / "WizardingWorld.csproj").exists():
            return p
    sys.exit("ERROR: cannot find WizardingWorld.csproj")


def main() -> int:
    parser = argparse.ArgumentParser(description="Generate a local release manifest")
    parser.add_argument("--phase", default="local", help="Phase label (e.g. 'Phase 30')")
    parser.add_argument("--commit", default="local", help="Commit SHA (default: 'local')")
    parser.add_argument("--output", default=None, help="Output file (default: release_manifest.json at repo root)")
    args = parser.parse_args()

    root = find_repo_root()
    manifest_path = root / "scripts" / "content_manifest.json"

    if not manifest_path.exists():
        sys.exit(
            f"ERROR: {manifest_path} not found.\n"
            "Run  python scripts/scan_content.py  first."
        )

    with open(manifest_path, encoding="utf-8") as f:
        manifest = json.load(f)

    # Mirror the CI structure in .github/workflows/guide-ci.yml
    release = {
        "phase": args.phase,
        "timestamp": datetime.datetime.now(datetime.timezone.utc).isoformat().replace("+00:00", "Z"),
        "commit": args.commit,
        "counts": manifest["summary"],
        "verification": "run verify_guide.py --strict to confirm",
        "artifacts": [
            "WizardingWorld_Guide_EN.pdf",
            "WizardingWorld_Guide_ZH.pdf",
            "scripts/content_manifest.json",
            "scripts/guide_content.json",
            "scripts/mechanical_data/",
            "scripts/zh_translations.json",
        ],
        "source": "local (scripts/generate_release_manifest.py)",
    }

    output_path = Path(args.output) if args.output else root / "release_manifest.json"
    with open(output_path, "w", encoding="utf-8") as f:
        json.dump(release, f, indent=2, ensure_ascii=False)

    print(f"Release manifest written: {output_path}")
    print(f"  phase:  {release['phase']}")
    print(f"  commit: {release['commit']}")
    print(f"  counts: {len(release['counts'])} summary keys")

    return 0


if __name__ == "__main__":
    sys.exit(main())
