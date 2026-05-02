using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Armor.Slytherin
{
	[AutoloadEquip(EquipType.Legs)]
	public class SlytherinLeggings : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Orange;
			Item.defense = 5;
		}

		public override void UpdateEquip(Player player)
		{
			player.moveSpeed += 0.08f;
			player.aggro -= 100; // Slytherin: sneakier
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 12)
				.AddIngredient(ItemID.Emerald, 2)
				.AddIngredient(ItemID.SilverBar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
