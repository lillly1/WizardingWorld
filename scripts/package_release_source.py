#!/usr/bin/env python3
"""Create a clean source release ZIP for local handoff or GitHub release assets."""

from __future__ import annotations

import argparse
import sys
import zipfile
from pathlib import Path


EXCLUDED_DIRS = {
    ".git",
    ".vs",
    ".pytest_cache",
    "bin",
    "dist",
    "obj",
    "__pycache__",
}
EXCLUDED_SUFFIXES = {
    ".log",
    ".pyc",
    ".tmp",
    ".user",
    ".zip",
}


def find_repo_root() -> Path:
    here = Path(__file__).resolve().parent.parent
    if (here / "WizardingWorld.csproj").exists():
        return here
    sys.exit("ERROR: cannot find WizardingWorld.csproj from script location")


def iter_release_files(root: Path):
    for path in sorted(root.rglob("*")):
        if not path.is_file():
            continue
        rel = path.relative_to(root)
        if any(part in EXCLUDED_DIRS for part in rel.parts):
            continue
        if path.suffix.lower() in EXCLUDED_SUFFIXES:
            continue
        yield path, rel


def main() -> int:
    parser = argparse.ArgumentParser(description="Package clean WizardingWorld source release ZIP")
    parser.add_argument(
        "--output",
        default=None,
        help="Output ZIP path (default: dist/WizardingWorld_asset-ready_source.zip)",
    )
    args = parser.parse_args()

    root = find_repo_root()
    output = Path(args.output) if args.output else root / "dist" / "WizardingWorld_asset-ready_source.zip"
    if not output.is_absolute():
        output = root / output
    output.parent.mkdir(parents=True, exist_ok=True)

    count = 0
    with zipfile.ZipFile(output, "w", compression=zipfile.ZIP_DEFLATED, compresslevel=9) as archive:
        for path, rel in iter_release_files(root):
            archive.write(path, rel.as_posix())
            count += 1

    print(f"Source release package written: {output}")
    print(f"  files: {count}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
