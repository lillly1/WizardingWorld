using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Wizard's Pocket Watch — magical timepiece that shows time and boosts speed.
	/// Grants full watch display (time of day), +5% movement speed, +5% attack speed.
	/// "Time is a mysterious thing. Powerful, and when meddled with, dangerous."
	/// </summary>
	public class WizardPocketWatch : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(gold: 3);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.accWatch = 3; // Full time display
			player.moveSpeed += 0.05f;
			player.GetAttackSpeed(DamageClass.Generic) += 0.05f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.GoldWatch, 1)
				.AddIngredient(ItemID.FallenStar, 3)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			CreateRecipe()
				.AddIngredient(ItemID.PlatinumWatch, 1)
				.AddIngredient(ItemID.FallenStar, 3)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
