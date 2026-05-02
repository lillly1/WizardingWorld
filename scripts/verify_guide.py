#!/usr/bin/env python3
"""
Wizarding World — Guide Verification Script

Reads content_manifest.json (produced by scan_content.py) and the PDF generator
source to check for count drift, stale references, and lore contradictions.

Usage:
    python scripts/scan_content.py          # produce the manifest first
    python scripts/verify_guide.py          # check for drift
    python scripts/verify_guide.py --strict # exit code 1 on any failure

Exit codes:
    0 = all checks pass
    1 = at least one check failed (--strict mode)
"""

import argparse
import json
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


class Verifier:
    def __init__(self, manifest: dict, repo_root: Path):
        self.manifest = manifest
        self.root = repo_root
        self.summary = manifest["summary"]
        self.passes = 0
        self.fails = 0
        self.warnings = 0

    def check(self, name: str, condition: bool, detail: str = ""):
        if condition:
            self.passes += 1
            print(f"  PASS  {name}")
        else:
            self.fails += 1
            msg = f"  FAIL  {name}"
            if detail:
                msg += f" — {detail}"
            print(msg)

    def warn(self, name: str, detail: str):
        self.warnings += 1
        print(f"  WARN  {name} — {detail}")

    def verify_manifest_self_consistency(self):
        """Check that the manifest's own numbers are internally consistent."""
        print("\n=== Manifest Self-Consistency ===")
        s = self.summary
        w = self.manifest["wands"]

        self.check("wands_active = base + upgrade + utility",
                    w["count_active"] == w["count_base_combat"] + w["count_upgrades"] + w["count_utility"],
                    f"{w['count_active']} != {w['count_base_combat']}+{w['count_upgrades']}+{w['count_utility']}")

        self.check("wands total = active + obsolete",
                    len(w["all_files"]) == w["count_active"] + w["count_obsolete"],
                    f"{len(w['all_files'])} != {w['count_active']}+{w['count_obsolete']}")

        self.check("armor pieces count matches sets * expected",
                    self.manifest["armor"]["count_pieces"] >= self.manifest["armor"]["count_sets"] * 2,
                    f"{self.manifest['armor']['count_pieces']} pieces for {self.manifest['armor']['count_sets']} sets seems low")

    def verify_no_stale_references(self):
        """Check for stale references to retired items in .cs files."""
        print("\n=== Stale Reference Check ===")

        stale_patterns = [
            ("Wand of Destiny as separate item", r"WandOfDestiny", ["WandOfDestiny.cs"]),
            ("Shadow Elder Wand as separate item", r"ShadowElderWand", ["ShadowElderWand.cs"]),
            ("CorruptedPatronus (renamed to PhantomStag)", r"CorruptedPatronus", []),
            ("HousePointsSystem (renamed to HouseRenownSystem)", r"HousePointsSystem", []),
        ]

        for desc, pattern, allowed_files in stale_patterns:
            found_in = []
            for cs_file in self.root.rglob("*.cs"):
                if cs_file.name in allowed_files:
                    continue
                text = cs_file.read_text(encoding="utf-8", errors="replace")
                if re.search(pattern, text):
                    # Allow references inside [Obsolete] migration stubs
                    if "[Obsolete" in text and cs_file.name in [f"{pattern}.cs" for _ in [1]]:
                        continue
                    found_in.append(cs_file.relative_to(self.root))

            # Filter: WandOfDestiny/ShadowElderWand references in their own stubs are OK
            # Also OK in WizardPlayer.cs for migration logic
            migration_ok = {"WizardPlayer.cs", "WandOfDestiny.cs", "ShadowElderWand.cs",
                            "WandMasteryGlobalItem.cs", "ContentManifest.md", "CLAUDE.md"}
            found_in = [f for f in found_in if f.name not in migration_ok]

            self.check(f"No stale ref: {desc}",
                        len(found_in) == 0,
                        f"found in: {', '.join(str(f) for f in found_in[:5])}")

    def verify_lore_state(self):
        """Check that key lore items are in the expected state."""
        print("\n=== Lore State Verification ===")

        # InvisibilityCloak should NOT be craftable
        cloak_path = self.root / "Content" / "Items" / "Accessories" / "InvisibilityCloak.cs"
        if cloak_path.exists():
            text = cloak_path.read_text(encoding="utf-8", errors="replace")
            self.check("True Invisibility Cloak is NOT craftable",
                        "AddRecipes" not in text,
                        "Found AddRecipes in InvisibilityCloak.cs — should be quest-only")
        else:
            self.warn("InvisibilityCloak.cs", "file not found")

        # DemiguiseCloak should exist and BE craftable
        demiguise_path = self.root / "Content" / "Items" / "Accessories" / "DemiguiseCloak.cs"
        if demiguise_path.exists():
            text = demiguise_path.read_text(encoding="utf-8", errors="replace")
            self.check("DemiguiseCloak IS craftable",
                        "AddRecipes" in text,
                        "DemiguiseCloak.cs has no AddRecipes — should be craftable")
        else:
            self.warn("DemiguiseCloak.cs", "file not found")

        # ResurrectionStone should reference GauntsRing in comments/code
        stone_path = self.root / "Content" / "Items" / "Accessories" / "ResurrectionStone.cs"
        if stone_path.exists():
            text = stone_path.read_text(encoding="utf-8", errors="replace")
            self.check("ResurrectionStone references Gaunt's Ring",
                        "Gaunt" in text,
                        "No Gaunt's Ring reference in ResurrectionStone.cs")

        # HallowsSystem should exist
        hallows_path = self.root / "Common" / "Systems" / "HallowsSystem.cs"
        self.check("HallowsSystem.cs exists",
                    hallows_path.exists(),
                    "Missing centralized Hallows progression system")

        # HorcruxHuntSystem should exist
        horcrux_path = self.root / "Common" / "Systems" / "HorcruxHuntSystem.cs"
        self.check("HorcruxHuntSystem.cs exists",
                    horcrux_path.exists(),
                    "Missing Horcrux Hunt system")

        # ShadowElderWand should be a migration stub
        shadow_path = self.root / "Content" / "Items" / "Weapons" / "Wands" / "ShadowElderWand.cs"
        if shadow_path.exists():
            text = shadow_path.read_text(encoding="utf-8", errors="replace")
            self.check("ShadowElderWand is [Obsolete] stub",
                        "[Obsolete" in text,
                        "ShadowElderWand.cs exists but is not marked [Obsolete]")

        # WandOfDestiny should be a migration stub
        wod_path = self.root / "Content" / "Items" / "Weapons" / "Wands" / "WandOfDestiny.cs"
        if wod_path.exists():
            text = wod_path.read_text(encoding="utf-8", errors="replace")
            self.check("WandOfDestiny is [Obsolete] stub",
                        "[Obsolete" in text,
                        "WandOfDestiny.cs exists but is not marked [Obsolete]")

        # Riddikulus should target Boggarts, not Dementors
        riddikulus_path = self.root / "Content" / "Projectiles" / "Spells" / "RiddikulusProjectile.cs"
        if riddikulus_path.exists():
            text = riddikulus_path.read_text(encoding="utf-8", errors="replace")
            self.check("Riddikulus targets Boggarts",
                        "Boggart" in text,
                        "RiddikulusProjectile.cs does not reference Boggart")
            # Dementor refs are OK if they are clarifying comments (NOT, removed, Patronus instead)
            has_dementor_bonus = False
            if "Dementor" in text:
                for line in text.splitlines():
                    if "Dementor" in line and not line.strip().startswith("//"):
                        # Non-comment Dementor reference = actual code targeting Dementors
                        if "ModContent.NPCType" in line and "Dementor" in line:
                            has_dementor_bonus = True
            self.check("Riddikulus does NOT have Dementor bonus damage code",
                        not has_dementor_bonus,
                        "RiddikulusProjectile.cs has active code granting Dementor bonus")

        # Peeves should be dungeon-only
        peeves_path = self.root / "Content" / "NPCs" / "Enemies" / "Peeves.cs"
        if peeves_path.exists():
            text = peeves_path.read_text(encoding="utf-8", errors="replace")
            self.check("Peeves is dungeon-only spawn",
                        "ZoneDungeon" in text,
                        "Peeves.cs does not check for dungeon zone")

        # Dementor should have contextual spawning
        dementor_path = self.root / "Content" / "NPCs" / "Enemies" / "Dementor.cs"
        if dementor_path.exists():
            text = dementor_path.read_text(encoding="utf-8", errors="replace")
            has_context = ("bloodMoon" in text or "BloodMoon" in text) and \
                          ("invasionActive" in text or "eventActive" in text)
            self.check("Dementor spawns require narrative context",
                        has_context,
                        "Dementor.cs may still have generic night spawning")

    def verify_guide_content_semantic(self):
        """Check guide_content.json for stale or contradictory text patterns."""
        print("\n=== Semantic Drift Detection (guide_content.json) ===")
        gc_path = self.root / "scripts" / "guide_content.json"
        if not gc_path.exists():
            self.warn("guide_content.json", "not found, skipping semantic checks")
            return

        text = gc_path.read_text(encoding="utf-8", errors="replace")

        # FORBIDDEN PATTERNS: these indicate stale content
        forbidden = [
            ('"Wand of Destiny" as separate weapon',
             r'"Wand of Destiny".*(?:upgrade|craft|recipe|endgame weapon)',
             "Wand of Destiny should only appear as Elder Wand flavor/alias"),

            ('"Shadow Elder Wand" as current item',
             r'"Shadow Elder Wand".*(?:craft|recipe|tier|upgrade path)',
             "Shadow Elder Wand is retired"),

            ('Invisibility Cloak as 33% Voldemort drop',
             r'Invisibility Cloak.*33%',
             "True Cloak is quest-obtained, not a percentage drop"),

            ('Resurrection Stone as 33% Voldemort drop',
             r'Resurrection Stone.*33%',
             "Stone is awakened from Gaunt's Ring, not a percentage drop"),

            ('Riddikulus as anti-Dementor spell',
             r'Riddikulus.{0,40}(?:vs\.? Dementor|Dementor bonus|3x.*?Dementor|damage.{0,20}Dementor)',
             "Riddikulus is anti-Boggart; Patronus counters Dementors"),

            ('Reparo as healing aura',
             r'Reparo.*(?:healing aura|heals.*living|biological healing|HP.*heal)',
             "Reparo repairs objects/wards, not living targets"),

            ('Dementor as generic Hardmode night enemy',
             r'Dementor.*(?:Hardmode night|generic night|always.*night)',
             "Dementors are contextual: Blood Moon/Invasion/Forest/Azkaban"),

            ('CorruptedPatronus (renamed to PhantomStag)',
             r'CorruptedPatronus',
             "Enemy was renamed to Phantom Stag"),

            ('House Points as system name (renamed to House Renown)',
             r"(?<!')House Points(?!' since)(?! (?:are|is|were|was|system|\(|\.))(?<!based on house points)(?<!on house points)",
             "System was renamed to House Renown"),

            ('House Cup (renamed to House Champion)',
             r'House Cup Victory',
             "Milestone was renamed to House Champion"),

            ('Voldemort as Post-Golem instead of Post-Cultist',
             r'Voldemort.*Post-Golem',
             "Voldemort is Post-Lunatic Cultist (true final boss)"),

            ('Dementor King as Post-Cultist instead of Post-Golem',
             r'Dementor King.*Post-(?:Lunatic|Cultist)',
             "Dementor King is Post-Golem (penultimate boss)"),

            ('Fluffy with fire breath',
             r'Fluffy.*fire breath',
             "Fluffy uses bark shockwaves and boulder barrage, not fire"),
        ]

        for desc, pattern, reason in forbidden:
            matches = re.findall(pattern, text, re.IGNORECASE)
            self.check(f"No stale text: {desc}",
                        len(matches) == 0,
                        f"{reason}. Found: {matches[0][:80]}..." if matches else "")

        # REQUIRED PATTERNS: these must be present
        required = [
            ("Gaunt's Ring in Hallows acquisition",
             r"Gaunt.s Ring",
             "Resurrection Stone acquisition must reference Gaunt's Ring"),

            ("Patronus as anti-Dementor answer",
             r"Patronus.*(?:anti-Dementor|Dementor|counter|defense)",
             "Patronus should be positioned as anti-Dementor defense"),

            ("Wand Mastery mentions XP levels",
             r"(?:Familiar|Attuned|Mastered)",
             "Wand Mastery should describe the actual level names"),

            ("Boss progression ends with Voldemort as final",
             r"(?:True Final Boss|Post-Lunatic Cultist.*True Final)",
             "Voldemort should be described as True Final Boss somewhere"),

            ("Canon disclosure section exists",
             r"canon_disclosure",
             "Guide should include canon/mod-original disclosure"),
        ]

        for desc, pattern, reason in required:
            matches = re.findall(pattern, text, re.IGNORECASE)
            self.check(f"Required text: {desc}",
                        len(matches) > 0,
                        reason)

    def verify_mechanical_schemas(self):
        """Validate mechanical JSON exports against their JSON schemas."""
        print("\n=== Mechanical Data Schema Validation ===")

        try:
            from jsonschema import validate, ValidationError
        except ImportError:
            self.warn("jsonschema", "package not installed, skipping schema validation (pip install jsonschema)")
            return

        mech_dir = self.root / "scripts" / "mechanical_data"
        schema_dir = self.root / "schemas"

        if not mech_dir.exists():
            self.warn("mechanical_data/", "directory not found, skipping schema validation")
            return
        if not schema_dir.exists():
            self.warn("schemas/", "directory not found, skipping schema validation")
            return

        schema_map = {
            "bosses.json": "mechanical_bosses.schema.json",
            "wands.json": "mechanical_wands.schema.json",
            "enemies.json": "mechanical_enemies.schema.json",
            "accessories.json": "mechanical_accessories.schema.json",
            "shops.json": "mechanical_shops.schema.json",
        }

        for data_file, schema_file in schema_map.items():
            data_path = mech_dir / data_file
            schema_path = schema_dir / schema_file

            if not data_path.exists():
                self.warn(data_file, f"not found in mechanical_data/, skipping")
                continue
            if not schema_path.exists():
                self.warn(schema_file, f"not found in schemas/, skipping")
                continue

            with open(data_path, encoding="utf-8") as f:
                data = json.load(f)
            with open(schema_path, encoding="utf-8") as f:
                schema = json.load(f)

            try:
                validate(instance=data, schema=schema)
                self.check(f"{data_file} validates against {schema_file}", True)
            except ValidationError as e:
                self.check(f"{data_file} validates against {schema_file}", False,
                           f"{e.message} (path: {list(e.absolute_path)})")

    def verify_mechanical_consistency(self):
        """Check guide_content.json tables against mechanical_data extraction."""
        print("\n=== Mechanical Data Consistency ===")

        mech_dir = self.root / "scripts" / "mechanical_data"
        gc_path = self.root / "scripts" / "guide_content.json"

        if not mech_dir.exists():
            self.warn("mechanical_data/", "directory not found, skipping mechanical checks")
            return
        if not gc_path.exists():
            self.warn("guide_content.json", "not found, skipping mechanical checks")
            return

        # Load mechanical data
        wands_path = mech_dir / "wands.json"
        bosses_path = mech_dir / "bosses.json"
        enemies_path = mech_dir / "enemies.json"

        with open(gc_path, encoding="utf-8") as f:
            guide = json.load(f)

        # ── Wand stat verification ──
        if wands_path.exists():
            with open(wands_path, encoding="utf-8") as f:
                mech_wands = {w["id"]: w for w in json.load(f)}

            # Check each guide wand row against mechanical data
            mismatches = []
            for tier_key, tier in guide.get("wands", {}).get("tiers", {}).items():
                for row in tier.get("rows", []):
                    wand_name = row[0]  # Display name
                    guide_dmg = row[2]  # Damage column
                    guide_mana = row[4] if len(row) > 4 else None  # Mana column

                    # Find matching mechanical entry by fuzzy name matching
                    mech_id = self._find_wand_id(wand_name, mech_wands)
                    if not mech_id:
                        continue  # Can't match, skip

                    mech = mech_wands[mech_id]
                    if mech.get("obsolete"):
                        continue

                    # Check damage
                    if mech.get("damage") is not None and guide_dmg:
                        try:
                            if int(guide_dmg) != mech["damage"]:
                                mismatches.append(
                                    f"{wand_name}: guide damage={guide_dmg}, code damage={mech['damage']}")
                        except ValueError:
                            pass

                    # Check mana
                    if mech.get("mana") is not None and guide_mana:
                        try:
                            if int(guide_mana) != mech["mana"]:
                                mismatches.append(
                                    f"{wand_name}: guide mana={guide_mana}, code mana={mech['mana']}")
                        except ValueError:
                            pass

            self.check("Guide wand stats match extracted mechanical data",
                        len(mismatches) == 0,
                        "; ".join(mismatches[:5]))

        # ── Boss stat verification ──
        if bosses_path.exists():
            with open(bosses_path, encoding="utf-8") as f:
                mech_bosses = {b["id"]: b for b in json.load(f)}

            mismatches = []
            for boss in guide.get("bosses", {}).get("list", []):
                boss_name = boss["name"]
                guide_hp_str = boss["hp"].replace(",", "")

                mech_id = self._find_boss_id(boss_name, mech_bosses)
                if not mech_id:
                    continue

                mech = mech_bosses[mech_id]
                actual_hp = mech.get("lifeMax_base") or mech.get("lifeMax")
                if actual_hp is not None:
                    try:
                        guide_hp = int(guide_hp_str)
                        if guide_hp != actual_hp:
                            mismatches.append(
                                f"{boss_name}: guide HP={guide_hp}, code HP={actual_hp}")
                    except ValueError:
                        pass

            self.check("Guide boss HP matches extracted mechanical data",
                        len(mismatches) == 0,
                        "; ".join(mismatches[:5]))

        # ── Voldemort drops verification ──
        if bosses_path.exists():
            voldemort = mech_bosses.get("VoldemortBoss", {})
            bag_drops = voldemort.get("bagDrops", [])
            drop_items = {d["item"] for d in bag_drops}

            self.check("Voldemort drops Elder Wand (mechanical)",
                        "ElderWand" in drop_items,
                        f"Bag drops: {drop_items}")
            self.check("Voldemort drops Gaunt's Ring (mechanical)",
                        "GauntsRing" in drop_items,
                        f"Bag drops: {drop_items}")
            self.check("Voldemort does NOT drop InvisibilityCloak",
                        "InvisibilityCloak" not in drop_items,
                        "Invisibility Cloak should not be a Voldemort drop")
            self.check("Voldemort does NOT drop ResurrectionStone directly",
                        "ResurrectionStone" not in drop_items,
                        "Resurrection Stone comes from Gaunt's Ring purification")

        # ── Enemy spawn context verification ──
        if enemies_path.exists():
            with open(enemies_path, encoding="utf-8") as f:
                mech_enemies = {e["id"]: e for e in json.load(f)}

            dementor = mech_enemies.get("Dementor", {})
            contexts = dementor.get("spawnContexts", [])
            self.check("Dementor has contextual spawn (blood_moon in mechanical data)",
                        "blood_moon" in contexts,
                        f"Spawn contexts: {contexts}")
            self.check("Dementor has azkaban_event spawn context",
                        "azkaban_event" in contexts,
                        f"Spawn contexts: {contexts}")

            peeves = mech_enemies.get("Peeves", {})
            peeves_contexts = peeves.get("spawnContexts", [])
            self.check("Peeves spawn context is dungeon-only (mechanical)",
                        "dungeon" in peeves_contexts and len(peeves_contexts) == 1,
                        f"Spawn contexts: {peeves_contexts}")

        # ── Shop verification ──
        shops_path = mech_dir / "shops.json"
        if shops_path.exists():
            with open(shops_path, encoding="utf-8") as f:
                shops = json.load(f)

            # Normalize names: strip spaces and lowercase for comparison
            def norm(name: str) -> str:
                return name.replace(" ", "").replace("and", "And").lower()

            shop_npc_norm = {norm(s["npc"]) for s in shops}
            guide_npc_norm = {norm(row[0]) for row in guide.get("town_npcs", {}).get("rows", [])}

            self.check("Shop NPC count matches guide NPC count",
                        len(shop_npc_norm) == len(guide_npc_norm),
                        f"Shops: {len(shop_npc_norm)}, Guide: {len(guide_npc_norm)}")

            # Check that every shop NPC has a fuzzy match in guide
            unmatched = shop_npc_norm - guide_npc_norm
            self.check("All shop NPCs match guide NPCs (normalized)",
                        len(unmatched) == 0,
                        f"Unmatched: {unmatched}")
        else:
            self.warn("shops.json", "not found, skipping shop checks")

    def _find_wand_id(self, display_name: str, mech_wands: dict) -> str:
        """Fuzzy match guide display name to mechanical wand ID."""
        # Direct camelCase match
        camel = display_name.replace(" ", "").replace("'", "")
        if camel in mech_wands:
            return camel
        # Try adding "Wand" suffix
        if camel + "Wand" in mech_wands:
            return camel + "Wand"
        # Partial match
        for wand_id in mech_wands:
            if display_name.replace(" ", "").lower() in wand_id.lower():
                return wand_id
        return None

    def _find_boss_id(self, display_name: str, mech_bosses: dict) -> str:
        """Fuzzy match guide boss name to mechanical boss ID."""
        name_map = {
            "Mountain Troll": "TrollBoss",
            "Professor Quirrell": "QuirrellBoss",
            "Basilisk": "BasiliskBoss",
            "Aragog": "AragogBoss",
            "Fluffy": "FluffyBoss",
            "Hungarian Horntail": "HorntailBoss",
            "Fenrir Greyback": "FenrirBoss",
            "Dolores Umbridge": "UmbridgeBoss",
            "Barty Crouch Jr": "BartyCrouchBoss",
            "Bellatrix Lestrange": "BellatrixBoss",
            "Dementor King": "DementorKingBoss",
            "Lord Voldemort": "VoldemortBoss",
        }
        return name_map.get(display_name)

    def verify_hogsmeade_closure(self):
        """Check that Hogsmeade expansion pack is fully closed."""
        print("\n=== Hogsmeade Pack Closure ===")

        # Sprite existence checks
        items_dir = self.root / "Content" / "Items" / "Consumables"
        for item in ["FizzyWhizbee", "PepperImp", "SugarQuill", "AcidPop", "DrooblesBestBlowingGum"]:
            self.check(f"Sprite exists: {item}.png",
                        (items_dir / f"{item}.png").exists(),
                        f"Missing {item}.png in Content/Items/Consumables/")

        npc_dir = self.root / "Content" / "NPCs" / "Town"
        self.check("Sprite exists: MadamRosmerta.png",
                    (npc_dir / "MadamRosmerta.png").exists(),
                    "Missing NPC body sprite")
        self.check("Sprite exists: MadamRosmerta_Head.png",
                    (npc_dir / "MadamRosmerta_Head.png").exists(),
                    "Missing NPC head sprite")

        # Localization checks
        en_path = self.root / "Localization" / "en-US_Mods.WizardingWorld.hjson"
        zh_path = self.root / "Localization" / "zh-Hans_Mods.WizardingWorld.hjson"

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["FizzyWhizbee", "PepperImp", "SugarQuill", "AcidPop",
                        "DrooblesBestBlowingGum", "MadamRosmerta"]:
                self.check(f"EN localization: {key}",
                            key in en_text,
                            f"Missing {key} in en-US HJSON")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            for key in ["FizzyWhizbee", "PepperImp", "SugarQuill", "AcidPop",
                        "DrooblesBestBlowingGum", "MadamRosmerta"]:
                self.check(f"ZH localization: {key}",
                            key in zh_text,
                            f"Missing {key} in zh-Hans HJSON")

        # Owl Post wiring check
        rosmerta_path = self.root / "Content" / "NPCs" / "Town" / "MadamRosmerta.cs"
        if rosmerta_path.exists():
            text = rosmerta_path.read_text(encoding="utf-8", errors="replace")
            self.check("Owl Post wired to Rosmerta chat",
                        "TryCompleteRequest" in text or "OwlPostSystem" in text,
                        "MadamRosmerta.cs does not reference OwlPostSystem")
            self.check("Rosmerta has Owl Post button",
                        "Owl Post" in text,
                        "MadamRosmerta.cs missing 'Owl Post' chat button")
            # Lore check: should NOT imply she owns Honeydukes
            self.check("Rosmerta does not claim Honeydukes ownership",
                        "owns Honeydukes" not in text.lower() and
                        "my sweet shop" not in text.lower() and
                        "honeydukes is mine" not in text.lower(),
                        "MadamRosmerta.cs implies Honeydukes ownership")

        # Azkaban content checks
        azkaban_enemies = ["PrisonWraith", "DespairAnchor"]
        for enemy in azkaban_enemies:
            self.check(f"Sprite exists: {enemy}.png",
                        (self.root / "Content" / "NPCs" / "Enemies" / f"{enemy}.png").exists(),
                        f"Missing {enemy}.png")
            self.check(f"EN localization: {enemy}",
                        enemy in (self.root / "Localization" / "en-US_Mods.WizardingWorld.hjson").read_text(encoding="utf-8", errors="replace"),
                        f"Missing {enemy} in EN HJSON")

        azkaban_items = ["SoulAsh", "PatronusFocus"]
        for item in azkaban_items:
            sprite_dir = "Consumables" if item == "SoulAsh" else "Accessories"
            self.check(f"Sprite exists: {item}.png",
                        (self.root / "Content" / "Items" / sprite_dir / f"{item}.png").exists(),
                        f"Missing {item}.png")
            self.check(f"EN localization: {item}",
                        item in (self.root / "Localization" / "en-US_Mods.WizardingWorld.hjson").read_text(encoding="utf-8", errors="replace"),
                        f"Missing {item} in EN HJSON")

        # WardOfHopeBuff localization
        self.check("EN localization: WardOfHopeBuff",
                    "WardOfHopeBuff" in (self.root / "Localization" / "en-US_Mods.WizardingWorld.hjson").read_text(encoding="utf-8", errors="replace"),
                    "Missing WardOfHopeBuff in EN HJSON")

        # Shop export check
        shops_path = self.root / "scripts" / "mechanical_data" / "shops.json"
        if shops_path.exists():
            with open(shops_path, encoding="utf-8") as f:
                shops = json.load(f)
            rosmerta_shops = [s for s in shops if "Rosmerta" in s.get("npc", "")]
            self.check("Rosmerta shop in mechanical export",
                        len(rosmerta_shops) > 0,
                        "MadamRosmerta not found in shops.json")

        # Room of Requirement checks
        room_key_path = self.root / "Content" / "Items" / "Consumables" / "RoomOfRequirementKey.cs"
        self.check("RoomOfRequirementKey.cs exists",
                    room_key_path.exists(),
                    "Missing Room of Requirement key item")

        room_buffs = ["RoomRecoveryBuff", "RoomTrainingBuff", "RoomVaultBuff",
                      "RoomSanctuaryBuff", "RoomRestorationBuff"]
        for buff in room_buffs:
            self.check(f"Sprite exists: {buff}.png",
                        (self.root / "Content" / "Buffs" / f"{buff}.png").exists(),
                        f"Missing {buff}.png")

        en_path = self.root / "Localization" / "en-US_Mods.WizardingWorld.hjson"
        zh_path = self.root / "Localization" / "zh-Hans_Mods.WizardingWorld.hjson"
        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["RoomOfRequirementKey", "RoomRecoveryBuff", "RoomTrainingBuff",
                        "RoomVaultBuff", "RoomSanctuaryBuff", "RoomRestorationBuff"]:
                self.check(f"EN localization: {key}",
                            key in en_text,
                            f"Missing {key} in EN HJSON")

            # Owl Post localization check
            self.check("EN localization: OwlPostComplete",
                        "OwlPostComplete" in en_text,
                        "Owl Post completion text not localized")
            self.check("EN localization: OwlPostSuccess",
                        "OwlPostSuccess" in en_text,
                        "Owl Post success text not localized")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            for key in ["RoomOfRequirementKey", "RoomRecoveryBuff", "RoomRestorationBuff"]:
                self.check(f"ZH localization: {key}",
                            key in zh_text,
                            f"Missing {key} in ZH HJSON")

        # Owl Post hardcoded string check
        if room_key_path.parent.exists():
            rosmerta_path = self.root / "Content" / "NPCs" / "Town" / "MadamRosmerta.cs"
            if rosmerta_path.exists():
                rosmerta_text = rosmerta_path.read_text(encoding="utf-8", errors="replace")
                # Check that Owl Post strings use Language.GetTextValue, not hardcoded
                has_hardcoded_owl = False
                for line in rosmerta_text.splitlines():
                    if "npcChatText" in line and not line.strip().startswith("//"):
                        if '"Today' in line or '"Wonderful' in line or '"Check back' in line:
                            has_hardcoded_owl = True
                self.check("Owl Post strings use localization (not hardcoded)",
                            not has_hardcoded_owl,
                            "MadamRosmerta.cs still has hardcoded Owl Post English strings")

        # Great Hall / House Cup checks
        self.check("GreatHallSystem.cs exists",
                    (self.root / "Common" / "Systems" / "GreatHallSystem.cs").exists(),
                    "Missing Great Hall system")
        self.check("Sprite exists: GreatHallBell.png",
                    (self.root / "Content" / "Items" / "Consumables" / "GreatHallBell.png").exists(),
                    "Missing Great Hall Bell sprite")

        feast_buffs = ["GryffindorFeastBuff", "SlytherinFeastBuff", "RavenclawFeastBuff", "HufflepuffFeastBuff"]
        for buff in feast_buffs:
            self.check(f"Sprite exists: {buff}.png",
                        (self.root / "Content" / "Buffs" / f"{buff}.png").exists(),
                        f"Missing {buff}.png")

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["GreatHallBell", "GryffindorFeastBuff", "SlytherinFeastBuff",
                        "RavenclawFeastBuff", "HufflepuffFeastBuff"]:
                self.check(f"EN localization: {key}",
                            key in en_text,
                            f"Missing {key} in EN HJSON")
            self.check("EN localization: GreatHall system text",
                        "FeastAvailable" in en_text and "HouseCupWon" in en_text,
                        "Missing GreatHall system text in EN HJSON")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            self.check("ZH localization: GreatHallBell",
                        "GreatHallBell" in zh_text,
                        "Missing GreatHallBell in ZH HJSON")
            self.check("ZH localization: GreatHall system text",
                        "FeastAvailable" in zh_text,
                        "Missing GreatHall system text in ZH HJSON")

        # Verify Great Hall uses localization, not hardcoded English
        gh_path = self.root / "Common" / "Systems" / "GreatHallSystem.cs"
        if gh_path.exists():
            gh_text = gh_path.read_text(encoding="utf-8", errors="replace")
            self.check("GreatHall system uses Language.GetTextValue",
                        "Language.GetTextValue" in gh_text,
                        "GreatHallSystem.cs should use localization keys")

        # Quidditch Cup checks
        self.check("QuidditchCupSystem.cs exists",
                    (self.root / "Common" / "Systems" / "QuidditchCupSystem.cs").exists(),
                    "Missing Quidditch Cup system")
        self.check("Sprite exists: QuidditchWhistle.png",
                    (self.root / "Content" / "Items" / "Consumables" / "QuidditchWhistle.png").exists(),
                    "Missing whistle sprite")
        self.check("Sprite exists: QuidditchCupBuff.png",
                    (self.root / "Content" / "Buffs" / "QuidditchCupBuff.png").exists(),
                    "Missing cup buff sprite")

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            self.check("EN localization: QuidditchWhistle",
                        "QuidditchWhistle" in en_text,
                        "Missing QuidditchWhistle in EN HJSON")
            self.check("EN localization: QuidditchCupBuff",
                        "QuidditchCupBuff" in en_text,
                        "Missing QuidditchCupBuff in EN HJSON")
            self.check("EN localization: Quidditch system text",
                        "MatchStart" in en_text and "CupWon" in en_text and "SnitchReleased" in en_text,
                        "Missing Quidditch system text in EN HJSON")

            # Ensure Quidditch Cup and House Cup are separate in guide content
            gc_text = ""
            gc_path = self.root / "scripts" / "guide_content.json"
            if gc_path.exists():
                gc_text = gc_path.read_text(encoding="utf-8", errors="replace")
            self.check("Guide distinguishes Quidditch Cup from House Cup",
                        "Quidditch Cup" in gc_text and "House Cup" in gc_text
                        and "separate from the House Cup" in gc_text,
                        "Guide should clearly separate Quidditch Cup and House Cup")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            self.check("ZH localization: QuidditchWhistle",
                        "QuidditchWhistle" in zh_text,
                        "Missing QuidditchWhistle in ZH HJSON")
            self.check("ZH localization: Quidditch system text",
                        "MatchStart" in zh_text,
                        "Missing Quidditch system text in ZH HJSON")

        qc_path = self.root / "Common" / "Systems" / "QuidditchCupSystem.cs"
        if qc_path.exists():
            qc_text = qc_path.read_text(encoding="utf-8", errors="replace")
            self.check("QuidditchCup uses Language.GetTextValue",
                        "Language.GetTextValue" in qc_text,
                        "QuidditchCupSystem.cs should use localization keys")

        # Phase 14: Quidditch Authenticity checks
        self.check("QuidditchHoop.cs exists",
                    (self.root / "Content" / "NPCs" / "Enemies" / "QuidditchHoop.cs").exists(),
                    "Missing hoop goal target")
        self.check("Bludger.cs exists",
                    (self.root / "Content" / "NPCs" / "Enemies" / "Bludger.cs").exists(),
                    "Missing Bludger hazard")
        self.check("MadamHooch NPC exists",
                    (self.root / "Content" / "NPCs" / "Town" / "MadamHooch.cs").exists(),
                    "Missing Madam Hooch NPC")
        self.check("Sprite exists: QuidditchHoop.png",
                    (self.root / "Content" / "NPCs" / "Enemies" / "QuidditchHoop.png").exists(),
                    "Missing hoop sprite")
        self.check("Sprite exists: Bludger.png",
                    (self.root / "Content" / "NPCs" / "Enemies" / "Bludger.png").exists(),
                    "Missing Bludger sprite")
        self.check("Sprite exists: MadamHooch.png",
                    (self.root / "Content" / "NPCs" / "Town" / "MadamHooch.png").exists(),
                    "Missing Hooch body sprite")
        self.check("Sprite exists: MadamHooch_Head.png",
                    (self.root / "Content" / "NPCs" / "Town" / "MadamHooch_Head.png").exists(),
                    "Missing Hooch head sprite")

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            self.check("EN localization: MadamHooch",
                        "MadamHooch" in en_text,
                        "Missing MadamHooch in EN HJSON")
            self.check("EN localization: Bludger",
                        "Bludger" in en_text,
                        "Missing Bludger in EN HJSON")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            self.check("ZH localization: MadamHooch",
                        "MadamHooch" in zh_text,
                        "Missing MadamHooch in ZH HJSON")

        # Quaffle hoop-based scoring check
        quaffle_path = self.root / "Content" / "Projectiles" / "QuaffleProjectile.cs"
        if quaffle_path.exists():
            qt = quaffle_path.read_text(encoding="utf-8", errors="replace")
            self.check("Quaffle scores on QuidditchHoop (not generic NPC)",
                        "QuidditchHoop" in qt,
                        "QuaffleProjectile should check for QuidditchHoop type, not generic NPC hits")

        # Hooch shop in mechanical export
        shops_path = self.root / "scripts" / "mechanical_data" / "shops.json"
        if shops_path.exists():
            import json as _json
            with open(shops_path, encoding="utf-8") as f:
                shops = _json.load(f)
            hooch_shops = [s for s in shops if "Hooch" in s.get("npc", "")]
            self.check("MadamHooch shop in mechanical export",
                        len(hooch_shops) > 0,
                        "MadamHooch not found in shops.json")

        # Phase 15: Triwizard Tournament checks
        self.check("TriwizardTournamentSystem.cs exists",
                    (self.root / "Common" / "Systems" / "TriwizardTournamentSystem.cs").exists(),
                    "Missing Triwizard Tournament system")

        triwizard_items = {
            "LakeRescueToken": "Items/Consumables",
            "ChampionsTrophy": "Items/Accessories",
        }
        for item, subdir in triwizard_items.items():
            self.check(f"Sprite exists: {item}.png",
                        (self.root / "Content" / subdir / f"{item}.png").exists(),
                        f"Missing {item}.png")

        self.check("Sprite exists: TriwizardChampionBuff.png",
                    (self.root / "Content" / "Buffs" / "TriwizardChampionBuff.png").exists(),
                    "Missing champion buff sprite")

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["LakeRescueToken", "ChampionsTrophy", "TriwizardChampionBuff"]:
                self.check(f"EN localization: {key}",
                            key in en_text,
                            f"Missing {key} in EN HJSON")
            self.check("EN localization: Triwizard system text",
                        "Task1Start" in en_text and "Champion" in en_text,
                        "Missing Triwizard system text in EN HJSON")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            self.check("ZH localization: ChampionsTrophy",
                        "ChampionsTrophy" in zh_text,
                        "Missing ChampionsTrophy in ZH HJSON")
            self.check("ZH localization: Triwizard system text",
                        "Task1Start" in zh_text,
                        "Missing Triwizard system text in ZH HJSON")

        # Phase 16: Naming refactor + school identity checks
        tw_cup_path = self.root / "Content" / "Items" / "Accessories" / "TriwizardCup.cs"
        if tw_cup_path.exists():
            tw_text = tw_cup_path.read_text(encoding="utf-8", errors="replace")
            self.check("TriwizardCup.cs is [Obsolete] migration stub",
                        "[Obsolete" in tw_text,
                        "TriwizardCup.cs should be a migration stub, not an active combat accessory")

        self.check("ChampionsMedallion.cs exists",
                    (self.root / "Content" / "Items" / "Accessories" / "ChampionsMedallion.cs").exists(),
                    "Missing ChampionsMedallion accessory")

        # Guide content check
        gc_path = self.root / "scripts" / "guide_content.json"
        if gc_path.exists():
            gc_text = gc_path.read_text(encoding="utf-8", errors="replace")
            self.check("Guide mentions Triwizard Tournament",
                        "Triwizard Tournament" in gc_text,
                        "Missing Triwizard in guide_content.json")
            self.check("Guide mentions Hogwarts/Durmstrang/Beauxbatons in Triwizard",
                        "Hogwarts" in gc_text and "Durmstrang" in gc_text and "Beauxbatons" in gc_text,
                        "Triwizard guide text must reference all three schools")

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            self.check("EN localization: GobletSelection mentions three schools",
                        "GobletSelection" in en_text and "Durmstrang" in en_text,
                        "Missing GobletSelection with school names in EN HJSON")
            self.check("EN localization: ChampionsMedallion",
                        "ChampionsMedallion" in en_text,
                        "Missing ChampionsMedallion in EN HJSON")

        # Phase 17: Department of Mysteries / Order of the Phoenix
        self.check("ProphecyMissionSystem.cs exists",
                    (self.root / "Common" / "Systems" / "ProphecyMissionSystem.cs").exists(),
                    "Missing Prophecy Mission system")
        self.check("Kingsley NPC exists",
                    (self.root / "Content" / "NPCs" / "Town" / "Kingsley.cs").exists(),
                    "Missing Kingsley Shacklebolt NPC")

        ministry_items = ["MinistryVisitorBadge", "ProphecyDust", "OrderBadge"]
        for item in ministry_items:
            subdir = "Accessories" if item == "OrderBadge" else "Consumables"
            self.check(f"Sprite exists: {item}.png",
                        (self.root / "Content" / "Items" / subdir / f"{item}.png").exists(),
                        f"Missing {item}.png")

        self.check("Sprite exists: Kingsley.png",
                    (self.root / "Content" / "NPCs" / "Town" / "Kingsley.png").exists(),
                    "Missing Kingsley body sprite")
        self.check("Sprite exists: OrderCommendationBuff.png",
                    (self.root / "Content" / "Buffs" / "OrderCommendationBuff.png").exists(),
                    "Missing Order buff sprite")

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["MinistryVisitorBadge", "ProphecyDust", "OrderBadge", "Kingsley", "OrderCommendationBuff"]:
                self.check(f"EN localization: {key}",
                            key in en_text,
                            f"Missing {key} in EN HJSON")
            self.check("EN localization: Ministry system text",
                        "MissionStart" in en_text and "CollectOrbs" in en_text,
                        "Missing Ministry system text in EN HJSON")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            self.check("ZH localization: Kingsley",
                        "Kingsley" in zh_text,
                        "Missing Kingsley in ZH HJSON")
            self.check("ZH localization: Ministry system text",
                        "MissionStart" in zh_text,
                        "Missing Ministry system text in ZH HJSON")

        # Phase 18: Time Chamber + Death Chamber
        self.check("TimeChamberSystem.cs exists",
                    (self.root / "Common" / "Systems" / "TimeChamberSystem.cs").exists(),
                    "Missing Time Chamber system")
        self.check("DeathChamberSystem.cs exists",
                    (self.root / "Common" / "Systems" / "DeathChamberSystem.cs").exists(),
                    "Missing Death Chamber system")

        chamber_items = {"ChronalSand": "Consumables", "VeilThread": "Consumables"}
        for item, subdir in chamber_items.items():
            self.check(f"Sprite exists: {item}.png",
                        (self.root / "Content" / "Items" / subdir / f"{item}.png").exists(),
                        f"Missing {item}.png")

        chamber_buffs = ["TemporalInsightBuff", "VeilWardBuff"]
        for buff in chamber_buffs:
            self.check(f"Sprite exists: {buff}.png",
                        (self.root / "Content" / "Buffs" / f"{buff}.png").exists(),
                        f"Missing {buff}.png")

        chamber_npcs = ["UnstableHourglass", "VeilFracture"]
        for npc in chamber_npcs:
            self.check(f"Sprite exists: {npc}.png",
                        (self.root / "Content" / "NPCs" / "Enemies" / f"{npc}.png").exists(),
                        f"Missing {npc}.png")

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["ChronalSand", "VeilThread", "TemporalInsightBuff", "VeilWardBuff"]:
                self.check(f"EN localization: {key}", key in en_text, f"Missing {key} in EN HJSON")
            self.check("EN localization: Time/Death Chamber text",
                        "TimeChamberStart" in en_text and "DeathChamberStart" in en_text,
                        "Missing chamber system text in EN HJSON")

            # Veil safety check: guide must NOT claim Veil resurrects
            gc_path = self.root / "scripts" / "guide_content.json"
            if gc_path.exists():
                gc_text = gc_path.read_text(encoding="utf-8", errors="replace")
                # Only flag if "Veil" appears near "resurrect" (not "Resurrection Stone")
                veil_resurrect = any(
                    "veil" in gc_text.lower()[max(0,m.start()-80):m.start()]
                    for m in __import__('re').finditer("resurrect", gc_text.lower())
                )
                self.check("Veil does NOT promise resurrection in guide",
                            not veil_resurrect,
                            "Guide text must not claim the Veil enables resurrection")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            self.check("ZH localization: ChronalSand", "ChronalSand" in zh_text, "Missing in ZH")
            self.check("ZH localization: Death Chamber text", "DeathChamberStart" in zh_text, "Missing in ZH")

        # Kingsley shop export
        shops_path = self.root / "scripts" / "mechanical_data" / "shops.json"
        if shops_path.exists():
            import json as _json
            with open(shops_path, encoding="utf-8") as f:
                shops = _json.load(f)
            kingsley_shops = [s for s in shops if "Kingsley" in s.get("npc", "")]
            self.check("Kingsley shop in mechanical export",
                        len(kingsley_shops) > 0,
                        "Kingsley not found in shops.json")

        # Phase 19: Gringotts
        self.check("GringottsVaultSystem.cs exists",
                    (self.root / "Common" / "Systems" / "GringottsVaultSystem.cs").exists(),
                    "Missing Gringotts system")
        self.check("GoblinTeller NPC exists",
                    (self.root / "Content" / "NPCs" / "Town" / "GoblinTeller.cs").exists(),
                    "Missing Goblin Teller NPC")

        gringotts_sprites = {
            "GringottsVaultKey": "Items/Consumables",
            "GalleonDust": "Items/Consumables",
            "GoblinLedger": "Items/Accessories",
        }
        for item, subdir in gringotts_sprites.items():
            self.check(f"Sprite exists: {item}.png",
                        (self.root / "Content" / subdir / f"{item}.png").exists(),
                        f"Missing {item}.png")

        self.check("Sprite exists: GoblinTeller.png",
                    (self.root / "Content" / "NPCs" / "Town" / "GoblinTeller.png").exists(),
                    "Missing Goblin Teller sprite")
        self.check("Sprite exists: VaultFortuneBuff.png",
                    (self.root / "Content" / "Buffs" / "VaultFortuneBuff.png").exists(),
                    "Missing vault buff sprite")

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["GringottsVaultKey", "GalleonDust", "GoblinLedger", "GoblinTeller", "VaultFortuneBuff"]:
                self.check(f"EN localization: {key}", key in en_text, f"Missing {key}")
            self.check("EN: Gringotts system text",
                        "MissionStart" in en_text and "CollectRelics" in en_text,
                        "Missing Gringotts system text")
            # Canon safety: not framed as robbery
            gc_path = self.root / "scripts" / "guide_content.json"
            if gc_path.exists():
                gc_text = gc_path.read_text(encoding="utf-8", errors="replace")
                # Allow "not robbery" negation, reject positive robbery framing
                gc_lower = gc_text.lower()
                has_positive_robbery = ("robbery" in gc_lower or "rob " in gc_lower) and \
                    "not robbery" not in gc_lower
                self.check("Gringotts NOT framed as robbery",
                            not has_positive_robbery,
                            "Guide must not frame vault runs as robbery")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            self.check("ZH localization: GoblinTeller", "GoblinTeller" in zh_text, "Missing in ZH")
            self.check("ZH localization: Gringotts text", "CollectRelics" in zh_text or "MissionStart" in zh_text, "Missing in ZH")

        # Phase 20: Diagon Alley authenticity
        self.check("DiagonAlleySystem.cs exists",
                    (self.root / "Common" / "Systems" / "DiagonAlleySystem.cs").exists(),
                    "Missing Diagon Alley system")
        self.check("Sprite exists: LeakyCauldronToken.png",
                    (self.root / "Content" / "Items" / "Consumables" / "LeakyCauldronToken.png").exists(),
                    "Missing Leaky Cauldron Token sprite")
        self.check("Sprite exists: ShoppingDayBuff.png",
                    (self.root / "Content" / "Buffs" / "ShoppingDayBuff.png").exists(),
                    "Missing Shopping Day buff sprite")

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            self.check("EN localization: LeakyCauldronToken",
                        "LeakyCauldronToken" in en_text,
                        "Missing LeakyCauldronToken in EN HJSON")
            self.check("EN localization: ShoppingDayBuff",
                        "ShoppingDayBuff" in en_text,
                        "Missing ShoppingDayBuff in EN HJSON")
            self.check("EN localization: Diagon Alley street text",
                        "StreetDescription" in en_text and "Ollivanders" in en_text,
                        "Missing DiagonAlley street identity text")
            self.check("EN localization: Gringotts cart descent",
                        "CartDescent" in en_text,
                        "Missing cart descent framing text")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            self.check("ZH localization: LeakyCauldronToken",
                        "LeakyCauldronToken" in zh_text,
                        "Missing in ZH HJSON")
            self.check("ZH localization: DiagonAlley text",
                        "CartDescent" in zh_text or "StreetDescription" in zh_text,
                        "Missing DiagonAlley text in ZH")

        # Guide content check
        gc_path = self.root / "scripts" / "guide_content.json"
        if gc_path.exists():
            gc_text = gc_path.read_text(encoding="utf-8", errors="replace")
            self.check("Guide mentions Leaky Cauldron gateway",
                        "Leaky Cauldron" in gc_text,
                        "Guide should mention Leaky Cauldron as gateway")
            self.check("Guide mentions cart descent",
                        "cart" in gc_text.lower() or "Cart" in gc_text,
                        "Guide should mention Gringotts cart framing")

        # Shop export
        shops_path = self.root / "scripts" / "mechanical_data" / "shops.json"
        if shops_path.exists():
            import json as _json
            with open(shops_path, encoding="utf-8") as f:
                shops = _json.load(f)
            goblin_shops = [s for s in shops if "Goblin" in s.get("npc", "")]
            self.check("GoblinTeller shop in export", len(goblin_shops) > 0, "Missing in shops.json")

        # Phase 21: Knockturn Alley
        self.check("KnockturnAlleySystem.cs exists",
                    (self.root / "Common" / "Systems" / "KnockturnAlleySystem.cs").exists(),
                    "Missing Knockturn system")
        self.check("MrBorgin NPC exists",
                    (self.root / "Content" / "NPCs" / "Town" / "MrBorgin.cs").exists(),
                    "Missing Mr Borgin NPC")
        self.check("HandOfGlory.cs exists",
                    (self.root / "Content" / "Items" / "Accessories" / "HandOfGlory.cs").exists(),
                    "Missing Hand of Glory")

        knockturn_sprites = {
            "KnockturnPass": "Items/Consumables",
            "CursedResidue": "Items/Consumables",
            "HandOfGlory": "Items/Accessories",
        }
        for item, subdir in knockturn_sprites.items():
            self.check(f"Sprite exists: {item}.png",
                        (self.root / "Content" / subdir / f"{item}.png").exists(),
                        f"Missing {item}.png")

        self.check("Sprite exists: MrBorgin.png",
                    (self.root / "Content" / "NPCs" / "Town" / "MrBorgin.png").exists(),
                    "Missing Borgin sprite")
        self.check("Sprite exists: DarkAppraiserBuff.png",
                    (self.root / "Content" / "Buffs" / "DarkAppraiserBuff.png").exists(),
                    "Missing dark buff sprite")

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["KnockturnPass", "CursedResidue", "HandOfGlory", "MrBorgin", "DarkAppraiserBuff"]:
                self.check(f"EN localization: {key}", key in en_text, f"Missing {key}")
            self.check("EN: Knockturn system text",
                        "MissionStart" in en_text and "OpalContained" in en_text,
                        "Missing Knockturn system text")
            # Hand of Glory framing check
            self.check("Hand of Glory: holder-only light framing",
                        "holder" in en_text.lower() and "light" in en_text.lower(),
                        "Hand of Glory should mention 'light only to the holder'")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            self.check("ZH localization: MrBorgin", "MrBorgin" in zh_text, "Missing in ZH")
            self.check("ZH: Knockturn text", "MissionStart" in zh_text or "OpalContained" in zh_text, "Missing in ZH")

        # Shop export
        shops_path = self.root / "scripts" / "mechanical_data" / "shops.json"
        if shops_path.exists():
            import json as _json
            with open(shops_path, encoding="utf-8") as f:
                shops = _json.load(f)
            borgin_shops = [s for s in shops if "Borgin" in s.get("npc", "")]
            self.check("MrBorgin shop in export", len(borgin_shops) > 0, "Missing in shops.json")

        # Guide check
        gc_path = self.root / "scripts" / "guide_content.json"
        if gc_path.exists():
            gc_text = gc_path.read_text(encoding="utf-8", errors="replace")
            self.check("Guide mentions Knockturn Alley",
                        "Knockturn" in gc_text and "Borgin" in gc_text,
                        "Guide should mention Knockturn and Borgin")

        # Phase 25: Whomping Willow / Shrieking Shack
        self.check("ShriekingShackSystem.cs exists",
                    (self.root / "Common" / "Systems" / "ShriekingShackSystem.cs").exists(),
                    "Missing Shrieking Shack system")

        shack_sprites = {
            "WillowPassageToken": "Items/Consumables",
            "MoonsilverThread": "Items/Consumables",
            "MoonwardPendant": "Items/Accessories",
        }
        for item, subdir in shack_sprites.items():
            self.check(f"Sprite exists: {item}.png",
                        (self.root / "Content" / subdir / f"{item}.png").exists(),
                        f"Missing {item}.png")

        self.check("Sprite exists: MoonwardBuff.png",
                    (self.root / "Content" / "Buffs" / "MoonwardBuff.png").exists(),
                    "Missing MoonwardBuff sprite")

        for npc in ["WillowKnot", "TunnelWard", "MoonWardFracture"]:
            self.check(f"Sprite exists: {npc}.png",
                        (self.root / "Content" / "NPCs" / "Enemies" / f"{npc}.png").exists(),
                        f"Missing {npc}.png")

        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["WillowPassageToken", "MoonsilverThread", "MoonwardPendant", "MoonwardBuff"]:
                self.check(f"EN localization: {key}", key in en_text, f"Missing {key}")
            self.check("EN: Shack system text",
                        "WillowStart" in en_text and "ShackComplete" in en_text,
                        "Missing Shack system text")
            # Lore check: no tourist/public/cheerful Shack
            self.check("Shack NOT framed as tourist destination",
                        "tourist" not in en_text.lower().split("Shack")[0][-100:] if "Shack" in en_text else True,
                        "Shack should not be framed as tourist destination")

        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            self.check("ZH-Hans: MoonwardPendant", "MoonwardPendant" in zh_text, "Missing in ZH-Hans")
            self.check("ZH-Hans: Shack text", "WillowStart" in zh_text, "Missing Shack text in ZH-Hans")

        # UnicornBlood safety check
        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            self.check("UnicornBlood: no positive hunting framing",
                        "hunt unicorn" not in en_text.lower() and "kill unicorn" not in en_text.lower(),
                        "UnicornBlood text must not encourage hunting/killing unicorns")

        # Guide content
        gc_path = self.root / "scripts" / "guide_content.json"
        if gc_path.exists():
            gc_text = gc_path.read_text(encoding="utf-8", errors="replace")
            self.check("Guide mentions Whomping Willow / Shrieking Shack",
                        "Whomping Willow" in gc_text and "Shrieking Shack" in gc_text,
                        "Guide should mention both Willow and Shack")

    def verify_st_mungos_closure(self):
        """Check that St Mungo's triage system is fully wired."""
        print("\n=== St Mungo's Triage Closure ===")

        en_path = self.root / "Localization" / "en-US_Mods.WizardingWorld.hjson"
        zh_path = self.root / "Localization" / "zh-Hans_Mods.WizardingWorld.hjson"

        # System exists
        self.check("StMungosTriageSystem.cs exists",
                    (self.root / "Common" / "Systems" / "StMungosTriageSystem.cs").exists(),
                    "Missing St Mungo's triage system")

        # Healer NPC exists
        self.check("Healer NPC exists",
                    (self.root / "Content" / "NPCs" / "Town" / "Healer.cs").exists(),
                    "Missing Healer town NPC")

        # 3 node NPCs
        for node in ["HexResidueNode", "VenomWoundNode", "CauldronSpillNode"]:
            self.check(f"{node}.cs exists",
                        (self.root / "Content" / "NPCs" / "Enemies" / f"{node}.cs").exists(),
                        f"Missing {node} NPC")
            self.check(f"Sprite exists: {node}.png",
                        (self.root / "Content" / "NPCs" / "Enemies" / f"{node}.png").exists(),
                        f"Missing {node}.png")

        # Material, accessory, buff
        self.check("HealersSalts.cs exists",
                    (self.root / "Content" / "Items" / "Consumables" / "HealersSalts.cs").exists(),
                    "Missing Healer's Salts material")
        self.check("Sprite exists: HealersSalts.png",
                    (self.root / "Content" / "Items" / "Consumables" / "HealersSalts.png").exists(),
                    "Missing HealersSalts sprite")
        self.check("HealersSatchel.cs exists",
                    (self.root / "Content" / "Items" / "Accessories" / "HealersSatchel.cs").exists(),
                    "Missing Healer's Satchel accessory")
        self.check("Sprite exists: HealersSatchel.png",
                    (self.root / "Content" / "Items" / "Accessories" / "HealersSatchel.png").exists(),
                    "Missing HealersSatchel sprite")
        self.check("TriageResolvedBuff.cs exists",
                    (self.root / "Content" / "Buffs" / "TriageResolvedBuff.cs").exists(),
                    "Missing Triage Resolved buff")
        self.check("Sprite exists: TriageResolvedBuff.png",
                    (self.root / "Content" / "Buffs" / "TriageResolvedBuff.png").exists(),
                    "Missing TriageResolvedBuff sprite")

        # Healer sprites
        self.check("Sprite exists: Healer.png",
                    (self.root / "Content" / "NPCs" / "Town" / "Healer.png").exists(),
                    "Missing Healer body sprite")
        self.check("Sprite exists: Healer_Head.png",
                    (self.root / "Content" / "NPCs" / "Town" / "Healer_Head.png").exists(),
                    "Missing Healer head sprite")
        self.check("Sprite exists: StMungosPass.png",
                    (self.root / "Content" / "Items" / "Consumables" / "StMungosPass.png").exists(),
                    "Missing StMungosPass sprite")

        # EN localization
        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["HealersSalts", "HealersSatchel", "TriageResolvedBuff",
                        "HexResidueNode", "VenomWoundNode", "CauldronSpillNode",
                        "Healer", "StMungosPass"]:
                self.check(f"EN localization: {key}",
                            key in en_text,
                            f"Missing {key} in EN HJSON")
            self.check("EN localization: StMungos system text",
                        "SpellWardStart" in en_text and "CreatureWardStart" in en_text
                        and "PotionWardStart" in en_text and "TriageButton" in en_text,
                        "Missing StMungos system text in EN HJSON")
            # Healer dialogue
            self.check("EN localization: Healer dialogue",
                        "Dialogue" in en_text and "Healer" in en_text
                        and "Standard1" in en_text,
                        "Missing Healer dialogue in EN HJSON")

        # ZH localization
        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            for key in ["HealersSalts", "HealersSatchel", "TriageResolvedBuff",
                        "HexResidueNode", "VenomWoundNode", "CauldronSpillNode",
                        "Healer"]:
                self.check(f"ZH localization: {key}",
                            key in zh_text,
                            f"Missing {key} in ZH HJSON")
            self.check("ZH localization: StMungos system text",
                        "SpellWardStart" in zh_text and "TriageButton" in zh_text,
                        "Missing StMungos system text in ZH HJSON")

        # Guide content: St Mungo's system entry
        gc_path = self.root / "scripts" / "guide_content.json"
        if gc_path.exists():
            gc_text = gc_path.read_text(encoding="utf-8", errors="replace")
            self.check("Guide mentions St Mungo's Triage",
                        "St Mungo" in gc_text and "Triage" in gc_text,
                        "Missing St Mungo's system in guide_content.json")
            self.check("Guide mentions Healer NPC",
                        "Healer" in gc_text and "Medical Shop" in gc_text,
                        "Missing Healer NPC row in guide_content.json")
            # Safety: no resurrection claims in St Mungo's text
            import re as _re
            stmungos_resurrect = any(
                "mungo" in gc_text.lower()[max(0, m.start()-120):m.start()]
                for m in _re.finditer("resurrect", gc_text.lower())
                if "not resurrection" not in gc_text.lower()[max(0, m.start()-30):m.end()+5]
            )
            self.check("St Mungo's does NOT promise resurrection in guide",
                        not stmungos_resurrect,
                        "Guide text must not claim St Mungo's enables resurrection")

        # Healer shop export
        shops_path = self.root / "scripts" / "mechanical_data" / "shops.json"
        if shops_path.exists():
            import json as _json
            with open(shops_path, encoding="utf-8") as f:
                shops = _json.load(f)
            healer_shops = [s for s in shops if "Healer" in s.get("npc", "")]
            self.check("Healer shop in mechanical export",
                        len(healer_shops) > 0,
                        "Healer not found in shops.json")

    def verify_pdf_generator(self):
        """Check the PDF generator for consistency with the manifest."""
        print("\n=== PDF Generator Consistency ===")
        gen_path = self.root / "generate_english_pdf.py"
        if not gen_path.exists():
            self.warn("generate_english_pdf.py", "not found, skipping PDF checks")
            return

        text = gen_path.read_text(encoding="utf-8", errors="replace")
        s = self.summary

        # Check if generator uses hardcoded path
        self.check("No hardcoded Windows desktop path in generator",
                    r"C:\Users" not in text.replace("\\\\", "/").replace("\\", "/") or
                    "os.path.dirname(__file__)" in text or "__file__" in text,
                    "Generator still uses hardcoded local path")

        # Check for misleading "auto-generated from source code" claim
        if "auto-generated from source code" in text.lower() or "Auto-generated from source" in text:
            has_manifest_read = "content_manifest" in text or "json.load" in text or "manifest" in text.lower()
            if not has_manifest_read:
                self.warn("Misleading auto-generated claim",
                          "Generator claims 'auto-generated from source code' but does not read a manifest")

    def verify_thestral_and_lupin(self):
        """Verify Thestral Clearing, Lupin, Shack full-moon, passage network, and tri-language loc."""
        print("\n=== Thestral Clearing / Lupin / Shack Full-Moon Verification ===")

        # 1. ThestralBeacon.cs exists and is non-hostile
        beacon_path = self.root / "Content" / "NPCs" / "Enemies" / "ThestralBeacon.cs"
        if beacon_path.exists():
            text = beacon_path.read_text(encoding="utf-8", errors="replace")
            self.check("ThestralBeacon.cs exists", True)
            self.check("ThestralBeacon is friendly (non-hostile)",
                        "NPC.friendly = true" in text,
                        "ThestralBeacon should be friendly/non-hostile")
            self.check("ThestralBeacon has no damage",
                        "NPC.damage = 0" in text,
                        "ThestralBeacon should deal no damage")
            self.check("ThestralBeacon hidden from bestiary",
                        "Hide = true" in text,
                        "Objective NPCs should be hidden from bestiary")
        else:
            self.check("ThestralBeacon.cs exists", False, "File not found")

        # 2. SpectralEssence.cs exists
        essence_path = self.root / "Content" / "Items" / "Consumables" / "SpectralEssence.cs"
        self.check("SpectralEssence.cs exists", essence_path.exists(),
                    "Missing SpectralEssence crafting material")

        # 3. ForestExpeditionSystem has 4 loops
        forest_path = self.root / "Common" / "Systems" / "ForestExpeditionSystem.cs"
        if forest_path.exists():
            text = forest_path.read_text(encoding="utf-8", errors="replace")
            self.check("Forest expedition has 4 loops (mod 4)",
                        "expeditionsCompleted % 4" in text,
                        "Should rotate among 4 loops, not 3")
            self.check("Forest expedition references ThestralBeacon",
                        "ThestralBeacon" in text,
                        "Loop 3 should spawn ThestralBeacon")
            self.check("Forest expedition references SpectralEssence",
                        "SpectralEssence" in text,
                        "Loop 3 should reward SpectralEssence")
            self.check("Thestral loop uses NightOwl (non-hostile atmosphere)",
                        "NightOwl" in text,
                        "Thestral clearing should grant NightOwl, not hostile debuffs")

        # 4. ShriekingShackSystem full-moon enhancement
        shack_path = self.root / "Common" / "Systems" / "ShriekingShackSystem.cs"
        if shack_path.exists():
            text = shack_path.read_text(encoding="utf-8", errors="replace")
            self.check("Shack system checks moonPhase for full moon",
                        "Main.moonPhase == 0" in text,
                        "Should check moon phase for full-moon authenticity")
            self.check("Shack system references FullMoonWarning localization",
                        "FullMoonWarning" in text,
                        "Should display full moon warning text")
            self.check("Shack full-moon gives bonus MoonsilverThread",
                        text.count("MoonsilverThread") >= 2,
                        "Should give bonus MoonsilverThread on full moon completion")
            self.check("Shack is NOT framed as tourism",
                        "tourist" not in text.lower() and "tourism" not in text.lower(),
                        "Shrieking Shack should not be framed as a tourist destination")

        # 5. Lupin NPC exists and is properly framed
        lupin_path = self.root / "Content" / "NPCs" / "Town" / "Lupin.cs"
        if lupin_path.exists():
            text = lupin_path.read_text(encoding="utf-8", errors="replace")
            self.check("Lupin.cs exists", True)
            self.check("Lupin is a town NPC",
                        "NPC.townNPC = true" in text,
                        "Lupin should be a town NPC")
            self.check("Lupin references SecretPassageSystem",
                        "SecretPassageSystem" in text,
                        "Lupin should be passage network hub")
            self.check("Lupin sells WolfsbanePotion",
                        "WolfsbanePotion" in text,
                        "Lupin should sell Wolfsbane Potion")
            self.check("Wolfsbane is sold, not described as a cure",
                        "cure" not in text.lower(),
                        "Wolfsbane manages lycanthropy, it is not a cure")
        else:
            self.check("Lupin.cs exists", False, "File not found")

        # 6. Lupin sprites exist
        self.check("Lupin.png exists",
                    (self.root / "Content" / "NPCs" / "Town" / "Lupin.png").exists())
        self.check("Lupin_Head.png exists",
                    (self.root / "Content" / "NPCs" / "Town" / "Lupin_Head.png").exists())

        # 7. MaraudersMap enhanced tooltip
        map_path = self.root / "Content" / "Items" / "Accessories" / "MaraudersMap.cs"
        if map_path.exists():
            text = map_path.read_text(encoding="utf-8", errors="replace")
            self.check("MaraudersMap.cs exists", True)

        # 8. Passage network system exists
        passage_path = self.root / "Common" / "Systems" / "SecretPassageSystem.cs"
        self.check("SecretPassageSystem.cs exists", passage_path.exists(),
                    "Missing passage network system")

        # 9. UnicornBlood safety -- should not encourage hunting
        ub_path = self.root / "Content" / "Items" / "Consumables" / "UnicornBlood.cs"
        if ub_path.exists():
            text = ub_path.read_text(encoding="utf-8", errors="replace")
            self.check("UnicornBlood does not encourage hunting",
                        "hunt" not in text.lower() or "NOT" in text or "never" in text.lower() or "protect" in text.lower(),
                        "UnicornBlood should not frame unicorn hunting positively")

        # 10. Tri-language localization checks
        loc_dir = self.root / "Localization"
        for lang, label in [("en-US", "EN"), ("zh-Hans", "ZH-Hans"), ("zh-Hant", "ZH-Hant")]:
            loc_path = loc_dir / f"{lang}_Mods.WizardingWorld.hjson"
            if loc_path.exists():
                text = loc_path.read_text(encoding="utf-8", errors="replace")
                self.check(f"{label}: ThestralStart key exists",
                            "ThestralStart" in text,
                            f"Missing ThestralStart in {lang}")
                self.check(f"{label}: ThestralComplete key exists",
                            "ThestralComplete" in text,
                            f"Missing ThestralComplete in {lang}")
                self.check(f"{label}: FullMoonWarning key exists",
                            "FullMoonWarning" in text,
                            f"Missing FullMoonWarning in {lang}")
                self.check(f"{label}: SpectralEssence item exists",
                            "SpectralEssence" in text,
                            f"Missing SpectralEssence in {lang}")
                self.check(f"{label}: Lupin DisplayName exists",
                            "Lupin.DisplayName" in text or "Lupin:" in text,
                            f"Missing Lupin in {lang}")
                self.check(f"{label}: PassageButton exists",
                            "PassageButton" in text,
                            f"Missing PassageButton in {lang}")
            else:
                self.warn(f"{label} localization file", "not found")

        # 11. Shops.json has Lupin
        shops_path = self.root / "scripts" / "mechanical_data" / "shops.json"
        if shops_path.exists():
            text = shops_path.read_text(encoding="utf-8", errors="replace")
            self.check("shops.json includes Lupin",
                        'Lupin' in text,
                        "Missing Lupin shop in shops.json")

        # 12. Guide content has 14 NPCs
        guide_path = self.root / "scripts" / "guide_content.json"
        if guide_path.exists():
            text = guide_path.read_text(encoding="utf-8", errors="replace")
            self.check("Guide content says Sixteen town NPCs",
                        "Sixteen" in text,
                        "Town NPC count should be Sixteen")
            self.check("Guide content mentions Thestral Clearing",
                        "Thestral Clearing" in text,
                        "Guide should describe the 4th expedition loop")

    def verify_da_resistance(self):
        """Verify Dumbledore's Army, WardAnchor, Aberforth, and resistance system."""
        print("\n=== Dumbledore's Army & Hogwarts Resistance ===")

        en_path = self.root / "Localization" / "en-US_Mods.WizardingWorld.hjson"
        zh_path = self.root / "Localization" / "zh-Hans_Mods.WizardingWorld.hjson"
        hant_path = self.root / "Localization" / "zh-Hant_Mods.WizardingWorld.hjson"

        # Aberforth NPC (not a full town NPC .cs yet, but localization + shop)
        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            self.check("EN localization: Aberforth DisplayName",
                        "Aberforth" in en_text and "DisplayName" in en_text,
                        "Missing Aberforth DisplayName in EN HJSON")

        # WardAnchor NPC
        self.check("WardAnchor.cs exists",
                    (self.root / "Content" / "NPCs" / "Enemies" / "WardAnchor.cs").exists(),
                    "Missing WardAnchor NPC")
        self.check("Sprite exists: WardAnchor.png",
                    (self.root / "Content" / "NPCs" / "Enemies" / "WardAnchor.png").exists(),
                    "Missing WardAnchor sprite")

        # HogwartsWardSystem
        self.check("HogwartsWardSystem.cs exists",
                    (self.root / "Common" / "Systems" / "HogwartsWardSystem.cs").exists(),
                    "Missing Hogwarts Ward System")

        # DAGalleon
        self.check("DAGalleon.cs exists",
                    (self.root / "Content" / "Items" / "Consumables" / "DAGalleon.cs").exists(),
                    "Missing DAGalleon item")
        self.check("Sprite exists: DAGalleon.png",
                    (self.root / "Content" / "Items" / "Consumables" / "DAGalleon.png").exists(),
                    "Missing DAGalleon sprite")

        # CastleWardThread
        self.check("CastleWardThread.cs exists",
                    (self.root / "Content" / "Items" / "Consumables" / "CastleWardThread.cs").exists(),
                    "Missing CastleWardThread item")
        self.check("Sprite exists: CastleWardThread.png",
                    (self.root / "Content" / "Items" / "Consumables" / "CastleWardThread.png").exists(),
                    "Missing CastleWardThread sprite")

        # DefendersSignet
        self.check("DefendersSignet.cs exists",
                    (self.root / "Content" / "Items" / "Accessories" / "DefendersSignet.cs").exists(),
                    "Missing DefendersSignet accessory")
        self.check("Sprite exists: DefendersSignet.png",
                    (self.root / "Content" / "Items" / "Accessories" / "DefendersSignet.png").exists(),
                    "Missing DefendersSignet sprite")

        # Buff sprites
        for buff in ["DAMeetingBuff", "ResistanceResolveBuff", "RoomResistanceBuff"]:
            self.check(f"Sprite exists: {buff}.png",
                        (self.root / "Content" / "Buffs" / f"{buff}.png").exists(),
                        f"Missing {buff}.png")

        # EN localization checks
        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["DAGalleon", "CastleWardThread", "DefendersSignet",
                        "DAMeetingBuff", "ResistanceResolveBuff", "RoomResistanceBuff",
                        "WardAnchor"]:
                self.check(f"EN localization: {key}",
                            key in en_text,
                            f"Missing {key} in EN HJSON")

            # DA system text
            self.check("EN localization: DA system text",
                        "RoomResistance" in en_text and "DefenseStart" in en_text
                        and "WardHolds" in en_text,
                        "Missing DA system text in EN HJSON")

            # Aberforth dialogue
            self.check("EN localization: Aberforth dialogue",
                        "Aberforth" in en_text and "Hog" in en_text,
                        "Missing Aberforth dialogue in EN HJSON")

            # Aberforth bestiary
            self.check("EN localization: Aberforth bestiary",
                        "Aberforth" in en_text and "Hog's Head" in en_text,
                        "Missing Aberforth bestiary in EN HJSON")

            # DA NOT framed as weapon
            self.check("DA NOT framed as weapon",
                        "DANotWeapon" in en_text or "defensive organization" in en_text,
                        "DA should be framed as defensive, not offensive")

            # Hog's Head NOT luxury inn
            self.check("Hog's Head NOT luxury inn",
                        "luxury inn" not in en_text.lower().replace("not a luxury inn", "")
                        or "Not a luxury inn" in en_text,
                        "Hog's Head should not be framed as luxury")

            # Resistance NOT tournament
            self.check("Resistance NOT tournament",
                        "ResistanceNotTournament" in en_text or "no tournament" in en_text,
                        "Resistance should not be framed as tournament")

        # ZH localization
        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            for key in ["DAGalleon", "CastleWardThread", "DefendersSignet",
                        "DAMeetingBuff", "ResistanceResolveBuff", "RoomResistanceBuff",
                        "WardAnchor", "Aberforth"]:
                self.check(f"ZH-Hans localization: {key}",
                            key in zh_text,
                            f"Missing {key} in zh-Hans HJSON")

        # ZH-Hant localization
        if hant_path.exists():
            hant_text = hant_path.read_text(encoding="utf-8", errors="replace")
            for key in ["DAGalleon", "CastleWardThread", "DefendersSignet",
                        "DAMeetingBuff", "ResistanceResolveBuff", "RoomResistanceBuff",
                        "WardAnchor", "Aberforth"]:
                self.check(f"ZH-Hant localization: {key}",
                            key in hant_text,
                            f"Missing {key} in zh-Hant HJSON")

        # Room resistance mode in RoomOfRequirementKey.cs
        room_path = self.root / "Content" / "Items" / "Consumables" / "RoomOfRequirementKey.cs"
        if room_path.exists():
            text = room_path.read_text(encoding="utf-8", errors="replace")
            self.check("Room of Requirement has resistance mode",
                        "RoomResistanceBuff" in text and "DAGalleon" in text,
                        "RoomOfRequirementKey.cs missing resistance HQ mode")

        # Guide content checks
        gc_path = self.root / "scripts" / "guide_content.json"
        if gc_path.exists():
            gc_text = gc_path.read_text(encoding="utf-8", errors="replace")
            self.check("Guide mentions DA / Hogwarts Resistance",
                        "Dumbledore's Army" in gc_text and "Hogwarts Resistance" in gc_text,
                        "Missing DA system in guide_content.json")
            self.check("Guide mentions Aberforth NPC",
                        "Aberforth" in gc_text,
                        "Missing Aberforth in guide_content.json")
            self.check("Guide says Sixteen town NPCs",
                        "Sixteen" in gc_text,
                        "Town NPC count should be Sixteen")
            self.check("Guide Room mentions resistance HQ",
                        "Resistance HQ" in gc_text,
                        "Room of Requirement guide should mention resistance mode")

        # Shops export
        shops_path = self.root / "scripts" / "mechanical_data" / "shops.json"
        if shops_path.exists():
            import json as _json
            with open(shops_path, encoding="utf-8") as f:
                shops = _json.load(f)
            aberforth_shops = [s for s in shops if "Aberforth" in s.get("npc", "")]
            self.check("Aberforth shop in mechanical export",
                        len(aberforth_shops) > 0,
                        "Aberforth not found in shops.json")

        # Wolfsbane not-cure check (re-verify)
        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            wolfsbane_cure = False
            for line in en_text.splitlines():
                if "Wolfsbane" in line and "cure" in line.lower():
                    if "not a cure" not in line.lower() and "no cure" not in line.lower():
                        wolfsbane_cure = True
            self.check("Wolfsbane NOT described as cure",
                        not wolfsbane_cure,
                        "Wolfsbane manages lycanthropy, not a cure")

        # UnicornBlood safety re-check
        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            self.check("UnicornBlood: no positive hunting",
                        "hunt unicorn" not in en_text.lower() and "kill unicorn" not in en_text.lower(),
                        "Must not encourage hunting/killing unicorns")

    def verify_battle_of_hogwarts(self):
        """Verify Battle of Hogwarts content: NPCs, items, sprites, localization, lore."""
        print("\n=== Battle of Hogwarts Verification ===")

        # 1. Core system file
        self.check("BattleOfHogwartsSystem.cs exists",
                    (self.root / "Common" / "Systems" / "BattleOfHogwartsSystem.cs").exists(),
                    "Missing battle system")

        # 2. Objective NPCs
        enemies_dir = self.root / "Content" / "NPCs" / "Enemies"
        self.check("CastleWardNode.cs exists",
                    (enemies_dir / "CastleWardNode.cs").exists(),
                    "Missing Phase 1 objective NPC")
        self.check("BreachSeal.cs exists",
                    (enemies_dir / "BreachSeal.cs").exists(),
                    "Missing Phase 2 objective NPC")

        # 3. Neville NPC
        town_dir = self.root / "Content" / "NPCs" / "Town"
        self.check("Neville.cs exists",
                    (town_dir / "Neville.cs").exists(),
                    "Missing Neville town NPC")

        # 4. HorcruxTracker item
        self.check("HorcruxTracker.cs exists",
                    (self.root / "Content" / "Items" / "Consumables" / "HorcruxTracker.cs").exists(),
                    "Missing Horcrux Tracker item")

        # 5. Sprites
        sprite_checks = [
            ("CastleWardNode.png", enemies_dir),
            ("BreachSeal.png", enemies_dir),
            ("CastleDefenseRune.png", self.root / "Content" / "Items" / "Consumables"),
            ("ProtectorsBadge.png", self.root / "Content" / "Items" / "Accessories"),
            ("CastleVictoryBuff.png", self.root / "Content" / "Buffs"),
            ("Neville.png", town_dir),
            ("Neville_Head.png", town_dir),
            ("HorcruxTracker.png", self.root / "Content" / "Items" / "Consumables"),
        ]
        for sprite, folder in sprite_checks:
            self.check(f"Sprite exists: {sprite}",
                        (folder / sprite).exists(),
                        f"Missing {sprite}")

        # 6. EN localization
        en_path = self.root / "Localization" / "en-US_Mods.WizardingWorld.hjson"
        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            for key in ["CastleWardNode", "BreachSeal", "Neville", "HorcruxTracker",
                        "CastleDefenseRune", "ProtectorsBadge", "CastleVictoryBuff",
                        "Battle"]:
                self.check(f"EN localization: {key}",
                            key in en_text,
                            f"Missing {key} in EN HJSON")

            # Battle text keys
            for key in ["Phase1Start", "NaginiSlain", "Victory", "VoldemortReady",
                        "TrackerTitle", "VoldemortPower"]:
                self.check(f"EN Battle text: {key}",
                            key in en_text,
                            f"Missing Battle.{key} in EN HJSON")

        # 7. ZH-Hans localization
        zh_path = self.root / "Localization" / "zh-Hans_Mods.WizardingWorld.hjson"
        if zh_path.exists():
            zh_text = zh_path.read_text(encoding="utf-8", errors="replace")
            for key in ["CastleWardNode", "BreachSeal", "Neville", "HorcruxTracker",
                        "CastleDefenseRune", "ProtectorsBadge", "CastleVictoryBuff"]:
                self.check(f"ZH-Hans localization: {key}",
                            key in zh_text,
                            f"Missing {key} in zh-Hans HJSON")

        # 8. ZH-Hant localization
        hant_path = self.root / "Localization" / "zh-Hant_Mods.WizardingWorld.hjson"
        if hant_path.exists():
            hant_text = hant_path.read_text(encoding="utf-8", errors="replace")
            for key in ["CastleWardNode", "BreachSeal", "Neville", "HorcruxTracker",
                        "CastleDefenseRune", "ProtectorsBadge", "CastleVictoryBuff"]:
                self.check(f"ZH-Hant localization: {key}",
                            key in hant_text,
                            f"Missing {key} in zh-Hant HJSON")

        # 9. Guide content
        gc_path = self.root / "scripts" / "guide_content.json"
        if gc_path.exists():
            gc_text = gc_path.read_text(encoding="utf-8", errors="replace")
            self.check("Guide mentions Battle of Hogwarts",
                        "Battle of Hogwarts" in gc_text,
                        "Missing Battle system in guide_content.json")
            self.check("Guide mentions Neville NPC",
                        "Neville" in gc_text,
                        "Missing Neville in guide_content.json")

        # 10. Lore integrity checks
        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")

            # Battle NOT framed as tournament/festival
            battle_section = ""
            in_battle = False
            for line in en_text.splitlines():
                if "Battle:" in line or "Battle :" in line:
                    in_battle = True
                if in_battle:
                    battle_section += line + "\n"
                    if line.strip() == "}":
                        in_battle = False

            self.check("Battle NOT framed as tournament",
                        "tournament" not in battle_section.lower() and
                        "festival" not in battle_section.lower(),
                        "Battle text should not use tournament/festival framing")

            # Nagini framed as final Horcrux
            self.check("Nagini framed as final Horcrux in battle text",
                        "final" in battle_section.lower() and "horcrux" in battle_section.lower(),
                        "Battle should describe Nagini as the final Horcrux")

        # 11. No Hallows regression
        if en_path.exists():
            en_text = en_path.read_text(encoding="utf-8", errors="replace")
            self.check("No resurrection claims in battle text",
                        "resurrect the dead" not in en_text.lower(),
                        "Resurrection Stone does not literally resurrect")

        # 12. Horcrux diadem NOT casual accessory in battle text
        if gc_path.exists():
            gc_text = gc_path.read_text(encoding="utf-8", errors="replace")
            # Find battle section text
            battle_text = ""
            for line in gc_text.splitlines():
                if "Battle of Hogwarts" in line:
                    battle_text = line
            self.check("Diadem not treated as casual accessory in battle description",
                        "wear the diadem" not in battle_text.lower(),
                        "Diadem is a Horcrux to destroy, not a casual accessory")

        # 13. Shops export
        shops_path = self.root / "scripts" / "mechanical_data" / "shops.json"
        if shops_path.exists():
            import json as _json
            with open(shops_path, encoding="utf-8") as f:
                shops = _json.load(f)
            neville_shops = [s for s in shops if "Neville" in s.get("npc", "")]
            self.check("Neville shop in mechanical export",
                        len(neville_shops) > 0,
                        "Neville not found in shops.json")

    def verify_phase29_productionization(self):
        """Phase 29: Productionization, localization parity, presentation, tooling."""
        print("\n=== Phase 29 — Productionization Megapack ===")

        # 1. Tri-language localization parity
        en_path = self.root / "Localization" / "en-US_Mods.WizardingWorld.hjson"
        hans_path = self.root / "Localization" / "zh-Hans_Mods.WizardingWorld.hjson"
        hant_path = self.root / "Localization" / "zh-Hant_Mods.WizardingWorld.hjson"

        required_sections = [
            "Houses:", "DarkArts:", "RoomOfRequirement:", "OwlPost:",
            "HorcruxHunt:", "Azkaban:", "SceneEffects:", "QuidditchMatch:",
            "Transit:", "BattleEndgame:"
        ]

        for lang_name, lang_path in [("EN", en_path), ("ZH-Hans", hans_path), ("ZH-Hant", hant_path)]:
            if lang_path.exists():
                text = lang_path.read_text(encoding="utf-8", errors="replace")
                for section in required_sections:
                    self.check(f"{lang_name} has {section.rstrip(':')} section",
                               section in text,
                               f"Missing {section} in {lang_name}")

        # 2. House names localized (not hardcoded in GreatHallSystem)
        ghs_path = self.root / "Common" / "Systems" / "GreatHallSystem.cs"
        if ghs_path.exists():
            ghs_text = ghs_path.read_text(encoding="utf-8", errors="replace")
            self.check("House names use localization keys",
                        'Language.GetTextValue("Mods.WizardingWorld.Houses' in ghs_text,
                        "GetHouseName should use localization keys")
            self.check("No hardcoded Gryffindor string in GetHouseName",
                        '=> "Gryffindor"' not in ghs_text,
                        "House names should not be hardcoded English")

        # 3. HorcruxTracker uses localized names
        ht_path = self.root / "Content" / "Items" / "Consumables" / "HorcruxTracker.cs"
        if ht_path.exists():
            ht_text = ht_path.read_text(encoding="utf-8", errors="replace")
            self.check("HorcruxTracker uses localized Horcrux names",
                        "HorcruxHunt.DiaryName" in ht_text,
                        "Tracker should use localization keys for Horcrux names")

        # 4. Scene effects exist
        scene_path = self.root / "Common" / "Systems" / "WizardingSceneEffects.cs"
        self.check("ModSceneEffect implementations exist",
                    scene_path.exists(),
                    "WizardingSceneEffects.cs should exist")
        if scene_path.exists():
            scene_text = scene_path.read_text(encoding="utf-8", errors="replace")
            for effect in ["AzkabanSceneEffect", "BattleSceneEffect"]:
                self.check(f"{effect} class exists",
                            effect in scene_text,
                            f"Missing {effect}")

        # 5. Dark Arts corruption localized
        dac_path = self.root / "Common" / "Players" / "DarkArtsCorruptionPlayer.cs"
        if dac_path.exists():
            dac_text = dac_path.read_text(encoding="utf-8", errors="replace")
            self.check("Dark Arts warnings localized",
                        "DarkArts.Tier2Warning" in dac_text,
                        "Corruption warnings should use localization keys")

        # 6. OwlPost descriptions localized
        op_path = self.root / "Common" / "Systems" / "OwlPostSystem.cs"
        if op_path.exists():
            op_text = op_path.read_text(encoding="utf-8", errors="replace")
            self.check("OwlPost descriptions use localization keys",
                        "OwlPost.Desc" in op_text,
                        "Request descriptions should be localized")
            self.check("OwlPost no hardcoded delivery text",
                        '"Owl Post delivery complete' not in op_text,
                        "Delivery text should use localization")

        # 7. QuidditchEvent uses localization
        qe_path = self.root / "Common" / "Systems" / "QuidditchEvent.cs"
        if qe_path.exists():
            qe_text = qe_path.read_text(encoding="utf-8", errors="replace")
            self.check("QuidditchEvent uses localization",
                        "Language.GetTextValue" in qe_text,
                        "Snitch event text should be localized")

        # 8. AzkabanDespairEvent localized
        ade_path = self.root / "Common" / "Systems" / "AzkabanDespairEvent.cs"
        if ade_path.exists():
            ade_text = ade_path.read_text(encoding="utf-8", errors="replace")
            self.check("Azkaban event start text localized",
                        "Azkaban.EventStart1" in ade_text,
                        "Event start should use localization keys")

        # 9. HorcruxHuntSystem localized
        hh_path = self.root / "Common" / "Systems" / "HorcruxHuntSystem.cs"
        if hh_path.exists():
            hh_text = hh_path.read_text(encoding="utf-8", errors="replace")
            self.check("Horcrux destruction text localized",
                        "HorcruxHunt.DiaryFlavor" in hh_text,
                        "Destruction flavor text should use localization")
            self.check("Nagini defeat text localized",
                        "HorcruxHunt.NaginiFalls" in hh_text,
                        "Nagini text should use localization keys")

        # 10. Transit system framing exists
        sp_path = self.root / "Common" / "Systems" / "SecretPassageSystem.cs"
        if sp_path.exists():
            sp_text = sp_path.read_text(encoding="utf-8", errors="replace")
            self.check("Secret passages have transit framing",
                        "Transit." in sp_text,
                        "Passage system should use Transit localization keys")

        # 11. Battle endgame framing
        bh_path = self.root / "Common" / "Systems" / "BattleOfHogwartsSystem.cs"
        if bh_path.exists():
            bh_text = bh_path.read_text(encoding="utf-8", errors="replace")
            self.check("Battle has McGonagall rally text",
                        "McGonagallRally" in bh_text,
                        "Battle should have castle defense flavor")
            self.check("Battle has Nagini phase text",
                        "NaginiPhase" in bh_text,
                        "Battle should have explicit Nagini subphase")

        # 12. CastleDefenseBuff exists
        cdb_path = self.root / "Content" / "Buffs" / "CastleDefenseBuff.cs"
        self.check("CastleDefenseBuff file exists",
                    cdb_path.exists(),
                    "Castle defense buff should exist for Battle of Hogwarts")

        # 13. Schema coverage expanded
        for schema_name in ["mechanical_enemies", "mechanical_accessories", "mechanical_shops"]:
            schema_path = self.root / "schemas" / f"{schema_name}.schema.json"
            self.check(f"Schema {schema_name} exists",
                        schema_path.exists(),
                        f"Missing {schema_name}.schema.json")

        # 14. Asset backlog documented
        backlog_path = self.root / "docs" / "ASSET_BACKLOG.md"
        self.check("Asset backlog manifest exists",
                    backlog_path.exists(),
                    "docs/ASSET_BACKLOG.md should document sprite status")

        # 15. Zh-Hant retroactive coverage (previously missing sections)
        if hant_path.exists():
            hant_text = hant_path.read_text(encoding="utf-8", errors="replace")
            for section in ["Triwizard:", "Ministry:", "Gringotts:", "DiagonAlley:",
                           "StMungos:", "Knockturn:", "Grimmauld:"]:
                self.check(f"ZH-Hant has retroactive {section.rstrip(':')} section",
                            section in hant_text,
                            f"Missing {section} in zh-Hant (pre-Phase-24 gap)")

        # 16. Chinese PDF uses manifest variables
        zh_pdf_path = self.root / "generate_chinese_pdf.py"
        if zh_pdf_path.exists():
            zh_pdf_text = zh_pdf_path.read_text(encoding="utf-8", errors="replace")
            self.check("Chinese PDF uses manifest for wand count",
                        "S['wands_base_combat']" in zh_pdf_text or "S[\"wands_base_combat\"]" in zh_pdf_text,
                        "Chinese PDF should derive counts from manifest, not inline")

        # 17. RoomOfRequirement localized
        ror_path = self.root / "Content" / "Items" / "Consumables" / "RoomOfRequirementKey.cs"
        if ror_path.exists():
            ror_text = ror_path.read_text(encoding="utf-8", errors="replace")
            self.check("Room of Requirement names localized",
                        "RoomOfRequirement.Restoration" in ror_text,
                        "Room names should use localization keys")

    def verify_phase30_landmarks_and_sweep(self):
        """Phase 30: landmark tile wiring, filter registration, localization sweep, Chinese parity."""
        print("\n=== Phase 30 — Landmarks, Filters, Localization Sweep ===")

        # 1. Scene filter registration system exists with all 5 filters
        wfs_path = self.root / "Common" / "Systems" / "WizardingFilterSystem.cs"
        self.check("WizardingFilterSystem.cs exists",
                    wfs_path.exists(),
                    "Filter registration module required for visible scene tints")
        if wfs_path.exists():
            wfs_text = wfs_path.read_text(encoding="utf-8", errors="replace")
            for fkey in ["AzkabanDespair", "BattleOfHogwarts", "GringottsDescent",
                         "KnockturnAlley", "ShriekingShack"]:
                self.check(f"Filter registered: WizardingWorld:{fkey}",
                            f"WizardingWorld:{fkey}" in wfs_text,
                            f"Filters.Scene key for {fkey} missing")
            self.check("WizardingAmbientSystem exists",
                        "WizardingAmbientSystem" in wfs_text,
                        "Ambient particle system missing")

        # 2. Scene effect filter-key symmetry (ManageSpecialBiomeVisuals must match Filters.Scene keys)
        wse_path = self.root / "Common" / "Systems" / "WizardingSceneEffects.cs"
        if wse_path.exists() and wfs_path.exists():
            wse_text = wse_path.read_text(encoding="utf-8", errors="replace")
            for fkey in ["AzkabanDespair", "BattleOfHogwarts", "GringottsDescent",
                         "KnockturnAlley", "ShriekingShack"]:
                self.check(f"Scene effect uses filter key: {fkey}",
                            f'"WizardingWorld:{fkey}"' in wse_text,
                            f"ManageSpecialBiomeVisuals call for {fkey} missing or mis-named")

        # 3. All 12 landmark tiles exist
        landmark_tiles = [
            "HousePointHourglass", "QuidditchGoalpost", "CastleWardStone",
            "WhompingWillowStump", "ShriekingShackSign", "GrimmauldDoorway",
            "LeakyCauldronSign", "GringottsFacade", "BorginStorefront",
            "StMungosMannequin", "ProphecyShelf", "VeilArch",
        ]
        tiles_dir = self.root / "Content" / "Tiles" / "Landmarks"
        for lm in landmark_tiles:
            self.check(f"Landmark tile: {lm}.cs",
                        (tiles_dir / f"{lm}.cs").exists(),
                        f"Missing {lm}.cs tile class")

        # 4. All 12 landmark placement items exist with recipes
        items_dir = self.root / "Content" / "Items" / "Placeable"
        for lm in landmark_tiles:
            item_path = items_dir / f"{lm}Item.cs"
            self.check(f"Landmark item: {lm}Item.cs",
                        item_path.exists(),
                        f"Missing {lm}Item.cs placement class")
            if item_path.exists():
                itext = item_path.read_text(encoding="utf-8", errors="replace")
                self.check(f"Landmark recipe: {lm}Item uses EnchantingTable",
                            "Tiles.EnchantingTable" in itext,
                            f"{lm}Item should craft at Enchanting Table")

        # 5. All 12 landmark sprites exist (procedural placeholders — see ASSET_BACKLOG.md)
        for lm in landmark_tiles:
            self.check(f"Landmark tile PNG: Content/Tiles/Landmarks/{lm}.png",
                        (tiles_dir / f"{lm}.png").exists(),
                        f"Sprite missing — run generate_sprites.py")
            self.check(f"Landmark item PNG: Content/Items/Placeable/{lm}Item.png",
                        (items_dir / f"{lm}Item.png").exists(),
                        f"Sprite missing — run generate_sprites.py")

        # 6. Landmark localization in all 3 languages (MapEntry + Item + Tooltip)
        en_path = self.root / "Localization" / "en-US_Mods.WizardingWorld.hjson"
        hans_path = self.root / "Localization" / "zh-Hans_Mods.WizardingWorld.hjson"
        hant_path = self.root / "Localization" / "zh-Hant_Mods.WizardingWorld.hjson"
        for lang_name, lang_path in [("EN", en_path), ("ZH-Hans", hans_path), ("ZH-Hant", hant_path)]:
            if not lang_path.exists():
                continue
            lang_text = lang_path.read_text(encoding="utf-8", errors="replace")
            for lm in landmark_tiles:
                self.check(f"{lang_name} has {lm}.MapEntry",
                            f"{lm}.MapEntry" in lang_text,
                            f"Map entry localization missing for {lm} in {lang_name}")
                self.check(f"{lang_name} has {lm}Item DisplayName",
                            f"{lm}Item:" in lang_text or f"{lm}Item: " in lang_text,
                            f"Item localization missing for {lm} in {lang_name}")

        # 7. Phase 30 new localization sections exist in all 3 languages
        phase30_sections = ["DailyChallenge:", "HogwartsLetterEvent:", "Renown:", "Resurrection:"]
        for lang_name, lang_path in [("EN", en_path), ("ZH-Hans", hans_path), ("ZH-Hant", hant_path)]:
            if not lang_path.exists():
                continue
            lang_text = lang_path.read_text(encoding="utf-8", errors="replace")
            for section in phase30_sections:
                self.check(f"{lang_name} has Phase 30 {section.rstrip(':')} section",
                            section in lang_text,
                            f"{section} missing in {lang_name}")

        # 8. Hardcoded strings targeted by Phase 30 sweep are gone
        sweep_targets = [
            ("DailyChallengeSystem.cs", "Common/Systems", '"Daily Wizard Challenge COMPLETE'),
            ("HogwartsLetterSystem.cs", "Common/Systems", '"An owl swoops down'),
            ("HogwartsLetterSystem.cs", "Common/Systems", '"You\'ve been accepted'),
            ("HouseRenownSystem.cs", "Common/Systems", '"HOUSE CHAMPION!'),
            ("HouseRenownSystem.cs", "Common/Systems", '"150 House Renown'),
            ("HouseRenownSystem.cs", "Common/Systems", '"50 House Renown'),
            ("ResurrectionBuff.cs", "Content/Buffs", '"You rise from the ashes!'),
        ]
        for fname, subdir, needle in sweep_targets:
            fpath = self.root / subdir / fname
            if not fpath.exists():
                continue
            ftext = fpath.read_text(encoding="utf-8", errors="replace")
            self.check(f"Hardcoded removed: {fname} :: {needle[:35]}...",
                        needle not in ftext,
                        f"Phase 30 sweep target still present as hardcoded string")

        # 9. Chinese PDF loads translations from external JSON (not inline dicts)
        zh_pdf_path = self.root / "generate_chinese_pdf.py"
        zh_json_path = self.root / "scripts" / "zh_translations.json"
        self.check("zh_translations.json exists",
                    zh_json_path.exists(),
                    "Curated Chinese translation JSON missing")
        if zh_pdf_path.exists():
            zh_pdf_text = zh_pdf_path.read_text(encoding="utf-8", errors="replace")
            self.check("generate_chinese_pdf.py loads zh_translations.json",
                        "zh_translations.json" in zh_pdf_text,
                        "Chinese generator should source translations from JSON (Phase 30 refactor)")
            # Sanity: the old inline dict definitions should be gone or at least pointing at _ZH
            self.check("No inline WAND_NAMES_ZH literal in Chinese generator",
                        'WAND_NAMES_ZH = {\n    "OakWand"' not in zh_pdf_text,
                        "Inline Chinese dict should be loaded from JSON")

        # 10. Local release manifest generator exists
        local_manifest_path = self.root / "scripts" / "generate_release_manifest.py"
        self.check("Local release manifest generator exists",
                    local_manifest_path.exists(),
                    "scripts/generate_release_manifest.py should allow local release snapshots")

        # 11. ASSET_BACKLOG.md mentions landmarks honestly
        backlog_path = self.root / "docs" / "ASSET_BACKLOG.md"
        if backlog_path.exists():
            backlog_text = backlog_path.read_text(encoding="utf-8", errors="replace")
            self.check("Asset backlog lists Phase 30 landmarks",
                        "Phase 30 Landmark" in backlog_text or "Phase 30 landmark" in backlog_text.lower(),
                        "Backlog should document new landmark placeholder assets")
            self.check("Asset backlog documents audio gap",
                        "Audio Gap" in backlog_text or "No audio" in backlog_text or "no custom audio" in backlog_text.lower(),
                        "Backlog should honestly state that no audio exists")

        # 12. Canon guardrails — Veil Arch must stay Death Chamber, NOT resurrection
        veil_item_path = self.root / "Content" / "Items" / "Placeable" / "VeilArchItem.cs"
        if veil_item_path.exists():
            veil_text = veil_item_path.read_text(encoding="utf-8", errors="replace")
            self.check("Veil Arch lore stays Death Chamber (no resurrection)",
                        "resurrection" not in veil_text.lower() and "revive" not in veil_text.lower(),
                        "Veil Arch must NOT have resurrection mechanics (Dept of Mysteries lock)")

        # 13. Canon guardrails — St Mungo's mannequin stays hospital-themed (no resurrection)
        mung_item_path = self.root / "Content" / "Items" / "Placeable" / "StMungosMannequinItem.cs"
        if mung_item_path.exists():
            mung_text = mung_item_path.read_text(encoding="utf-8", errors="replace")
            self.check("St Mungo's stays treatment-framed (no resurrection)",
                        "resurrection" not in mung_text.lower() and "revive" not in mung_text.lower(),
                        "St Mungo's must NOT be reframed as resurrection site")

    def report(self):
        print(f"\n{'='*50}")
        print(f"Results: {self.passes} passed, {self.fails} failed, {self.warnings} warnings")
        if self.fails == 0:
            print("All checks passed.")
        else:
            print(f"*** {self.fails} CHECK(S) FAILED ***")
        print(f"{'='*50}")
        return self.fails


