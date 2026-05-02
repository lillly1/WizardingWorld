using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Stealth Draught — brewed from Dugbog Hide + Demiguise Hair.
	/// Grants invisibility, massive aggro reduction, and speed boost.
	/// "Blend into the shadows like a Dugbog blends into a log."
	/// The 900th total file in the Wizarding World mod.
	/// </summary>
	public class StealthDraught : ModItem
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
			Item.value = Item.buyPrice(silver: 40);
			Item.UseSound = SoundID.Item3;
			Item.buffType = ModContent.BuffType<Buffs.StealthDraughtBuff>();
			Item.buffTime = 14400; // 4 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ItemID.BottledWater, 3)
				.AddIngredient(ModContent.ItemType<DugbogHide>(), 3)
				.AddIngredient(ModContent.ItemType<DemiguiseHair>(), 1)
				.AddIngredient(ItemID.Moonglow, 2)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
