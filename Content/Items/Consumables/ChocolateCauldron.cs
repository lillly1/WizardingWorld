using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Chocolate Cauldron — Honeydukes sweet filled with Firewhisky.
	/// Restores 100 mana instantly + grants Magic Power buff.
	/// The mana equivalent of Chocolate Frog's healing.
	/// </summary>
	public class ChocolateCauldron : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(silver: 10);
			Item.UseSound = SoundID.Item3;
			Item.healMana = 100;
			Item.potion = true;
		}

		public override void OnConsumeItem(Player player)
		{
			player.AddBuff(BuffID.MagicPower, 14400); // 4 minutes
			player.AddBuff(BuffID.ManaRegeneration, 14400);
		}

		public override void AddRecipes()
		{
			CreateRecipe(5)
				.AddIngredient(ItemID.Bottle, 5)
				.AddIngredient(ItemID.Mushroom, 2)
				.AddIngredient(ItemID.Moonglow, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
