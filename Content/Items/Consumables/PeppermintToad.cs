using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Peppermint Toad — Honeydukes sweet.
	/// Grants Swiftness + Jump Boost for 5 minutes. The toad hops in your stomach!
	/// </summary>
	public class PeppermintToad : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(silver: 8);
			Item.UseSound = SoundID.Item2;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				player.AddBuff(BuffID.Swiftness, 18000); // 5 minutes
				player.AddBuff(BuffID.Flipper, 18000);
				player.AddBuff(BuffID.WellFed, 18000);
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(5)
				.AddIngredient(ItemID.Mushroom, 2)
				.AddIngredient(ItemID.Shiverthorn, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
