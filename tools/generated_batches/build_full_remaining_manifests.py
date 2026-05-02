#!/usr/bin/env python3
from __future__ import annotations

import json
import math
import re
import struct
from pathlib import Path


ROOT = Path(__file__).resolve().parent
MOD_ROOT = ROOT.parent.parent
CONTENT_ROOT = MOD_ROOT / "Content"
CHUNK_COUNT = 6

NPC_FRAME_RE = re.compile(r"Main\.npcFrameCount\[Type\]\s*=\s*(\d+)\s*;")
PROJ_FRAME_RE = re.compile(r"Main\.projFrames\[Type\]\s*=\s*(\d+)\s*;")


def read_png_size(path: Path) -> tuple[int, int]:
    with path.open("rb") as f:
        if f.read(8) != b"\x89PNG\r\n\x1a\n":
            raise ValueError(f"Not a PNG: {path}")
        length = struct.unpack(">I", f.read(4))[0]
        if f.read(4) != b"IHDR" or length < 8:
            raise ValueError(f"Invalid PNG header: {path}")
        return struct.unpack(">II", f.read(8))


def split_words(name: str) -> str:
    name = name.replace("_", " ")
    name = re.sub(r"([a-z0-9])([A-Z])", r"\1 \2", name)
    return re.sub(r"\s+", " ", name).strip()


def load_covered() -> set[Path]:
    manifest = ROOT / "deduped_manifest.json"
    rows = json.loads(manifest.read_text(encoding="utf-8"))
    covered: set[Path] = set()
    for row in rows:
        abs_output = row.get("_abs_output")
        if abs_output:
            covered.add(Path(abs_output).resolve())
        else:
            covered.add((MOD_ROOT / row["OutputPath"]).resolve())
    return covered


def scan_frame_counts() -> tuple[dict[str, int], dict[str, int]]:
    npc_counts: dict[str, int] = {}
    proj_counts: dict[str, int] = {}

    for cs in (CONTENT_ROOT / "NPCs").rglob("*.cs"):
        text = cs.read_text(encoding="utf-8", errors="ignore")
        match = NPC_FRAME_RE.search(text)
        if match:
            npc_counts[cs.stem] = int(match.group(1))

    for cs in (CONTENT_ROOT / "Projectiles").rglob("*.cs"):
        text = cs.read_text(encoding="utf-8", errors="ignore")
        match = PROJ_FRAME_RE.search(text)
        if match:
            proj_counts[cs.stem] = int(match.group(1))

    npc_counts.setdefault("Inferius", 15)
    return npc_counts, proj_counts


def base_name(stem: str) -> str:
    for suffix in ("_Head_Boss", "_Head", "_Body", "_Legs", "Projectile", "Buff", "Item", "Boss"):
        if stem.endswith(suffix):
            return stem[: -len(suffix)]
    return stem


def category_for(path: Path) -> str:
    rel = path.relative_to(CONTENT_ROOT)
    parts = rel.parts
    if parts[:2] == ("Items", "Accessories"):
        return "accessory"
    if parts[:2] == ("Items", "Consumables"):
        return "consumable"
    if parts[:2] == ("Items", "Weapons"):
        return "weapon"
    if parts[:2] == ("Items", "Armor"):
        return "armor"
    if parts[:2] == ("Items", "Placeable"):
        return "placeable"
    if parts[:2] == ("NPCs", "Town"):
        return "town_npc"
    if parts[:2] == ("NPCs", "Enemies"):
        return "enemy"
    if parts[:2] == ("NPCs", "Bosses"):
        return "boss"
    if parts[:2] == ("Projectiles", "Spells"):
        return "spell"
    if parts[:2] == ("Tiles", "Landmarks"):
        return "landmark"
    if parts[:1] == ("Buffs",):
        return "buff"
    if parts[:1] == ("Biomes",):
        return "biome"
    return "misc"


def infer_frame_count(path: Path, width: int, height: int, npc_counts: dict[str, int], proj_counts: dict[str, int]) -> int:
    category = category_for(path)
    stem = path.stem

    if category == "town_npc" and not stem.endswith("_Head"):
        return npc_counts.get(stem, 25)
    if category == "boss" and not stem.endswith("_Head_Boss"):
        return npc_counts.get(stem, 6)
    if category == "enemy":
        if stem in npc_counts:
            return npc_counts[stem]
        for candidate in (15, 8, 6, 4, 3, 2, 1):
            if height % candidate == 0:
                frame_height = height // candidate
                if 12 <= frame_height <= 160:
                    return candidate
        return 1
    if category == "spell":
        return proj_counts.get(stem, 1)
    return 1


