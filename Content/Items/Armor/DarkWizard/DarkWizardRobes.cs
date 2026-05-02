using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.DarkWizard
{
	[AutoloadEquip(EquipType.Body)]
	public class DarkWizardRobes : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 8);
			Item.rare = ItemRarityID.Pink;
			Item.defense = 10;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.10f;
			player.statManaMax2 += 40;
			player.manaCost -= 0.15f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 20)
				.AddIngredient(ItemID.SoulofNight, 15)
				.AddIngredient(ItemID.DarkShard, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
