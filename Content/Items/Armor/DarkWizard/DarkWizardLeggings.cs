using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Armor.DarkWizard
{
	[AutoloadEquip(EquipType.Legs)]
	public class DarkWizardLeggings : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 8);
			Item.rare = ItemRarityID.Pink;
			Item.defense = 8;
		}

		public override void UpdateEquip(Player player)
		{
			player.moveSpeed += 0.12f;
			player.manaRegen += 5;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 15)
				.AddIngredient(ItemID.SoulofNight, 10)
				.AddIngredient(ItemID.DarkShard, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
