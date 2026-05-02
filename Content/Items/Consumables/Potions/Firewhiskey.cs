using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Ogden's Old Firewhiskey — strong wizard drink.
	/// Grants +12% all damage and +8% crit for 4 minutes.
	/// Side effect: slight Confused chance (you're tipsy) and fire particles.
	/// "The drink that warms you from the inside out... violently."
	/// </summary>
	public class Firewhiskey : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(silver: 40);
			Item.UseSound = SoundID.Item3;
			Item.buffType = ModContent.BuffType<Buffs.FirewhiskeyBuff>();
			Item.buffTime = 14400; // 4 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ItemID.Bottle, 3)
				.AddIngredient(ItemID.Fireblossom, 2)
				.AddIngredient(ItemID.Ale, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Alt recipe with Imp Flame (fire creature ingredient)
			CreateRecipe(3)
				.AddIngredient(ItemID.Bottle, 3)
				.AddIngredient(ModContent.ItemType<ImpFlame>(), 3)
				.AddIngredient(ItemID.Ale, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
