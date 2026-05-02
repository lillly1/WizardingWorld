using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Wand Holster — quick-draw accessory for spell casters.
	/// +10% spell casting speed (attack speed for SpellDamage class).
	/// +5% spell damage. Reduces mana cost by 5%.
	/// "Draw faster, cast faster, win faster."
	/// </summary>
	public class WandHolster : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(gold: 6);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetAttackSpeed(ModContent.GetInstance<SpellDamage>()) += 0.10f;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.05f;
			player.manaCost -= 0.05f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Leather, 8)
				.AddIngredient(ItemID.GoldBar, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
