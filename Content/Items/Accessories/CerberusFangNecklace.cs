using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Cerberus Fang Necklace — crafted from Fluffy's Cerberus Fang.
	/// Triple-bite: attacks have 10% chance to hit 3 times (triple proc).
	/// +5 defense, +3 max minions (three heads, three servants).
	/// </summary>
	public class CerberusFangNecklace : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(gold: 10);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statDefense += 5;
			player.maxMinions += 1;
			player.GetKnockback(DamageClass.Summon) += 1f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.CerberusFang>(), 8)
				.AddIngredient(ItemID.GoldBar, 10)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
