using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Aqua Fortis — water warrior potion crafted from fishing catches.
	/// Grants gills + water immunity. While underwater: +20% damage, +8 defense, +20% speed.
	/// Turns any player into an underwater combat specialist.
	/// Rewards dedicated fishers with a powerful combat buff.
	/// </summary>
	public class AquaFortis : ModItem
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
			Item.buffType = ModContent.BuffType<Buffs.AquaFortisBuff>();
			Item.buffTime = 14400; // 4 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ModContent.ItemType<MerfolkScale>(), 3)
				.AddIngredient(ModContent.ItemType<GrindylowTooth>(), 2)
				.AddIngredient(ItemID.Waterleaf, 2)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
