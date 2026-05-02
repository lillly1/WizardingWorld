"""
巫师世界 (Wizarding World) 泰拉瑞亚模组 — 中文完整指南 PDF 生成器 v4

Phase 30 data-layer refactor:
  Layer A (auto-derived):   scripts/content_manifest.json       via scan_content.py
                            scripts/mechanical_data/*.json      via export_mechanical_data.py
  Layer B (curated prose):  scripts/guide_content.json          via export_guide_data.py
  Layer B (zh translations): scripts/zh_translations.json       (Phase 30 — externalized from inline dicts)
  Layer C (this file):      presentation only (tables, fonts, page layout)

使用 fpdf2 + SimHei 字体生成 A4 中文文档
"""

from fpdf import FPDF
from fpdf.enums import XPos, YPos
import argparse
import json
import os
import sys
import datetime

_SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
_MANIFEST_PATH = os.path.join(_SCRIPT_DIR, "scripts", "content_manifest.json")

if not os.path.exists(_MANIFEST_PATH):
    sys.exit("ERROR: scripts/content_manifest.json not found. Run scan_content.py first.")

with open(_MANIFEST_PATH, encoding="utf-8") as _f:
    MANIFEST = json.load(_f)

S = MANIFEST["summary"]

_GUIDE_CONTENT_PATH = os.path.join(_SCRIPT_DIR, "scripts", "guide_content.json")

if not os.path.exists(_GUIDE_CONTENT_PATH):
    sys.exit(
        "ERROR: scripts/guide_content.json not found.\n"
        "Run  python scripts/export_guide_data.py  first."
    )

with open(_GUIDE_CONTENT_PATH, encoding="utf-8") as _f:
    G = json.load(_f)  # guide content – wired up for future refactoring

# ---------------------------------------------------------------------------
# Load structured mechanical data (auto-derived from C# source)
# ---------------------------------------------------------------------------
_WANDS_JSON_PATH = os.path.join(_SCRIPT_DIR, "scripts", "mechanical_data", "wands.json")
_BOSSES_JSON_PATH = os.path.join(_SCRIPT_DIR, "scripts", "mechanical_data", "bosses.json")

if not os.path.exists(_WANDS_JSON_PATH):
    sys.exit("ERROR: scripts/mechanical_data/wands.json not found. Run export_mechanical_data.py first.")
if not os.path.exists(_BOSSES_JSON_PATH):
    sys.exit("ERROR: scripts/mechanical_data/bosses.json not found. Run export_mechanical_data.py first.")

with open(_WANDS_JSON_PATH, encoding="utf-8") as _f:
    _WANDS_RAW = json.load(_f)

with open(_BOSSES_JSON_PATH, encoding="utf-8") as _f:
    _BOSSES_RAW = json.load(_f)

_FONT_PATH_CANDIDATES = [
    r"C:\Windows\Fonts\simhei.ttf",
    "/mnt/c/Windows/Fonts/simhei.ttf",
]
FONT_PATH = next((p for p in _FONT_PATH_CANDIDATES if os.path.exists(p)), _FONT_PATH_CANDIDATES[0])
OUTPUT_PATH = os.path.join(_SCRIPT_DIR, "WizardingWorld_Guide_ZH.pdf")

# ---------------------------------------------------------------------------
# Phase 30: curated Chinese translations loaded from JSON (not inline)
# Source: scripts/zh_translations.json (manually maintained)
# ---------------------------------------------------------------------------
_ZH_TRANSLATIONS_PATH = os.path.join(_SCRIPT_DIR, "scripts", "zh_translations.json")

if not os.path.exists(_ZH_TRANSLATIONS_PATH):
    sys.exit(
        "ERROR: scripts/zh_translations.json not found.\n"
        "This file holds the curated Chinese translation dictionaries. "
        "It was added in Phase 30; see PHASE30_STATUS.md."
    )

with open(_ZH_TRANSLATIONS_PATH, encoding="utf-8") as _f:
    _ZH = json.load(_f)

WAND_NAMES_ZH = _ZH["wand_names"]
BOSS_NAMES_ZH = _ZH["boss_names"]
BOSS_PROGRESSION_ORDER = _ZH["boss_progression_order"]
WAND_SPELLS_ZH = _ZH["wand_spells"]
# wand_tiers: JSON arrays -> tuples for existing downstream use
WAND_TIERS = {k: tuple(v) for k, v in _ZH["wand_tiers"].items()}
_RARITY_ZH = _ZH["rarity"]
_WAND_TIER_ORDER = _ZH["wand_tier_order"]


# ---------------------------------------------------------------------------
# Helper: build wand table rows from mechanical data
# ---------------------------------------------------------------------------
def _build_wand_rows_by_tier():
    """Return dict[tier_name] -> list of [name, damage, spell, stage, material] rows.

    Stats (damage) are auto-derived from wands.json.
    Names, spells, tier, stage, materials are curated Chinese content.
    """
    # Index wands by id
    wand_by_id = {w["id"]: w for w in _WANDS_RAW if not w.get("obsolete", False)}

    tiers = {t: [] for t in _WAND_TIER_ORDER}
    for wand_id, (tier, stage, materials) in WAND_TIERS.items():
        if wand_id not in wand_by_id:
            continue
        w = wand_by_id[wand_id]
        name_zh = WAND_NAMES_ZH.get(wand_id, wand_id)
        spell_zh = WAND_SPELLS_ZH.get(wand_id, "")
        damage = str(w.get("damage", "?"))
        tiers[tier].append([name_zh, damage, spell_zh, stage, materials])
    return tiers


# ---------------------------------------------------------------------------
# Helper: build boss stat rows from mechanical data
# ---------------------------------------------------------------------------
def _get_boss_stats(boss_id):
    """Return (hp_str, atk_str, def_str, kb_resist_str) from bosses.json."""
    for b in _BOSSES_RAW:
        if b["id"] == boss_id:
            hp = b.get("lifeMax") or b.get("lifeMax_base")
            atk = b.get("damage") or b.get("damage_base")
            defense = b.get("defense") or b.get("defense_base")
            kb = b.get("knockBackResist", 0.0)
            hp_str = f"{hp:,}" if hp else "?"
            atk_str = str(atk) if atk else "?"
            def_str = str(defense) if defense else "?"
            kb_pct = f"{int((1.0 - kb) * 100)}%"
            return hp_str, atk_str, def_str, kb_pct
    return "?", "?", "?", "?"

# ── Color palette ──
DARK_PURPLE = (45, 0, 70)
GOLD = (180, 150, 50)
HEADER_BG = (45, 0, 70)
HEADER_FG = (255, 255, 255)
ROW_ALT = (235, 230, 245)
ROW_NORMAL = (255, 255, 255)
SECTION_BG = (60, 20, 90)
SUBSECTION_CLR = (80, 40, 120)
BODY_CLR = (30, 30, 30)
ACCENT = (140, 90, 200)


