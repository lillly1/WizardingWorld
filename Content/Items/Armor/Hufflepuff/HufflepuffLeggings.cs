using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Armor.Hufflepuff
{
	[AutoloadEquip(EquipType.Legs)]
	public class HufflepuffLeggings : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Orange;
			Item.defense = 7;
		}

		public override void UpdateEquip(Player player)
		{
			player.moveSpeed += 0.05f;
			player.statDefense += 2;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 12)
				.AddIngredient(ItemID.Topaz, 2)
				.AddIngredient(ItemID.IronBar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
