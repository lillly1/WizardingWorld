using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Portkey — instantly teleports the player to their spawn point. Single-use.</summary>
	public class Portkey : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(silver: 50);
			Item.UseSound = SoundID.Item6;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				// Teleport dust at departure
				for (int i = 0; i < 30; i++)
				{
					Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 50, default, 1.5f);
					dust.noGravity = true;
					dust.velocity *= 3f;
				}

				// Teleport to spawn
				player.Spawn(PlayerSpawnContext.RecallFromItem);

				// Arrival dust
				for (int i = 0; i < 30; i++)
				{
					Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 50, default, 1.5f);
					dust.noGravity = true;
					dust.velocity *= 3f;
				}
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ItemID.RecallPotion, 3)
				.AddIngredient(ItemID.FallenStar, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Can also use any junk item + magic
			CreateRecipe(1)
				.AddIngredient(ItemID.OldShoe, 1)
				.AddIngredient(ItemID.FallenStar, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