class ChinesePDF(FPDF):
    """SimHei A4 PDF with header/footer."""

    def __init__(self):
        super().__init__(orientation="P", unit="mm", format="A4")
        self.set_auto_page_break(auto=True, margin=20)
        self._title_page = False

    # ── header / footer ──
    def header(self):
        if self._title_page:
            return
        self.set_font("SimHei", "", 8)
        self.set_text_color(120, 120, 120)
        self.cell(0, 6, "巫师世界 — 泰拉瑞亚哈利·波特模组完整指南", align="C")
        self.ln(2)
        self.set_draw_color(*ACCENT)
        self.set_line_width(0.4)
        self.line(10, self.get_y(), 200, self.get_y())
        self.ln(5)

    def footer(self):
        if self._title_page:
            return
        self.set_y(-15)
        self.set_font("SimHei", "", 8)
        self.set_text_color(140, 140, 140)
        self.cell(0, 10, f"— {self.page_no()} —", align="C")

    # ── helpers ──
    def section_title(self, text):
        self.set_font("SimHei", "", 16)
        self.set_fill_color(*SECTION_BG)
        self.set_text_color(255, 255, 255)
        self.cell(0, 10, f"  {text}", new_x=XPos.LMARGIN, new_y=YPos.NEXT, fill=True)
        self.ln(3)
        self.set_text_color(*BODY_CLR)

    def subsection(self, text):
        self.set_font("SimHei", "", 12)
        self.set_text_color(*SUBSECTION_CLR)
        self.cell(0, 7, text, new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        self.ln(1)
        self.set_text_color(*BODY_CLR)

    def body(self, text):
        self.set_font("SimHei", "", 10)
        self.set_text_color(*BODY_CLR)
        self.set_x(self.l_margin)
        self.multi_cell(self.w - self.l_margin - self.r_margin, 5.5, text)
        self.ln(1)

    def small(self, text):
        self.set_font("SimHei", "", 9)
        self.set_text_color(60, 60, 60)
        self.set_x(self.l_margin)
        self.multi_cell(self.w - self.l_margin - self.r_margin, 4.5, text)
        self.ln(1)

    def bullet(self, text):
        self.set_font("SimHei", "", 10)
        self.set_text_color(*BODY_CLR)
        indent = 8
        self.set_x(self.l_margin + indent)
        avail = self.w - self.l_margin - self.r_margin - indent
        self.multi_cell(avail, 5.5, f"-  {text}")

    def ensure_space(self, mm):
        if self.get_y() + mm > 275:
            self.add_page()

    def table(self, headers, rows, col_widths=None):
        """Bordered table with alternating row colors, auto page-break."""
        if col_widths is None:
            n = len(headers)
            col_widths = [190 / n] * n
        lh = 5.5  # line height

        def _draw_header():
            self.set_font("SimHei", "", 8)
            self.set_fill_color(*HEADER_BG)
            self.set_text_color(*HEADER_FG)
            self.set_draw_color(180, 180, 180)
            for i, h in enumerate(headers):
                self.cell(col_widths[i], 7, h, border=1, fill=True, align="C")
            self.ln()

        _draw_header()

        self.set_font("SimHei", "", 7.5)
        self.set_text_color(*BODY_CLR)
        for r_idx, row in enumerate(rows):
            if r_idx % 2 == 0:
                self.set_fill_color(*ROW_NORMAL)
            else:
                self.set_fill_color(*ROW_ALT)

            max_lines = 1
            for i, cell_text in enumerate(row):
                w = col_widths[i]
                tw = self.get_string_width(str(cell_text))
                lines = max(1, int(tw / (w - 2)) + 1)
                max_lines = max(max_lines, lines)
            row_h = lh * max_lines

            if self.get_y() + row_h > 275:
                self.add_page()
                _draw_header()
                self.set_font("SimHei", "", 7.5)
                self.set_text_color(*BODY_CLR)
                if r_idx % 2 == 0:
                    self.set_fill_color(*ROW_NORMAL)
                else:
                    self.set_fill_color(*ROW_ALT)

            x0, y0 = self.l_margin, self.get_y()
            for i, cell_text in enumerate(row):
                self.set_xy(x0 + sum(col_widths[:i]), y0)
                self.rect(x0 + sum(col_widths[:i]), y0, col_widths[i], row_h, "DF")
                self.set_xy(x0 + sum(col_widths[:i]) + 1, y0 + 0.5)
                self.multi_cell(col_widths[i] - 2, lh, str(cell_text))
            self.set_xy(self.l_margin, y0 + row_h)


def build_pdf():
    pdf = ChinesePDF()
    pdf.add_font("SimHei", "", FONT_PATH)
    pdf.set_font("SimHei", "", 12)

    # ==================================================================
    # 封面
    # ==================================================================
    pdf._title_page = True
    pdf.add_page()
    pdf.set_fill_color(*DARK_PURPLE)
    pdf.rect(0, 0, 210, 297, "F")

    pdf.ln(50)
    pdf.set_font("SimHei", "", 42)
    pdf.set_text_color(220, 190, 80)
    pdf.cell(0, 18, "巫 师 世 界", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

    pdf.ln(4)
    pdf.set_font("SimHei", "", 18)
    pdf.set_text_color(200, 200, 220)
    pdf.cell(0, 10, "泰拉瑞亚哈利·波特模组", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

    pdf.ln(10)
    pdf.set_draw_color(*GOLD)
    pdf.set_line_width(0.8)
    pdf.line(50, pdf.get_y(), 160, pdf.get_y())

    pdf.ln(12)
    pdf.set_font("SimHei", "", 20)
    pdf.set_text_color(255, 255, 255)
    pdf.cell(0, 10, "完整内容指南", align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

    pdf.ln(16)
    pdf.set_font("SimHei", "", 13)
    pdf.set_text_color(180, 180, 200)
    pdf.cell(0, 8, f"{S['cs_files']}个源文件  |  {S['png_sprites']}个精灵图  |  {S['total_files']}个项目文件",
             align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.ln(2)
    pdf.cell(0, 8, f"{S['bosses']}个Boss  |  {S['wands_base_combat']}根魔杖  |  {S['armor_sets']}套盔甲  |  {S['accessories']}饰品",
             align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

    pdf.ln(25)
    pdf.set_font("SimHei", "", 11)
    pdf.set_text_color(140, 140, 165)
    pdf.cell(0, 8, "tModLoader 1.4+  —  .NET 6 / C#",
             align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

    pdf._title_page = False

    # ==================================================================
    # 目录
    # ==================================================================
    pdf.add_page()
    pdf.section_title("目  录")
    pdf.ln(2)

    toc = [
        ("第一章", "入门指南"),
        ("第二章", f"魔杖与咒语 ({S['wands_base_combat']}根魔杖 + {S['wands_upgrades']}升级版)"),
        ("第三章", f"Boss战 ({S['bosses']}个Boss)"),
        ("第四章", f"盔甲套装 ({S['armor_sets']}套)"),
        ("第五章", "学院专属武器"),
        ("第六章", f"敌人与生物 ({S['enemies']}个敌人)"),
        ("第七章", "神奇动物 (21种)"),
        ("第八章", f"城镇NPC ({S['town_npcs']}个)"),
        ("第九章", f"饰品 ({S['accessories']})"),
        ("第十章", "坐骑、宠物与仆从"),
        ("第十一章", f"药水 ({S['potions']}种)"),
        ("第十二章", "自定义系统"),
        ("第十三章", "制作材料"),
        ("第十四章", "死亡圣器隐藏套装效果"),
    ]
    pdf.set_font("SimHei", "", 11)
    for ch, title in toc:
        pdf.set_text_color(*SUBSECTION_CLR)
        pdf.cell(25, 8, ch)
        pdf.set_text_color(*BODY_CLR)
        pdf.cell(0, 8, title, new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.ln(4)

    # ==================================================================
    # 第一章：入门指南
    # ==================================================================
    pdf.add_page()
    pdf.section_title("第一章  入门指南")

    pdf.subsection("安装说明")
    pdf.body(
        "1. 通过 Steam 安装 tModLoader (1.4+)。\n"
        "2. 在模组浏览器中搜索「Wizarding World」(巫师世界)。\n"
        "3. 下载并启用模组，重新加载模组列表。\n"
        "4. 创建新角色和新世界以获得最佳体验。"
    )

    pdf.subsection("霍格沃茨录取通知书")
    pdf.body(
        "新角色第一次进入世界时将收到「霍格沃茨录取通知书」(HogwartsLetter)。"
        "右键使用后触发分院仪式，分配你到格兰芬多、斯莱特林、拉文克劳或赫奇帕奇。"
        "学院归属影响盔甲套装加成、专属武器和部分NPC对话。"
    )

    pdf.subsection("第一步：附魔台 -> 橡木魔杖 -> 奥利凡德")
    pdf.body("模组核心进度路线：")
    pdf.bullet("收集15块木材 + 3颗坠落之星，在工作台制作「橡木魔杖」(OakWand)。")
    pdf.bullet("橡木魔杖释放昏迷咒 (Stupefy)，是你的第一把魔法武器。")
    pdf.bullet("在世界中找到自然生成的「附魔台」(Enchanting Table)。")
    pdf.bullet("在附魔台上制作更高级的魔杖和魔法物品。")
    pdf.bullet("NPC「奥利凡德」(Ollivander) 入住后，出售魔杖材料和咒语书。")

    pdf.subsection("巫师塔世界生成")
    pdf.body(
        "模组在世界生成时创建以下结构：\n"
        "- 巫师塔：多层石塔，内含附魔台、宝箱和模组物品。\n"
        "- 禁林生物群系：黑暗魔法森林，栖息着八眼巨蛛、夜骐、博格特等。\n"
        "- 密室区域：蛇怪Boss的战场，位于地下深处。\n"
        "- 魁地奇球场：可触发飞贼追逐小游戏。"
    )

    pdf.subsection("伤害类型：咒语伤害 (Spell Damage)")
    pdf.body(
        "模组引入自定义伤害类型「咒语伤害」(SpellDamage)。所有魔杖均造成此类型伤害，"
        "受魔法属性加成影响，拥有独立的暴击计算和伤害加成系统。"
        "咒语连击系统(Spell Combo)可通过快速切换不同咒语来获得额外伤害加成。"
    )

    # ==================================================================
    # 第二章：魔杖与咒语
    # ==================================================================
    pdf.add_page()
    pdf.section_title(f"第二章  魔杖与咒语 ({S['wands_base_combat']}根 + {S['wands_upgrades']}升级版)")

    pdf.body(
        f"巫师世界模组包含{S['wands_base_combat']}根独特魔杖和{S['wands_upgrades']}根升级版，每根魔杖发射不同咒语弹幕。"
        "所有魔杖消耗魔力并造成「咒语伤害」，按游戏进度分为多个阶段。"
    )

    # ── Wand stat tables (auto-derived from scripts/mechanical_data/wands.json) ──
    _wand_tiers = _build_wand_rows_by_tier()
    _wand_headers = ["魔杖", "伤害", "咒语", "阶段", "关键材料"]
    _wand_widths_5 = [30, 14, 28, 22, 96]
    _wand_widths_5b = [30, 14, 30, 20, 96]
    _wand_widths_5c = [32, 14, 30, 20, 94]

    for tier_name in _WAND_TIER_ORDER:
        rows = _wand_tiers.get(tier_name, [])
        if not rows:
            continue
        pdf.subsection(tier_name)
        # Pick wider description cols for later tiers
        if tier_name in ("骷髅王后魔杖", "困难模式魔杖"):
            w = _wand_widths_5b
        elif tier_name == "终局魔杖 + 升级版":
            w = _wand_widths_5c
        else:
            w = _wand_widths_5
        pdf.table(_wand_headers, rows, w)
        pdf.ln(2)

    pdf.ln(1)
    pdf.small(
        "注：暗影老魔杖和命运之杖已并入魔杖精通系统 — 魔杖通过使用获得力量，而非制作升级。"
    )
    pdf.ln(1)
    pdf.small(
        "速度 = useTime（数值越低越快）。所有魔杖使用咒语伤害类型。"
        "魔力消耗范围5-30不等。大部分魔杖在附魔台制作。"
    )

    # ── 咒语详解 ──
    pdf.add_page()
    pdf.subsection("咒语详解")
    spells = [
        ("昏迷咒 (Stupefy)", "红色光束，击中敌人造成眩晕。橡木/赤桤木魔杖基础咒语。"),
        ("缴械咒 (Expelliarmus)", "橙色闪光，高击退力。柳木/冬青魔杖使用。"),
        ("火焰咒 (Incendio)", "喷射火焰弹，造成着火减益。白蜡木/凤凰羽毛魔杖。"),
        ("悬浮咒 (Wingardium Leviosa)", "使敌人悬浮失控。藤木魔杖使用。"),
        ("悬挂咒 (Levicorpus)", "将敌人倒吊。桦木魔杖使用。"),
        ("荧光闪烁 (Lumos Maxima)", "巨大光球，对不死生物额外伤害。雪松魔杖。"),
        ("爆破咒 (Bombarda)", "爆炸性弹幕。花楸木魔杖使用。"),
        ("结膜炎咒 (Conjunctivitis)", "致盲咒语，降低敌人命中率。柏木魔杖。"),
        ("滑稽滑稽 (Riddikulus)", "反博格特咒，克制恐惧与幻象。红橡木。"),
        ("障碍咒 (Impedimenta)", "范围减速。黑刺木魔杖使用。"),
        ("连锁昏迷咒 (Chain Stupefy)", "在敌人之间弹跳。山楂木魔杖使用。"),
        ("神锋无影 (Sectumsempra)", "黑暗撕裂诅咒，持续流血。龙心弦魔杖。"),
        ("钻心咒 (Crucio)", "引导光束，持续伤害。紫杉魔杖使用。"),
        ("粉碎咒 (Reducto)", "爆炸弹幕。紫杉魔杖副咒语。"),
        ("守护神咒 (Expecto Patronum)", "银色守护灵追踪敌人。独角兽毛魔杖。"),
        ("厉火咒 (Fiendfyre)", "诅咒火焰，自动追踪并不断增长。厉火魔杖。"),
        ("杀戮咒 (Avada Kedavra)", "绿色死光，极高伤害。老魔杖（“命运之杖”仅为称号）。"),
        ("盔甲护身 (Protego)", "短暂防御护盾。冬青魔杖副咒语。"),
        ("清水如泉 (Aguamenti)", "水流弹幕。藤木魔杖副咒语。"),
        ("终止咒 (Finite Incantatem)", "清除敌人增益状态。落叶松魔杖。"),
        ("修复咒 (Reparo)", "修复护盾、房间增益与结界。榆木魔杖使用。"),
        ("幻影显形 (Apparition)", "短距传送。乌木魔杖使用。"),
    ]
    for name, desc in spells:
        pdf.set_font("SimHei", "", 10)
        pdf.set_text_color(*BODY_CLR)
        pdf.cell(0, 5.5, name, new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        pdf.set_font("SimHei", "", 8)
        pdf.set_text_color(60, 60, 60)
        pdf.set_x(pdf.l_margin + 6)
        pdf.multi_cell(pdf.w - pdf.l_margin - pdf.r_margin - 6, 5, desc)
        pdf.ln(0.5)

    # ==================================================================
    # 第三章：Boss战
    # ==================================================================
    pdf.add_page()
    pdf.section_title(f"第三章  Boss战 ({S['bosses']}个Boss)")

    pdf.body(
        f"巫师世界模组包含{S['bosses']}个独特Boss，每个都有多阶段战斗机制、独特掉落物和专家模式奖励。"
        "推荐挑战顺序如下："
    )
    pdf.body(
        "山怪 -> 奎洛尔 -> 蛇怪 -> 阿拉戈克 -> 路威 -> 匈牙利树蜂龙 -> "
        "乌姆里奇 -> 芬里尔 -> 贝拉特里克斯 -> 小巴蒂 -> 摄魂怪王 -> 伏地魔"
    )

    # ── Boss stat tables (HP/ATK/DEF auto-derived from scripts/mechanical_data/bosses.json) ──
    # Curated content: narrative descriptions, phase mechanics, immunities, and drop lists
    # are maintained inline below -- only the numeric stat rows come from JSON.
    boss_cols = ["属性", "数值"]
    boss_w = [30, 60]

    # Helper to build stat rows for a boss from mechanical data
    def _boss_stat_rows(boss_id, *, immunities="", atk_override=None):
        hp, atk, defense, kb = _get_boss_stats(boss_id)
        if atk_override:
            atk = atk_override
        return [["生命值", hp], ["攻击力", atk], ["防御力", defense],
                ["击退抗性", kb], ["免疫", immunities]]

    # Boss 1: 山怪
    pdf.ln(2)
    pdf.subsection("1. 山怪 (Mountain Troll) — 前困难模式初期")
    pdf.body(
        "来自万圣节地牢的巨型山怪，模组第一个Boss。"
        "适合在克苏鲁之眼前后挑战。使用「山怪引诱物」召唤。"
    )
    pdf.table(boss_cols, _boss_stat_rows("TrollBoss", immunities="混乱"), boss_w)
    pdf.ln(1)
    pdf.body("阶段机制：")
    pdf.bullet("第一阶段：挥舞巨棒横扫攻击，偶尔跳跃造成震地伤害。")
    pdf.bullet("第二阶段 (50%HP以下)：狂暴化，攻速翻倍，投掷石块远程攻击。")
    pdf.body("掉落物：山怪之棍 (武器)、山怪皮革 (盔甲材料)、奖杯。")

    # Boss 2: 奎洛尔
    pdf.ln(2)
    pdf.subsection("2. 奎洛尔教授 (Quirrell) — 前困难模式")
    pdf.body(
        "被伏地魔附身的黑魔法防御术教授。使用「裹头巾召唤物」在地下召唤。"
        "双形态Boss：奎洛尔形态和伏地魔寄生形态交替战斗。"
    )
    _q_hp, _q_atk, _q_def, _q_kb = _get_boss_stats("QuirrellBoss")
    pdf.table(boss_cols,
              [["生命值", _q_hp], ["攻击力", f"{_q_atk} / 45(寄生)"], ["防御力", _q_def],
               ["击退抗性", _q_kb], ["免疫", "混乱/中毒"]], boss_w)
    pdf.ln(1)
    pdf.body("掉落物：伏地魔残影 (饰品)、奎洛尔裹头巾 (虚荣)、魔法精华。")

    # Boss 3: 蛇怪
    pdf.ln(2)
    pdf.subsection("3. 蛇怪 (Basilisk) — 前困难模式")
    pdf.body(
        "密室之王，巨型蛇类Boss。使用「蛇怪召唤物」(BasiliskSummonItem) 在地下召唤。"
    )
    pdf.table(boss_cols, _boss_stat_rows("BasiliskBoss", immunities="中毒/着火/混乱"), boss_w)
    pdf.ln(1)
    pdf.body("阶段机制：")
    pdf.bullet("第一阶段 — 穴地：蛇怪在地下穿行，突然冲出攻击玩家。")
    pdf.bullet("第二阶段 (50%HP以下)：快速冲锋 + 石化凝视攻击。")
    pdf.body("掉落物：蛇怪之眼、蛇怪毒牙 (3-5)、格兰芬多之剑 (33%)、宝袋 (专家)。")

    # Boss 4: 阿拉戈克
    pdf.add_page()
    pdf.subsection("4. 阿拉戈克 (Aragog) — 前困难模式")
    pdf.body(
        "禁林深处的八眼巨蛛之王。使用「巨蛛巢穴钥匙」在禁林生物群系召唤。"
        "战斗中会持续召唤小型八眼巨蛛援军。"
    )
    pdf.table(boss_cols, _boss_stat_rows("AragogBoss", immunities="中毒/蛛网"), boss_w)
    pdf.ln(1)
    pdf.body("阶段机制：")
    pdf.bullet("第一阶段：在地面快速移动，喷射蛛网减速，召唤蜘蛛群。")
    pdf.bullet("第二阶段 (40%HP以下)：爬上天花板进行毒液雨攻击，蜘蛛群加倍。")
    pdf.body("掉落物：蛛丝斗篷 (饰品)、阿拉戈克毒牙、蛛网材料、宝袋 (专家)。")

    # Boss 5: 路威
    pdf.ln(2)
    pdf.subsection("5. 路威 (Fluffy) — 前困难模式后期")
    pdf.body(
        "三头犬，负责看守魔法石的巨型魔法犬。使用「魔法口琴」催眠后开始战斗。"
        "三个头部各有独立攻击模式。"
    )
    pdf.table(boss_cols, _boss_stat_rows("FluffyBoss", immunities="混乱/中毒/催眠"), boss_w)
    pdf.ln(1)
    pdf.body("掉落物：三头犬项圈 (坐骑)、路威之牙、保护石 (饰品)。")

    # Boss 6: 匈牙利树蜂龙
    pdf.ln(2)
    pdf.subsection("6. 匈牙利树蜂龙 (Hungarian Horntail) — 困难模式")
    pdf.body(
        "火龙，困难模式飞行Boss。使用「龙蛋召唤物」(HorntailSummonItem) 召唤。"
        "三强争霸赛中最危险的龙种之一。"
    )
    pdf.table(boss_cols, _boss_stat_rows("HorntailBoss", immunities="中毒/着火/混乱/寒焰"), boss_w)
    pdf.ln(1)
    pdf.body("阶段机制：")
    pdf.bullet("第一阶段 — 飞翔：空中盘旋，发射火球弹幕。")
    pdf.bullet("第二阶段 — 狂暴冲锋：加速突击 + 尾部扫击 + 火焰吐息。")
    pdf.bullet("第三阶段 (25%HP以下)：极速攻击，召唤火雨。")
    pdf.body("掉落物：龙鳞 (5-10)、龙心 (专家)、金蛋 (33%)、奖杯。")

    # Boss 7: 乌姆里奇
    pdf.add_page()
    pdf.subsection("7. 乌姆里奇 (Umbridge) — 困难模式")
    pdf.body(
        "霍格沃茨高级调查官，以教育法令为武器的邪恶官僚。"
        "使用「教育法令第24号」在地表召唤。她会召唤调查小队和施加减益法令。"
    )
    pdf.table(boss_cols, _boss_stat_rows("UmbridgeBoss", immunities="混乱/中毒/沉默"), boss_w)
    pdf.ln(1)
    pdf.body("阶段机制：")
    pdf.bullet("第一阶段：施放禁令减益，召唤调查小队小兵，发射粉红色咒语弹。")
    pdf.bullet("第二阶段 (40%HP以下)：施放「教育法令」全屏减益，调查小队精英化。")
    pdf.body("掉落物：乌姆里奇之笔 (武器)、粉红蝴蝶结 (虚荣)、法令碎片 (材料)。")

    # Boss 8: 芬里尔
    pdf.ln(2)
    pdf.subsection("8. 芬里尔·格雷伯克 (Fenrir Greyback) — 困难模式")
    pdf.body(
        "最凶残的狼人首领。仅在满月夜间使用「狼人之牙召唤物」召唤。"
        "满月期间获得额外伤害加成。"
    )
    pdf.table(boss_cols, _boss_stat_rows("FenrirBoss", immunities="中毒/混乱/减速"), boss_w)
    pdf.ln(1)
    pdf.body("阶段机制：")
    pdf.bullet("第一阶段：高速冲刺撕咬，施加狼人诅咒减益。")
    pdf.bullet("第二阶段 (50%HP以下)：嚎叫召唤狼群，攻速大幅提升。")
    pdf.bullet("第三阶段 (20%HP以下)：完全狂化，无视防御的致命撕裂。")
    pdf.body("掉落物：狼人之爪 (武器)、狼牙项链 (饰品)、狼毒原液 (药水材料)。")

    # Boss 9: 贝拉特里克斯
    pdf.ln(2)
    pdf.subsection("9. 贝拉特里克斯·莱斯特兰奇 (Bellatrix) — 后石巨人")
    pdf.body(
        "伏地魔最忠实的食死徒。使用「黑魔标记碎片」在地表夜间召唤。"
        "擅长快速传送和黑魔法连击。"
    )
    pdf.table(boss_cols, _boss_stat_rows("BellatrixBoss", immunities="所有常见减益"), boss_w)
    pdf.ln(1)
    pdf.body("阶段机制：")
    pdf.bullet("第一阶段：传送连续施咒，钻心咒/昏迷咒交替使用。")
    pdf.bullet("第二阶段 (50%HP以下)：疯狂连击，使用杀戮咒，召唤暗影刀刃。")
    pdf.body("掉落物：贝拉的匕首 (武器)、莱斯特兰奇金库钥匙 (饰品)、暗黑精华。")

    # Boss 10: 小巴蒂
    pdf.add_page()
    pdf.subsection("10. 小巴蒂·克劳奇 (Barty Crouch Jr.) — 后石巨人")
    pdf.body(
        "伪装大师，使用复方汤剂变形的食死徒。使用「复方汤剂残渣」召唤。"
        "战斗中会变形为不同形态，每种形态有不同攻击模式。"
    )
    pdf.table(boss_cols, _boss_stat_rows("BartyCrouchBoss", immunities="所有常见减益"), boss_w)
    pdf.ln(1)
    pdf.body("阶段机制：")
    pdf.bullet("第一阶段(伪装)：模仿疯眼汉，使用不可饶恕咒。")
    pdf.bullet("第二阶段(暴露)：真身显露，极速施咒+传送，释放黑暗标记。")
    pdf.body("掉落物：疯眼汉之眼 (饰品)、复方汤剂配方 (材料)、暗黑精华。")

    # Boss 11: 摄魂怪王
    pdf.ln(2)
    pdf.subsection("11. 摄魂怪王 (Dementor King) — 后石巨人 (倒数第二Boss)")
    pdf.body(
        "阿兹卡班绝望事件的源头。使用「阿兹卡班钥匙」在地表夜间召唤。"
        "会以黑暗、绝望与灵魂压迫压制玩家，守护神是关键解法。"
    )
    pdf.table(boss_cols, _boss_stat_rows("DementorKingBoss", immunities="所有减益"), boss_w)
    pdf.ln(1)
    pdf.body("阶段机制：")
    pdf.bullet("第一阶段：全屏黑暗，释放灵魂吸取光束，召唤摄魂怪群。")
    pdf.bullet("第二阶段：绝望之吻与更强的黑暗压制，绝望值快速上升。")
    pdf.bullet("第三阶段：灵魂风暴与追踪攻击，守护神伤害明显更有效。")
    pdf.body("掉落物：摄魂怪之魂、阿兹卡班王冠、暗影月光精华。")

    # Boss 12: 伏地魔 (uses _base fields for dynamic scaling)
    pdf.ln(2)
    pdf.subsection("12. 伏地魔 (Lord Voldemort) — 后拜月教徒 (真·终极Boss)")
    pdf.body(
        "黑魔王，模组主线终极Boss。使用「黑魔标记召唤物」"
        "(VoldemortSummonItem) 在地表夜间召唤。魂器狩猎会直接削弱这场战斗。"
    )
    _v_hp, _v_atk, _v_def, _v_kb = _get_boss_stats("VoldemortBoss")
    pdf.table(boss_cols,
              [["生命值", _v_hp], ["攻击力", f"{_v_atk}-120"], ["防御力", _v_def],
               ["击退抗性", _v_kb], ["免疫", "所有减益"]], boss_w)
    pdf.ln(1)
    pdf.body("阶段机制：")
    pdf.bullet("第一阶段 (100-55%)：悬浮施法，传送频率受魂器狩猎进度影响。")
    pdf.bullet("第二阶段 (55-20%)：召唤食死徒与黑魔法压力，准备越充分敌人越少。")
    pdf.bullet("第三阶段 (<20%)：死亡射线环与魂器护盾；核心魂器未清理时会更危险。")
    pdf.body("掉落物：老魔杖 (100%)、高特戒指 (100%)、灵魂碎片 (专家)、黑魔法之书。")

    # ==================================================================
    # 第四章：盔甲套装 (8套)
    # ==================================================================
    pdf.add_page()
    pdf.section_title(f"第四章  盔甲套装 ({S['armor_sets']}套)")

    pdf.body(
        "模组包含8套完整盔甲套装，每套含兜帽/头盔+长袍/胸甲+护腿，"
        "提供独特套装加成。四大学院盔甲为前困难模式，高级盔甲需困难模式材料。"
    )

    pdf.table(
        ["套装", "防御(头/身/腿)", "阶段", "套装加成"],
        [
            ["格兰芬多", "5/7/6 (18)", "前困难", "+15%伤害, +10%近战速度, 勇气之心增益"],
            ["斯莱特林", "4/6/5 (15)", "前困难", "+12%暴击, 5%生命偷取, 野心增益"],
            ["拉文克劳", "3/5/4 (12)", "前困难", "+20%咒语伤害, +40魔力, +5魔力再生, 智慧增益"],
            ["赫奇帕奇", "7/9/7 (23)", "前困难", "+8防御, +4生命再生, 30%荆棘, 忠诚增益"],
            ["巫师长袍", "2/3/2 (7)", "前困难", "+10%魔力再生, +10%咒语伤害"],
            ["龙鳞套", "12/16/12 (40)", "困难(火龙)", "火焰免疫, 熔岩免疫, +18%咒语伤害"],
            ["黑巫师套", "8/10/8 (26)", "困难", "+25%咒语伤害, +15%暴击, 生命偷取"],
            ["摄魂怪套", "10/14/10 (34)", "终局", "+20%所有伤害, 灵魂护盾, 黑暗视觉"],
        ],
        [24, 26, 20, 120],
    )
    pdf.ln(3)

    pdf.subsection("盔甲获取方式")
    pdf.bullet("四大学院盔甲：附魔台 + 丝绸 + 宝石 + 魔法精华 (格兰芬多=红宝石, 斯莱特林=翡翠, 拉文克劳=蓝宝石, 赫奇帕奇=黄玉)")
    pdf.bullet("巫师长袍：附魔台 + 丝绸 + 坠落之星")
    pdf.bullet("龙鳞套：附魔台 + 匈牙利树蜂龙掉落的龙鳞和龙心")
    pdf.bullet("黑巫师套：附魔台 + 暗影精华 + 灵魂碎片")
    pdf.bullet("摄魂怪套：附魔台 + 摄魂怪王掉落的暗影月光精华")

    # ==================================================================
    # 第五章：学院武器
    # ==================================================================
    pdf.add_page()
    pdf.section_title("第五章  学院专属武器")

    pdf.body(
        "每个霍格沃茨学院都有一把传奇武器，使用时根据学院归属获得额外加成。"
    )

    pdf.table(
        ["武器", "类型", "伤害", "特殊效果"],
        [
            ["格兰芬多之剑", "近战", "55", "对蛇类Boss双倍伤害, 击杀回复生命, 中毒效果"],
            ["斯莱特林匕首", "近战", "42", "高暴击率, 毒液减益, 背刺三倍伤害, 极速"],
            ["拉文克劳法杖", "咒语", "50", "5魔力消耗, -10%持握魔耗, 穿透光束"],
            ["赫奇帕奇战锤", "近战", "65", "10%生命偷取, +5持握防御, 极高击退"],
        ],
        [30, 14, 14, 122],
    )
    pdf.ln(3)

    pdf.subsection("其他特殊武器")
    pdf.table(
        ["武器", "类型", "伤害", "特殊效果"],
        [
            ["蛇怪毒牙匕首", "近战", "38", "投掷型, 剧毒减益, 对魂器有效"],
            ["击球手球棒", "近战", "32", "极高击退力的魁地奇球棒"],
            ["鬼飞球", "投掷", "28", "可投掷魁地奇球, 弹射回手"],
            ["吼叫信", "魔法", "25", "投掷爆炸信封, 范围伤害"],
            ["韦斯莱烟花", "远程", "45", "发射追踪烟花弹幕"],
            ["韦斯莱火焰盒", "魔法", "35", "释放多个火焰精灵"],
            ["凤凰羽毛法杖", "召唤", "42", "召唤凤凰仆从"],
            ["家养小精灵铃", "召唤", "35", "召唤家养小精灵仆从"],
            ["狼人之爪", "近战", "70", "芬里尔掉落, 满月增伤"],
            ["乌姆里奇之笔", "魔法", "55", "施加沉默减益, 墨水弹幕"],
        ],
        [30, 14, 14, 122],
    )

    # ==================================================================
    # 第六章：敌人与生物 (51个)
    # ==================================================================
    pdf.add_page()
    pdf.section_title(f"第六章  敌人与生物 ({S['enemies']}个)")

    pdf.body(
        "模组增加了51种新敌人，分布在地面、地下、禁林、海洋等多种生物群系中。"
    )

    pdf.subsection("前困难模式敌人")
    pdf.table(
        ["名称", "生命", "攻击", "区域", "特殊"],
        [
            ["康沃尔郡小精灵", "50", "18", "地面", "飞行骚扰, 成群出没"],
            ["护树罗锅", "20", "5", "森林", "被动生物, 可捕捉"],
            ["弗洛伯毛虫", "5", "0", "地下", "无害, 掉落黏液"],
            ["曼德拉草", "80", "20", "地下", "尖叫: 范围混乱减益"],
            ["仙子精灵", "35", "22", "森林", "小型, 毒液, 成群"],
            ["博格特", "200", "30", "地下", "变形, 施加缓慢+黑暗"],
            ["水鬼", "120", "25", "海洋", "水下拖拽玩家"],
            ["八眼巨蛛", "250", "35", "禁林", "蛛网攻击, 群体"],
            ["山怪(小型)", "400", "35", "洞穴", "高血量, 高击退抗性"],
            ["皮皮鬼", "180", "25", "地面", "恶作剧, 投掷物品"],
            ["不死鸟(Fwooper)", "100", "28", "森林", "彩色飞鸟, 叫声减益"],
            ["活尸铠甲", "150", "22", "地下", "守卫宝箱的魔法铠甲"],
            ["惊退咒陷阱", "0", "30", "巫师塔", "固定陷阱, 触发弹幕"],
        ],
        [28, 12, 12, 16, 122],
    )
    pdf.ln(2)

    pdf.subsection("困难模式敌人")
    pdf.table(
        ["名称", "生命", "攻击", "区域", "特殊"],
        [
            ["摄魂怪", "400", "55", "夜间", "飞行, 吸取生命, 弱于守护神"],
            ["食死徒", "500", "60", "入侵", "飞行, 黑魔法咒语"],
            ["狼人", "500", "65", "满月夜", "高速撕咬, 掉落狼毒"],
            ["阴尸", "350", "50", "地牢", "不死, 弱于火焰"],
            ["夜骐", "300", "40", "地面", "骨马, 杀怪后可见"],
            ["人鱼", "200", "35", "海洋", "水下战士, 三叉戟"],
            ["雪幽灵", "220", "40", "雪地", "冰魂, 寒焰减益"],
            ["诅咒木乃伊", "300", "45", "沙漠", "黑暗诅咒减益"],
            ["炸尾螺", "450", "55", "禁林", "正面装甲, 背后脆弱"],
            ["纳吉尼", "3,000", "55", "地面", "精英蛇, 掉落精华"],
            ["默默然", "800", "70", "随机", "暗能量, 高伤不稳定"],
            ["巨人", "2,000", "90", "入侵", "极高伤害, 震屏"],
            ["狮身人面兽", "1,200", "60", "沙漠", "50%HP后冲锋"],
            ["阿兹卡班守卫", "800", "75", "夜间", "强化摄魂怪"],
            ["半人马弓手", "350", "45", "禁林", "远程箭矢攻击"],
            ["暗影蛇", "280", "50", "地下", "隐形, 毒液攻击"],
            ["咒文石像鬼", "600", "55", "地牢", "飞行, 魔法抗性"],
            ["暗影凤凰", "500", "60", "禁林", "火焰+暗影混合攻击"],
        ],
        [28, 12, 12, 16, 122],
    )
    pdf.ln(2)

    pdf.subsection("入侵专属敌人")
    pdf.table(
        ["名称", "生命", "攻击", "事件", "特殊"],
        [
            ["食死徒精英", "800", "70", "食死徒入侵", "强化咒语, 传送"],
            ["巨人打手", "2,500", "95", "食死徒入侵", "精英巨人"],
            ["摄魂怪群", "300x5", "45", "食死徒入侵", "成群摄魂怪"],
            ["黑魔法傀儡", "600", "55", "食死徒入侵", "不死战士"],
            ["蛇群", "100x8", "25", "食死徒入侵", "纳吉尼召唤"],
        ],
        [28, 14, 12, 24, 112],
    )

    pdf.ln(2)
    pdf.small("决斗假人 (999,999HP, 0伤害) 为训练用目标NPC，可通过决斗假人物品放置。")

    # ==================================================================
    # 第七章：神奇动物 (21种)
    # ==================================================================
    pdf.add_page()
    pdf.section_title("第七章  神奇动物 (21种)")

    pdf.body(
        "巫师世界模组包含21种神奇动物 (Fantastic Beasts)，"
        "涵盖被动生物、友好动物和可驯服的魔法生物。部分可作为宠物或坐骑。"
    )

    pdf.table(
        ["名称", "类型", "栖息地", "行为/特殊"],
        [
            ["独角兽", "被动", "禁林/神圣", "发光, 掉落独角兽角和独角兽血"],
            ["凤凰 (福克斯)", "友好", "特殊", "稀有生成, 掉落凤凰羽毛, 死后重生"],
            ["鹰头马身有翼兽", "中立", "禁林", "鞠躬后可骑乘, 飞行坐骑来源"],
            ["夜骐", "被动", "禁林/夜间", "半隐形, 仅击杀Boss后可见"],
            ["嗅嗅", "被动", "禁林", "追逐金币和闪亮物品, 宠物来源"],
            ["护树罗锅", "被动", "丛林", "小型树人, 可捕捉为宠物"],
            ["弗洛伯毛虫", "被动", "地下", "完全无害, 掉落黏液"],
            ["猫狸子 (Kneazle)", "友好", "地面", "魔法猫科, 探测敌人, 宠物来源"],
            ["鸟蛇 (Occamy)", "中立", "丛林", "蛇身鸟首, 受惊时膨胀"],
            ["雷鸟", "中立", "沙漠/天空", "雷电翅膀, 风暴天气生成"],
            ["驺吾", "中立", "丛林", "大型猫科, 极快移动"],
            ["默默然", "敌对", "随机", "暗黑力量实体, 极其危险"],
            ["比利威格", "被动", "禁林", "蓝色昆虫, 蜇伤使人漂浮"],
            ["角驼兽", "被动", "平原", "大型食草兽, 掉落兽角"],
            ["月痴兽", "被动", "夜间", "月光下出现, 掉落月光精华"],
            ["火蜥蜴", "被动", "地下/熔岩", "火焰生物, 掉落火种"],
            ["人鱼 (Lake)", "中立", "湖泊", "水下歌者, 友好但领地意识强"],
            ["精灵 (House-Elf)", "友好", "地下", "偶尔出现, 赠送礼物"],
            ["三头犬幼崽", "友好", "禁林", "路威后代, 可驯服为宠物"],
            ["玻璃兽", "被动", "地下", "透明蜥蜴, 掉落宝石"],
            ["蜷翼魔", "敌对", "洞穴", "类似蝙蝠的脑部寄生虫"],
        ],
        [28, 14, 22, 126],
    )

    # ==================================================================
    # 第八章：城镇NPC (8个)
    # ==================================================================
    pdf.add_page()
    pdf.section_title(f"第八章  城镇NPC ({S['town_npcs']}个)")

    pdf.body(
        "模组增加了7位城镇NPC，每位提供独特商品和服务。"
    )

    pdf.table(
        ["NPC", "职能", "入住条件", "主要出售物品"],
        [
            ["奥利凡德", "魔杖师", "拥有任意魔杖或3+NPC", "基础魔杖, 魔杖材料, 咒语书, 坠落之星"],
            ["海格", "护林员", "击败蛇怪或5+NPC", "召唤物, 宠物, 蛛丝, 独角兽角(困难)"],
            ["魔药大师", "药剂师", "酿造过药水", "药水, 材料, 瓶子, 草药, 坩埚"],
            ["多比", "小精灵", "6+NPC或击败蛇怪", "火把, 绳索, 药水, 分院帽, 黄油啤酒"],
            ["半人马", "占星师", "夜间+森林生物群系", "记忆球, 窥探镜, 活点地图, 吐真剂"],
            ["韦斯莱双胞胎", "恶作剧", "4+NPC", "烟花, 火焰盒, 诡计炸弹, 飞路粉"],
            ["邓布利多", "校长", "击败伏地魔", "厉火魔杖, 凤凰法杖, 幻影护符, 时间转换器"],
        ],
        [24, 16, 32, 118],
    )
    pdf.ln(3)

    pdf.subsection("NPC详情")
    npc_details = [
        ("奥利凡德 (Ollivander)", "传奇魔杖制作师。出售基础魔杖木材和魔杖核心材料，以及咒语入门书。满月时出售稀有材料。"),
        ("海格 (Hagrid)", "霍格沃茨猎场看守。击败蛇怪后入住，出售魔法生物相关物品和宠物。"),
        ("魔药大师 (Potions Master)", "斯内普风格的药剂师。出售药水原料和成品药水。"),
        ("多比 (Dobby)", "自由的家养小精灵。出售辅助物品，给他袜子可获得特殊buff!"),
        ("邓布利多 (Dumbledore)", "最伟大的巫师。击败伏地魔后出现，出售终局级传奇物品。"),
        ("半人马 (Centaur)", "星象专家。夜间提供占卜服务和星象增益物品。"),
        ("韦斯莱双胞胎 (Fred & George)", "恶作剧专家。出售各种韦斯莱魔法把戏商店产品。"),
    ]
    for name, desc in npc_details:
        pdf.set_font("SimHei", "", 10)
        pdf.set_text_color(*BODY_CLR)
        pdf.cell(0, 6, name, new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        pdf.set_font("SimHei", "", 9)
        pdf.set_text_color(60, 60, 60)
        pdf.set_x(pdf.l_margin + 4)
        pdf.multi_cell(pdf.w - pdf.l_margin - pdf.r_margin - 4, 5, desc)
        pdf.ln(1.5)

    # ==================================================================
    # 第九章：饰品 (53)
    # ==================================================================
    pdf.add_page()
    pdf.section_title(f"第九章  饰品 ({S['accessories']})")

    pdf.body(
        "模组包含53件饰品，涵盖死亡圣器、魂器、Boss掉落、掠夺者物品、"
        "韦斯莱产品、探测/实用、终局和虚荣饰品。"
    )

    acc_cols = ["饰品", "效果", "获取方式"]
    acc_w = [32, 90, 68]

    pdf.subsection("死亡圣器 (3件)")
    pdf.table(acc_cols,
              [["隐形衣", "隐形+减少仇恨+5%伤害", "伏地魔33%掉落"],
               ["复活石", "生命再生+死亡复活(1次/生)", "伏地魔33%掉落"],
               ["老魔杖(武器)", "见魔杖章节, 150伤害", "伏地魔100%掉落"]], acc_w)
    pdf.ln(2)

    pdf.subsection("魂器饰品 (4件)")
    pdf.table(acc_cols,
              [["里德尔日记", "+15%咒语伤害, 每2秒扣1HP", "制作: 精华+书+灵魂"],
               ["斯莱特林挂坠盒", "毒免疫, +8%减伤, 减速", "制作: 精华+金+翡翠"],
               ["拉文克劳冠冕", "大幅魔力增加, 减少最大生命", "制作: 精华+蓝宝石+魔力"],
               ["赫奇帕奇金杯", "生命再生, 药水增强, 施加黑暗", "制作: 精华+黄玉+金"]], acc_w)
    pdf.ln(2)

    pdf.subsection("Boss专家掉落 (12件)")
    pdf.table(acc_cols,
              [["蛇怪之眼", "石化免疫+注视伤害加成", "蛇怪宝袋"],
               ["蛇怪毒牙", "攻击施加剧毒+对蛇增伤", "蛇怪掉落"],
               ["龙心", "火焰免疫+15%咒语伤害", "火龙宝袋"],
               ["灵魂碎片", "高级材料+暗黑增益", "伏地魔宝袋"],
               ["山怪之盾", "+12防御, 击退抗性", "山怪宝袋"],
               ["奎洛尔头巾", "后方免疫攻击", "奎洛尔宝袋"],
               ["蛛丝斗篷", "隐形+移速+蛛网免疫", "阿拉戈克宝袋"],
               ["路威项圈", "三头犬坐骑", "路威宝袋"],
               ["乌姆里奇法令", "NPC价格降低50%", "乌姆里奇宝袋"],
               ["狼牙项链", "满月+20%伤害, 再生", "芬里尔宝袋"],
               ["莱斯特兰奇钥匙", "宝箱掉落翻倍", "贝拉宝袋"],
               ["疯眼汉魔眼", "360度敌人探测", "小巴蒂宝袋"]], acc_w)
    pdf.ln(2)

    pdf.subsection("掠夺者物品 (5件)")
    pdf.table(acc_cols,
              [["活点地图", "小地图显示所有敌人/NPC", "半人马出售"],
               ["大脚板护符", "掠夺者加成, 隐身, 闪避", "韦斯莱(困难)"],
               ["尖头叉子护符", "+10%咒语, +5%暴击, 守护神+25%", "韦斯莱(困难)"],
               ["月亮脸护符", "+15%夜间伤害, 变身加速", "制作(满月材料)"],
               ["虫尾巴指环", "+10%召唤伤害, 鼠形态", "制作(暗影材料)"]], acc_w)

    pdf.add_page()
    pdf.subsection("韦斯莱产品 (4件)")
    pdf.table(acc_cols,
              [["盾帽", "15%几率抵消伤害", "韦斯莱出售"],
               ["延伸耳朵", "加宽探测, 猎手增益", "韦斯莱出售"],
               ["窥探镜", "接近隐形敌人时警告", "制作: 玻璃+精华"],
               ["防御耳罩", "免疫尖叫/混乱减益", "韦斯莱出售"]], acc_w)
    pdf.ln(2)

    pdf.subsection("探测/实用 (9件)")
    pdf.table(acc_cols,
              [["分院帽", "增强学院盔甲套装加成", "多比出售"],
               ["记忆球", "危险感知+矿石探测", "半人马出售"],
               ["闪电疤痕", "小幅咒语伤害加成", "发现/制作"],
               ["冥想盆", "全地图+敌人探测+夜视", "制作(困难)"],
               ["熄灯器", "光源控制, 夜视", "制作(困难)"],
               ["飞路粉", "传送至标记点(消耗品)", "魔药大师出售"],
               ["门钥匙", "随机传送(消耗品)", "制作/掉落"],
               ["巫师长须", "虚荣饰品", "出售/发现"],
               ["巫师眼镜", "虚荣饰品", "出售/发现"]], acc_w)
    pdf.ln(2)

    pdf.subsection("战斗饰品 (6件)")
    pdf.table(acc_cols,
              [["幻影移形护符", "传送至光标(权杖风格)", "邓布利多出售"],
               ["时间转换器", "闪避(10秒CD)+10%移速", "邓布利多出售"],
               ["级长徽章", "+5防御, +5%咒语, NPC增伤", "制作"],
               ["牢不可破誓言", "+25%召唤, +1仆从, 多受15%伤", "制作"],
               ["恒常警觉之眼", "自动闪避(20秒CD)+危险感知", "疯眼汉出售"],
               ["凤凰之泪", "濒死时全回复(1次/天)", "邓布利多出售(终局)"]], acc_w)
    pdf.ln(2)

    pdf.subsection("终局饰品 (7件)")
    pdf.table(acc_cols,
              [["贤者之石", "+10%全伤, +8再生, 减少药水CD", "制作(石巨人后)"],
               ["三强杯", "+15%全伤, +10%暴击, +8防御, +40魔力", "制作(火龙后)"],
               ["大师巫师旗帜", "+10%全伤, +5%咒语, +8防御", "制作(月亮领主后)"],
               ["守护神护符", "召唤守护灵(65伤害, 2x摄魂怪)", "制作(困难)"],
               ["摄魂怪之魂", "+25%全伤, 灵魂护盾, 生命偷取", "摄魂怪王掉落"],
               ["霍格沃茨徽章", "全学院加成+10%所有属性", "制作(全Boss后)"],
               ["魔法部勋章", "+20%全伤, +15%暴击, 全免疫", "制作(超终局)"]], acc_w)

    # ==================================================================
    # 第十章：坐骑、宠物与仆从
    # ==================================================================
    pdf.add_page()
    pdf.section_title("第十章  坐骑、宠物与仆从")

    pdf.subsection("坐骑 (5种)")
    pdf.body("模组包含5种飞行坐骑，按进度阶段解锁：")
    pdf.table(
        ["坐骑", "飞行速度", "阶段", "获取方式"],
        [
            ["光轮2000", "中速 (14)", "前困难", "附魔台: 扫帚+魔法精华"],
            ["鹰头马身有翼兽", "中高速 (16)", "前困难", "鹰头马身有翼兽羽毛(鞠躬后骑乘)"],
            ["火弩箭", "高速 (18)", "困难", "附魔台: 灵魂+精华+光轮升级"],
            ["三头犬", "高速+冲刺 (17)", "困难(路威后)", "路威Boss掉落项圈"],
            ["光轮2001", "极速 (22)", "终局", "附魔台: 火弩箭+月亮碎片"],
        ],
        [30, 20, 24, 116],
    )
    pdf.ln(3)

    pdf.subsection("宠物 (6种)")
    pdf.body("模组包含6种宠物伙伴，跟随玩家（纯外观/探测功能）：")
    pdf.table(
        ["宠物", "行为", "获取方式"],
        [
            ["海德薇 (光源宠物)", "雪鸮，提供光源", "巫师塔宝箱"],
            ["嗅嗅", "追逐闪光物品", "禁林敌人掉落 / 海格出售"],
            ["金色飞贼", "围绕玩家高速飞行", "海格出售 / 魁地奇奖励"],
            ["猫狸子", "猫科，探测敌人", "海格出售(困难)"],
            ["小龙崽", "迷你龙伙伴", "龙蛋孵化"],
            ["迷你毛绒绒", "可爱小生物", "韦斯莱出售"],
        ],
        [28, 50, 112],
    )
    pdf.ln(3)

    pdf.subsection("仆从 (3种)")
    pdf.body("模组包含3种召唤仆从，主动攻击附近敌人：")
    pdf.table(
        ["仆从", "伤害", "来源", "行为"],
        [
            ["凤凰仆从", "65", "邓布利多出售(凤凰法杖)", "飞行火鸟, 火焰攻击"],
            ["家养小精灵", "35", "家养小精灵铃(制作)", "投掷物品攻击敌人"],
            ["守护灵仆从", "65", "守护神护符(饰品)", "追踪, 2x摄魂怪, +10防"],
        ],
        [28, 14, 56, 92],
    )

    # ==================================================================
    # 第十一章：药水 (19种)
    # ==================================================================
    pdf.add_page()
    pdf.section_title(f"第十一章  药水 ({S['potions']}种)")

    pdf.body(
        "模组增加了19种魔法药水，灵感来自哈利·波特系列中的经典魔药。"
        "大部分可在附魔台或坩埚中制作，部分从魔药大师NPC购买。"
    )

    pdf.table(
        ["药水", "持续", "效果", "阶段", "获取"],
        [
            ["黄油啤酒", "10分", "饱食+温暖+小回复", "前期", "魔药大师/多比"],
            ["福灵剂", "4分", "+10%全伤, +10%暴击, 运气", "困难", "魔药大师/邓布"],
            ["复方汤剂", "5分", "伪装/隐身, 减少仇恨", "困难", "魔药大师出售"],
            ["狼毒药剂", "8分", "狼人力量, 夜间增强", "困难", "魔药大师出售"],
            ["迷情剂", "6分", "减少敌人仇恨, 被动再生", "前期", "制作: 精华+月光"],
            ["吐真剂", "5分", "显示敌人/陷阱/宝藏", "困难", "半人马出售"],
            ["鳃囊草", "6分", "水下呼吸+游泳+水免疫", "前期", "制作: 水叶+珊瑚"],
            ["胡椒药剂", "4分", "速度+温暖+小火抗", "前期", "制作: 火花+日光"],
            ["生骨灵", "3分", "防御+骨骼再生+生命恢复", "前期", "制作: 骨+月光花"],
            ["缩身药水", "3分", "体型缩小, 闪避+30%", "前期", "制作: 蘑菇+精华"],
            ["石化解药", "即时", "解除石化减益", "前期", "制作: 曼德拉草+精华"],
            ["清醒药剂", "即时", "解除所有心智减益", "前期", "制作: 草药+精华"],
            ["活地狱药水", "5分", "火焰免疫+火伤加成", "困难", "制作: 狱石+精华"],
            ["隐身药水", "4分", "完全隐形+减少仇恨", "困难", "制作: 月光+精华"],
            ["力量药水(强化)", "5分", "+15%近战伤害+击退", "困难", "制作: 魔法草+精华"],
            ["智慧药水", "5分", "+20%咒语伤害+魔力再生", "困难", "制作: 蓝宝石+精华"],
            ["勇气药水", "5分", "+10%全伤+恐惧免疫", "困难", "制作: 红宝石+精华"],
            ["时间药水", "2分", "攻速+50%, 移速+30%", "终局", "制作: 时间沙+精华"],
            ["涅盘药水", "即时", "全回复+清除所有减益", "终局", "制作: 凤凰泪+精华"],
        ],
        [24, 12, 54, 14, 86],
    )
    pdf.ln(3)

    pdf.subsection("食品消耗品")
    pdf.table(
        ["物品", "效果", "获取"],
        [
            ["巧克力蛙", "恢复生命+随机巫师卡片", "韦斯莱/制作"],
            ["比比多味豆", "随机正面或负面效果", "韦斯莱出售"],
            ["巧克力坩埚", "恢复生命+魔力", "韦斯莱出售"],
            ["薄荷蟾蜍", "恢复生命+跳跃增益", "韦斯莱出售"],
            ["南瓜汁", "恢复生命+魔力", "海格出售"],
            ["岩皮蛋糕", "+5防御(30秒)", "海格出售"],
        ],
        [28, 90, 72],
    )

    # ==================================================================
    # 第十二章：自定义系统
    # ==================================================================
    pdf.add_page()
    pdf.section_title("第十二章  自定义系统")

    pdf.subsection("1. 禁林生物群系 (Forbidden Forest)")
    pdf.body(
        "自定义地表生物群系，拥有独特背景、音乐和水色。\n"
        "- 独特黑暗树木和发光蘑菇环境\n"
        "- 八眼巨蛛、夜骐、博格特等禁林专属敌人\n"
        "- 更高敌人生成率和更强怪物变体\n"
        "- 特殊宝箱含禁林专属战利品"
    )

    pdf.subsection("2. 食死徒入侵 (Death Eater Invasion)")
    pdf.body(
        "自定义入侵事件，使用「黑魔标记召唤物」触发。\n"
        "- 多波次攻击：食死徒、巨人、摄魂怪群\n"
        "- 击败所有波次获得稀有材料和「黑暗抵抗者」成就\n"
        "- 掉落黑巫师盔甲材料和魂器制作组件\n"
        "- 困难模式夜间有小概率自动触发"
    )

    pdf.subsection("3. 魁地奇飞贼追逐 (Quidditch Snitch Chase)")
    pdf.body(
        "独特小游戏系统：\n"
        "- 金色飞贼以极快速度在空中移动\n"
        "- 玩家需骑扫帚坐骑追逐并接触飞贼\n"
        "- 成功捕捉获得大量魔法精华和稀有奖励\n"
        "- 更高级扫帚可更容易追上飞贼\n"
        "- 击球手球棒和鬼飞球为魁地奇主题武器"
    )

    pdf.subsection("4. 学院积分系统 (House Points)")
    pdf.body(
        "模组追踪玩家的学院积分：\n"
        "- 击杀Boss、完成事件、捕捉飞贼等获得积分\n"
        "- 积分达到阈值解锁学院专属奖励\n"
        "- 积分排行在多人服务器中可见\n"
        "- 学院杯奖励：特殊虚荣物品和永久增益"
    )

    pdf.subsection("5. 咒语连击系统 (Spell Combo)")
    pdf.body(
        "快速切换不同魔杖/咒语可触发连击加成：\n"
        "- 2连击：+10%伤害\n"
        "- 3连击：+20%伤害 + 减少魔力消耗\n"
        "- 5连击：+35%伤害 + 连击爆发(额外弹幕)\n"
        "- 连击在2秒内不切换咒语则重置"
    )

    pdf.subsection("6. 每日挑战 (Daily Challenges)")
    pdf.body(
        "每天随机生成一个魔法挑战任务：\n"
        "- 击杀特定数量敌人\n"
        "- 在限时内完成飞贼追逐\n"
        "- 使用指定咒语击败敌人\n"
        "- 完成挑战获得额外魔法精华和稀有材料"
    )

    pdf.subsection("7. 钓鱼系统 (Fishing)")
    pdf.body(
        "模组扩展了原版钓鱼系统：\n"
        "- 禁林湖中可钓到魔法鱼类\n"
        "- 人鱼之泉中可获取特殊鱼饵\n"
        "- 魔法鱼可制作独特药水\n"
        "- 稀有钓鱼收获：人鱼鳞片、海妖之歌、格林迪洛之爪"
    )

    pdf.subsection("8. 魔杖精通系统 (Wand Mastery)")
    pdf.body(
        "魔杖通过使用积累经验，解锁四个精通等级：\n"
        "- 初学 (Novice)：基础状态，无额外加成\n"
        "- 熟练 (Adept)：+10%伤害，-5%魔力消耗\n"
        "- 精通 (Expert)：+20%伤害，-10%魔力消耗，解锁副咒语\n"
        "- 大师 (Master)：+35%伤害，-20%魔力消耗，咒语视觉强化，特殊被动效果\n"
        "精通进度绑定角色，每根魔杖独立追踪。"
    )

    pdf.subsection("9. 阿兹卡班绝望事件 (Azkaban's Despair)")
    pdf.body(
        "后困难模式入侵事件，使用「阿兹卡班钥匙」在夜间触发。\n"
        "- 多波次摄魂怪攻击，持续施加绝望减益\n"
        "- 全屏黑暗+寒冷效果，守护神咒为关键反制手段\n"
        "- 最终波次出现摄魂怪王作为事件Boss\n"
        "- 奖励：阿兹卡班王冠、暗影月光精华、绝望纪念碑"
    )

    pdf.subsection("10. 有求必应屋 (Room of Requirement)")
    pdf.body(
        "使用「有求必应屋钥匙」打开的特殊房间系统：\n"
        "- 训练模式：与训练假人练习咒语连击和魔杖精通\n"
        "- 储藏模式：访问隐藏宝箱和稀有制作材料\n"
        "- 庇护模式：安全区域，加速生命和魔力恢复\n"
        "钥匙从霍格沃茨宝箱中获取或从多比购买。"
    )

    pdf.ln(2)
    pdf.subsection("其他系统")
    pdf.body(
        "- 自定义伤害类型 (SpellDamage)：所有魔杖专属\n"
        "- 自定义减益：石化凝视、灵魂吸取、黑暗诅咒、恐惧、毒液灼烧、狼人诅咒\n"
        "- ModConfig选项：Boss难度、生物群系频率、掉落率等可配置\n"
        f"- Boss Checklist集成：{S['bosses']}个Boss按正确顺序显示\n"
        "- 魔法精华经济：统一的模组货币驱动进度循环"
    )

    # ==================================================================
    # 第十三章：制作材料
    # ==================================================================
    pdf.add_page()
    pdf.section_title("第十三章  制作材料")

    pdf.body("驱动模组进度的关键制作材料：")

    pdf.table(
        ["材料", "来源", "主要用途"],
        [
            ["魔法精华", "所有模组敌人/Boss", "几乎所有附魔台配方的核心"],
            ["龙鳞", "匈牙利树蜂龙(5-10)", "龙鳞盔甲、龙类武器"],
            ["金蛋", "匈牙利树蜂龙(33%)", "火龙召唤、火弩箭、三强杯"],
            ["蛇怪毒牙", "蛇怪Boss(3-5)", "毒液武器、蛇怪饰品"],
            ["独角兽血", "稀有独角兽掉落", "终局饰品、贤者之石"],
            ["独角兽角", "独角兽/海格", "独角兽毛魔杖、守护神"],
            ["灵魂碎片", "伏地魔专家宝袋", "伏地魔召唤、终局制作"],
            ["暗影精华", "困难模式暗影敌人", "黑巫师盔甲、暗黑物品"],
            ["月光精华", "夜间花朵/满月", "隐形衣、高级饰品"],
            ["凤凰羽毛", "凤凰/罕见掉落", "凤凰魔杖、火弩箭"],
            ["蛛丝", "八眼巨蛛/阿拉戈克", "蛛丝斗篷、蛛网武器"],
            ["狼毒原液", "芬里尔Boss掉落", "狼毒药剂、狼牙武器"],
            ["暗黑精华", "贝拉/小巴蒂掉落", "暗黑终局物品制作"],
            ["暗影月光精华", "摄魂怪王掉落", "摄魂怪套装、超终局物品"],
            ["坠落之星", "夜空/奥利凡德", "早期魔杖、附魔台"],
            ["丝绸+宝石", "制作/采矿", "学院盔甲(红/翡/蓝/黄)"],
            ["月亮碎片+夜明矿", "月亮领主", "光轮2001、月后通用终局制作"],
        ],
        [28, 56, 106],
    )
    pdf.ln(3)

    pdf.subsection("附魔台 (Enchanting Table)")
    pdf.body(
        "模组核心制作站。自然生成在巫师塔中，也可用石块+宝石+魔法精华在工作台制作，"
        "或从奥利凡德购买。可制作：所有高级魔杖、学院盔甲、大部分饰品和魔法消耗品。"
    )

    pdf.subsection("制作进度路线总结")
    pdf.body(
        "1. 前期：橡木魔杖 -> 学院盔甲 -> 巫师长袍\n"
        "2. 前Boss期：冬青/藤木/白蜡木魔杖 -> 掠夺者饰品\n"
        "3. 山怪/奎洛尔后：初级Boss材料装备\n"
        "4. 蛇怪后：蛇怪毒牙系列 -> 龙心弦魔杖\n"
        "5. 阿拉戈克/路威后：高级前困难模式装备\n"
        "6. 困难模式：独角兽毛魔杖 -> 黑巫师盔甲 -> 火弩箭\n"
        "7. 火龙后：龙鳞盔甲 -> 厉火魔杖\n"
        "8. 乌姆里奇/芬里尔后：中级困难模式装备\n"
        "9. 贝拉/小巴蒂后：高级困难模式装备\n"
        "10. 贝拉/小巴蒂后：高级困难模式装备\n"
        "11. 摄魂怪王后：阿兹卡班生存装备 -> 真正隐形衣线索\n"
        "12. 伏地魔后：老魔杖 + 高特戒指 -> 复活石与死亡圣器路线"
    )

    # ==================================================================
    # 第十四章：死亡圣器隐藏套装效果
    # ==================================================================
    pdf.add_page()
    pdf.section_title("第十四章  死亡圣器隐藏套装效果")

    pdf.body(
        "当玩家同时携带老魔杖，并装备真正的隐形衣与复活石时，"
        "将激活隐藏的「死亡之主」状态。这条路线由魂器狩猎、邓布利多指引与圣器净化共同推动。"
    )

    pdf.subsection("三件死亡圣器")
    pdf.table(
        ["圣器", "类型", "单独效果"],
        [
            ["老魔杖", "武器", "150咒语伤害, 杀戮咒+昏迷咒交替, 40魔力消耗"],
            ["隐形衣", "饰品", "隐形+减少仇恨+5%伤害"],
            ["复活石", "饰品", "生命再生+40最大生命值"],
        ],
        [30, 16, 144],
    )
    pdf.ln(3)

    pdf.subsection("「死亡之主」隐藏套装加成")
    pdf.body("同时满足三圣器条件时激活以下效果：")
    pdf.bullet("+10 防御，+4生命再生，+8%伤害减免")
    pdf.bullet("+10% 咒语伤害，作为克制而非爆炸式输出")
    pdf.bullet("免疫石化、被施咒、黑暗诅咒、冻结等关键减益")
    pdf.bullet("显示隐藏敌人、陷阱、宝藏并获得夜视")
    pdf.bullet("大幅降低仇恨值，黑魔法腐化会被持续净化")
    pdf.bullet("每个泰拉瑞亚昼夜获得1次濒死保命")

    pdf.ln(3)
    pdf.subsection("获取难度")
    pdf.body(
        "真正隐形衣：完成4个核心魂器并战胜摄魂怪王后，由邓布利多画像一次性归还。\n"
        "老魔杖与高特戒指：伏地魔100%掉落。\n"
        "复活石：击败伏地魔后，把高特戒指带给邓布利多净化。\n\n"
        "这条路线强调剧情准备与生存意义，而不是单纯重复刷Boss赌掉落。"
    )

    pdf.ln(3)
    pdf.subsection("魂器与死亡圣器的互动")
    pdf.body(
        "装备任何魂器饰品时，死亡圣器套装加成的「死亡之触」效果概率"
        "从5%提升到8%，但玩家会受到每秒1点的暗黑侵蚀伤害。"
        "这反映了原作中魂器对使用者灵魂的腐蚀效果。\n\n"
        "四件魂器全部装备时（里德尔日记+挂坠盒+冠冕+金杯），"
        "触发「灵魂分裂」状态：\n"
        "- 死亡之触概率提升到12%\n"
        "- 所有伤害再+10%\n"
        "- 但最大生命值降低30%\n"
        "- 暗黑侵蚀伤害提升到每秒3点"
    )

    # ==================================================================
    # 附录：项目统计
    # ==================================================================
    pdf.add_page()
    pdf.section_title("附录  项目统计")
    pdf.body("以下是巫师世界模组的最新项目统计信息：")
    pdf.ln(2)

    pdf.table(
        ["类别", "数量"],
        [
            ["C#源文件", str(S["cs_files"])],
            ["精灵图(PNG)", str(S["png_sprites"])],
            ["项目文件数", str(S["total_files"])],
            ["魔杖", f"{S['wands_base_combat']} (+{S['wands_upgrades']}升级版)"],
            ["Boss", str(S["bosses"])],
            ["盔甲套装", f"{S['armor_sets']} ({S['armor_pieces']}件)"],
            ["饰品", str(S["accessories"])],
            ["普通敌人", str(S["enemies"])],
            ["神奇动物", "21"],  # curated: subset of enemies classified as Fantastic Beasts
            ["城镇NPC", str(S["town_npcs"])],
            ["坐骑", str(S["mounts"])],
            ["宠物", str(S["pets"])],
            ["仆从", "3"],  # curated: PatronusCharm, HouseElfBell, PhoenixFeatherStaff
            ["药水", str(S["potions"])],
            ["自定义伤害类型", "1 (咒语伤害)"],
            ["自定义生物群系", "1 (禁林)"],
            ["自定义入侵事件", "2 (食死徒入侵 + 阿兹卡班绝望)"],
            ["小游戏系统", "1 (魁地奇飞贼追逐)"],
            ["自定义制作站", "1 (附魔台)"],
        ],
        [60, 40],
    )

    pdf.ln(6)
    pdf.set_font("SimHei", "", 9)
    pdf.set_text_color(100, 100, 100)
    pdf.multi_cell(0, 5,
                   "计数来自源代码扫描 (scripts/scan_content.py)，叙述文本手动维护。\n"
                   "模组持续开发中，内容可能随版本更新而变化。\n"
                   "如需最新信息，请查看项目源代码或 tModLoader 模组页面。")

    # ── Save ──
    pdf.output(OUTPUT_PATH)
    print(f"PDF saved to: {OUTPUT_PATH}")
    print(f"Total pages: {pdf.page_no()}")


if __name__ == "__main__":
    build_pdf()
