using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Ambient Magic System — makes the world feel magical.
	/// Creates subtle visual effects:
	/// - Sparkle particles around placed Enchanting Tables
	/// - Floating candle-like lights near Town NPCs from this mod
	/// - Magical mist in the Forbidden Forest biome
	/// - Subtle star particles at night when the player has spell damage items
	/// This doesn't affect gameplay — purely atmospheric.
	/// </summary>
	public class AmbientMagicSystem : ModSystem
	{
		public override void PostUpdateWorld()
		{
			if (Main.dedServ || Main.gameMenu)
				return;

			// Only run for the local player
			Player player = Main.LocalPlayer;
			if (player == null || !player.active)
				return;

			EnchantingTableParticles(player);
			ForbiddenForestMist(player);
			WizardStarParticles(player);
		}

		private void EnchantingTableParticles(Player player)
		{
			// Search for nearby Enchanting Tables and spawn purple sparkles
			int tableType = ModContent.TileType<Content.Tiles.EnchantingTable>();
			int px = (int)(player.Center.X / 16f);
			int py = (int)(player.Center.Y / 16f);
			int searchRadius = 30; // tiles

			for (int x = px - searchRadius; x <= px + searchRadius; x++)
			{
				for (int y = py - searchRadius; y <= py + searchRadius; y++)
				{
					if (!WorldGen.InWorld(x, y, 10))
						continue;

					Tile tile = Main.tile[x, y];
					if (tile.HasTile && tile.TileType == tableType)
					{
						// Subtle sparkle above the table (very rare to avoid spam)
						if (Main.rand.NextBool(120))
						{
							Vector2 pos = new Vector2(x * 16 + 8, y * 16 - 8);
							Dust dust = Dust.NewDustDirect(pos, 8, 8, DustID.PurpleTorch, 0f, -0.5f, 100, default, 0.5f);
							dust.noGravity = true;
							dust.velocity *= 0.3f;
						}

						return; // Only process one table
					}
				}
			}
		}

		private void ForbiddenForestMist(Player player)
		{
			if (!player.InModBiome<Content.Biomes.ForbiddenForestBiome>())
				return;

			// Eerie green mist particles in the Forbidden Forest
			if (Main.rand.NextBool(15))
			{
				Vector2 pos = player.Center + Main.rand.NextVector2Circular(400, 200);
				Dust dust = Dust.NewDustDirect(pos, 8, 8, DustID.GreenTorch, Main.rand.NextFloat(-0.5f, 0.5f), -0.3f, 180, default, 0.4f);
				dust.noGravity = true;
				dust.fadeIn = 1.2f;
			}

			// Occasional dark wisps
			if (Main.rand.NextBool(40))
			{
				Vector2 pos = player.Center + Main.rand.NextVector2Circular(300, 150);
				Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.Shadowflame, 0f, -0.2f, 200, default, 0.3f);
				dust.noGravity = true;
			}
		}

		private void WizardStarParticles(Player player)
		{
			// At night, if player has any wizard equipment, subtle star particles
			if (Main.dayTime)
				return;

			var wp = player.GetModPlayer<Players.WizardPlayer>();
			bool hasWizardGear = wp.houseSet > 0 || wp.hasMaraudersMap || wp.patronusActive;

			if (!hasWizardGear)
				return;

			// Very subtle star twinkle near the player
			if (Main.rand.NextBool(60))
			{
				Vector2 pos = player.Center + Main.rand.NextVector2Circular(100, 60) + new Vector2(0, -40);
				Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.YellowStarDust, 0f, 0f, 100, default, 0.3f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
				dust.fadeIn = 0.8f;
			}
		}
	}
}
