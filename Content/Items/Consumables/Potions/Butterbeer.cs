using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	public class Butterbeer : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(silver: 10);
			Item.UseSound = SoundID.Item3;
			Item.buffType = ModContent.BuffType<Buffs.ButterbeerBuff>();
			Item.buffTime = 36000; // 10 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ItemID.Bottle, 3)
				.AddIngredient(ItemID.Mushroom, 2)
				.AddIngredient(ItemID.Daybloom, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
