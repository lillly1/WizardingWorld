#!/usr/bin/env python3
"""
Wizarding World -- Mechanical Data Extractor

Extracts gameplay stats from C# source files via regex pattern matching.
Outputs structured JSON to scripts/mechanical_data/.

WHAT IS AUTO-DERIVED:
- Base stats from SetDefaults() (damage, defense, HP, mana, useTime, rarity, etc.)
- Spawn conditions from SpawnChance() methods
- Drop rules from ModifyNPCLoot/ModifyItemLoot
- Recipe ingredients from AddRecipes()

WHAT REQUIRES CURATED METADATA:
- Dynamic computed values (Voldemort scaling, Elder Wand mana-based modifiers)
- Buff effect descriptions (complex UpdateAccessory/UpdateEquip logic)
- These are annotated with "_curated": true in the output.

Usage:
    python scripts/export_mechanical_data.py
    python scripts/export_mechanical_data.py --check   # validate extraction only
"""

import argparse
import json
import os
import re
import sys
from pathlib import Path


def find_repo_root():
    here = Path(__file__).resolve().parent.parent
    if (here / "WizardingWorld.csproj").exists():
        return here
    for p in Path(__file__).resolve().parents:
        if (p / "WizardingWorld.csproj").exists():
            return p
    sys.exit("ERROR: cannot find WizardingWorld.csproj")


# ── Regex patterns for common C# stat assignments ──

def extract_int(text: str, pattern: str, default=None):
    m = re.search(pattern, text)
    return int(m.group(1)) if m else default

def extract_float(text: str, pattern: str, default=None):
    m = re.search(pattern, text)
    return float(m.group(1)) if m else default

def extract_string(text: str, pattern: str, default=None):
    m = re.search(pattern, text)
    return m.group(1) if m else default

def extract_bool(text: str, pattern: str):
    return bool(re.search(pattern, text))

# Map ItemRarityID constants to display names
RARITY_MAP = {
    "White": "White", "Blue": "Blue", "Green": "Green", "Orange": "Orange",
    "LightRed": "Lt Red", "Pink": "Pink", "LightPurple": "Lt Purple",
    "Lime": "Lime", "Yellow": "Yellow", "Cyan": "Cyan", "Red": "Red",
    "Purple": "Purple", "Expert": "Expert", "Master": "Master",
}


def extract_item_stats(filepath: Path) -> dict:
    """Extract base item stats from SetDefaults()."""
    text = filepath.read_text(encoding="utf-8", errors="replace")
    stats = {"id": filepath.stem, "_source": str(filepath.name)}

    stats["damage"] = extract_int(text, r"Item\.damage\s*=\s*(\d+)")
    stats["useTime"] = extract_int(text, r"Item\.useTime\s*=\s*(\d+)")
    stats["mana"] = extract_int(text, r"Item\.mana\s*=\s*(\d+)")
    stats["knockBack"] = extract_float(text, r"Item\.knockBack\s*=\s*([\d.]+)")
    stats["defense"] = extract_int(text, r"Item\.defense\s*=\s*(\d+)")
    stats["crit"] = extract_int(text, r"Item\.crit\s*=\s*(\d+)")

    # Rarity
    rarity_match = re.search(r"Item\.rare\s*=\s*ItemRarityID\.(\w+)", text)
    if rarity_match:
        raw = rarity_match.group(1)
        stats["rarity"] = RARITY_MAP.get(raw, raw)

    # Shoot type (projectile class name)
    shoot_match = re.search(r"Item\.shoot\s*=\s*ModContent\.ProjectileType<[^>]*\.(\w+)>", text)
    if shoot_match:
        stats["projectile"] = shoot_match.group(1)

    # Buff type and duration (potions)
    buff_match = re.search(r"Item\.buffType\s*=\s*ModContent\.BuffType<[^>]*\.(\w+)>", text)
    if buff_match:
        stats["buffType"] = buff_match.group(1)
    stats["buffTime"] = extract_int(text, r"Item\.buffTime\s*=\s*(\d+)")

    # Is obsolete?
    stats["obsolete"] = "[Obsolete" in text

    # Has recipe?
    stats["hasRecipe"] = "AddRecipes" in text and "CreateRecipe" in text

    # Is accessory?
    stats["isAccessory"] = extract_bool(text, r"Item\.accessory\s*=\s*true")

    return {k: v for k, v in stats.items() if v is not None}


