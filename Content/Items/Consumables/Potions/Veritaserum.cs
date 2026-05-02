using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>Veritaserum — the truth serum. Reveals everything hidden: enemies, traps, and treasure.</summary>
	public class Veritaserum : ModItem
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
			Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.Item3;
			Item.buffType = ModContent.BuffType<Buffs.VeritaserumBuff>();
			Item.buffTime = 18000; // 5 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ItemID.Moonglow, 2)
				.AddIngredient(ItemID.Waterleaf, 2)
				.AddIngredient(ItemID.Lens, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Lore-accurate recipe with Jobberknoll Feather (truth serum ingredient)
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
				.AddIngredient(ModContent.ItemType<JobberknollFeather>(), 3)
				.AddIngredient(ItemID.Moonglow, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
