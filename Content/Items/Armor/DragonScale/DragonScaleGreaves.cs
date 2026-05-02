using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Armor.DragonScale
{
	[AutoloadEquip(EquipType.Legs)]
	public class DragonScaleGreaves : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 8);
			Item.rare = ItemRarityID.Pink;
			Item.defense = 12;
		}

		public override void UpdateEquip(Player player)
		{
			player.moveSpeed += 0.15f;
			player.maxRunSpeed += 1f;
			player.manaRegen += 5;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 10)
				.AddIngredient(ItemID.HallowedBar, 12)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