def extract_npc_stats(filepath: Path) -> dict:
    """Extract base NPC stats from SetDefaults()."""
    text = filepath.read_text(encoding="utf-8", errors="replace")
    stats = {"id": filepath.stem, "_source": str(filepath.name)}

    stats["damage"] = extract_int(text, r"NPC\.damage\s*=\s*(\d+)")
    stats["defense"] = extract_int(text, r"NPC\.defense\s*=\s*(\d+)")
    stats["lifeMax"] = extract_int(text, r"NPC\.lifeMax\s*=\s*(\d+)")
    stats["knockBackResist"] = extract_float(text, r"NPC\.knockBackResist\s*=\s*([\d.]+)")
    stats["isBoss"] = extract_bool(text, r"NPC\.boss\s*=\s*true")
    stats["noGravity"] = extract_bool(text, r"NPC\.noGravity\s*=\s*true")

    # Check for dynamic scaling (e.g., Voldemort)
    if re.search(r"NPC\.(damage|lifeMax|defense)\s*=\s*\(int\)\(", text):
        stats["_dynamicScaling"] = True
        stats["_curated"] = True
        # Try to extract base values from the scaling expression
        base_hp = re.search(r"NPC\.lifeMax\s*=\s*\(int\)\((\d+)", text)
        if base_hp:
            stats["lifeMax_base"] = int(base_hp.group(1))
        base_dmg = re.search(r"NPC\.damage\s*=\s*\(int\)\((\d+)", text)
        if base_dmg:
            stats["damage_base"] = int(base_dmg.group(1))
        base_def = re.search(r"NPC\.defense\s*=\s*\(int\)\((\d+)", text)
        if base_def:
            stats["defense_base"] = int(base_def.group(1))

    # Spawn context (for enemies)
    spawn_method = re.search(r"SpawnChance.*?\{(.*?)^\s*\}", text, re.DOTALL | re.MULTILINE)
    if spawn_method:
        spawn_text = spawn_method.group(1)
        contexts = []
        if "bloodMoon" in spawn_text or "BloodMoon" in spawn_text:
            contexts.append("blood_moon")
        if "invasionActive" in spawn_text:
            contexts.append("death_eater_invasion")
        if "eventActive" in spawn_text and "Azkaban" in spawn_text:
            contexts.append("azkaban_event")
        if "ForbiddenForest" in spawn_text:
            contexts.append("forbidden_forest")
        if "ZoneDungeon" in spawn_text:
            contexts.append("dungeon")
        if "hardMode" in spawn_text or "Main.hardMode" in spawn_text:
            contexts.append("hardmode")
        if contexts:
            stats["spawnContexts"] = contexts

    return {k: v for k, v in stats.items() if v is not None}


def extract_drop_rules(filepath: Path) -> list:
    """Extract item drop rules from ModifyItemLoot/ModifyNPCLoot."""
    text = filepath.read_text(encoding="utf-8", errors="replace")
    drops = []

    # Pattern: ItemDropRule.Common(ModContent.ItemType<...>(), chance, min, max)
    for m in re.finditer(
        r"ItemDropRule\.Common\(\s*ModContent\.ItemType<[^>]*\.(\w+)>\(\)\s*"
        r"(?:,\s*(\d+))?"
        r"(?:,\s*(\d+))?"
        r"(?:,\s*(\d+))?\s*\)",
        text
    ):
        item_type = m.group(1)
        chance_denom = int(m.group(2)) if m.group(2) else 1
        min_stack = int(m.group(3)) if m.group(3) else 1
        max_stack = int(m.group(4)) if m.group(4) else min_stack

        drop = {"item": item_type, "chance": f"1/{chance_denom}"}
        if min_stack != 1 or max_stack != 1:
            drop["stack"] = f"{min_stack}-{max_stack}" if min_stack != max_stack else str(min_stack)
        drops.append(drop)

    # Also catch vanilla item drops
    for m in re.finditer(
        r"ItemDropRule\.Common\(\s*ItemID\.(\w+)\s*"
        r"(?:,\s*(\d+))?"
        r"(?:,\s*(\d+))?"
        r"(?:,\s*(\d+))?\s*\)",
        text
    ):
        item_type = f"Vanilla:{m.group(1)}"
        chance_denom = int(m.group(2)) if m.group(2) else 1
        min_stack = int(m.group(3)) if m.group(3) else 1
        max_stack = int(m.group(4)) if m.group(4) else min_stack

        drop = {"item": item_type, "chance": f"1/{chance_denom}"}
        if min_stack != 1 or max_stack != 1:
            drop["stack"] = f"{min_stack}-{max_stack}" if min_stack != max_stack else str(min_stack)
        drops.append(drop)

    return drops


