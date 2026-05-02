#!/usr/bin/env python3
from __future__ import annotations

import argparse
from pathlib import Path

from PIL import Image


def _avg_rgba(samples: list[tuple[int, int, int, int]]) -> tuple[float, float, float, float]:
    count = max(1, len(samples))
    return tuple(sum(pixel[index] for pixel in samples) / count for index in range(4))


def _color_distance(a: tuple[int, int, int, int], b: tuple[float, float, float, float]) -> float:
    return abs(a[0] - b[0]) + abs(a[1] - b[1]) + abs(a[2] - b[2])


def remove_green_background(image: Image.Image) -> tuple[Image.Image, tuple[int, int, int, int] | None]:
    image = image.convert("RGBA")
    pixels = image.load()
    width, height = image.size
    min_x, min_y = width, height
    max_x, max_y = -1, -1
    corners = [
        pixels[0, 0],
        pixels[max(0, width - 1), 0],
        pixels[0, max(0, height - 1)],
        pixels[max(0, width - 1), max(0, height - 1)],
    ]
    bg = _avg_rgba(corners)
    bg_greenish = bg[1] > bg[0] * 1.2 and bg[1] > bg[2] * 1.2 and bg[1] > 80

    for y in range(height):
        for x in range(width):
            r, g, b, a = pixels[x, y]
            if a == 0:
                continue
            same_as_green = g >= 220 and r <= 50 and b <= 50
            same_as_bg = bg_greenish and _color_distance((r, g, b, a), bg) <= 120
            if same_as_green or same_as_bg:
                pixels[x, y] = (0, 0, 0, 0)
                continue
            if x < min_x:
                min_x = x
            if y < min_y:
                min_y = y
            if x > max_x:
                max_x = x
            if y > max_y:
                max_y = y

    if max_x < min_x or max_y < min_y:
        return image, None
    return image, (min_x, min_y, max_x + 1, max_y + 1)


def fit_to_canvas(image: Image.Image, target_width: int, target_height: int, padding: int) -> Image.Image:
    canvas = Image.new("RGBA", (target_width, target_height), (0, 0, 0, 0))
    source_width, source_height = image.size
    usable_width = max(1, target_width - padding * 2)
    usable_height = max(1, target_height - padding * 2)
    scale = min(usable_width / source_width, usable_height / source_height)
    resized = image.resize(
        (max(1, int(round(source_width * scale))), max(1, int(round(source_height * scale)))),
        Image.Resampling.NEAREST,
    )
    offset_x = (target_width - resized.width) // 2
    offset_y = (target_height - resized.height) // 2
    canvas.alpha_composite(resized, (offset_x, offset_y))
    return canvas


def process_image(source: Path, output: Path, width: int, height: int, kind: str, frame_count: int) -> None:
    original = Image.open(source)
    keyed, bbox = remove_green_background(original)
    cropped = keyed.crop(bbox) if bbox else keyed
    padding = 1 if min(width, height) >= 20 else 0

    if kind == "sheet" and frame_count > 1 and height % frame_count == 0:
        frame_height = height // frame_count
        frame = fit_to_canvas(cropped, width, frame_height, padding)
        sheet = Image.new("RGBA", (width, height), (0, 0, 0, 0))
        for index in range(frame_count):
            sheet.alpha_composite(frame, (0, index * frame_height))
        output.parent.mkdir(parents=True, exist_ok=True)
        sheet.save(output)
        return

    final = fit_to_canvas(cropped, width, height, padding)
    output.parent.mkdir(parents=True, exist_ok=True)
    final.save(output)


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--input", required=True)
    parser.add_argument("--output", required=True)
    parser.add_argument("--width", required=True, type=int)
    parser.add_argument("--height", required=True, type=int)
    parser.add_argument("--kind", required=True, choices=("fit", "icon", "sheet"))
    parser.add_argument("--frame-count", required=True, type=int)
    args = parser.parse_args()

    process_image(
        source=Path(args.input),
        output=Path(args.output),
        width=args.width,
        height=args.height,
        kind=args.kind,
        frame_count=args.frame_count,
    )


if __name__ == "__main__":
    main()
