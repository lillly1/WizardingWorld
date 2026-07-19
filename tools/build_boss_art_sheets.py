from __future__ import annotations

import math
import argparse
from collections import Counter
from dataclasses import dataclass
from pathlib import Path
from typing import Iterable

import numpy as np
from PIL import Image, ImageDraw


ROOT = Path(__file__).resolve().parents[1]
SOURCE_DIR = Path("C:/Users/Lily/Downloads/hp图/角色")
PREVIEW_DIR = ROOT / "release_assets" / "visual_preview"
BOSS_DIR = ROOT / "Content" / "NPCs" / "Bosses"


@dataclass(frozen=True)
class BossArtJob:
    source: str
    folder: str
    texture: str
    frame_size: tuple[int, int]
    npc_size: tuple[int, int]
    head_focus: tuple[float, float]
    motion: str
    runtime_long_edge: int = 768
    frame_count: int = 8
    atlas_columns: int = 4
    blue_gray_background: bool = False


@dataclass(frozen=True)
class BossPreviewEntry:
    job: BossArtJob
    frames: tuple[Image.Image, ...]
    source_frame_size: tuple[int, int]


JOBS: tuple[BossArtJob, ...] = (
    BossArtJob("Mountain Troll.png", "Troll", "TrollBoss", (112, 128), (88, 104), (0.46, 0.20), "heavy", runtime_long_edge=768),
    BossArtJob("Quirrell.png", "Quirrell", "QuirrellBoss", (62, 92), (44, 64), (0.50, 0.16), "hover"),
    BossArtJob("Basilisk.png", "Basilisk", "BasiliskBoss", (192, 78), (160, 58), (0.63, 0.44), "serpent", runtime_long_edge=1536, frame_count=12, atlas_columns=3),
    BossArtJob("Aragog.png", "Aragog", "AragogBoss", (144, 108), (112, 78), (0.50, 0.46), "crawl"),
    BossArtJob("Fluffy.png", "Fluffy", "FluffyBoss", (152, 128), (116, 90), (0.50, 0.25), "heavy", runtime_long_edge=896),
    BossArtJob("Hungarian Horntail.png", "Horntail", "HorntailBoss", (192, 132), (148, 92), (0.54, 0.37), "flight", runtime_long_edge=1152),
    BossArtJob("Dolores Umbridge.png", "Umbridge", "UmbridgeBoss", (62, 92), (44, 64), (0.50, 0.20), "human", blue_gray_background=True),
    BossArtJob("Fenrir Greyback.png", "Fenrir", "FenrirBoss", (78, 100), (58, 76), (0.50, 0.16), "stalk"),
    BossArtJob("Bellatrix Lestrange.png", "Bellatrix", "BellatrixBoss", (66, 104), (46, 72), (0.50, 0.18), "human"),
    BossArtJob("Barty Crouch Jr.png", "BartyCrouch", "BartyCrouchBoss", (66, 104), (46, 72), (0.50, 0.18), "human"),
    BossArtJob("Dementor King.png", "DementorKing", "DementorKingBoss", (104, 140), (76, 104), (0.50, 0.25), "specter", runtime_long_edge=896),
    BossArtJob("Lord Voldemort.png", "Voldemort", "VoldemortBoss", (84, 132), (58, 92), (0.50, 0.15), "final", runtime_long_edge=1536),
)


def edge_pixels(arr: np.ndarray) -> np.ndarray:
    top = arr[0, :, :]
    bottom = arr[-1, :, :]
    left = arr[:, 0, :]
    right = arr[:, -1, :]
    return np.concatenate([top, bottom, left, right], axis=0)


