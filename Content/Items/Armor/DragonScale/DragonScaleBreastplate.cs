using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.DragonScale
{
	[AutoloadEquip(EquipType.Body)]
	public class DragonScaleBreastplate : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 8);
			Item.rare = ItemRarityID.Pink;
			Item.defense = 16;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.12f;
			player.endurance += 0.06f;
			player.statLifeMax2 += 40;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 12)
				.AddIngredient(ItemID.HallowedBar, 15)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
