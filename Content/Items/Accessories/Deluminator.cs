using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Deluminator (Put-Outer) — Dumbledore's invention.
	/// As accessory: grants permanent Shine buff (player glows) and Night Owl vision.
	/// Toggleable: provides massive light radius around the player.
	/// </summary>
	public class Deluminator : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 10);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// Permanent shine and night owl
			player.nightVision = true;
			Lighting.AddLight(player.Center, 1.5f, 1.4f, 1.0f);

			// Light absorption aura — slightly reduce enemy spawn rate nearby
			player.AddBuff(BuffID.Calm, 2);

			// Silver smoke particles
			if (Main.rand.NextBool(12))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.SilverCoin, 0f, -0.5f, 100, default, 0.5f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SilverBar, 10)
				.AddIngredient(ItemID.FallenStar, 15)
				.AddIngredient(ItemID.SoulofLight, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
