using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Armor.Wizengamot
{
	[AutoloadEquip(EquipType.Legs)]
	public class WizengamotBoots : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 20);
			Item.rare = ItemRarityID.Red;
			Item.defense = 16;
		}

		public override void UpdateEquip(Player player)
		{
			player.moveSpeed += 0.15f;
			player.maxRunSpeed += 1.5f;
			player.manaRegen += 6;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DementorsShroud>(), 5)
				.AddIngredient(ItemID.LunarBar, 8)
				.AddIngredient(ItemID.FragmentNebula, 8)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 20)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
