using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Draconis Elixir — the 400th C# file in the Wizarding World mod.
	/// Dragon blood potion granting +15% all damage, fire immunity, and inferno aura.
	/// "Dragon blood has twelve uses — this potion harnesses them all."
	/// Crafted from Dragon Scale + Phoenix Ash + Essence.
	/// </summary>
	public class DraconisElixir : ModItem
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
			Item.value = Item.buyPrice(gold: 5);
			Item.UseSound = SoundID.Item3;
			Item.buffType = BuffID.Inferno;
			Item.buffTime = 14400; // 4 minutes
		}

		public override void OnConsumeItem(Player player)
		{
			player.AddBuff(BuffID.ObsidianSkin, 14400);
			player.AddBuff(BuffID.Wrath, 14400);
			player.AddBuff(BuffID.Rage, 14400);
			player.AddBuff(BuffID.Inferno, 14400);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<DragonScale>(), 3)
				.AddIngredient(ModContent.ItemType<PhoenixAsh>(), 2)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Alt recipe using Imp Flame instead of Phoenix Ash
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<DragonScale>(), 3)
				.AddIngredient(ModContent.ItemType<ImpFlame>(), 5)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
