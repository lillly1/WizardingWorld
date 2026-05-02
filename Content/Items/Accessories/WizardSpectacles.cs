using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Wizard Spectacles — round glasses vanity accessory.
	/// Harry's iconic round spectacles. Pure vanity — no stats.
	/// </summary>
	[AutoloadEquip(EquipType.Face)]
	public class WizardSpectacles : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 14;
			Item.vanity = true;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 50);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Glass, 5)
				.AddIngredient(ItemID.IronBar, 2)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
