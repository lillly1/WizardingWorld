using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Resurrection Potion — crafted from Phoenix Ash + Phoenix Tear ingredients.
	/// Grants a one-time death prevention: if you would die, instead heal to 50% HP.
	/// The buff lasts 10 minutes but only triggers once.
	/// "Rise from the ashes."
	/// </summary>
	public class ResurrectionPotion : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.maxStack = 10;
			Item.consumable = true;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(gold: 5);
			Item.UseSound = SoundID.Item3;
			Item.buffType = ModContent.BuffType<Buffs.ResurrectionBuff>();
			Item.buffTime = 36000; // 10 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<PhoenixAsh>(), 3)
				.AddIngredient(ItemID.LifeCrystal, 1)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
