"""
Wizarding World - Complete Content Guide PDF Generator (English)

Counts and category totals are read from scripts/content_manifest.json
(produced by scripts/scan_content.py).  Narrative descriptions, boss
phase text, and curated tables are maintained in this file.

Usage:
    python scripts/scan_content.py          # refresh the manifest first
    python generate_english_pdf.py          # produce the guide PDF
    python generate_english_pdf.py --output path/to/out.pdf  # custom output
"""

import argparse
import json
import os
import sys
from fpdf import FPDF
from fpdf.enums import XPos, YPos

# ---------------------------------------------------------------------------
# Load the source-of-truth manifest
# ---------------------------------------------------------------------------
_SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
_MANIFEST_PATH = os.path.join(_SCRIPT_DIR, "scripts", "content_manifest.json")

if not os.path.exists(_MANIFEST_PATH):
    sys.exit(
        "ERROR: scripts/content_manifest.json not found.\n"
        "Run  python scripts/scan_content.py  first."
    )

with open(_MANIFEST_PATH, encoding="utf-8") as _f:
    MANIFEST = json.load(_f)

S = MANIFEST["summary"]  # shorthand for counts

# ---------------------------------------------------------------------------
# Load curated guide content
# ---------------------------------------------------------------------------
_GUIDE_CONTENT_PATH = os.path.join(_SCRIPT_DIR, "scripts", "guide_content.json")

if not os.path.exists(_GUIDE_CONTENT_PATH):
    sys.exit(
        "ERROR: scripts/guide_content.json not found.\n"
        "Run  python scripts/export_guide_data.py  first."
    )

with open(_GUIDE_CONTENT_PATH, encoding="utf-8") as _f:
    G = json.load(_f)  # shorthand for guide content


# ---------------------------------------------------------------------------
# Custom PDF class
# ---------------------------------------------------------------------------

