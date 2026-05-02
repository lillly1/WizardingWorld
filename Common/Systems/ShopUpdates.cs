using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Centralized shop updates — adds newer items to existing NPC shops
	/// without modifying the original NPC files. This keeps the codebase clean
	/// and makes it easy to add/remove shop items.
	/// </summary>
	public class ShopUpdates : GlobalNPC
	{
		public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
		{
			// Ollivander — add newer wands
			if (npc.type == ModContent.NPCType<Content.NPCs.Town.Ollivander>())
			{
				AddToShop(items, ModContent.ItemType<Content.Items.Weapons.Wands.HollyWand>());
				AddToShop(items, ModContent.ItemType<Content.Items.Weapons.Wands.RowanWand>());
				AddToShop(items, ModContent.ItemType<Content.Items.Weapons.Wands.BirchWand>());

				if (DownedBossSystem.downedBasilisk)
					AddToShop(items, ModContent.ItemType<Content.Items.Weapons.Wands.RedOakWand>());

				if (Main.hardMode)
				{
					AddToShop(items, ModContent.ItemType<Content.Items.Weapons.Wands.HawthornWand>());
					AddToShop(items, ModContent.ItemType<Content.Items.Weapons.Wands.BlackthornWand>());
					AddToShop(items, ModContent.ItemType<Content.Items.Weapons.Wands.CypressWand>());
					AddToShop(items, ModContent.ItemType<Content.Items.Weapons.Wands.LarchWand>());
					AddToShop(items, ModContent.ItemType<Content.Items.Weapons.Wands.EbonyWand>());
				}

				// Always sell the Spell Book reference + Wand Holster
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.SpellBook>());
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.WandHolster>());
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.DirigiblePlumEarring>());
			}

			// Potions Master — add newer potions
			if (npc.type == ModContent.NPCType<Content.NPCs.Town.PotionsMaster>())
			{
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.PepperupPotion>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.SkeleGro>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.Gillyweed>());

				if (Main.hardMode)
					AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.Amortentia>());
			}

			// Dobby — add sweets, utility items
			if (npc.type == ModContent.NPCType<Content.NPCs.Town.Dobby>())
			{
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.ChocolateFrog>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.PeppermintToad>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.ChocolateCauldron>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.BertieBottsBeans>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.FlooPowder>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.DailyProphet>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.DuelingDummyItem>());
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.BossCompass>());
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.WizardPocketWatch>());
			}

			// Hagrid — creatures, pets, ALL boss summons (progression-gated)
			if (npc.type == ModContent.NPCType<Content.NPCs.Town.Hagrid>())
			{
				AddToShop(items, ModContent.ItemType<Content.Pets.Kneazle.KneazleItem>());
				AddToShop(items, ModContent.ItemType<Content.Pets.Hedwig.HedwigItem>());
				AddToShop(items, ModContent.ItemType<Content.Pets.PygmyPuff.PygmyPuffItem>());

				// Boss summons — gated by previous boss kills
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.TrollSummonItem>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.QuirrellSummonItem>(),
					NPC.downedBoss1);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.BasiliskSummonItem>(),
					DownedBossSystem.downedTroll || DownedBossSystem.downedQuirrell);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.AragogSummonItem>(),
					Main.hardMode);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.FluffySummonItem>(),
					Main.hardMode && DownedBossSystem.downedAragog);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.HorntailSummonItem>(),
					Main.hardMode);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.FenrirSummonItem>(),
					Main.hardMode);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.UmbridgeSummonItem>(),
					Main.hardMode);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.BellatrixSummonItem>(),
					NPC.downedPlantBoss);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.BartyCrouchSummonItem>(),
					NPC.downedPlantBoss);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.DementorKingSummonItem>(),
					NPC.downedGolemBoss); // Azkaban's Despair: post-Golem (moved)
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.VoldemortSummonItem>(),
					NPC.downedAncientCultist); // Voldemort: post-Cultist (TRUE FINAL)
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.DarkMarkSummon>(),
					Main.hardMode && DownedBossSystem.downedBasilisk);
			}

			// Potions Master — all newer potions
			if (npc.type == ModContent.NPCType<Content.NPCs.Town.PotionsMaster>())
			{
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.Firewhiskey>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.MandrakeRestorative>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.AquaFortis>(),
					Main.hardMode);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.StealthDraught>(),
					Main.hardMode);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.FenrirsWolfsbane>(),
					Main.hardMode && DownedBossSystem.downedFenrir);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.SeekersReflexes>(),
					Main.hardMode);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.DraconisElixir>(),
					Main.hardMode && DownedBossSystem.downedHorntail);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.ResurrectionPotion>(),
					Main.hardMode);
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.ShieldCharmPotion>());
				AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.MemoryPotion>());
			}

			// Centaur — detection items, Boss Compass, Moody's Eye
			if (npc.type == ModContent.NPCType<Content.NPCs.Town.Centaur>())
			{
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.BossCompass>());
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.Sneakoscope>());
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.FoeGlass>());
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.MoodysEye>(),
					Main.hardMode);
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.Deluminator>(),
					Main.hardMode);
			}

			// Hagrid — beast-related accessories + creature gear
			if (npc.type == ModContent.NPCType<Content.NPCs.Town.Hagrid>())
			{
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.BeastHuntersCharm>(),
					Main.hardMode && DownedBossSystem.downedHorntail);
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.DragonScaleRing>(),
					DownedBossSystem.downedHorntail);
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.CamouflageCloak>(),
					Main.hardMode);
				AddToShop(items, ModContent.ItemType<Content.Items.Accessories.FairyWings>(),
					Main.hardMode);
			}

			// Dumbledore — endgame items + encyclopedia
			if (npc.type == ModContent.NPCType<Content.NPCs.Town.Dumbledore>())
			{
				if (DownedBossSystem.downedVoldemort)
				{
					AddToShop(items, ModContent.ItemType<Content.Items.Accessories.PatronusCharm>());
					AddToShop(items, ModContent.ItemType<Content.Items.Consumables.WizardChessSet>());
					AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.ResurrectionPotion>());
					AddToShop(items, ModContent.ItemType<Content.Items.Weapons.Wands.FiendfyreWand>());
					AddToShop(items, ModContent.ItemType<Content.Items.Weapons.PhoenixFeatherStaff>());
					AddToShop(items, ModContent.ItemType<Content.Items.Accessories.ApparitionCharm>());
					AddToShop(items, ModContent.ItemType<Content.Items.Accessories.TimeTurner>());
					AddToShop(items, ModContent.ItemType<Content.Items.Consumables.Potions.FelixFelicis>());
				}
			}
		}

		private static void AddToShop(Item[] items, int itemType, bool condition = true)
		{
			if (!condition)
				return;

			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] == null || items[i].type == ItemID.None)
				{
					items[i] = new Item();
					items[i].SetDefaults(itemType);
					return;
				}

				// Don't add duplicates
				if (items[i].type == itemType)
					return;
			}
		}
	}
}
