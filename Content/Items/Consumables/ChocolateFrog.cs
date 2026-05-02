using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Chocolate Frog — heals 100 HP instantly and grants brief speed boost. Recommended by Madam Pomfrey.</summary>
	public class ChocolateFrog : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(silver: 20);
			Item.UseSound = SoundID.Item2;
			Item.healLife = 100;
			Item.potion = true;
		}

		public override void OnConsumeItem(Player player)
		{
			// Brief speed boost — the frog energy
			player.AddBuff(BuffID.Swiftness, 600); // 10 seconds
			player.AddBuff(BuffID.WellFed, 1800); // 30 seconds well-fed
		}

		public override void AddRecipes()
		{
			CreateRecipe(5)
				.AddIngredient(ItemID.Bottle, 5)
				.AddIngredient(ItemID.Mushroom, 3)
				.AddIngredient(ItemID.GoldBar, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
