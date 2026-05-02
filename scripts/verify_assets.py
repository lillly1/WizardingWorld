#!/usr/bin/env python3
"""Verify release readiness for in-game PNG assets."""

from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image


KEY_COLORS = {(0, 255, 0), (255, 0, 255)}
EXPECTED_HUGE = {
    "Content/NPCs/Enemies/PhantomStag.png",
}
EXPECTED_HUGE_PREFIXES = (
    "Content/NPCs/Town/",
)


def is_expected_huge(rel: str) -> bool:
    return rel in EXPECTED_HUGE or rel.startswith(EXPECTED_HUGE_PREFIXES)


def has_key_pixels(image: Image.Image) -> bool:
    rgba = image.convert("RGBA")
    raw = rgba.tobytes()
    for i in range(0, len(raw), 4):
        r, g, b, a = raw[i], raw[i + 1], raw[i + 2], raw[i + 3]
        if a and (r, g, b) in KEY_COLORS:
            return True
    return False


def main() -> int:
    root = Path(__file__).resolve().parents[1]
    content = root / "Content"
    done_file = root / "tools" / "generated_batches" / "manual_review_done.txt"

    pngs = sorted(content.rglob("*.png"))
    done_entries: set[str] = set()
    if done_file.exists():
        done_entries = {
            line.strip()
            for line in done_file.read_text(encoding="utf-8", errors="ignore").splitlines()
            if line.strip()
        }

    open_errors: list[tuple[str, str]] = []
    key_files: list[str] = []
    wrong_buff_sizes: list[tuple[str, tuple[int, int]]] = []
    unexpected_huge: list[tuple[str, tuple[int, int]]] = []

    for path in pngs:
        rel = path.relative_to(root).as_posix()
        try:
            with Image.open(path) as image:
                size = image.size
                if has_key_pixels(image):
                    key_files.append(rel)
        except Exception as exc:  # pragma: no cover - diagnostic path
            open_errors.append((rel, str(exc)))
            continue

        if path.name.endswith("Buff.png") and size != (32, 32):
            wrong_buff_sizes.append((rel, size))
        if (size[0] > 1000 or size[1] > 1000) and not is_expected_huge(rel):
            unexpected_huge.append((rel, size))

    expected_entries = {p.relative_to(root).as_posix() for p in pngs}
    missing_done = sorted(expected_entries - done_entries)
    extra_done = sorted(done_entries - expected_entries)

    checks = {
        "content_png": len(pngs),
        "done_entries": len(done_entries),
        "missing_done": len(missing_done),
        "extra_done": len(extra_done),
        "open_errors": len(open_errors),
        "exact_key_pixel_files": len(key_files),
        "wrong_buff_sizes": len(wrong_buff_sizes),
        "unexpected_huge_files": len(unexpected_huge),
    }
    for key, value in checks.items():
        print(f"{key}: {value}")

    failures = {
        "missing_done": missing_done,
        "extra_done": extra_done,
        "open_errors": open_errors,
        "exact_key_pixel_files": key_files,
        "wrong_buff_sizes": wrong_buff_sizes,
        "unexpected_huge_files": unexpected_huge,
    }
    failed = False
    for name, values in failures.items():
        if values:
            failed = True
            print(f"\n{name}:")
            for value in values[:50]:
                print(f"  {value}")

    if failed:
        print("\nAsset verification failed.")
        return 1

    print("\nAsset verification passed.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
