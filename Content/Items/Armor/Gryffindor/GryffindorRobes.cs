using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.Gryffindor
{
	[AutoloadEquip(EquipType.Body)]
	public class GryffindorRobes : ModItem
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
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.05f;
			player.statLifeMax2 += 20;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 15)
				.AddIngredient(ItemID.Ruby, 3)
				.AddIngredient(ItemID.GoldBar, 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
