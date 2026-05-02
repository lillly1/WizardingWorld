using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Skele-Gro — regrows bones. Massive life regeneration but painful (reduced speed).
	/// "You're in for a rough night" — Madam Pomfrey.
	/// </summary>
	public class SkeleGro : ModItem
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
			Item.buffType = ModContent.BuffType<Buffs.SkeleGroBuff>();
			Item.buffTime = 10800; // 3 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ItemID.Bone, 10)
				.AddIngredient(ItemID.Daybloom, 2)
				.AddIngredient(ItemID.Blinkroot, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
