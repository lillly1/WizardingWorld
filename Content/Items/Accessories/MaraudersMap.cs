using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Marauder's Map — reveals all enemies, NPCs, treasure, and hazards on the minimap.
	/// Enhanced: grants spelunker, danger sense, night vision, and map tile reveal.
	/// Synergy: reduces Secret Passage Network cooldown from 5 minutes to 3 minutes.
	/// </summary>
	public class MaraudersMap : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(gold: 5);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
			wp.hasMaraudersMap = true;
			player.detectCreature = true;
			player.dangerSense = true;

			// "I solemnly swear that I am up to no good" — reveal map
			// Reveals tiles around the player in a large radius (spelunker effect on steroids)
			player.findTreasure = true;
			player.nightVision = true;

			// Slowly reveal map tiles in a 100-tile radius via lighting
			if (Main.GameUpdateCount % 5 == 0)
			{
				int revealRadius = 100;
				int px = (int)(player.Center.X / 16f);
				int py = (int)(player.Center.Y / 16f);
				int rx = px + Main.rand.Next(-revealRadius, revealRadius);
				int ry = py + Main.rand.Next(-revealRadius, revealRadius);

				if (WorldGen.InWorld(rx, ry, 10))
					Lighting.AddLight(new Vector2(rx * 16f, ry * 16f), 0.5f, 0.5f, 0.5f);
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Book, 1)
				.AddIngredient(ItemID.GoldBar, 5)
				.AddIngredient(ItemID.Silk, 5)
				.AddIngredient(ItemID.FallenStar, 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
