using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Fairy Wings — magical wings crafted from Pixie Dust and Essence.
	/// Grants flight + slow fall + sparkle trail.
	/// "The fairies of the wizarding world are far more beautiful — and more dangerous."
	/// Mid-game wing accessory.
	/// </summary>
	[AutoloadEquip(EquipType.Wings)]
	public class FairyWings : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 28;
			Item.accessory = true;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(gold: 8);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.wingTimeMax = 120; // 2 seconds flight time

			// Sparkle trail while flying
			if (!hideVisual && player.wingTime > 0 && Main.rand.NextBool(3))
			{
				int[] colors = { DustID.PinkTorch, DustID.BlueTorch, DustID.YellowStarDust };
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
					colors[Main.rand.Next(3)], 0f, 1f, 100, default, 0.5f);
				dust.noGravity = true;
			}
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 0.85f;
			ascentWhenRising = 0.15f;
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 3f;
			constantAscend = 0.135f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.PixieDust, 20)
				.AddIngredient(ItemID.SoulofFlight, 15)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