def main():
    parser = argparse.ArgumentParser(description="Verify guide consistency")
    parser.add_argument("--strict", action="store_true", help="Exit 1 on any failure")
    args = parser.parse_args()

    root = find_repo_root()
    manifest_path = root / "scripts" / "content_manifest.json"

    if not manifest_path.exists():
        sys.exit("ERROR: content_manifest.json not found. Run scan_content.py first.")

    with open(manifest_path, encoding="utf-8") as f:
        manifest = json.load(f)

    print("Wizarding World — Guide Verification")
    print(f"Manifest: {manifest_path}")

    v = Verifier(manifest, root)
    v.verify_manifest_self_consistency()
    v.verify_no_stale_references()
    v.verify_lore_state()
    v.verify_guide_content_semantic()
    v.verify_mechanical_schemas()
    v.verify_mechanical_consistency()
    v.verify_hogsmeade_closure()
    v.verify_st_mungos_closure()
    v.verify_pdf_generator()
    v.verify_thestral_and_lupin()
    v.verify_da_resistance()
    v.verify_battle_of_hogwarts()
    v.verify_phase29_productionization()
    v.verify_phase30_landmarks_and_sweep()
    fails = v.report()

    if args.strict and fails > 0:
        sys.exit(1)


if __name__ == "__main__":
    main()
