using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Armor.Hufflepuff
{
	[AutoloadEquip(EquipType.Body)]
	public class HufflepuffRobes : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Orange;
			Item.defense = 9;
		}

		public override void UpdateEquip(Player player)
		{
			player.statLifeMax2 += 40;
			player.lifeRegen += 2;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 15)
				.AddIngredient(ItemID.Topaz, 3)
				.AddIngredient(ItemID.IronBar, 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
