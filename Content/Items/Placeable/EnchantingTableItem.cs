using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
	public class EnchantingTableItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 24;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
			Item.createTile = ModContent.TileType<Tiles.EnchantingTable>();
		}

		public override void AddRecipes()
		{
			// Main recipe
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 20)
				.AddIngredient(ItemID.Book, 5)
				.AddIngredient(ItemID.FallenStar, 10)
				.AddIngredient(ItemID.Amethyst, 5)
				.AddTile(TileID.WorkBenches)
				.Register();

			// Easier alternative recipe — no gems required, just more common materials
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 30)
				.AddIngredient(ItemID.Book, 3)
				.AddIngredient(ItemID.FallenStar, 15)
				.AddIngredient(ItemID.Torch, 10)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