class WizardPDF(FPDF):
    """Custom FPDF subclass with headers, footers, and helper methods."""

    DARK_PURPLE = (45, 0, 70)
    GOLD = (180, 150, 50)
    HEADER_BG = (45, 0, 70)
    HEADER_FG = (255, 255, 255)
    ROW_ALT = (235, 230, 245)
    ROW_NORMAL = (255, 255, 255)
    SECTION_BG = (60, 20, 90)
    SUBSECTION_COLOR = (80, 40, 120)
    BODY_COLOR = (30, 30, 30)
    ACCENT = (140, 90, 200)

    def __init__(self):
        super().__init__(orientation="P", unit="mm", format="A4")
        self.set_auto_page_break(auto=True, margin=20)
        self._is_title_page = False

    def header(self):
        if self._is_title_page:
            return
        self.set_font("Helvetica", "B", 9)
        self.set_text_color(*self.DARK_PURPLE)
        self.cell(0, 6, "Wizarding World  -  Complete Content Guide", align="L")
        self.ln(2)
        self.set_draw_color(*self.ACCENT)
        self.set_line_width(0.4)
        self.line(10, self.get_y(), 200, self.get_y())
        self.ln(5)

    def footer(self):
        if self._is_title_page:
            return
        self.set_y(-15)
        self.set_font("Helvetica", "I", 8)
        self.set_text_color(120, 120, 120)
        self.cell(0, 10, f"Page {self.page_no()}", align="C")

    def section_title(self, text):
        self.set_font("Helvetica", "B", 16)
        self.set_fill_color(*self.SECTION_BG)
        self.set_text_color(255, 255, 255)
        self.cell(0, 10, f"  {text}", new_x=XPos.LMARGIN, new_y=YPos.NEXT, fill=True)
        self.ln(3)
        self.set_text_color(*self.BODY_COLOR)

    def subsection(self, text):
        self.set_font("Helvetica", "B", 12)
        self.set_text_color(*self.SUBSECTION_COLOR)
        self.cell(0, 7, text, new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        self.ln(1)
        self.set_text_color(*self.BODY_COLOR)

    def body_text(self, text):
        self.set_font("Helvetica", "", 10)
        self.set_text_color(*self.BODY_COLOR)
        self.multi_cell(0, 5, text)
        self.ln(1)

    def small_text(self, text):
        self.set_font("Helvetica", "", 9)
        self.set_text_color(60, 60, 60)
        self.multi_cell(0, 4.5, text)
        self.ln(1)

    def bullet(self, text):
        self.set_font("Helvetica", "", 10)
        self.set_text_color(*self.BODY_COLOR)
        self.set_x(10)
        self.multi_cell(190, 5, "  - " + text)

    def table(self, headers, rows, col_widths=None):
        if col_widths is None:
            n = len(headers)
            col_widths = [190 / n] * n

        line_height = 5.5

        def draw_header():
            self.set_font("Helvetica", "B", 7.5)
            self.set_fill_color(*self.HEADER_BG)
            self.set_text_color(*self.HEADER_FG)
            self.set_draw_color(180, 180, 180)
            for i, h in enumerate(headers):
                self.cell(col_widths[i], 7, h, border=1, fill=True, align="C")
            self.ln()

        draw_header()

        self.set_font("Helvetica", "", 7)
        self.set_text_color(*self.BODY_COLOR)
        for r_idx, row in enumerate(rows):
            if r_idx % 2 == 0:
                self.set_fill_color(*self.ROW_NORMAL)
            else:
                self.set_fill_color(*self.ROW_ALT)

            max_lines = 1
            for i, cell_text in enumerate(row):
                w = col_widths[i]
                text_width = self.get_string_width(str(cell_text))
                lines_needed = max(1, int(text_width / (w - 2)) + 1)
                max_lines = max(max_lines, lines_needed)

            row_h = line_height * max_lines

            if self.get_y() + row_h > 275:
                self.add_page()
                draw_header()
                self.set_font("Helvetica", "", 7)
                self.set_text_color(*self.BODY_COLOR)
                if r_idx % 2 == 0:
                    self.set_fill_color(*self.ROW_NORMAL)
                else:
                    self.set_fill_color(*self.ROW_ALT)

            x0 = self.get_x()
            y0 = self.get_y()
            for i, cell_text in enumerate(row):
                self.set_xy(x0 + sum(col_widths[:i]), y0)
                self.rect(x0 + sum(col_widths[:i]), y0, col_widths[i], row_h, "DF")
                self.set_xy(x0 + sum(col_widths[:i]) + 1, y0 + 0.5)
                self.multi_cell(col_widths[i] - 2, line_height, str(cell_text))
            self.set_xy(x0, y0 + row_h)

    def ensure_space(self, mm):
        if self.get_y() + mm > 275:
            self.add_page()


# ===========================================================================
# Build the PDF
# ===========================================================================

pdf = WizardPDF()

# ========================== TITLE PAGE =====================================
pdf._is_title_page = True
pdf.add_page()
pdf.set_fill_color(*WizardPDF.DARK_PURPLE)
pdf.rect(0, 0, 210, 297, "F")

pdf.ln(50)
pdf.set_font("Helvetica", "B", 42)
pdf.set_text_color(220, 190, 80)
pdf.cell(0, 18, "Wizarding World", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

pdf.ln(4)
pdf.set_font("Helvetica", "I", 18)
pdf.set_text_color(200, 200, 220)
pdf.cell(0, 10, "A Harry Potter Mod for Terraria", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

pdf.ln(10)
pdf.set_draw_color(180, 150, 50)
pdf.set_line_width(0.8)
pdf.line(50, pdf.get_y(), 160, pdf.get_y())

pdf.ln(12)
pdf.set_font("Helvetica", "B", 20)
pdf.set_text_color(255, 255, 255)
pdf.cell(0, 10, "Complete Content Guide", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

pdf.ln(8)
pdf.set_font("Helvetica", "", 13)
pdf.set_text_color(200, 200, 220)
stats_lines = [
    f"{S['cs_files']} C# Source Files  |  {S['png_sprites']} Sprites  |  {S['total_files']} Total Files",
    f"{S['bosses']} Bosses  |  {S['enemies']} Enemies  |  21 Fantastic Beasts",
    f"{S['wands_base_combat']} Wands + {S['wands_upgrades']} Upgrade  |  {S['armor_sets']} Armor Sets  |  {S['accessories']} Accessories",
    f"{S['potions']} Potions  |  {S['mounts']} Mounts  |  {S['pets']} Pets  |  {S['town_npcs']} Town NPCs",
    f"10 Custom Systems  |  {S['crafting_materials']} Crafting Materials",
]
for line in stats_lines:
    pdf.cell(0, 7, line, align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

pdf.ln(20)
pdf.set_font("Helvetica", "I", 11)
pdf.set_text_color(180, 170, 200)
pdf.cell(0, 7, "\"Nitwit! Blubber! Oddment! Tweak!\"", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)
pdf.cell(0, 6, "- Albus Dumbledore", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

pdf.ln(40)
pdf.set_font("Helvetica", "", 9)
pdf.set_text_color(140, 140, 160)
pdf.cell(0, 5, "Counts from source scan  |  Narrative curated  |  tModLoader 1.4.4+", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

pdf._is_title_page = False

# ========================== TABLE OF CONTENTS ==============================
pdf.add_page()
pdf.section_title("Table of Contents")
toc_items = [
    "1. Getting Started",
    f"2. Wands ({S['wands_base_combat']} + {S['wands_upgrades']} Upgrade)",
    "3. Bosses (12)",
    "4. Armor Sets (8)",
    "5. House Signature Weapons (4)",
    "6. Enemies (51)",
    f"7. Town NPCs ({S['town_npcs']})",
    f"8. Accessories ({S['accessories']})",
    f"9. Mounts ({S['mounts']}) & Pets ({S['pets']}) & Minions (3)",
    "10. Potions & Consumables (19)",
    "11. Custom Systems (10)",
    "12. Crafting Materials (18)",
    "13. Deathly Hallows Hidden Set Bonus",
    "14. Boss Progression Guide",
]
for item in toc_items:
    pdf.bullet(item)
pdf.ln(3)

# ========================== 1. GETTING STARTED =============================
pdf.add_page()
pdf.section_title("1. Getting Started")

gs = G["getting_started"]
pdf.subsection("The Hogwarts Letter")
pdf.body_text(gs["hogwarts_letter"])
pdf.subsection("The Enchanting Table")
pdf.body_text(gs["enchanting_table"])
pdf.subsection("The Wizard Tower")
pdf.body_text(gs["wizard_tower"])
pdf.subsection("Spell Damage Class")
pdf.body_text(gs["spell_damage"])
pdf.subsection("The Forbidden Forest Biome")
pdf.body_text(gs["forbidden_forest"])

# ========================== 2. WANDS =======================================
pdf.add_page()
pdf.section_title(f"2. Wands ({S['wands_base_combat']} + {S['wands_upgrades']} Upgrade)")

wands = G["wands"]
pdf.body_text(wands["intro"])

for tier_key in ["tier1", "tier2", "tier3", "tier4", "tier5"]:
    tier = wands["tiers"][tier_key]
    pdf.subsection(tier["name"])
    pdf.table(wands["headers"], tier["rows"], tier["col_widths"])
    pdf.ln(2)

pdf.small_text(wands["upgrade_note"])

# ========================== 3. BOSSES ======================================
pdf.add_page()
pdf.section_title("3. Bosses (12)")

bosses_data = G["bosses"]
pdf.body_text(bosses_data["intro"])

overview_rows = [[b["name"], b["tier"], b["hp"], b["dmg"], b["def"]] for b in bosses_data["list"]]
pdf.table(bosses_data["overview_headers"], overview_rows, bosses_data["overview_col_widths"])

pdf.ln(3)
for b in bosses_data["list"]:
    pdf.ensure_space(28)
    pdf.subsection(b["name"])
    pdf.small_text(f"Tier: {b['tier']}  |  HP: {b['hp']}  |  Damage: {b['dmg']}  |  Defense: {b['def']}")
    pdf.small_text(f"Phases: {b['phases']}")
    pdf.small_text(f"Drops: {b['drops']}")
    pdf.small_text(f"Expert Drop: {b['expert']}")
    pdf.ln(1)

# ========================== 4. ARMOR SETS ==================================
pdf.add_page()
pdf.section_title("4. Armor Sets (8)")

pdf.body_text(
    "Eight armor sets span the entire game progression. Four Hogwarts house sets are available "
    "early game, with advanced sets unlocking in Hardmode and endgame. The Wizard Robes set is "
    "vanity-only with no stats."
)

armor_sets = [
    ["Wizard Robes", "Vanity", "0 / 0 / 0", "0", "Vanity only. Classic wizard hat, robes, and boots.", "N/A"],
    ["Gryffindor", "Pre-HM", "5 / 7 / 6", "18", "+15% all damage, +10% melee speed", "Hood: +8% spell dmg. Robes: +20 max HP"],
    ["Slytherin", "Pre-HM", "4 / 6 / 5", "15", "+12% crit chance, 5% life steal on crits", "Poison-themed bonuses"],
    ["Ravenclaw", "Pre-HM", "3 / 5 / 4", "12", "+20% spell damage, +40 max mana, mana regen +5", "Hood: mana regen. Robes: -10% mana cost"],
    ["Hufflepuff", "Pre-HM", "7 / 9 / 7", "23", "+8 defense, +4 life regen, thorns", "Robes: +40 max HP. Tankiest house set"],
    ["Dark Wizard", "Hardmode", "8 / 10 / 8", "26", "+25% spell dmg, +15% crit, life steal on spell hits", "Hood: +15% mana regen. Robes: -15% mana cost"],
    ["Dragon Scale", "Post-Horntail", "12 / 16 / 12", "40", "Fire Shield: fire immunity + fire nova when hit", "Chest: +40 max HP. Legs: +5 mana regen"],
    ["Wizengamot", "Post-Moon Lord", "16 / 20 / 16", "52", "Arcane Authority: +30% spell dmg, -20% mana cost, +8 mana regen, arcane aura", "Hood: highest def. Robes: +60 max HP"],
]

pdf.table(
    ["Set", "Tier", "Def (H/B/L)", "Total", "Set Bonus", "Piece Notes"],
    armor_sets,
    [24, 20, 24, 12, 60, 50],
)

# ========================== 5. HOUSE SIGNATURE WEAPONS =====================
pdf.ln(4)
pdf.section_title("5. House Signature Weapons (4)")

pdf.body_text(
    "Each Hogwarts house has a unique signature weapon matching its identity. These are powerful "
    "melee/magic weapons designed to complement their house armor set."
)

pdf.table(
    ["Weapon", "House", "Type", "Dmg", "Use", "KB", "Special Effect"],
    [
        ["Sword of Gryffindor", "Gryffindor", "Melee Sword", "55", "18", "6", "Legendary goblin-made sword. Drops from Basilisk. Bonus damage on hit effects."],
        ["Hufflepuff's Mace", "Hufflepuff", "Melee Mace", "58", "28", "10", "Heavy smash. Heals on hit (10% dmg dealt, max 8 HP). Tank sustain weapon."],
        ["Ravenclaw's Staff", "Ravenclaw", "Magic Staff", "50", "18", "3", "Reduces mana cost while held. Hits restore mana. 5 mana cost. Wisdom conserves energy."],
        ["Slytherin's Dagger", "Slytherin", "Melee Dagger", "42", "10", "2", "Very fast attacks. +50% damage when hitting from behind. Backstab assassin weapon."],
    ],
    [32, 22, 22, 10, 10, 10, 84],
)

# ========================== 6. ENEMIES =====================================
pdf.add_page()
pdf.section_title("6. Enemies (51)")

enemies_data = G["enemies"]
pdf.body_text(enemies_data["intro"])

for biome, group in enemies_data["groups"].items():
    pdf.ensure_space(20)
    pdf.subsection(biome)
    pdf.table(enemies_data["headers"], group["rows"], group["col_widths"])
    pdf.ln(2)

# ========================== 7. TOWN NPCs ===================================
pdf.add_page()
pdf.section_title(f"7. Town NPCs ({S['town_npcs']})")  # from manifest

npcs = G["town_npcs"]
pdf.body_text(npcs["intro"])
pdf.table(npcs["headers"], npcs["rows"], npcs["col_widths"])

# ========================== 8. ACCESSORIES =================================
pdf.add_page()
pdf.section_title(f"8. Accessories ({S['accessories']})")

pdf.body_text(
    "53 accessories spanning vanity, utility, combat, and endgame power. Organized by category."
)

pdf.subsection("Deathly Hallows (3)")
pdf.table(
    ["Accessory", "Rarity", "Effect"],
    [
        ["Invisibility Cloak", "Red", "The unique Hallow cloak. Invisibility, -400 aggro, +5% damage."],
        ["Resurrection Stone", "Red", "+40 max HP, life regen. Awakened from Gaunt's Ring."],
        ["Elder Wand", "Red", "(Weapon) 150 dmg Avada Kedavra. Completes the Deathly Hallows set."],
    ],
    [38, 16, 136],
)

pdf.ln(2)
pdf.subsection("Horcruxes (4)")
pdf.table(
    ["Accessory", "Rarity", "Effect"],
    [
        ["Riddle's Diary", "Lt Purple", "+spell damage, but slowly drains 1 HP every 2 seconds."],
        ["Slytherin's Locket", "Lt Purple", "Poison immunity, +8% damage reduction, reduced speed."],
        ["Hufflepuff's Cup", "Lt Purple", "+60 max HP, life regen, potion effectiveness."],
        ["Diadem of Ravenclaw", "Lt Purple", "Massive mana boost, +spell power, -40 max HP."],
    ],
    [38, 16, 136],
)

pdf.ln(2)
pdf.subsection("Combat Accessories")
pdf.table(
    ["Accessory", "Rarity", "Effect"],
    [
        ["Dark Lord's Bane", "Endgame", "+15% all dmg, +10% spell, +10 def, +4 regen, poison/venom/fire immune"],
        ["Dementor's Embrace", "Endgame", "+15% all dmg, 5% life steal, but -30 max HP + Darkness. Enemies -3 def."],
        ["Master Wizard Banner", "Purple", "+10% all dmg, +5% spell, +8 def, +20 max mana, +2 life regen"],
        ["Triwizard Cup", "Yellow", "+15% all dmg, +10% crit, +8 def, +40 mana, +4 life regen"],
        ["Beast Hunter's Charm", "Yellow", "+12% dmg to enemies, +15% crit vs bosses"],
        ["Philosopher's Stone", "Endgame", "+10% all dmg, +8 life regen, extra gold drops, reduced potion cooldown"],
        ["Dragon Tooth Necklace", "Lt Purple", "+12% all dmg, +8 armor pen, fire trail while moving"],
        ["Dragon Scale Ring", "Mid", "Fire immunity, +8% spell dmg, +5% all dmg"],
        ["Venom Silk Wraps", "Lt Purple", "+10% spell dmg, all attacks inflict venom, +15% crit"],
        ["Quill of Acceptance", "Lt Purple", "+8% spell dmg, +5% crit, spells +5 armor pen"],
        ["Prongs Charm", "Mid", "+10% spell dmg, +5% crit, Patronus Guardian +25% dmg"],
        ["Wand Holster", "Mid", "+5% spell dmg, -5% mana cost"],
        ["Patronus Charm", "Mid", "Summon weapon (65 dmg). +10 def, +2 regen. 2x vs Dementors. 2 minion slots."],
        ["Gaunt's Ring", "Red", "100% drop from Voldemort. Cursed Horcrux shell hiding the Resurrection Stone."],
    ],
    [38, 16, 136],
)

pdf.ln(2)
pdf.subsection("Defense Accessories")
pdf.table(
    ["Accessory", "Rarity", "Effect"],
    [
        ["Keeper Gloves", "Orange", "+8 def, +10% KB resist, thorns (20% contact dmg reflected)"],
        ["Shield Hat", "Mid", "15% chance to negate incoming damage completely"],
        ["Cerberus Fang Necklace", "Pink", "+5 def, +3 max minions"],
        ["Prefect Badge", "Lt Red", "+5 def, +5% spell dmg, nearby town NPCs deal more dmg"],
        ["Werewolf Cloak", "Mid", "Night: +20% melee, +15% speed, +10 def. Day: +5% dmg, +5% speed"],
        ["Azkaban Warden's Key", "Lt Purple", "Dementor resistance, +8 def, immune to Dark Curse, prison ward aura."],
    ],
    [38, 16, 136],
)

pdf.ln(2)
pdf.subsection("Utility / Exploration Accessories")
pdf.table(
    ["Accessory", "Rarity", "Effect"],
    [
        ["Marauder's Map", "Orange", "Reveals all enemies and NPCs on minimap"],
        ["Remembrall", "Blue", "Glows red near enemies. Danger sense + spelunker effect"],
        ["Sneakoscope", "Orange", "Danger sense, hunter, +def when enemies within 400px"],
        ["Foe Glass", "Lt Red", "Reveals all enemies on minimap + boss HP % in tooltip"],
        ["Pensieve", "Lt Purple", "Full minimap reveal (GPS), enemy detection, night vision"],
        ["Moody's Eye", "Lt Purple", "Advanced detection and awareness"],
        ["Extendable Ears", "Green", "Wider enemy detection, Hunter effect, +50 aggro"],
        ["Deluminator", "Lt Purple", "Permanent Shine buff (glow) + Night Owl vision"],
        ["Spectrespecs", "Orange", "Luna Lovegood's glasses. Visual enhancement."],
        ["Boss Compass", "Mid", "Boss progression status in tooltip, +3% damage"],
        ["Enchanted Mirror", "Orange", "+3% spell damage, mirror utility"],
        ["Sorting Hat", "Orange", "Boosts set bonus of any Hogwarts house armor currently worn"],
    ],
    [38, 16, 136],
)

pdf.ln(2)
pdf.subsection("Mobility / Stealth Accessories")
pdf.table(
    ["Accessory", "Rarity", "Effect"],
    [
        ["Apparition Charm", "Yellow", "Teleport to cursor (Rod of Discord-like)"],
        ["Time Turner", "Lt Purple", "Dodge (once per 10s) + 10% movement speed"],
        ["Camouflage Cloak", "Lt Red", "-400 aggro, +10% speed, invisibility when still for 2s"],
        ["Spider Silk Cloak", "Lt Red", "-300 aggro, +10% speed, +8% spell dmg"],
        ["Padfoot Amulet", "Lt Purple", "Stealth: -200 aggro, enemies less likely to target you"],
        ["Dirigible Plum Earring", "Mid", "Luna's earring. Quirky mobility accessory."],
        ["Fairy Wings", "Pink", "Mid-game wing accessory. Flight."],
        ["Merfolk Amulet", "Mid", "Gills + flipper + 10% dmg while submerged"],
        ["Demiguise Weave Cloak", "Lt Purple", "Ordinary stealth cloak from Demiguise Hair. Invisibility, aggro reduction, +5% speed."],
    ],
    [38, 16, 136],
)

pdf.ln(2)
pdf.subsection("Summon / Minion Accessories")
pdf.table(
    ["Accessory", "Rarity", "Effect"],
    [
        ["Unbreakable Vow", "Lt Purple", "+25% summon dmg, +1 max minion, but +15% dmg taken"],
    ],
    [38, 16, 136],
)

pdf.ln(2)
pdf.subsection("Vanity Accessories")
pdf.table(
    ["Accessory", "Rarity", "Effect"],
    [
        ["Lightning Scar", "Orange", "Harry's forehead scar. Face slot vanity."],
        ["Wizard Spectacles", "Blue", "Round glasses. Face slot vanity."],
        ["Wizard Beard", "Blue", "Classic wizard beard. Face slot vanity."],
        ["Wizard Pocket Watch", "Blue", "Time-keeping accessory."],
        ["Basilisk Fang", "Orange", "Crafting material + accessory. Venom immunity + poison attacks."],
    ],
    [38, 16, 136],
)

# ========================== 9. MOUNTS & PETS & MINIONS =====================
pdf.add_page()
pdf.section_title(f"9. Mounts ({S['mounts']}) & Pets ({S['pets']}) & Minions (3)")

pdf.subsection("Flying Mounts (5)")
pdf.body_text("All mounts are flying broomsticks or magical creatures. Speed increases with tier.")
pdf.table(
    ["Mount", "Run Speed", "Jump Height", "Accel", "Notes"],
    [
        ["Nimbus 2000", "14", "10", "0.25", "Starter broomstick. Solid speed."],
        ["Hippogriff", "16", "12", "0.30", "Must bow before riding. Earned from Hagrid."],
        ["Firebolt", "18", "12", "0.35", "Upgraded broomstick. Faster than Nimbus 2000."],
        ["Thestral", "Mid", "10", "0.28", "Moderate speed. Between Nimbus and Firebolt."],
        ["Nimbus 2001", "20+", "15", "0.40", "Post-Moon Lord. Fastest broomstick. 'Malfoy money buys speed.'"],
    ],
    [30, 22, 28, 18, 92],
)

pdf.ln(3)
pdf.subsection("Pets (6 + 1 Light Pet)")
pdf.table(
    ["Pet", "Type", "Source", "Special Effect"],
    [
        ["Hedwig", "Light Pet", "Owl Treat", "Snowy owl. Provides light."],
        ["Niffler", "Pet", "Niffler Pouch", "Coin-hunting pet. Seeks treasure."],
        ["Golden Snitch", "Pet", "Golden Snitch item", "Zooms around erratically."],
        ["Kneazle", "Pet", "Cat Treats", "Magical cat. Grants enemy detection."],
        ["Baby Dragon", "Pet", "Dragon Egg", "Miniature dragon companion."],
        ["Pygmy Puff", "Pet", "Pygmy Puff item", "Miniature Puffskein. Weasleys' best-seller."],
    ],
    [30, 20, 32, 108],
)

pdf.ln(3)
pdf.subsection("Minion / Summon Weapons (3)")
pdf.table(
    ["Item", "Type", "Damage", "Notes"],
    [
        ["Patronus Charm", "Summon", "65", "Summons Patronus guardian. +10 def, +2 regen. 2x vs Dementors. 2 minion slots."],
        ["House Elf Bell", "Summon", "Varies", "Summons house-elf minion."],
        ["Phoenix Feather Staff", "Summon", "Varies", "Summons phoenix minion."],
    ],
    [35, 20, 20, 115],
)

# ========================== 10. POTIONS & CONSUMABLES ======================
pdf.add_page()
pdf.section_title("10. Potions & Consumables (19)")

pdf.body_text(
    "19 potions provide powerful buffs themed around the wizarding world. "
    "Brewed at the Enchanting Table using magical ingredients."
)

pdf.table(
    ["Potion", "Duration", "Effect"],
    [
        ["Butterbeer", "10 min", "Comfort buff. Well Fed equivalent + social buff."],
        ["Felix Felicis", "4 min", "Liquid Luck. Major luck boost. Expensive but powerful."],
        ["Polyjuice Potion", "5 min", "Disguise/stealth. Reduced enemy aggro."],
        ["Wolfsbane Potion", "8 min", "Werewolf power boost. Stronger at night."],
        ["Veritaserum", "5 min", "Reveals hidden enemies, traps, and treasure."],
        ["Gillyweed", "6 min", "Gills + flipper + water immunity. Deep diving."],
        ["Amortentia", "6 min", "Love potion. Reduced aggro + passive regen."],
        ["Pepperup Potion", "4 min", "Energy boost. Speed and stat buffs."],
        ["Skele-Gro", "3 min", "Bone regrowth. Defense and regen boost."],
        ["Firewhiskey", "4 min", "+12% all damage, +8% crit."],
        ["Fenrir's Wolfsbane", "6 min", "Enhanced wolfsbane. Stronger werewolf power."],
        ["Shield Charm Potion", "5 min", "+10 defense, +8% damage reduction."],
        ["Seeker's Reflexes", "4 min", "Enhanced reflexes. Dodge and speed boost."],
        ["Stealth Draught", "4 min", "Invisibility/stealth enhancement."],
        ["Memory Potion", "Varies", "Memory restoration effects."],
        ["Draconis Elixir", "Varies", "Dragon blood: +15% all dmg, fire immunity, inferno aura."],
        ["Aqua Fortis", "Varies", "Gills + water immunity. Underwater: +20% dmg, +8 def, +20% speed."],
        ["Resurrection Potion", "10 min", "One-time death prevention. If you die, heal to 50% HP instead."],
        ["Mandrake Restorative", "Instant", "Cures petrification and all debilitating debuffs."],
    ],
    [36, 18, 136],
)

pdf.ln(3)
pdf.subsection("Consumable Foods & Sweets")
pdf.table(
    ["Item", "Effect"],
    [
        ["Chocolate Frog", "Healing consumable. Random wizard card collectible."],
        ["Chocolate Cauldron", "Sweet treat. Minor buff."],
        ["Bertie Bott's Beans", "Random effect: could be good or bad!"],
        ["Peppermint Toad", "Healing sweet."],
        ["Nosebleed Nougat", "Weasley prank. Causes bleeding (gag item)."],
        ["Puking Pastille", "Weasley prank. Causes nausea (gag item)."],
        ["Skiving Snackbox", "Weasley product. Escape item."],
    ],
    [40, 150],
)

pdf.ln(3)
pdf.subsection("Utility Consumables")
pdf.table(
    ["Item", "Effect"],
    [
        ["Hogwarts Letter", "Starter item. Grants Oak Wand + Enchanting Table recipe."],
        ["Floo Powder", "Teleport to a random NPC / home point."],
        ["Portkey", "Teleport utility item."],
        ["Dark Mark", "Triggers Death Eater Invasion event."],
        ["Spell Book", "Spell reference / lore item."],
        ["Dark Arts Tome", "Dark magic reference."],
        ["Daily Prophet", "Check daily challenge status."],
        ["Wizard's Almanac", "Wizard reference guide."],
        ["Wizard Chess Set", "Entertainment / NPC happiness item."],
        ["Golden Egg", "Quest / puzzle item."],
        ["Dueling Dummy Item", "Places a Dueling Dummy for DPS testing."],
    ],
    [40, 150],
)

# ========================== 11. CUSTOM SYSTEMS =============================
pdf.add_page()
pdf.section_title("11. Custom Systems (10)")  # curated count: 10 player-facing systems

for i, system in enumerate(G["systems"], 1):
    pdf.subsection(f"{i}. {system['name']}")
    pdf.body_text(system["text"])
    if "canon_note" in system:
        pdf.small_text(f"[{system['canon_note']}]")

# ========================== 12. CRAFTING MATERIALS =========================
pdf.add_page()
pdf.section_title("12. Crafting Materials (18)")

pdf.body_text(
    "18 interconnected crafting materials form the mod's crafting backbone. Most drop from enemies, "
    "bosses, or biome-specific activities. Essence of Magic is the universal crafting currency."
)

pdf.table(
    ["Material", "Source", "Used For"],
    [
        ["Essence of Magic", "All magical enemies, bosses, challenges", "Universal crafting material. Used in nearly every recipe."],
        ["Phoenix Ash", "Rare drop from fire enemies", "Endgame crafting, resurrection items."],
        ["Phoenix Tear", "Rare drop, quest reward", "Healing items, powerful potions."],
        ["Dragon Scale", "Horntail boss, dragon enemies", "Dragon Scale armor set, accessories."],
        ["Unicorn Blood", "Forbidden Forest exclusive drop", "Powerful potions and accessories."],
        ["Demiguise Hair", "Demiguise (Forbidden Forest)", "Ordinary stealth cloaks, concealment items."],
        ["Spider Silk Weave", "Acromantula, Aragog", "Spider Silk Cloak, web-themed items."],
        ["Ashwinder Egg", "Ashwinder drops", "Fire potions, incendiary items."],
        ["Werewolf Pelt", "Werewolf / Fenrir drops", "Werewolf Cloak, lycanthropic items."],
        ["Cerberus Fang", "Fluffy boss", "Cerberus Fang Necklace, beast accessories."],
        ["Merfolk Scale", "Merfolk / ocean fishing", "Merfolk Amulet, aquatic items."],
        ["Grindylow Tooth", "Grindylow drops", "Aquatic accessories."],
        ["Bowtruckle Catch", "Bowtruckle (surface)", "Nature-themed crafting."],
        ["Jobberknoll Feather", "Jobberknoll (sky)", "Memory Potion, feather items."],
        ["Imp Flame", "Magical Imp (underworld)", "Fire-themed crafting."],
        ["Dugbog Hide", "Dugbog (jungle)", "Leather armor upgrades."],
        ["Enchanted Tadpole", "Forbidden Forest fishing", "Bait + Essence ingredient."],
        ["Magical Koi", "Any water fishing (rare)", "Potion ingredient."],
    ],
    [32, 52, 106],
)

# ========================== 13. DEATHLY HALLOWS ============================
pdf.add_page()
pdf.section_title("13. Deathly Hallows Hidden Set Bonus")

dh = G["deathly_hallows"]
pdf.body_text(dh["intro"])

pdf.ln(1)
pdf.subsection("The Three Artifacts")
for a in dh["artifacts"]:
    pdf.bullet(a)

pdf.ln(2)
pdf.subsection("Hidden Set Bonus: Master of Death")
pdf.body_text(dh["master_of_death"])

pdf.ln(1)
pdf.subsection("How to Obtain")
for a in dh["acquisition"]:
    pdf.bullet(a)

# ========================== 14. BOSS PROGRESSION ===========================
pdf.add_page()
pdf.section_title("14. Boss Progression Guide")

prog = G["boss_progression"]
pdf.body_text(prog["intro"])
pdf.table(prog["headers"], prog["rows"], prog["col_widths"])

pdf.ln(4)
pdf.subsection("Endgame Goals")
for goal in prog["endgame_goals"]:
    pdf.bullet(goal)
pdf.bullet(f"Collect all {S['pets']} pets and {S['mounts']} mounts")

# ========================== CANON DISCLOSURE ==============================
pdf.add_page()
pdf.section_title("15. Canon & Mod-Original Disclosure")
cd = G["canon_disclosure"]
pdf.body_text(cd["intro"])
for cat in cd["categories"]:
    pdf.subsection(f"{cat['label']}: {cat['description']}")
    for ex in cat["examples"]:
        pdf.bullet(ex)
    pdf.ln(2)

# ========================== BACK COVER =====================================
pdf._is_title_page = True
pdf.add_page()
pdf.set_fill_color(*WizardPDF.DARK_PURPLE)
pdf.rect(0, 0, 210, 297, "F")

pdf.ln(100)
pdf.set_font("Helvetica", "B", 28)
pdf.set_text_color(220, 190, 80)
pdf.cell(0, 14, "Wizarding World", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

pdf.ln(4)
pdf.set_font("Helvetica", "I", 14)
pdf.set_text_color(200, 200, 220)
pdf.cell(0, 8, "A Harry Potter Mod for Terraria", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

pdf.ln(6)
pdf.set_draw_color(180, 150, 50)
pdf.set_line_width(0.6)
pdf.line(60, pdf.get_y(), 150, pdf.get_y())

pdf.ln(8)
pdf.set_font("Helvetica", "", 11)
pdf.set_text_color(180, 170, 200)
lines = [
    f"{S['cs_files']} C# Files  |  {S['bosses']} Bosses  |  {S['enemies']} Enemies",
    f"{S['wands_base_combat']} Wands + {S['wands_upgrades']} Upgrade  |  {S['accessories']} Accessories  |  {S['potions']} Potions",
    f"{S['armor_sets']} Armor Sets  |  {S['mounts']} Mounts  |  {S['pets']} Pets",
    "",
    "tModLoader 1.4.4+",
]
for line in lines:
    pdf.cell(0, 6, line, align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

pdf.ln(20)
pdf.set_font("Helvetica", "I", 10)
pdf.set_text_color(140, 140, 160)
pdf.cell(0, 5, "\"After all this time?\" \"Always.\"", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

pdf._is_title_page = False

# ===========================================================================
# Output
# ===========================================================================
_parser = argparse.ArgumentParser(description="Generate Wizarding World English Guide PDF")
_parser.add_argument("--output", type=str, default=None, help="Output PDF path")
_args, _ = _parser.parse_known_args()

output_path = _args.output or os.path.join(_SCRIPT_DIR, "WizardingWorld_Guide_EN.pdf")
pdf.output(output_path)
print(f"PDF generated: {output_path}")
print(f"Total pages: {pdf.page_no()}")
print(f"Counts from manifest: {S['cs_files']} C# | {S['bosses']} bosses | {S['wands_active']} wands | {S['accessories']} acc")
