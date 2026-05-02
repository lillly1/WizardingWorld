using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>Wolfsbane Potion — grants werewolf-like power boost, stronger at night.</summary>
	public class WolfsbanePotion : ModItem
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
			Item.buffType = ModContent.BuffType<Buffs.WolfsbaneBuff>();
			Item.buffTime = 28800; // 8 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ItemID.Deathweed, 2)
				.AddIngredient(ItemID.Blinkroot, 1)
				.AddIngredient(ItemID.Shiverthorn, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Alt recipe with Grindylow Tooth (water creature parts for bestial potion)
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ModContent.ItemType<GrindylowTooth>(), 3)
				.AddIngredient(ItemID.Deathweed, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
