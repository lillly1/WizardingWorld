using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Fenrir's Wolfsbane — upgraded Wolfsbane Potion using Fenrir's Werewolf Pelt.
	/// Much stronger night bonuses than regular Wolfsbane.
	/// "Brewed from the alpha werewolf's own essence — fight fire with fire."
	/// </summary>
	public class FenrirsWolfsbane : ModItem
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
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.Item3;
			Item.buffType = ModContent.BuffType<Buffs.FenrirsWolfsbaneBuff>();
			Item.buffTime = 21600; // 6 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<WolfsbanePotion>(), 1)
				.AddIngredient(ModContent.ItemType<WerewolfPelt>(), 2)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
