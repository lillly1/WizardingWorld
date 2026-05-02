using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>Gillyweed — grants gills, flipper effect, and water immunity for deep diving.</summary>
	public class Gillyweed : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(silver: 50);
			Item.UseSound = SoundID.Item2;
			Item.buffType = ModContent.BuffType<Buffs.GillyweedBuff>();
			Item.buffTime = 21600; // 6 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ItemID.Waterleaf, 3)
				.AddIngredient(ItemID.Coral, 5)
				.AddIngredient(ItemID.SharkFin, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
