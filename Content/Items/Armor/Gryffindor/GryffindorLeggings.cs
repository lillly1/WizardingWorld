using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Armor.Gryffindor
{
	[AutoloadEquip(EquipType.Legs)]
	public class GryffindorLeggings : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Orange;
			Item.defense = 6;
		}

		public override void UpdateEquip(Player player)
		{
			player.moveSpeed += 0.10f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 12)
				.AddIngredient(ItemID.Ruby, 2)
				.AddIngredient(ItemID.GoldBar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
