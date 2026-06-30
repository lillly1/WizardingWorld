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
			Item.value = Item.buyPrice(gold: 1, silver: 50);
			Item.rare = ItemRarityID.Orange;
			Item.createTile = ModContent.TileType<Tiles.EnchantingTable>();
		}

		public override void AddRecipes()
		{
			// Main recipe: reachable before dungeon books or town NPC shopping.
			CreateRecipe()
				.AddRecipeGroup("WizardingWorld:AnyWood", 20)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ItemID.Amethyst, 2)
				.AddIngredient(ItemID.Torch, 5)
				.AddTile(TileID.WorkBenches)
				.Register();

			// Gem-free fallback for worlds where early amethyst is slow to find.
			CreateRecipe()
				.AddRecipeGroup("WizardingWorld:AnyWood", 30)
				.AddIngredient(ItemID.FallenStar, 8)
				.AddIngredient(ItemID.StoneBlock, 25)
				.AddIngredient(ItemID.Torch, 10)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
