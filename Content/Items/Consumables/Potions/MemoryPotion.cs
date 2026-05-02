using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Memory Potion — brewed from Jobberknoll Feather (lore-accurate).
	/// Grants full minimap reveal around the player + spelunker + hunter effects.
	/// "The Jobberknoll's final scream contains every sound it ever heard —
	/// its feather unlocks every memory the land holds."
	/// </summary>
	public class MemoryPotion : ModItem
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
			Item.value = Item.buyPrice(silver: 25);
			Item.UseSound = SoundID.Item3;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				player.AddBuff(BuffID.Spelunker, 14400);
				player.AddBuff(BuffID.Hunter, 14400);
				player.AddBuff(BuffID.NightOwl, 14400);
				player.AddBuff(BuffID.Dangersense, 14400);
			}
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ItemID.BottledWater, 3)
				.AddIngredient(ModContent.ItemType<JobberknollFeather>(), 2)
				.AddIngredient(ItemID.Moonglow, 1)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
