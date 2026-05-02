using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.Slytherin
{
	[AutoloadEquip(EquipType.Body)]
	public class SlytherinRobes : ModItem
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
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.08f;
			player.GetCritChance(DamageClass.Generic) += 4;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 15)
				.AddIngredient(ItemID.Emerald, 3)
				.AddIngredient(ItemID.SilverBar, 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
