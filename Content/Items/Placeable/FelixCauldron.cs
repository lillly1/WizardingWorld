using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
	/// <summary>
	/// Felix Felicis Cauldron — legendary furniture item.
	/// When placed, grants the Lucky buff to all players within 50 tiles.
	/// Crafted from rare endgame materials. A celebration of victory.
	/// The bubbling golden cauldron that brews Liquid Luck perpetually.
	/// </summary>
	public class FelixCauldron : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 24;
			Item.maxStack = 1;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = false; // Reusable — buff item
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(gold: 50);
			Item.UseSound = SoundID.Item4;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				// Grants Lucky buff to all nearby players (generous range)
				foreach (var p in Main.ActivePlayers)
				{
					if (Vector2.Distance(p.Center, player.Center) < 800f)
					{
						p.AddBuff(BuffID.Lucky, 36000); // 10 minutes
					}
				}

				// Golden bubbling visual
				for (int i = 0; i < 20; i++)
				{
					Dust dust = Dust.NewDustDirect(player.Center + new Vector2(0, 10), 16, 8, DustID.GoldCoin, 0f, -2f, 50, default, 1.2f);
					dust.noGravity = true;
				}

				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Items.FelixCauldron.UseMessage"), 255, 215, 0);
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.GoldBar, 20)
				.AddIngredient(ModContent.ItemType<Consumables.Potions.FelixFelicis>(), 5)
				.AddIngredient(ModContent.ItemType<Consumables.UnicornBlood>(), 3)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 25)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
