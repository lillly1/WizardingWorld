using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Wizard's Almanac — the comprehensive in-game encyclopedia.
	/// 10 pages covering EVERY system in the mod.
	/// Use repeatedly to browse pages. Sold by Dumbledore.
	/// The ultimate reference for new and experienced wizard players.
	/// </summary>
	public class WizardsAlmanac : ModItem
	{
		private int currentPage;
		private const int MaxPages = 9;

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 32;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.maxStack = 1;
			Item.consumable = false;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(gold: 5);
			Item.UseSound = SoundID.Item1;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return true;

			currentPage = (currentPage + 1) % (MaxPages + 1);

			switch (currentPage)
			{
				case 0: ShowContents(); break;
				case 1: ShowBosses(); break;
				case 2: ShowArmor(); break;
				case 3: ShowAccessories(); break;
				case 4: ShowPotions(); break;
				case 5: ShowMountsPets(); break;
				case 6: ShowSystems(); break;
				case 7: ShowCrafting(); break;
				case 8: ShowTips(); break;
				case 9: ShowStats(); break;
			}

			return true;
		}

		private void T(string text, byte r = 200, byte g = 200, byte b = 200)
		{
			Main.NewText(text, r, g, b);
		}

		private void ShowContents()
		{
			T("=== WIZARD'S ALMANAC ===", 255, 215, 0);
			T("The complete guide to the Wizarding World mod", 200, 180, 140);
			T("Use again to browse pages:", 180, 180, 180);
			T("  1: Boss Progression (12 bosses)", 255, 150, 150);
			T("  2: Armor Sets (8 sets)", 150, 200, 150);
			T("  3: Key Accessories (53 total)", 150, 150, 200);
			T("  4: Potions (19 types)", 200, 150, 200);
			T("  5: Mounts & Pets", 200, 200, 150);
			T("  6: Custom Systems", 150, 200, 200);
			T("  7: Crafting Materials", 200, 180, 100);
			T("  8: Pro Tips", 255, 215, 0);
			T("  9: Mod Statistics", 180, 180, 180);
		}

		private void ShowBosses()
		{
			T("=== BOSS PROGRESSION ===", 255, 150, 150);
			ShowBossLine("Mountain Troll", 2000, "Smelly Sock", DownedBossSystem.downedTroll);
			ShowBossLine("Prof. Quirrell", 3500, "Suspicious Turban", DownedBossSystem.downedQuirrell);
			ShowBossLine("Basilisk", 4500, "Serpent's Diary", DownedBossSystem.downedBasilisk);
			ShowBossLine("Aragog", 8000, "Acromantula Egg [HM]", DownedBossSystem.downedAragog);
			ShowBossLine("Fluffy", 18000, "Enchanted Flute [post-Mech]", DownedBossSystem.downedFluffy);
			ShowBossLine("Horntail", 28000, "Dragon Egg [HM]", DownedBossSystem.downedHorntail);
			ShowBossLine("Umbridge", 20000, "Educational Decree [post-Mech]", DownedBossSystem.downedUmbridge);
			ShowBossLine("Fenrir", 25000, "Bloodied Claw [Blood Moon]", DownedBossSystem.downedFenrir);
			ShowBossLine("Bellatrix", 35000, "Prisoner Tag [post-Plantera]", DownedBossSystem.downedBellatrix);
			ShowBossLine("Barty Crouch", 30000, "Suspicious Flask [post-Plantera]", DownedBossSystem.downedBartyCrouch);
			ShowBossLine("Dementor King", 45000, "Frozen Soul Lantern [post-Golem]", DownedBossSystem.downedDementorKing);
			ShowBossLine("Voldemort", 60000, "Dark Mark [post-Cultist]", DownedBossSystem.downedVoldemort);
		}

		private void ShowBossLine(string name, int hp, string summon, bool downed)
		{
			string status = downed ? "[DEFEATED]" : "[ALIVE]";
			byte r = downed ? (byte)100 : (byte)255;
			byte g = downed ? (byte)200 : (byte)100;
			T($"  {name} ({hp:N0} HP) — {summon} {status}", r, g, 100);
		}

		private void ShowArmor()
		{
			T("=== ARMOR SETS ===", 150, 200, 150);
			T("  Gryffindor: +15% dmg, +10% melee speed", 200, 50, 50);
			T("  Slytherin: +12% crit chance", 0, 150, 50);
			T("  Ravenclaw: +20% spell, +40 mana, regen", 50, 100, 200);
			T("  Hufflepuff: +8 def, +4 regen, thorns", 200, 170, 50);
			T("  Dark Wizard: +25% spell, +15% spell crit", 80, 40, 120);
			T("  Dragon Scale: Fire immune, +18% spell [HM]", 200, 80, 30);
			T("  Wizengamot: +30% spell, -20% mana cost [endgame]", 120, 50, 180);
			T("  Wizard Robes: Vanity set", 100, 0, 180);
		}

		private void ShowAccessories()
		{
			T("=== KEY ACCESSORIES (REDESIGNED) ===", 150, 150, 200);
			T("  Deathly Hallows: Elder Wand + true Invisibility Cloak + Stone", 255, 215, 0);
			T("    Demiguise Weave Cloak is useful stealth gear, but NOT the Hallow", 200, 200, 150);
			T("    Master of Death = survival/fate (NOT raw DPS). Immune to curses,", 200, 200, 150);
			T("    see all hidden things, death prevention, corruption immunity", 200, 200, 150);
			T("  Patronus Charm: Summon guardian (2 slots). CLEANSES corruption!", 200, 200, 240);
			T("    WARNING: Patronus weakens with Dark Arts corruption", 255, 150, 150);
			T("  Horcruxes: POWERFUL but CORRUPT you. Destroy them to weaken Voldemort!", 255, 100, 100);
			T("  Gaunt's Ring: bring it to Dumbledore after the Horcrux Hunt", 200, 170, 50);
			T("  Boss Compass: Shows next boss, Horcrux progress, and Hallows hints", 200, 170, 50);
			T("  Alohomora Key: Opens locked doors — the first utility spell!", 255, 215, 0);
		}

		private void ShowPotions()
		{
			T("=== POTIONS (19 types) ===", 200, 150, 200);
			T("  Butterbeer: Well Fed + regen + defense", 200, 160, 80);
			T("  Felix Felicis: Luck + crit + coins", 255, 215, 0);
			T("  Wolfsbane/Fenrir's: Werewolf power at night", 100, 80, 50);
			T("  Pepperup: Speed + attack speed + fire immune", 255, 100, 50);
			T("  Skele-Gro: Massive regen (but painful)", 200, 200, 200);
			T("  Phoenix Tear: FULL HEAL + clear ALL debuffs", 255, 200, 50);
			T("  Resurrection: Prevents one death", 255, 150, 50);
			T("  Draconis Elixir: Dragon blood — Wrath + Rage + Fire + Inferno", 200, 50, 20);
			T("  Stealth Draught: Invisibility + speed", 60, 100, 60);
		}

		private void ShowMountsPets()
		{
			T("=== MOUNTS & PETS ===", 200, 200, 150);
			T("  Mounts: Nimbus 2000 > Thestral > Firebolt > Hippogriff > Nimbus 2001", 150, 200, 255);
			T("  Pets: 6 total, including 1 light pet", 255, 200, 100);
			T("         Golden Snitch, Niffler, Hedwig (light), Kneazle, Pygmy Puff, Baby Dragon", 255, 200, 100);
			T("  Minions: Phoenix (heals), House-Elf (teleports), Patronus (anti-Dementor)", 200, 200, 255);
		}

		private void ShowSystems()
		{
			T("=== SYSTEMS (REDESIGNED) ===", 150, 200, 200);
			T("  Dark Arts Corruption: Dark spells build corruption (0-100%)", 255, 100, 100);
			T("    High corruption = weaker Patronus, paranoia, reduced healing", 200, 100, 100);
			T("    Cleanse with: Patronus casting, Phoenix Tears, Horcrux destruction", 100, 200, 100);
			T("  Horcrux Hunt: 4 core Horcruxes + Nagini + Gaunt's Ring purification", 255, 200, 50);
			T("  Pensieve Memories: All bosses are memories from wizarding history", 200, 200, 240);
			T("  Forbidden Forest: 3 depth zones with scaling danger", 100, 150, 80);
			T("  Class Quests: DADA, Potions, Creatures, Charms mastery", 100, 200, 255);
			T("  Quidditch Season: Seeker ranks, multi-Snitch events every 7 days", 255, 215, 0);
			T("  Azkaban Despair: survival event with a real despair meter and ward loop", 180, 220, 255);
			T("  Spell Combos: Stupefy+Incendio=Explosion, Impedimenta+AK=Execute", 200, 100, 100);
			T("  Hogsmeade Village: Town with Shrieking Shack (worldgen)", 180, 150, 100);
		}

		private void ShowCrafting()
		{
			T("=== CRAFTING MATERIALS ===", 200, 180, 100);
			T("  Essence of Magic: Drops from ALL wizard enemies. Used in 50+ recipes.", 150, 50, 255);
			T("  Dragon Scale (Horntail): Armor, ring, necklace, elixir", 200, 80, 30);
			T("  Basilisk Fang: Dagger, wraps, Dark Lord's Bane", 0, 120, 0);
			T("  Spider Silk (Aragog): Cloak, wraps", 220, 220, 230);
			T("  Cerberus Fang (Fluffy): Necklace, Beast Hunter's Charm", 200, 180, 150);
			T("  Werewolf Pelt (Fenrir): Cloak, Fenrir's Wolfsbane, Beast Hunter's", 100, 80, 50);
			T("  Unicorn Blood (Forbidden Forest): Banner, Cauldron, Nimbus 2001", 180, 200, 240);
			T("  Phoenix Ash (Phoenix minion kills): Resurrection Potion, Draconis", 255, 200, 80);
		}

		private void ShowTips()
		{
			T("=== PRO TIPS (REDESIGNED) ===", 255, 215, 0);
			T("  1. Destroy Horcruxes before Voldemort. Each one weakens his fight.", 255, 100, 100);
			T("  2. Dark spells (AK, Crucio, Fiendfyre) BUILD CORRUPTION. Use wisely.", 255, 150, 100);
			T("  3. Casting Patronus CLEANSES corruption. Balance dark and light.", 100, 255, 150);
			T("  4. Riddikulus works on BOGGARTS. Use Patronus for DEMENTORS.", 255, 255, 200);
			T("  5. Fiendfyre is UNSTABLE — it swerves and can damage YOU.", 255, 200, 100);
			T("  6. Dumbledore's Portrait appears after Azkaban's Despair, not after Voldemort.", 255, 255, 200);
			T("  7. The Forbidden Forest has 3 DEPTH ZONES — deeper = more dangerous.", 255, 255, 200);
			T("  8. Complete Class Quests for permanent mastery bonuses.", 100, 200, 255);
			T("  9. Hogsmeade generates on the far side of the world from spawn.", 255, 255, 200);
			T(" 10. Master of Death is SURVIVAL, not damage. It prevents death.", 255, 215, 0);
		}

		private void ShowStats()
		{
			T("=== MOD STATISTICS ===", 180, 180, 180);

			int bossesDefeated = 0;
			if (DownedBossSystem.downedTroll) bossesDefeated++;
			if (DownedBossSystem.downedQuirrell) bossesDefeated++;
			if (DownedBossSystem.downedBasilisk) bossesDefeated++;
			if (DownedBossSystem.downedAragog) bossesDefeated++;
			if (DownedBossSystem.downedFluffy) bossesDefeated++;
			if (DownedBossSystem.downedHorntail) bossesDefeated++;
			if (DownedBossSystem.downedUmbridge) bossesDefeated++;
			if (DownedBossSystem.downedFenrir) bossesDefeated++;
			if (DownedBossSystem.downedBellatrix) bossesDefeated++;
			if (DownedBossSystem.downedBartyCrouch) bossesDefeated++;
			if (DownedBossSystem.downedVoldemort) bossesDefeated++;
			if (DownedBossSystem.downedDementorKing) bossesDefeated++;

			T($"  Wizard Bosses Defeated: {bossesDefeated}/12", 200, 200, 200);
			T("  Mod Size: 443 source files, 458 sprites, 958 project files", 150, 150, 150);
			T("  Content: 23 wands + 1 upgrade, 51 enemies, 53 accessories, 19 potions", 150, 150, 150);
			T("  Features: 12 bosses, 8 armor sets, 5 mounts, 6 pets total, 3 languages", 150, 150, 150);
			T("  Major systems: 10 core progression/combat/world systems", 150, 150, 150);
			T("", 150, 150, 150);

			if (bossesDefeated == 12)
				T("  YOU ARE THE MASTER OF DEATH. All wizard bosses vanquished!", 255, 215, 0);
			else
				T($"  {12 - bossesDefeated} wizard bosses remain. Keep fighting!", 255, 150, 100);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<SpellBook>(), 1)
				.AddIngredient(ItemID.Book, 3)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
