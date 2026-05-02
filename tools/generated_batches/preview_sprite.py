#!/usr/bin/env python3
from __future__ import annotations

import argparse
from pathlib import Path

from PIL import Image


def make_checkerboard(width: int, height: int, cell: int = 8) -> Image.Image:
    image = Image.new("RGBA", (width, height), (24, 24, 24, 255))
    pixels = image.load()
    for y in range(height):
        for x in range(width):
            if ((x // cell) + (y // cell)) % 2 == 0:
                pixels[x, y] = (40, 40, 40, 255)
    return image


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("input")
    parser.add_argument("--scale", type=int, default=8)
    parser.add_argument("--output")
    args = parser.parse_args()

    source = Path(args.input)
    target = Path(args.output) if args.output else source.with_name(f"{source.stem}_preview_x{args.scale}.png")

    image = Image.open(source).convert("RGBA")
    resized = image.resize((image.width * args.scale, image.height * args.scale), Image.Resampling.NEAREST)
    background = make_checkerboard(resized.width, resized.height, max(4, args.scale))
    background.alpha_composite(resized)
    target.parent.mkdir(parents=True, exist_ok=True)
    background.save(target)
    print(target)


if __name__ == "__main__":
    main()