def dominant_edge_colors(arr: np.ndarray, limit: int = 16) -> np.ndarray:
    edges = edge_pixels(arr)
    brightness = edges.max(axis=1)
    filtered_edges = edges[brightness > 118]
    if len(filtered_edges) > 0:
        edges = filtered_edges
    quantized = (edges // 16).astype(np.uint8)
    counts = Counter(map(tuple, quantized.tolist()))
    common = [np.array(color, dtype=np.int16) * 16 + 8 for color, _ in counts.most_common(limit)]
    return np.stack(common, axis=0)


def palette_distance_mask(rgb: np.ndarray, palette: np.ndarray, threshold: float) -> np.ndarray:
    minimum = np.full(rgb.shape[:2], np.iinfo(np.int32).max, dtype=np.int32)
    # Squared RGB distance exceeds int16 for high-contrast foreground pixels.
    # Compute in int32 or dark robes/scales can wrap negative and look like the
    # pale checkerboard palette.
    work = rgb.astype(np.int32)
    for color in palette:
        difference = work - color.astype(np.int32)[None, None, :]
        distance = np.sum(difference * difference, axis=2, dtype=np.int32)
        minimum = np.minimum(minimum, distance)
    return minimum <= round(threshold * threshold)


def border_connected(candidate: np.ndarray) -> np.ndarray:
    # A one-pixel candidate border joins every eligible edge region into one fill.
    # Pillow's flood fill is considerably faster than walking millions of pixels
    # through a Python deque and lets the mask stay at the source resolution.
    padded = np.pad(candidate, 1, mode="constant", constant_values=True)
    # Image.fromarray can expose a read-only core; copy before in-place floodfill.
    flood = Image.fromarray((padded.astype(np.uint8) * 255), "L").copy()
    ImageDraw.floodfill(flood, (0, 0), 128, thresh=0)
    connected = np.asarray(flood, dtype=np.uint8) == 128
    return connected[1:-1, 1:-1]


def adjacent_to_transparency(mask: np.ndarray) -> np.ndarray:
    padded = np.pad(mask, 1, mode="constant", constant_values=False)
    neighbors = np.zeros_like(mask)
    for y_offset in range(3):
        for x_offset in range(3):
            if x_offset == 1 and y_offset == 1:
                continue
            neighbors |= ~padded[y_offset:y_offset + mask.shape[0], x_offset:x_offset + mask.shape[1]]
    return neighbors


def foreground_protection_mask(shape: tuple[int, int], job: BossArtJob) -> np.ndarray:
    height, width = shape
    yy, xx = np.ogrid[:height, :width]
    u = xx / max(1, width - 1)
    v = yy / max(1, height - 1)
    protected = np.zeros(shape, dtype=bool)

    if job.folder == "Aragog":
        protected = (((u - 0.50) / 0.17) ** 2 + ((v - 0.56) / 0.20) ** 2) <= 1.0
    elif job.folder == "Horntail":
        head = (((u - 0.75) / 0.15) ** 2 + ((v - 0.48) / 0.20) ** 2) <= 1.0
        chest = (((u - 0.64) / 0.15) ** 2 + ((v - 0.67) / 0.18) ** 2) <= 1.0
        protected = head | chest
    elif job.folder == "Voldemort":
        head = (((u - 0.50) / 0.18) ** 2 + ((v - 0.12) / 0.15) ** 2) <= 1.0
        wand_hand = (((u - 0.27) / 0.15) ** 2 + ((v - 0.44) / 0.17) ** 2) <= 1.0
        casting_hand = (((u - 0.76) / 0.18) ** 2 + ((v - 0.37) / 0.18) ** 2) <= 1.0
        protected = head | wand_hand | casting_hand
    elif job.folder == "Bellatrix":
        head = (((u - 0.50) / 0.12) ** 2 + ((v - 0.14) / 0.14) ** 2) <= 1.0
        chest = (((u - 0.45) / 0.10) ** 2 + ((v - 0.30) / 0.10) ** 2) <= 1.0
        neck = (((u - 0.49) / 0.07) ** 2 + ((v - 0.24) / 0.08) ** 2) <= 1.0
        left_hand = (((u - 0.25) / 0.11) ** 2 + ((v - 0.55) / 0.11) ** 2) <= 1.0
        right_hand = (((u - 0.76) / 0.13) ** 2 + ((v - 0.47) / 0.12) ** 2) <= 1.0
        protected = head | chest | neck | left_hand | right_hand
    elif job.folder == "Troll":
        face = (((u - 0.50) / 0.22) ** 2 + ((v - 0.23) / 0.18) ** 2) <= 1.0
        chest = (((u - 0.50) / 0.34) ** 2 + ((v - 0.43) / 0.23) ** 2) <= 1.0
        hands = (
            (((u - 0.18) / 0.14) ** 2 + ((v - 0.40) / 0.20) ** 2) <= 1.0
        ) | (
            (((u - 0.84) / 0.14) ** 2 + ((v - 0.48) / 0.20) ** 2) <= 1.0
        )
        protected = face | chest | hands

    return protected


def remove_enclosed_checker_regions(
    subject: np.ndarray,
    rgb: np.ndarray,
    palette: np.ndarray,
    job: BossArtJob,
) -> np.ndarray:
    if job.folder not in {"Aragog", "Horntail", "Voldemort", "Bellatrix", "Troll"}:
        return subject

    protected = foreground_protection_mask(subject.shape, job)

    strict_checker = palette_distance_mask(rgb, palette, 34)
    cleaned = subject.copy()
    cleaned[strict_checker & ~protected] = False
    return cleaned


def remove_visible_background_residue(subject: Image.Image, job: BossArtJob) -> Image.Image:
    if job.folder not in {"Bellatrix", "Voldemort", "Troll"}:
        return subject

    rgba = np.array(subject.convert("RGBA"), dtype=np.uint8)
    rgb = rgba[:, :, :3].astype(np.int16)
    alpha = rgba[:, :, 3] > 0
    maxc = rgb.max(axis=2)
    minc = rgb.min(axis=2)
    saturation = maxc - minc
    brightness = rgb[:, :, 0] * 0.299 + rgb[:, :, 1] * 0.587 + rgb[:, :, 2] * 0.114
    protected = foreground_protection_mask(alpha.shape, job)
    height, width = alpha.shape
    yy, xx = np.ogrid[:height, :width]
    u = xx / max(1, width - 1)
    v = yy / max(1, height - 1)
    forced = np.zeros(alpha.shape, dtype=bool)
    if job.folder == "Bellatrix":
        # Source backing trapped between hair locks and between the right arm and
        # waist. These are background holes, not skin or robe highlights.
        face_and_neck = (((u - 0.50) / 0.15) ** 2 + ((v - 0.17) / 0.18) ** 2) <= 1.0
        hair_gaps = (v > 0.08) & (v < 0.27) & (u > 0.28) & (u < 0.74) & ~face_and_neck
        waist_gap = (((u - 0.62) / 0.11) ** 2 + ((v - 0.39) / 0.17) ** 2) <= 1.0
        sleeve_gap = (((u - 0.31) / 0.10) ** 2 + ((v - 0.51) / 0.14) ** 2) <= 1.0
        forced = hair_gaps | waist_gap | sleeve_gap
        forced_color = ((brightness > 112) & (saturation < 190)) | ((minc > 122) & (saturation < 210))
    elif job.folder == "Voldemort":
        right_sleeve_gap = (((u - 0.68) / 0.15) ** 2 + ((v - 0.43) / 0.27) ** 2) <= 1.0
        right_hand_under_gap = (((u - 0.74) / 0.13) ** 2 + ((v - 0.50) / 0.20) ** 2) <= 1.0
        right_lower_cuff_gap = (((u - 0.79) / 0.11) ** 2 + ((v - 0.58) / 0.15) ** 2) <= 1.0
        right_robe_pinholes = (u > 0.58) & (u < 0.98) & (v > 0.36) & (v < 0.78)
        left_wand_gap = (((u - 0.33) / 0.08) ** 2 + ((v - 0.47) / 0.16) ** 2) <= 1.0
        forced = right_sleeve_gap | right_hand_under_gap | right_lower_cuff_gap | right_robe_pinholes | left_wand_gap
        forced_color = ((brightness > 92) & (saturation < 220)) | ((minc > 98) & (saturation < 236))
    elif job.folder == "Troll":
        arm_torso_gap = (((u - 0.73) / 0.07) ** 2 + ((v - 0.51) / 0.16) ** 2) <= 1.0
        club_hand_gap = (((u - 0.24) / 0.07) ** 2 + ((v - 0.37) / 0.15) ** 2) <= 1.0
        forced = arm_torso_gap | club_hand_gap
        forced_color = ((brightness > 148) & (saturation < 132)) | ((minc > 154) & (saturation < 162))
    else:
        forced_color = np.zeros(alpha.shape, dtype=bool)

    visible_white_backing = alpha & ~protected & (
        ((brightness > 205) & (saturation < 74)) |
        ((minc > 214) & (saturation < 96))
    )
    forced_white_backing = alpha & forced & forced_color

    # White backing usually survives as solid islands in gaps between arms,
    # hair, weapons, and torso. Remove it after extraction so opaque source
    # pixels that were misclassified no longer ship as visible white blocks.
    rgba[visible_white_backing | forced_white_backing, 3] = 0
    rgba[rgba[:, :, 3] == 0, :3] = 0
    rgba[:, :, 3] = np.where(rgba[:, :, 3] > 0, 255, 0).astype(np.uint8)
    return Image.fromarray(rgba, "RGBA")


def make_background_mask(image: Image.Image, job: BossArtJob) -> Image.Image:
    # The source paintings contain a rendered checkerboard rather than real alpha.
    # Work at full resolution: the old 768px proxy merged thin hat/limb outlines
    # with the checkerboard and punched holes through otherwise opaque pixels.
    rgba = np.asarray(image.convert("RGBA"), dtype=np.int16)
    rgb = rgba[:, :, :3]
    alpha = rgba[:, :, 3]
    maxc = rgb.max(axis=2)
    minc = rgb.min(axis=2)
    saturation = maxc - minc
    brightness = rgb[:, :, 0] * 0.299 + rgb[:, :, 1] * 0.587 + rgb[:, :, 2] * 0.114
    palette = dominant_edge_colors(rgb.astype(np.uint8), limit=24)

    palette_like = palette_distance_mask(rgb, palette, 44 if job.blue_gray_background else 50)
    if job.blue_gray_background:
        # Umbridge's pink hat contains dark blue-gray folds. Edge-palette matching
        # keeps those folds because it does not broadly classify every cool gray.
        candidate = palette_like | (alpha < 8)
    else:
        pale_background = (
            ((brightness > 182) & (saturation < 30))
            | ((brightness > 204) & (saturation < 48))
            | ((brightness > 232) & (saturation < 76))
        )
        candidate = palette_like | pale_background | (alpha < 8)

    subject = ~border_connected(candidate)
    subject = remove_enclosed_checker_regions(subject, rgb, palette, job)

    # Remove only a few rings of checker-colored antialiasing that remain attached
    # to the silhouette. This is the white seam visible around Voldemort, Horntail,
    # and Aragog; interior highlights and eyes are untouched.
    fringe_palette = palette_distance_mask(rgb, palette, 58 if job.blue_gray_background else 62)
    white_fringe = (brightness > 212) & (saturation < 72)
    neutral_fringe = (brightness > 175) & (saturation < 30)
    removable = fringe_palette | white_fringe | neutral_fringe
    for _ in range(4):
        trim = subject & adjacent_to_transparency(subject) & removable
        if not trim.any():
            break
        subject[trim] = False

    return Image.fromarray((subject.astype(np.uint8) * 255), "L")


def solidify_centered_subject(image: Image.Image, mask: Image.Image) -> tuple[Image.Image, Image.Image]:
    """Fill accidental transparent holes inside a centered humanoid silhouette.

    Umbridge's source has a blue-gray checker backing that leaks through small
    skirt openings. After edge-connected background removal those openings read
    as transparent in-game, so we repaint only the central body span per row.
    """
    rgb = np.array(image.convert("RGB"), dtype=np.uint8)
    old_mask = np.array(mask, dtype=np.uint8) > 0
    ys, xs = np.where(old_mask)
    if len(xs) == 0:
        return image, mask

    top = int(ys.min())
    bottom = int(ys.max()) + 1
    left = int(xs.min())
    right = int(xs.max()) + 1
    width = right - left
    lower_body = (ys > top + (bottom - top) * 0.25) & (ys < top + (bottom - top) * 0.95)
    center_x = int(np.median(xs[lower_body])) if lower_body.any() else int(np.median(xs))
    central_half_width = max(12, int(width * 0.23))
    min_span = max(10, int(width * 0.045))
    skirt_start = top + int((bottom - top) * 0.48)

    filled_mask = old_mask.copy()
    global_fill = np.median(rgb[old_mask], axis=0).astype(np.uint8)
    for y in range(top, bottom):
        if y < skirt_start:
            continue
        row_subject = np.where(old_mask[y])[0]
        if row_subject.size < 3:
            continue
        row_center = row_subject[
            (row_subject >= center_x - central_half_width) &
            (row_subject <= center_x + central_half_width)
        ]
        if row_center.size < 3:
            continue
        x0 = int(row_center.min())
        x1 = int(row_center.max())
        if x1 - x0 < min_span:
            continue

        before = filled_mask[y].copy()
        filled_mask[y, x0:x1 + 1] = True
        newly_filled = filled_mask[y] & ~before
        if not newly_filled.any():
            continue

        source_pixels = rgb[y, x0:x1 + 1][old_mask[y, x0:x1 + 1]]
        fallback = np.median(source_pixels, axis=0).astype(np.uint8) if len(source_pixels) else global_fill
        new_positions = np.where(newly_filled)[0]
        if new_positions.size == 0:
            continue

        segment_start = int(new_positions[0])
        previous = segment_start
        segments: list[tuple[int, int]] = []
        for x in new_positions[1:]:
            x = int(x)
            if x != previous + 1:
                segments.append((segment_start, previous))
                segment_start = x
            previous = x
        segments.append((segment_start, previous))

        for segment_left, segment_right in segments:
            left_sources = np.where(old_mask[y, max(x0, segment_left - 10):segment_left])[0]
            right_sources = np.where(old_mask[y, segment_right + 1:min(x1 + 1, segment_right + 11)])[0]
            colors: list[np.ndarray] = []
            if left_sources.size:
                left_base = max(x0, segment_left - 10)
                colors.append(rgb[y, left_base + left_sources[-1]])
            if right_sources.size:
                right_base = segment_right + 1
                colors.append(rgb[y, right_base + right_sources[0]])
            fill = np.mean(colors, axis=0).astype(np.uint8) if colors else fallback
            rgb[y, segment_left:segment_right + 1] = fill

    return Image.fromarray(rgb, "RGB"), Image.fromarray((filled_mask.astype(np.uint8) * 255), "L")


def underpaint_umbridge_skirt(subject: Image.Image) -> Image.Image:
    subject = subject.convert("RGBA")
    alpha = np.array(subject.getchannel("A"), dtype=np.uint8) > 0
    ys, xs = np.where(alpha)
    if len(xs) == 0:
        return subject

    top = int(ys.min())
    bottom = int(ys.max()) + 1
    left = int(xs.min())
    right = int(xs.max()) + 1
    height = bottom - top
    width = right - left
    lower_body = (ys > top + height * 0.42) & (ys < top + height * 0.92)
    center_x = int(np.median(xs[lower_body])) if lower_body.any() else int(np.median(xs))

    waist_y = top + int(height * 0.44)
    hem_y = top + int(height * 0.86)
    skirt_bottom = top + int(height * 0.91)
    upper_half = int(width * 0.12)
    lower_half = int(width * 0.27)

    underlay = Image.new("RGBA", subject.size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(underlay, "RGBA")
    skirt = [
        (center_x - upper_half, waist_y),
        (center_x + upper_half, waist_y),
        (center_x + lower_half, skirt_bottom),
        (center_x + int(lower_half * 0.74), hem_y),
        (center_x - int(lower_half * 0.78), hem_y),
        (center_x - lower_half, skirt_bottom),
    ]
    draw.polygon(skirt, fill=(197, 139, 154, 255))
    draw.polygon(
        [
            (center_x - upper_half, waist_y),
            (center_x - int(lower_half * 0.08), waist_y + int(height * 0.04)),
            (center_x - int(lower_half * 0.16), hem_y),
            (center_x - lower_half, skirt_bottom),
        ],
        fill=(156, 91, 112, 255),
    )
    draw.polygon(
        [
            (center_x + int(upper_half * 0.15), waist_y),
            (center_x + upper_half, waist_y),
            (center_x + lower_half, skirt_bottom),
            (center_x + int(lower_half * 0.32), hem_y),
        ],
        fill=(176, 111, 130, 255),
    )
    for y in range(waist_y + int(height * 0.12), hem_y, max(18, height // 28)):
        draw.line(
            [
                (center_x - int(lower_half * 0.68), y),
                (center_x + int(lower_half * 0.60), y + max(1, height // 120)),
            ],
            fill=(219, 170, 180, 255),
            width=max(3, height // 180),
        )
    draw.line([(center_x - lower_half, skirt_bottom), (center_x + lower_half, skirt_bottom)], fill=(118, 62, 82, 255), width=max(5, height // 90))

    underlay.alpha_composite(subject)
    rgba = np.array(underlay, dtype=np.uint8)
    rgba[:, :, 3] = np.where(rgba[:, :, 3] > 0, 255, 0).astype(np.uint8)
    return Image.fromarray(rgba, "RGBA")


def restore_umbridge_pigment(image: Image.Image, mask: Image.Image) -> Image.Image:
    """Restore pink costume pixels that are impossible to be the blue-gray backing.

    This protects the hat's dark folds and cape edge even when antialiasing makes a
    few pixels resemble the checker palette. The skirt's genuinely checker-colored
    openings are still handled by the opaque underpaint below.
    """
    rgb = np.asarray(image.convert("RGB"), dtype=np.int16)
    red = rgb[:, :, 0]
    green = rgb[:, :, 1]
    blue = rgb[:, :, 2]
    pink_pigment = (red > 58) & (red >= green + 10) & (red >= blue + 5)
    restored = np.asarray(mask, dtype=np.uint8).copy()
    restored[pink_pigment] = 255
    return Image.fromarray(restored, "L")


def erase_bottom_right_watermark(image: Image.Image) -> Image.Image:
    rgb = np.array(image.convert("RGB"), dtype=np.uint8)
    h, w, _ = rgb.shape
    x0 = int(w * 0.70)
    y0 = int(h * 0.86)
    region = rgb[y0:h, x0:w, :]
    maxc = region.max(axis=2)
    minc = region.min(axis=2)
    saturation = maxc - minc
    watermark = (maxc > 178) & (saturation < 58)
    if watermark.any():
        fill = np.median(edge_pixels(rgb).reshape(-1, 3), axis=0).astype(np.uint8)
        region[watermark] = fill
        rgb[y0:h, x0:w, :] = region
    return Image.fromarray(rgb, "RGB")


def bbox_from_mask(mask: Image.Image) -> tuple[int, int, int, int]:
    bbox = mask.getbbox()
    if bbox is None:
        raise ValueError("mask produced no subject")
    left, top, right, bottom = bbox
    pad_x = max(4, (right - left) // 48)
    pad_y = max(4, (bottom - top) // 48)
    return (
        max(0, left - pad_x),
        max(0, top - pad_y),
        min(mask.width, right + pad_x),
        min(mask.height, bottom + pad_y),
    )


def extract_subject(path: Path, job: BossArtJob) -> Image.Image:
    image = erase_bottom_right_watermark(Image.open(path).convert("RGB"))
    mask = make_background_mask(image, job)
    if job.blue_gray_background:
        mask = restore_umbridge_pigment(image, mask)
    subject = image.convert("RGBA")
    subject.putalpha(mask)
    if job.blue_gray_background:
        subject = underpaint_umbridge_skirt(subject)
    subject = remove_visible_background_residue(subject, job)

    rgba = np.array(subject, dtype=np.uint8)
    rgba[rgba[:, :, 3] == 0, :3] = 0
    rgba[:, :, 3] = np.where(rgba[:, :, 3] > 0, 255, 0).astype(np.uint8)
    return Image.fromarray(rgba, "RGBA")


def fit_subject(subject: Image.Image, frame_size: tuple[int, int], pad: int = 2) -> Image.Image:
    target_w, target_h = frame_size
    available_w = target_w - pad * 2
    available_h = target_h - pad * 2
    scale = min(available_w / subject.width, available_h / subject.height)
    new_size = (max(1, round(subject.width * scale)), max(1, round(subject.height * scale)))
    resized = subject.resize(new_size, Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", frame_size, (0, 0, 0, 0))
    x = (target_w - resized.width) // 2
    y = target_h - resized.height - pad
    canvas.alpha_composite(resized, (x, y))
    return canvas


def make_runtime_subject(subject: Image.Image, job: BossArtJob) -> Image.Image:
    longest = max(subject.size)
    if longest <= job.runtime_long_edge:
        return subject.copy()
    scale = job.runtime_long_edge / longest
    size = (max(1, round(subject.width * scale)), max(1, round(subject.height * scale)))
    resized = premultiply_alpha(subject).resize(size, Image.Resampling.LANCZOS)
    return restore_hard_alpha(resized)


def gaussian(u: float, v: float, cx: float, cy: float, rx: float, ry: float) -> float:
    return math.exp(-(((u - cx) / max(rx, 1e-6)) ** 2 + ((v - cy) / max(ry, 1e-6)) ** 2) * 2.0)


def rotation_delta(
    u: float,
    v: float,
    pivot: tuple[float, float],
    center: tuple[float, float],
    radius: tuple[float, float],
    angle: float,
) -> tuple[float, float]:
    weight = gaussian(u, v, center[0], center[1], radius[0], radius[1])
    px, py = pivot
    rel_x = u - px
    rel_y = v - py
    cosine = math.cos(angle)
    sine = math.sin(angle)
    rotated_x = rel_x * cosine - rel_y * sine
    rotated_y = rel_x * sine + rel_y * cosine
    return (rotated_x - rel_x) * weight, (rotated_y - rel_y) * weight


def rig_displacement(job: BossArtJob, u: float, v: float, phase: float) -> tuple[float, float]:
    """Small forward deformation in subject-bounding-box coordinates.

    The controls behave like a lightweight 2D bone rig: each joint rotation is
    spatially weighted, while cloth, wings, tails, and spider legs use secondary
    motion with delayed phases. Keeping deformation subtle preserves the painted
    anatomy instead of making the source illustration look rubbery.
    """
    sine = math.sin(phase)
    cosine = math.cos(phase)
    dx = 0.0
    dy = sine * 0.004

    def rotate(
        pivot: tuple[float, float],
        center: tuple[float, float],
        radius: tuple[float, float],
        angle: float,
    ) -> None:
        nonlocal dx, dy
        delta_x, delta_y = rotation_delta(u, v, pivot, center, radius, angle)
        dx += delta_x
        dy += delta_y

    if job.motion == "human":
        if job.folder == "Bellatrix":
            duel = 2.35
            torso = gaussian(u, v, 0.50, 0.46, 0.30, 0.25)
            hair = gaussian(u, v, 0.50, 0.16, 0.38, 0.26)
            skirt = max(0.0, min(1.0, (v - 0.45) / 0.50))
            wand_tip = gaussian(u, v, 0.08, 0.75, 0.16, 0.26)
            right_hand = gaussian(u, v, 0.84, 0.60, 0.18, 0.24)
            dx += (u - 0.50) * sine * 0.034 * torso
            dy -= abs(sine) * 0.010 * torso
            rotate((0.50, 0.31), (0.50, 0.17), (0.24, 0.22), -sine * 0.060)
            rotate((0.36, 0.36), (0.18, 0.55), (0.24, 0.38), math.sin(phase + 0.25) * 0.160 * duel)
            rotate((0.27, 0.58), (0.11, 0.82), (0.18, 0.36), math.sin(phase + 1.05) * 0.130 * duel)
            rotate((0.64, 0.36), (0.79, 0.53), (0.22, 0.34), -math.sin(phase + 1.20) * 0.105 * duel)
            rotate((0.72, 0.55), (0.86, 0.68), (0.18, 0.26), -math.sin(phase + 0.65) * 0.085 * duel)
            rotate((0.43, 0.68), (0.36, 0.88), (0.23, 0.28), -math.sin(phase + 0.85) * 0.070)
            rotate((0.58, 0.67), (0.65, 0.88), (0.23, 0.28), math.sin(phase + 0.40) * 0.070)
            dx += math.sin(phase + 0.45) * 0.075 * wand_tip
            dy += math.cos(phase + 0.65) * 0.055 * wand_tip
            dx -= math.sin(phase + 1.10) * 0.040 * right_hand
            dy += math.cos(phase + 1.40) * 0.030 * right_hand
            dx += math.sin(phase + u * 7.2 + v * 3.4) * skirt * skirt * 0.045
            dy += math.cos(phase + u * 4.1) * skirt * 0.014
            dx += math.sin(phase + u * 8.0) * 0.020 * hair
            dy += math.cos(phase + u * 5.0) * 0.010 * hair
            return dx, dy

        if job.folder == "BartyCrouch":
            duel = 2.20
            torso = gaussian(u, v, 0.50, 0.45, 0.28, 0.25)
            coat = max(0.0, min(1.0, (v - 0.42) / 0.52))
            wand_hand = gaussian(u, v, 0.22, 0.68, 0.17, 0.26)
            off_hand = gaussian(u, v, 0.78, 0.62, 0.18, 0.24)
            head = gaussian(u, v, 0.50, 0.16, 0.22, 0.20)
            dx += (u - 0.50) * sine * 0.030 * torso
            dy -= abs(sine) * 0.009 * torso
            rotate((0.50, 0.30), (0.50, 0.16), (0.23, 0.21), sine * 0.050)
            rotate((0.35, 0.36), (0.19, 0.54), (0.24, 0.36), math.sin(phase + 0.85) * 0.140 * duel)
            rotate((0.24, 0.57), (0.12, 0.73), (0.18, 0.30), math.sin(phase + 1.55) * 0.125 * duel)
            rotate((0.65, 0.36), (0.81, 0.54), (0.24, 0.36), -math.sin(phase + 0.20) * 0.145 * duel)
            rotate((0.76, 0.57), (0.90, 0.70), (0.18, 0.29), -math.sin(phase + 0.95) * 0.120 * duel)
            rotate((0.42, 0.70), (0.34, 0.88), (0.20, 0.28), -math.sin(phase + 0.65) * 0.055)
            rotate((0.58, 0.70), (0.66, 0.88), (0.20, 0.28), math.sin(phase + 0.95) * 0.055)
            dx += math.sin(phase + 0.75) * 0.070 * wand_hand
            dy += math.cos(phase + 1.20) * 0.045 * wand_hand
            dx -= math.sin(phase + 0.10) * 0.052 * off_hand
            dy += math.cos(phase + 0.50) * 0.032 * off_hand
            dx += math.sin(phase + 1.85) * 0.016 * head
            dy += math.cos(phase + 1.45) * 0.008 * head
            dx += math.sin(phase + u * 6.1 + v * 4.2) * coat * coat * 0.040
            dy += math.cos(phase + u * 3.6) * coat * 0.012
            return dx, dy

        energy = 1.35 if job.folder in {"Bellatrix", "Umbridge"} else 1.08
        torso = gaussian(u, v, 0.50, 0.47, 0.30, 0.24)
        dx += (u - 0.50) * sine * 0.016 * torso
        dy -= abs(sine) * 0.006 * torso
        rotate((0.50, 0.30), (0.50, 0.17), (0.24, 0.22), sine * 0.023 * energy)
        rotate((0.35, 0.38), (0.23, 0.52), (0.22, 0.31), math.sin(phase + 0.55) * 0.064 * energy)
        rotate((0.66, 0.39), (0.75, 0.53), (0.22, 0.31), -math.sin(phase + 0.18) * 0.050 * energy)
        lower = max(0.0, min(1.0, (v - 0.50) / 0.45))
        dx += math.sin(phase + v * 4.4) * lower * lower * 0.013 * energy
        if job.folder == "Umbridge":
            # Separate hat, arms, wand hand, and skirt hem so her animation reads
            # as an articulated character instead of a translated portrait.
            rotate((0.50, 0.30), (0.50, 0.08), (0.36, 0.19), -sine * 0.034)
            rotate((0.34, 0.37), (0.18, 0.55), (0.20, 0.34), math.sin(phase + 1.15) * 0.096)
            rotate((0.66, 0.37), (0.84, 0.53), (0.22, 0.35), -math.sin(phase + 0.75) * 0.110)
            rotate((0.72, 0.57), (0.86, 0.64), (0.17, 0.24), -math.sin(phase + 1.70) * 0.088)
            rotate((0.42, 0.60), (0.34, 0.78), (0.20, 0.28), math.sin(phase + 2.20) * 0.038)
            rotate((0.58, 0.60), (0.68, 0.78), (0.20, 0.28), -math.sin(phase + 1.85) * 0.038)
            skirt = max(0.0, min(1.0, (v - 0.42) / 0.48))
            dx += math.sin(phase + u * 5.2 + v * 3.0) * skirt * skirt * 0.024
            dy += math.cos(phase + u * 2.7) * skirt * 0.009

    elif job.motion == "hover":
        dy += sine * 0.008
        rotate((0.50, 0.33), (0.50, 0.18), (0.25, 0.23), sine * 0.024)
        rotate((0.38, 0.42), (0.22, 0.58), (0.24, 0.36), math.sin(phase + 0.45) * 0.052)
        rotate((0.61, 0.42), (0.73, 0.57), (0.22, 0.34), -math.sin(phase + 0.20) * 0.038)
        hem = max(0.0, min(1.0, (v - 0.55) / 0.42))
        dx += math.sin(phase + u * 3.2 + v * 4.0) * hem * 0.017

    elif job.motion == "stalk":
        dy += abs(sine) * 0.006
        rotate((0.50, 0.31), (0.50, 0.17), (0.23, 0.21), -sine * 0.043)
        rotate((0.36, 0.39), (0.22, 0.56), (0.23, 0.34), sine * 0.090)
        rotate((0.64, 0.39), (0.78, 0.56), (0.23, 0.34), -sine * 0.088)
        rotate((0.43, 0.69), (0.36, 0.84), (0.20, 0.25), -sine * 0.052)
        rotate((0.57, 0.69), (0.64, 0.84), (0.20, 0.25), sine * 0.052)
        chest = gaussian(u, v, 0.50, 0.46, 0.28, 0.24)
        dx += (u - 0.50) * abs(sine) * 0.018 * chest

    elif job.motion == "heavy" and job.folder == "Troll":
        stomp = (1.0 - cosine) * 0.5
        dy += stomp * 0.011
        rotate((0.50, 0.36), (0.50, 0.22), (0.28, 0.24), sine * 0.028)
        rotate((0.33, 0.37), (0.20, 0.58), (0.24, 0.38), sine * 0.060)
        rotate((0.66, 0.37), (0.79, 0.57), (0.24, 0.38), -sine * 0.070)
        body = gaussian(u, v, 0.50, 0.57, 0.36, 0.32)
        dx += (u - 0.50) * stomp * 0.024 * body
        dy -= (v - 0.83) * stomp * 0.014 * body

    elif job.motion == "heavy":
        # Fluffy: three heads breathe and recoil on staggered beats while the
        # forepaws take alternating weight.
        for index, head_x in enumerate((0.34, 0.50, 0.66)):
            head_phase = phase + index * 0.82
            weight = gaussian(u, v, head_x, 0.25, 0.16, 0.19)
            dx += math.sin(head_phase) * 0.008 * weight
            dy += math.cos(head_phase) * 0.014 * weight
        chest = gaussian(u, v, 0.50, 0.54, 0.34, 0.29)
        dx += (u - 0.50) * abs(sine) * 0.026 * chest
        for paw_x, sign in ((0.29, 1.0), (0.71, -1.0)):
            paw = gaussian(u, v, paw_x, 0.80, 0.20, 0.23)
            dy -= max(0.0, sine * sign) * 0.022 * paw
            dx += sine * sign * 0.011 * paw

    elif job.motion == "crawl":
        abdomen = gaussian(u, v, 0.50, 0.32, 0.30, 0.25)
        dx += (u - 0.50) * abs(sine) * 0.030 * abdomen
        dy -= abs(sine) * 0.008 * abdomen
        leg_centers = (
            (0.14, 0.34), (0.18, 0.49), (0.20, 0.66), (0.25, 0.82),
            (0.86, 0.34), (0.82, 0.49), (0.80, 0.66), (0.75, 0.82),
        )
        for index, (leg_x, leg_y) in enumerate(leg_centers):
            alternating = math.sin(phase + (index % 4) * 0.78 + (0 if index < 4 else math.pi))
            leg = gaussian(u, v, leg_x, leg_y, 0.18, 0.18)
            dy -= max(0.0, alternating) * 0.028 * leg
            dx += alternating * (0.016 if leg_x > 0.5 else -0.016) * leg
        head = gaussian(u, v, 0.50, 0.58, 0.25, 0.22)
        dy += sine * 0.006 * head

    elif job.motion == "flight":
        # Wing tips travel farther than their roots, while the body only pitches
        # slightly. Tail motion lags behind to avoid a rigid paper-cutout flap.
        left_wing = gaussian(u, v, 0.28, 0.24, 0.34, 0.30)
        right_wing = gaussian(u, v, 0.78, 0.22, 0.28, 0.28)
        dy += -sine * ((0.066 * left_wing) + (0.056 * right_wing))
        dx += sine * ((0.020 * left_wing) - (0.016 * right_wing))
        rotate((0.63, 0.55), (0.70, 0.55), (0.30, 0.28), sine * 0.026)
        tail = gaussian(u, v, 0.28, 0.68, 0.36, 0.26)
        dx += math.sin(phase - 0.75 + u * 4.2) * 0.032 * tail
        dy += math.cos(phase - 0.55 + u * 3.0) * 0.016 * tail
        head = gaussian(u, v, 0.78, 0.48, 0.20, 0.22)
        dy += math.sin(phase + 0.45) * 0.014 * head

    elif job.motion == "specter":
        dy += sine * 0.012
        rotate((0.50, 0.31), (0.50, 0.17), (0.26, 0.24), sine * 0.024)
        rotate((0.38, 0.39), (0.22, 0.55), (0.27, 0.34), sine * 0.075)
        rotate((0.62, 0.39), (0.78, 0.55), (0.27, 0.34), -sine * 0.075)
        lower = max(0.0, min(1.0, (v - 0.45) / 0.52))
        dx += math.sin(phase + u * 5.0 + v * 3.5) * lower * 0.028
        dy += math.cos(phase + u * 3.0) * lower * 0.013

    elif job.motion == "final":
        dy += sine * 0.008
        rotate((0.50, 0.28), (0.50, 0.13), (0.25, 0.22), sine * 0.022)
        rotate((0.39, 0.36), (0.21, 0.51), (0.26, 0.35), math.sin(phase + 0.35) * 0.065)
        rotate((0.61, 0.35), (0.82, 0.47), (0.27, 0.34), -math.sin(phase + 0.10) * 0.082)
        torso = gaussian(u, v, 0.50, 0.43, 0.31, 0.26)
        dx += sine * 0.010 * torso
        lower = max(0.0, min(1.0, (v - 0.45) / 0.52))
        dx += math.sin(phase + u * 5.8 + v * 4.5) * lower * lower * 0.026
        dy += math.cos(phase + u * 2.8) * lower * 0.008

    return dx, dy


def premultiply_alpha(image: Image.Image) -> Image.Image:
    rgba = np.array(image.convert("RGBA"), dtype=np.uint16)
    alpha = rgba[:, :, 3:4]
    rgba[:, :, :3] = (rgba[:, :, :3] * alpha + 127) // 255
    return Image.fromarray(rgba.astype(np.uint8), "RGBA")


def restore_hard_alpha(image: Image.Image, threshold: int = 96) -> Image.Image:
    rgba = np.array(image.convert("RGBA"), dtype=np.uint16)
    alpha = rgba[:, :, 3]
    visible = alpha >= threshold
    for channel in range(3):
        restored = np.zeros_like(alpha)
        restored[visible] = np.minimum(255, (rgba[:, :, channel][visible] * 255 + alpha[visible] // 2) // alpha[visible])
        rgba[:, :, channel] = restored
    rgba[:, :, 3] = np.where(visible, 255, 0)
    return Image.fromarray(rgba.astype(np.uint8), "RGBA")


def make_motion_frame(base: Image.Image, job: BossArtJob, frame_index: int) -> Image.Image:
    phase = frame_index / job.frame_count * math.tau
    bbox = base.getbbox()
    if bbox is None:
        return base.copy()
    left, top, right, bottom = bbox
    subject_width = max(1, right - left)
    subject_height = max(1, bottom - top)
    grid_columns = 22 if job.motion in {"flight", "crawl", "serpent", "human"} else 18
    grid_rows = 24 if job.motion == "human" else 20 if job.motion in {"hover", "stalk", "specter", "final"} else 18
    x_edges = [round(index * base.width / grid_columns) for index in range(grid_columns + 1)]
    y_edges = [round(index * base.height / grid_rows) for index in range(grid_rows + 1)]

    def source_point(x: int, y: int) -> tuple[float, float]:
        u = (x - left) / subject_width
        v = (y - top) / subject_height
        delta_u, delta_v = rig_displacement(job, u, v, phase)
        source_x = max(0.0, min(base.width - 1.0, x - delta_u * subject_width))
        source_y = max(0.0, min(base.height - 1.0, y - delta_v * subject_height))
        return source_x, source_y

    mesh: list[tuple[tuple[int, int, int, int], tuple[float, ...]]] = []
    for row in range(grid_rows):
        y0 = y_edges[row]
        y1 = y_edges[row + 1]
        for column in range(grid_columns):
            x0 = x_edges[column]
            x1 = x_edges[column + 1]
            northwest = source_point(x0, y0)
            southwest = source_point(x0, y1)
            southeast = source_point(x1, y1)
            northeast = source_point(x1, y0)
            quad = northwest + southwest + southeast + northeast
            mesh.append(((x0, y0, x1, y1), quad))

    premultiplied = premultiply_alpha(base)
    warped = premultiplied.transform(
        base.size,
        Image.Transform.MESH,
        mesh,
        resample=Image.Resampling.BILINEAR,
    )
    return remove_visible_background_residue(restore_hard_alpha(warped), job)


def harden_alpha(image: Image.Image) -> Image.Image:
    if image.mode != "RGBA":
        image = image.convert("RGBA")
    rgba = np.array(image, dtype=np.uint8)
    rgba[:, :, 3] = np.where(rgba[:, :, 3] > 0, 255, 0).astype(np.uint8)
    return Image.fromarray(rgba, "RGBA")


def serpent_path(width: int, height: int, frame_index: int, frame_count: int) -> list[tuple[int, int]]:
    phase = frame_index / frame_count * math.tau
    points: list[tuple[int, int]] = []
    for step in range(96):
        t = step / 95
        x = round(width * (0.055 + 0.755 * t))
        primary = math.sin(t * math.tau * 1.75 + phase)
        secondary = math.sin(t * math.tau * 3.5 + phase * 0.55) * 0.35
        y = round(height * (0.52 + 0.18 * (primary + secondary)))
        points.append((x, y))
    return points


def draw_basilisk_scales(
    draw: ImageDraw.ImageDraw,
    points: list[tuple[int, int]],
    width: int,
    height: int,
    frame_index: int,
    frame_count: int,
) -> None:
    phase = frame_index / frame_count * math.tau
    scale_r = max(8, height // 54)
    for index in range(7, len(points) - 7, 4):
        x, y = points[index]
        prev_x, prev_y = points[index - 2]
        next_x, next_y = points[index + 2]
        dx = next_x - prev_x
        dy = next_y - prev_y
        length = max(1.0, math.hypot(dx, dy))
        nx = -dy / length
        ny = dx / length
        offset = math.sin(index * 0.55 + phase) * height * 0.038
        sx = round(x + nx * offset)
        sy = round(y + ny * offset)
        color = (31, 73, 70, 255) if index % 8 else (92, 132, 112, 255)
        draw.ellipse((sx - scale_r, sy - scale_r // 2, sx + scale_r, sy + scale_r // 2), fill=color)
        if index % 12 == 0:
            hx = round(x - nx * height * 0.06)
            hy = round(y - ny * height * 0.06)
            draw.ellipse((hx - scale_r, hy - scale_r // 3, hx + scale_r, hy + scale_r // 3), fill=(134, 170, 143, 255))


def draw_basilisk_head(draw: ImageDraw.ImageDraw, end_x: int, end_y: int, height: int, mouth_open: float) -> None:
    head_w = max(320, int(height * 0.58))
    head_h = max(210, int(height * 0.35))
    cx = end_x + int(head_w * 0.20)
    cy = end_y - int(head_h * 0.03)
    x0 = cx - head_w // 2
    y0 = cy - head_h // 2
    x1 = cx + head_w // 2
    y1 = cy + head_h // 2
    snout = [
        (x1 - head_w // 5, cy - head_h // 3),
        (x1 + head_w // 4, cy - head_h // 7),
        (x1 + head_w // 5, cy + head_h // 5),
        (x1 - head_w // 6, cy + head_h // 3),
    ]

    draw.ellipse((x0 - 12, y0 - 12, x1 + 12, y1 + 12), fill=(6, 18, 20, 255))
    draw.polygon([(p[0] + 12, p[1]) for p in snout], fill=(6, 18, 20, 255))
    draw.ellipse((x0, y0, x1, y1), fill=(22, 55, 58, 255))
    draw.polygon(snout, fill=(24, 60, 61, 255))
    draw.ellipse((x0 + head_w // 8, y0 + head_h // 8, x1 - head_w // 8, y0 + head_h // 2), fill=(42, 88, 83, 255))

    crest_y = y0 - head_h // 8
    for offset in (-head_w // 10, 0, head_w // 10):
        draw.polygon(
            [
                (cx + offset, crest_y),
                (cx + offset + head_w // 16, y0 + head_h // 9),
                (cx + offset - head_w // 16, y0 + head_h // 9),
            ],
            fill=(154, 166, 138, 255),
        )

    eye_r = max(18, head_h // 12)
    for ex in (cx + head_w // 8, cx + head_w // 3):
        ey = cy - head_h // 7
        draw.ellipse((ex - eye_r, ey - eye_r, ex + eye_r, ey + eye_r), fill=(15, 15, 10, 255))
        draw.ellipse((ex - eye_r + 5, ey - eye_r + 5, ex + eye_r - 5, ey + eye_r - 5), fill=(226, 178, 45, 255))
        draw.rectangle((ex - 3, ey - eye_r + 7, ex + 3, ey + eye_r - 7), fill=(20, 16, 8, 255))

    mouth = [
        (cx + head_w // 10, cy + head_h // 10),
        (x1 + head_w // 5, cy + head_h // 7),
        (x1 - head_w // 14, cy + head_h // 3 + round(head_h * 0.12 * mouth_open)),
        (cx + head_w // 7, cy + head_h // 4 + round(head_h * 0.08 * mouth_open)),
    ]
    draw.polygon(mouth, fill=(44, 20, 28, 255))
    fang = (238, 232, 194, 255)
    for fx in (cx + head_w // 4, x1):
        draw.polygon([(fx, cy + head_h // 8), (fx + head_w // 28, cy + head_h // 8), (fx + head_w // 50, cy + head_h // 2)], fill=fang)
    tongue_x = x1 + head_w // 8
    tongue_y = cy + head_h // 5
    draw.line([(tongue_x, tongue_y), (tongue_x + head_w // 9, tongue_y + head_h // 5)], fill=(151, 207, 154, 255), width=max(6, height // 120))
    draw.line([(tongue_x + head_w // 9, tongue_y + head_h // 5), (tongue_x + head_w // 7, tongue_y + head_h // 4)], fill=(151, 207, 154, 255), width=max(4, height // 150))
    draw.line([(tongue_x + head_w // 9, tongue_y + head_h // 5), (tongue_x + head_w // 11, tongue_y + head_h // 3)], fill=(151, 207, 154, 255), width=max(4, height // 150))


def make_basilisk_frame(subject: Image.Image, job: BossArtJob, frame_index: int) -> Image.Image:
    width, height = subject.size
    frame = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(frame, "RGBA")
    phase = frame_index / job.frame_count * math.tau
    points = serpent_path(width, height, frame_index, job.frame_count)

    outline_width = max(76, height // 4)
    dark_width = max(60, int(outline_width * 0.82))
    mid_width = max(44, int(outline_width * 0.62))
    belly_width = max(24, int(outline_width * 0.28))
    highlight_width = max(10, int(outline_width * 0.10))

    draw.line(points, fill=(8, 22, 24, 255), width=outline_width)
    draw.line(points, fill=(18, 48, 49, 255), width=dark_width)
    draw.line(points, fill=(35, 82, 75, 255), width=mid_width)

    belly_points = [(x, y + height // 13) for x, y in points[7:]]
    draw.line(belly_points, fill=(151, 174, 153, 255), width=belly_width)
    draw.line([(x, y - height // 11) for x, y in points[8:-6]], fill=(108, 151, 128, 255), width=highlight_width)
    draw_basilisk_scales(draw, points, width, height, frame_index, job.frame_count)

    tail = points[0]
    tail_tip = (max(0, tail[0] - width // 18), tail[1] + round(math.sin(phase) * height * 0.04))
    draw.polygon(
        [
            tail_tip,
            (tail[0] + width // 28, tail[1] - outline_width // 3),
            (tail[0] + width // 24, tail[1] + outline_width // 3),
        ],
        fill=(8, 22, 24, 255),
    )
    draw.polygon(
        [
            (tail_tip[0] + width // 80, tail_tip[1]),
            (tail[0] + width // 34, tail[1] - dark_width // 4),
            (tail[0] + width // 34, tail[1] + dark_width // 4),
        ],
        fill=(28, 63, 58, 255),
    )

    end_x, end_y = points[-1]
    draw_basilisk_head(draw, end_x, end_y, height, (1.0 - math.cos(phase)) * 0.5)

    return harden_alpha(frame)


def make_basilisk_sheet(subject: Image.Image, job: BossArtJob) -> Image.Image:
    frames = [make_basilisk_frame(subject, job, index) for index in range(job.frame_count)]
    return make_atlas(frames, job)


def make_sheet(subject: Image.Image, job: BossArtJob) -> Image.Image:
    if job.motion == "serpent":
        return make_basilisk_sheet(subject, job)
    frames = [make_motion_frame(subject, job, index) for index in range(job.frame_count)]
    return make_atlas(frames, job)


def make_atlas(frames: list[Image.Image], job: BossArtJob) -> Image.Image:
    if not frames:
        raise ValueError(f"{job.texture} produced no animation frames")
    columns = job.atlas_columns
    rows = math.ceil(len(frames) / columns)
    frame_width, frame_height = frames[0].size
    atlas = Image.new("RGBA", (frame_width * columns, frame_height * rows), (0, 0, 0, 0))
    for index, frame in enumerate(frames):
        x = (index % columns) * frame_width
        y = (index // columns) * frame_height
        atlas.alpha_composite(frame, (x, y))
    return harden_alpha(atlas)


def extract_atlas_frame(atlas: Image.Image, job: BossArtJob, frame_index: int) -> Image.Image:
    columns = job.atlas_columns
    rows = math.ceil(job.frame_count / columns)
    frame_width = atlas.width // columns
    frame_height = atlas.height // rows
    index = frame_index % job.frame_count
    x = (index % columns) * frame_width
    y = (index // columns) * frame_height
    return atlas.crop((x, y, x + frame_width, y + frame_height))


def make_preview_entry(atlas: Image.Image, job: BossArtJob) -> BossPreviewEntry:
    source = extract_atlas_frame(atlas, job, 0)
    source_size = source.size
    previews: list[Image.Image] = []
    for frame_index in range(job.frame_count):
        frame = extract_atlas_frame(atlas, job, frame_index)
        scale = min(260 / frame.width, 220 / frame.height)
        previews.append(
            frame.resize(
                (max(1, round(frame.width * scale)), max(1, round(frame.height * scale))),
                Image.Resampling.LANCZOS,
            )
        )
    return BossPreviewEntry(job, tuple(previews), source_size)


def make_head_icon(subject: Image.Image, job: BossArtJob) -> Image.Image:
    bbox = subject.getbbox()
    if bbox is None:
        raise ValueError(f"{job.texture} has no subject bbox")
    left, top, right, bottom = bbox
    width = right - left
    height = bottom - top
    cx = left + width * job.head_focus[0]
    cy = top + height * job.head_focus[1]
    crop_size = max(width, height) * (0.30 if width < height else 0.24)
    if job.motion in {"serpent", "flight"}:
        crop_size = max(width, height) * 0.22
    if job.motion in {"specter", "final"}:
        crop_size = max(width, height) * 0.24
    crop_size = max(64, crop_size)
    box = (
        round(max(0, cx - crop_size / 2)),
        round(max(0, cy - crop_size / 2)),
        round(min(subject.width, cx + crop_size / 2)),
        round(min(subject.height, cy + crop_size / 2)),
    )
    cropped = subject.crop(box)
    icon = Image.new("RGBA", (40, 40), (0, 0, 0, 0))
    scale = min(34 / cropped.width, 34 / cropped.height)
    size = (max(1, round(cropped.width * scale)), max(1, round(cropped.height * scale)))
    resized = cropped.resize(size, Image.Resampling.LANCZOS)
    icon.alpha_composite(resized, ((40 - size[0]) // 2, (40 - size[1]) // 2))
    return icon


def checker(size: tuple[int, int], cell: int = 8) -> Image.Image:
    image = Image.new("RGBA", size, (42, 39, 48, 255))
    draw = ImageDraw.Draw(image)
    for y in range(0, size[1], cell):
        for x in range(0, size[0], cell):
            if (x // cell + y // cell) % 2 == 0:
                draw.rectangle((x, y, x + cell - 1, y + cell - 1), fill=(58, 54, 64, 255))
    return image


def save_preview_sheet(frames: Iterable[BossPreviewEntry]) -> None:
    entries = list(frames)
    preview_w = 220
    preview_h = 220
    cell_w = preview_w + 24
    cell_h = preview_h + 44
    cols = 4
    rows = math.ceil(len(entries) / cols)
    sheet = Image.new("RGBA", (cell_w * cols, cell_h * rows), (18, 17, 21, 255))
    draw = ImageDraw.Draw(sheet)
    for index, entry in enumerate(entries):
        job = entry.job
        frame = entry.frames[0]
        scale = min(preview_w / frame.width, preview_h / frame.height)
        frame = frame.resize((max(1, round(frame.width * scale)), max(1, round(frame.height * scale))), Image.Resampling.LANCZOS)
        x0 = (index % cols) * cell_w
        y0 = (index // cols) * cell_h
        bg = checker(frame.size)
        bg.alpha_composite(frame)
        sheet.alpha_composite(bg, (x0 + (cell_w - frame.width) // 2, y0 + 8))
        draw.text(
            (x0 + 8, y0 + cell_h - 28),
            f"{job.texture} {entry.source_frame_size[0]}x{entry.source_frame_size[1]} / {job.frame_count}f",
            fill=(235, 235, 240, 255),
        )
    PREVIEW_DIR.mkdir(parents=True, exist_ok=True)
    sheet.save(PREVIEW_DIR / "boss_sprite_sheet_preview_v4.png")


def render_preview_frame(entries: list[BossPreviewEntry], tick: int, total_ticks: int) -> Image.Image:
    cell_width = 280
    cell_height = 250
    columns = 4
    rows = math.ceil(len(entries) / columns)
    canvas = Image.new("RGBA", (cell_width * columns, cell_height * rows), (18, 17, 21, 255))
    draw = ImageDraw.Draw(canvas)
    for entry_index, entry in enumerate(entries):
        job = entry.job
        animation_index = (tick * job.frame_count // total_ticks) % job.frame_count
        frame = entry.frames[animation_index]
        available_width = cell_width - 24
        available_height = cell_height - 42
        scale = min(available_width / frame.width, available_height / frame.height)
        resized = frame.resize(
            (max(1, round(frame.width * scale)), max(1, round(frame.height * scale))),
            Image.Resampling.LANCZOS,
        )
        x0 = (entry_index % columns) * cell_width
        y0 = (entry_index // columns) * cell_height
        background = checker((available_width, available_height), cell=12)
        background.alpha_composite(
            resized,
            ((available_width - resized.width) // 2, (available_height - resized.height) // 2),
        )
        canvas.alpha_composite(background, (x0 + 12, y0 + 8))
        draw.text((x0 + 12, y0 + cell_height - 26), job.texture, fill=(238, 238, 244, 255))
    return canvas.convert("RGB")


def save_animation_preview(entries: list[BossPreviewEntry]) -> None:
    total_ticks = 12
    frames = [render_preview_frame(entries, tick, total_ticks) for tick in range(total_ticks)]
    frames[0].save(
        PREVIEW_DIR / "boss_animation_preview_v4.gif",
        save_all=True,
        append_images=frames[1:],
        duration=80,
        loop=0,
        disposal=2,
        optimize=False,
    )


def save_key_review_sheet(entries: list[BossPreviewEntry]) -> None:
    review_names = {"Basilisk", "Umbridge", "Voldemort", "Horntail", "Aragog", "Bellatrix", "BartyCrouch"}
    selected = [entry for entry in entries if entry.job.folder in review_names]
    if not selected:
        return
    samples = 6
    cell_width = 240
    cell_height = 215
    label_width = 120
    canvas = Image.new("RGBA", (label_width + cell_width * samples, cell_height * len(selected)), (18, 17, 21, 255))
    draw = ImageDraw.Draw(canvas)
    for row, entry in enumerate(selected):
        job = entry.job
        draw.text((8, row * cell_height + 12), f"{job.folder}\n{job.frame_count} frames", fill=(238, 238, 244, 255))
        for sample in range(samples):
            frame_index = sample * job.frame_count // samples
            frame = entry.frames[frame_index]
            available = (cell_width - 12, cell_height - 12)
            scale = min(available[0] / frame.width, available[1] / frame.height)
            resized = frame.resize(
                (max(1, round(frame.width * scale)), max(1, round(frame.height * scale))),
                Image.Resampling.LANCZOS,
            )
            background = checker(available, cell=10)
            background.alpha_composite(
                resized,
                ((available[0] - resized.width) // 2, (available[1] - resized.height) // 2),
            )
            canvas.alpha_composite(background, (label_width + sample * cell_width + 6, row * cell_height + 6))
    canvas.save(PREVIEW_DIR / "boss_animation_key_review_v4.png")


def main() -> None:
    parser = argparse.ArgumentParser(description="Build rigged Wizarding World boss atlases from the approved source paintings.")
    parser.add_argument("--boss", action="append", help="Generate only the named folder or texture (repeatable).")
    parser.add_argument("--skip-previews", action="store_true", help="Skip contact sheet and animated preview output.")
    arguments = parser.parse_args()
    selected_names = {name.casefold() for name in arguments.boss or []}
    jobs = [
        job for job in JOBS
        if not selected_names or job.folder.casefold() in selected_names or job.texture.casefold() in selected_names
    ]
    if not jobs:
        raise ValueError(f"No boss matched: {sorted(selected_names)}")

    generated: list[BossPreviewEntry] = []
    for job in jobs:
        source_path = SOURCE_DIR / job.source
        if not source_path.exists():
            raise FileNotFoundError(source_path)

        master_subject = extract_subject(source_path, job)
        runtime_subject = make_runtime_subject(master_subject, job)
        sheet = make_sheet(runtime_subject, job)
        head = make_head_icon(master_subject, job)

        dest_dir = BOSS_DIR / job.folder
        dest_dir.mkdir(parents=True, exist_ok=True)
        sheet.save(dest_dir / f"{job.texture}.png")
        head.save(dest_dir / f"{job.texture}_Head_Boss.png")
        if not arguments.skip_previews:
            generated.append(make_preview_entry(sheet, job))
        frame = extract_atlas_frame(sheet, job, 0)
        print(
            f"{job.texture}: atlas {sheet.width}x{sheet.height}, "
            f"master {master_subject.width}x{master_subject.height}, "
            f"runtime-frame {frame.width}x{frame.height}, {job.frame_count} frames, "
            f"grid {job.atlas_columns}x{math.ceil(job.frame_count / job.atlas_columns)}, "
            f"display-ref {job.frame_size[0]}x{job.frame_size[1]}, npc {job.npc_size[0]}x{job.npc_size[1]}"
        )

    if not arguments.skip_previews:
        save_preview_sheet(generated)
        save_animation_preview(generated)
        save_key_review_sheet(generated)
        print(f"preview: {PREVIEW_DIR / 'boss_sprite_sheet_preview_v4.png'}")
        print(f"animation: {PREVIEW_DIR / 'boss_animation_preview_v4.gif'}")
        print(f"key review: {PREVIEW_DIR / 'boss_animation_key_review_v4.png'}")


if __name__ == "__main__":
    main()
