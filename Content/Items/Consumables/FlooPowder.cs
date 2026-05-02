using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Floo Powder — teleport to your spawn point instantly with a green flame effect.
	/// Faster than Recall Potion, no channel time, but single-use.
	/// Cheaper to craft in bulk than Portkeys.
	/// </summary>
	public class FlooPowder : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 10; // Very fast — just throw and go
			Item.useAnimation = 10;
			Item.maxStack = 99;
			Item.consumable = true;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(silver: 15);
			Item.UseSound = SoundID.Item46;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				// Green flame departure effect
				for (int i = 0; i < 40; i++)
				{
					Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.GreenTorch, 0f, 0f, 50, default, 2f);
					dust.noGravity = true;
					dust.velocity = Main.rand.NextVector2Circular(4f, 6f);
					dust.velocity.Y -= 3f; // Flames go up
				}

				player.Spawn(PlayerSpawnContext.RecallFromItem);

				// Green flame arrival effect
				for (int i = 0; i < 40; i++)
				{
					Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.GreenTorch, 0f, 0f, 50, default, 2f);
					dust.noGravity = true;
					dust.velocity = Main.rand.NextVector2Circular(4f, 6f);
					dust.velocity.Y -= 3f;
				}
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(5)
				.AddIngredient(ItemID.SandBlock, 5)
				.AddIngredient(ItemID.FallenStar, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
