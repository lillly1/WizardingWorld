using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>Amortentia — the most powerful love potion. Reduces enemy aggro and grants passive regen.</summary>
	public class Amortentia : ModItem
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
			Item.buffType = ModContent.BuffType<Buffs.AmortentiaBuff>();
			Item.buffTime = 21600; // 6 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ItemID.Daybloom, 2)
				.AddIngredient(ItemID.Moonglow, 1)
				.AddIngredient(ItemID.PinkGel, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Alternative recipe with Ashwinder Egg (lore-accurate)
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ModContent.ItemType<AshwinderEgg>(), 2)
				.AddIngredient(ItemID.Moonglow, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
