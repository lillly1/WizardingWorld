using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Armor.Ravenclaw
{
	[AutoloadEquip(EquipType.Legs)]
	public class RavenclawLeggings : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Orange;
			Item.defense = 4;
		}

		public override void UpdateEquip(Player player)
		{
			player.moveSpeed += 0.06f;
			player.manaRegen += 3;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 12)
				.AddIngredient(ItemID.Sapphire, 2)
				.AddIngredient(ItemID.CopperBar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
