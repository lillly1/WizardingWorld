using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Wizard Fishing System — adds magical fish and wizard loot to fishing catches.
	/// Catches scale with fishing power and biome:
	/// - Any water: Magical Koi (potion ingredient, rare)
	/// - Ocean: Merfolk Scale (accessory craft material)
	/// - Forbidden Forest: Enchanted Tadpole (bait + Essence)
	/// - Any water in Hardmode: Wizard Loot Crate
	/// </summary>
	public class WizardFishing : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => true;

		public override void ExtractinatorUse(int extractType, int extractinatorBlockType, ref int resultType, ref int resultStack)
		{
			// Not used for extractinator — this is for fishing catches
		}
	}

	public class WizardFishingPlayer : ModPlayer
	{
		public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Microsoft.Xna.Framework.Vector2 sonarPosition)
		{
			// Only process if actually fishing (not crate fishing)
			if (attempt.playerFishingConditions.PoleItemType <= 0)
				return;

			bool inHardmode = Main.hardMode;
			bool inOcean = Player.ZoneBeach;
			bool inForest = Player.InModBiome<Content.Biomes.ForbiddenForestBiome>();

			// Magical Koi — rare catch in any water (3% base chance)
			if (Main.rand.NextBool(33))
			{
				itemDrop = ModContent.ItemType<Content.Items.Consumables.MagicalKoi>();
				return;
			}

			// Ocean: Merfolk Scale (5% chance)
			if (inOcean && Main.rand.NextBool(20))
			{
				itemDrop = ModContent.ItemType<Content.Items.Consumables.MerfolkScale>();
				return;
			}

			// Ocean/Underground water: Grindylow Tooth (4% chance)
			if ((inOcean || Player.ZoneRockLayerHeight) && Main.rand.NextBool(25))
			{
				itemDrop = ModContent.ItemType<Content.Items.Consumables.GrindylowTooth>();
				return;
			}

			// Forbidden Forest water: Enchanted Tadpole (8% chance)
			if (inForest && Main.rand.NextBool(12))
			{
				itemDrop = ModContent.ItemType<Content.Items.Consumables.EnchantedTadpole>();
				return;
			}

			// Hardmode any water: Wizard Crate (2%) or Archmage Crate (1% post-mech)
			if (inHardmode)
			{
				bool postMech = NPC.downedMechBoss1 || NPC.downedMechBoss2 || NPC.downedMechBoss3;
				if (postMech && Main.rand.NextBool(100))
				{
					itemDrop = ModContent.ItemType<Content.Items.Consumables.WizardCrateHardmode>();
					return;
				}
				if (Main.rand.NextBool(50))
				{
					itemDrop = ModContent.ItemType<Content.Items.Consumables.WizardCrate>();
					return;
				}
			}
		}
	}
}
