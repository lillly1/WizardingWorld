using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Seeker's Reflexes Potion — brewed from Golden Egg.
	/// Grants lightning reflexes: speed, dodge, attack speed.
	/// "You have to be quick to catch the Snitch."
	/// Uses the rare Golden Egg — rewards Horntail fighters.
	/// </summary>
	public class SeekersReflexes : ModItem
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
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.Item3;
			Item.buffType = ModContent.BuffType<Buffs.SeekersReflexesBuff>();
			Item.buffTime = 14400; // 4 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ItemID.BottledWater, 3)
				.AddIngredient(ModContent.ItemType<GoldenEgg>(), 1)
				.AddIngredient(ItemID.Feather, 5)
				.AddIngredient(ItemID.SwiftnessPotion, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
