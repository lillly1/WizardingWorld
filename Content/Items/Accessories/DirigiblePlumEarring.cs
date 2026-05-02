using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Dirigible Plum Earring — Luna Lovegood's signature accessory.
	/// Grants slow fall (featherfall), +2 extra jumps, and +10% jump speed.
	/// "Dirigible Plums help you accept the extraordinary."
	/// A quirky but genuinely useful mobility accessory.
	/// </summary>
	public class DirigiblePlumEarring : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(gold: 4);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.slowFall = true;
			player.extraFall += 30;
			player.jumpSpeedBoost += 2f;
			player.autoJump = true;

			// Orange plum glow while falling
			if (!hideVisual && player.velocity.Y > 2f && Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.OrangeTorch, 0f, 1f, 100, default, 0.5f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Feather, 5)
				.AddIngredient(ItemID.Cloud, 10)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
