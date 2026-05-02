using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Shield Charm Potion — liquid Protego in a bottle.
	/// Grants +10 defense and +8% damage reduction for 5 minutes.
	/// "A portable Protego charm, bottled for convenience."
	/// </summary>
	public class ShieldCharmPotion : ModItem
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
			Item.buffType = ModContent.BuffType<Buffs.ShieldCharmBuff>();
			Item.buffTime = 18000; // 5 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ItemID.BottledWater, 3)
				.AddIngredient(ItemID.IronBar, 5)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