def describe_subject(path: Path) -> str:
    category = category_for(path)
    stem = path.stem
    base = split_words(base_name(stem))
    rel = path.relative_to(CONTENT_ROOT)

    if category == "accessory":
        return f"a magical accessory themed around {base}, wizarding wearable trinket, readable silhouette"
    if category == "consumable":
        return f"a consumable magical item themed around {base}, wizarding utility object, readable silhouette"
    if category == "weapon":
        return f"a wizarding weapon themed around {base}, combat-ready magical gear, readable silhouette"
    if category == "placeable":
        return f"a placeable wizarding furnishing or structure item themed around {base}, readable silhouette"
    if category == "landmark":
        return f"a wizarding landmark or magical structure sprite themed around {base}, readable silhouette"
    if category == "buff":
        return f"a buff icon emblem themed around {base}, clean magical status symbol, readable at small size"
    if category == "biome":
        return f"a biome icon themed around {base}, atmospheric but readable at small size"
    if category == "spell":
        return f"a magical spell projectile for {base}, focused energy bolt or effect, readable silhouette"
    if category == "town_npc":
        if stem.endswith("_Head"):
            return f"a portrait head icon of {base}, friendly wizarding town NPC"
        return f"a single full-body sprite frame of {base}, friendly wizarding town NPC, animation-ready pose"
    if category == "boss":
        if stem.endswith("_Head_Boss"):
            return f"a boss head icon of {base}, intimidating wizarding villain portrait"
        return f"a single full-body sprite frame of {base}, major wizarding boss, animation-ready pose"
    if category == "enemy":
        return f"a single full-body sprite frame of {base}, hostile magical creature or encounter object, animation-ready pose"
    if category == "armor":
        theme = split_words(rel.parts[2]) if len(rel.parts) > 3 else ""
        if stem.endswith("_Head"):
            return f"an armor head layer sprite for {theme} {base}, clean equipment silhouette"
        if stem.endswith("_Body"):
            return f"an armor body layer sprite for {theme} {base}, clean equipment silhouette"
        if stem.endswith("_Legs"):
            return f"an armor leg layer sprite for {theme} {base}, clean equipment silhouette"
        return f"an armor item icon for {theme} {base}, wizarding apparel, readable silhouette"
    return f"a wizarding game sprite themed around {base}, readable silhouette"


def process_kind(path: Path, frame_count: int) -> str:
    stem = path.stem
    category = category_for(path)
    if category in {"buff", "biome"}:
        return "icon"
    if stem.endswith("_Head") or stem.endswith("_Head_Boss"):
        return "icon"
    if category in {"town_npc", "boss"} and frame_count > 1 and not stem.endswith("_Head") and not stem.endswith("_Head_Boss"):
        return "sheet"
    if category == "enemy" and frame_count > 1:
        return "sheet"
    if category == "spell" and frame_count > 1:
        return "sheet"
    return "fit"


def make_prompt(path: Path, kind: str) -> str:
    subject = describe_subject(path)
    if kind == "icon":
        prefix = "Terraria-style pixel art icon, centered emblem,"
    elif kind == "sheet":
        prefix = "Terraria-style pixel art sprite frame, single isolated figure,"
    else:
        prefix = "Terraria-style pixel art sprite, centered isolated object,"
    return (
        f"{prefix} {subject}. Put it on a completely flat pure bright green background (#00FF00), "
        "no transparency, no checkerboard, no texture, no shadow, no border, no text, no extra objects."
    )


def make_job(path: Path, npc_counts: dict[str, int], proj_counts: dict[str, int]) -> dict[str, object]:
    width, height = read_png_size(path)
    frame_count = infer_frame_count(path, width, height, npc_counts, proj_counts)
    kind = process_kind(path, frame_count)
    rel = path.relative_to(MOD_ROOT).as_posix()
    return {
        "Name": path.stem,
        "Width": width,
        "Height": height,
        "FrameCount": frame_count,
        "ProcessKind": kind,
        "OutputPath": rel,
        "_abs_output": str(path.resolve()),
        "Prompt": make_prompt(path, kind),
    }


def write_json(path: Path, rows: list[dict[str, object]]) -> None:
    path.write_text(json.dumps(rows, indent=2) + "\n", encoding="utf-8")


def main() -> None:
    covered = load_covered()
    npc_counts, proj_counts = scan_frame_counts()
    remaining = sorted(
        p.resolve()
        for p in CONTENT_ROOT.rglob("*.png")
        if p.resolve() not in covered
    )
    jobs = [make_job(path, npc_counts, proj_counts) for path in remaining]
    write_json(ROOT / "remaining_manifest_all.json", jobs)

    chunk_size = math.ceil(len(jobs) / CHUNK_COUNT)
    for index in range(CHUNK_COUNT):
        start = index * chunk_size
        chunk = jobs[start : start + chunk_size]
        if not chunk:
            break
        write_json(ROOT / f"remaining_chunk_{index + 1:02d}.json", chunk)
        print(f"remaining_chunk_{index + 1:02d}.json {len(chunk)}")
    print(f"remaining_manifest_all.json {len(jobs)}")


if __name__ == "__main__":
    main()
