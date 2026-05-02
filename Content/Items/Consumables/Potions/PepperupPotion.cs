using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Pepperup Potion — cures colds and boosts energy.
	/// Grants massive speed boost, attack speed, fire/cold immunity.
	/// Side effect: steam comes out of your ears for hours.
	/// </summary>
	public class PepperupPotion : ModItem
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
			Item.value = Item.buyPrice(silver: 30);
			Item.UseSound = SoundID.Item3;
			Item.buffType = ModContent.BuffType<Buffs.PepperupBuff>();
			Item.buffTime = 14400; // 4 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ItemID.Fireblossom, 2)
				.AddIngredient(ItemID.Shiverthorn, 1)
				.AddIngredient(ItemID.Blinkroot, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Alt recipe with Ashwinder Egg (lore — warms you from the inside)
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ModContent.ItemType<AshwinderEgg>(), 1)
				.AddIngredient(ItemID.Shiverthorn, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