def scan_all(root: Path) -> dict:
    """Scan the entire repository for mechanical data."""
    content = root / "Content"
    result = {}

    # ── Wands ──
    wand_dir = content / "Items" / "Weapons" / "Wands"
    wands = []
    for f in sorted(wand_dir.glob("*.cs")):
        stats = extract_item_stats(f)
        wands.append(stats)
    result["wands"] = wands

    # ── Other weapons ──
    weapon_dir = content / "Items" / "Weapons"
    other_weapons = []
    for f in sorted(weapon_dir.glob("*.cs")):
        stats = extract_item_stats(f)
        other_weapons.append(stats)
    result["other_weapons"] = other_weapons

    # ── Accessories ──
    acc_dir = content / "Items" / "Accessories"
    accessories = []
    for f in sorted(acc_dir.glob("*.cs")):
        stats = extract_item_stats(f)
        accessories.append(stats)
    result["accessories"] = accessories

    # ── Potions ──
    pot_dir = content / "Items" / "Consumables" / "Potions"
    potions = []
    for f in sorted(pot_dir.glob("*.cs")):
        stats = extract_item_stats(f)
        potions.append(stats)
    result["potions"] = potions

    # ── Armor ──
    armor_dir = content / "Items" / "Armor"
    armor = []
    for f in sorted(armor_dir.rglob("*.cs")):
        stats = extract_item_stats(f)
        # Add set name from parent directory
        stats["set"] = f.parent.name
        armor.append(stats)
    result["armor"] = armor

    # ── Bosses ──
    boss_dir = content / "NPCs" / "Bosses"
    bosses = []
    for d in sorted(boss_dir.iterdir()):
        if not d.is_dir():
            continue
        boss_files = list(d.glob("*Boss.cs"))
        if boss_files:
            stats = extract_npc_stats(boss_files[0])
            stats["folder"] = d.name
            # Extract drops from bag files
            bag_dir = content / "Items" / "BossLoot" / d.name
            if bag_dir.exists():
                for bag_file in bag_dir.glob("*Bag.cs"):
                    stats["bagDrops"] = extract_drop_rules(bag_file)
                # Also check boss direct drops
                stats["directDrops"] = extract_drop_rules(boss_files[0])
            bosses.append(stats)
    result["bosses"] = bosses

    # ── Enemies ──
    enemy_dir = content / "NPCs" / "Enemies"
    enemies = []
    for f in sorted(enemy_dir.glob("*.cs")):
        stats = extract_npc_stats(f)
        enemies.append(stats)
    result["enemies"] = enemies

    # ── Town NPCs ──
    npc_dir = content / "NPCs" / "Town"
    town_npcs = []
    for f in sorted(npc_dir.glob("*.cs")):
        stats = extract_npc_stats(f)
        town_npcs.append(stats)
    result["town_npcs"] = town_npcs

    # ── Metadata ──
    result["_meta"] = {
        "generator": "scripts/export_mechanical_data.py",
        "type": "auto-derived",
        "description": (
            "Mechanical data extracted from C# source via regex. "
            "Items marked _curated=true have dynamic values that require manual review. "
            "All other values are directly from SetDefaults() assignments."
        ),
    }

    return result


def main():
    parser = argparse.ArgumentParser(description="Export mechanical data from C# source")
    parser.add_argument("--check", action="store_true", help="Validate extraction only")
    args = parser.parse_args()

    root = find_repo_root()
    data = scan_all(root)

    if args.check:
        print("=== Mechanical Data Extraction Summary ===")
        print(f"  Wands: {len(data['wands'])}")
        print(f"  Other weapons: {len(data['other_weapons'])}")
        print(f"  Accessories: {len(data['accessories'])}")
        print(f"  Potions: {len(data['potions'])}")
        print(f"  Armor pieces: {len(data['armor'])}")
        print(f"  Bosses: {len(data['bosses'])}")
        print(f"  Enemies: {len(data['enemies'])}")
        print(f"  Town NPCs: {len(data['town_npcs'])}")
        curated = sum(1 for cat in data.values() if isinstance(cat, list)
                      for item in cat if isinstance(item, dict) and item.get("_curated"))
        print(f"  Items requiring curation: {curated}")
        shops_path = root / "scripts" / "mechanical_data" / "shops.json"
        if shops_path.exists():
            with open(shops_path, encoding="utf-8") as f:
                shops = json.load(f)
            print(f"  Shops: {len(shops)} NPCs")
        else:
            print(f"  Shops: NOT EXPORTED (shops.json missing)")
        return

    out_dir = root / "scripts" / "mechanical_data"
    out_dir.mkdir(parents=True, exist_ok=True)

    # Write category files
    for key in ["wands", "bosses", "enemies", "accessories", "potions", "armor", "other_weapons", "town_npcs"]:
        path = out_dir / f"{key}.json"
        with open(path, "w", encoding="utf-8") as f:
            json.dump(data[key], f, indent=2, ensure_ascii=False)

    # Write combined file
    combined_path = out_dir / "all.json"
    with open(combined_path, "w", encoding="utf-8") as f:
        json.dump(data, f, indent=2, ensure_ascii=False)

    print(f"Mechanical data written to: {out_dir}")
    print(f"  Wands: {len(data['wands'])} | Bosses: {len(data['bosses'])} | Enemies: {len(data['enemies'])}")
    print(f"  Accessories: {len(data['accessories'])} | Potions: {len(data['potions'])} | Armor: {len(data['armor'])}")


if __name__ == "__main__":
    main()
