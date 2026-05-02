using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Armor.WizardRobes
{
	/// <summary>Wizard Boots — vanity set leg piece. Classic wizard boots.</summary>
	[AutoloadEquip(EquipType.Legs)]
	public class WizardBoots : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.vanity = true;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ItemRarityID.Blue;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 8)
				.AddIngredient(ItemID.Leather, 3)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}
