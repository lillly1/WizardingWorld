using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.Ravenclaw
{
	[AutoloadEquip(EquipType.Body)]
	public class RavenclawRobes : ModItem
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
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.10f;
			player.statManaMax2 += 20;
			player.manaCost -= 0.10f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 15)
				.AddIngredient(ItemID.Sapphire, 3)
				.AddIngredient(ItemID.CopperBar, 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
