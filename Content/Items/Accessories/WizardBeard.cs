using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Long Wizard Beard — Dumbledore-style vanity. Pure cosmetic.
	/// "It was long enough to tuck into his belt."
	/// </summary>
	[AutoloadEquip(EquipType.Face)]
	public class WizardBeard : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 24;
			Item.vanity = true;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 50);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.SilverBar, 2)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}
