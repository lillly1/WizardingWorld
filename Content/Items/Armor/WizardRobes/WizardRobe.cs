using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Armor.WizardRobes
{
	/// <summary>Wizard Robe — vanity set body piece. Classic flowing wizard robes.</summary>
	[AutoloadEquip(EquipType.Body)]
	public class WizardRobe : ModItem
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
				.AddIngredient(ItemID.Silk, 12)
				.AddIngredient(ItemID.FallenStar, 2)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}
