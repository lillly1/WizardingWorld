using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.Wizengamot
{
	[AutoloadEquip(EquipType.Body)]
	public class WizengamotRobes : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 20);
			Item.rare = ItemRarityID.Red;
			Item.defense = 20;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.15f;
			player.GetCritChance(ModContent.GetInstance<SpellDamage>()) += 15;
			player.endurance += 0.08f;
			player.statLifeMax2 += 60;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DementorsShroud>(), 8)
				.AddIngredient(ItemID.LunarBar, 12)
				.AddIngredient(ItemID.FragmentNebula, 12)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 25)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
