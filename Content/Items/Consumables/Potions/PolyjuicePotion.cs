using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>Polyjuice Potion — grants disguise/stealth.</summary>
	public class PolyjuicePotion : ModItem
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
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item3;
			Item.buffType = ModContent.BuffType<Buffs.PolyjuiceBuff>();
			Item.buffTime = 18000; // 5 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ItemID.Deathweed, 1)
				.AddIngredient(ItemID.Moonglow, 1)
				.AddIngredient(ItemID.Cobweb, 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Alt recipe with Demiguise Hair (lore — invisibility/shapeshifting)
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ModContent.ItemType<DemiguiseHair>(), 3)
				.AddIngredient(ItemID.Deathweed, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
