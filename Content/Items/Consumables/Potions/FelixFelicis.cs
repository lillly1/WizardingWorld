using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>Felix Felicis — Liquid Luck. Expensive but powerful.</summary>
	public class FelixFelicis : ModItem
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
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item3;
			Item.buffType = ModContent.BuffType<Buffs.FelixFelicisBuff>();
			Item.buffTime = 14400; // 4 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ItemID.GoldBar, 3)
				.AddIngredient(ItemID.Moonglow, 2)
				.AddIngredient(ItemID.Waterleaf, 2)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Alternative with Ashwinder Egg (lore-accurate ingredient)
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ModContent.ItemType<AshwinderEgg>(), 1)
				.AddIngredient(ItemID.GoldBar, 2)
				.AddIngredient(ItemID.Moonglow, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
