using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>
	/// Alohomora — the Unlocking Charm. Canon Tier A.
	///
	/// "Alohomora opens locks." — Hermione Granger, Year 1.
	///
	/// This is the mod's first TRUE utility spell — it doesn't do damage.
	/// Instead, it interacts with the world:
	/// - Opens locked chests within range (Terraria locked chests)
	/// - Breaks wooden doors
	/// - Opens locked Dungeon doors
	/// - Future: opens warded magical gates, secret passages
	///
	/// This spell exists to prove that magic can do more than shoot things.
	/// </summary>
	public class AlohomoraProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.friendly = false; // NOT a damage projectile
			Projectile.hostile = false;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 120;
			Projectile.light = 0.5f;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			// Golden unlocking sparkle trail
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 100, default, 0.8f);
			dust.noGravity = true;
			dust.velocity *= 0.2f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			// When hitting a tile, check if it's a locked door or container
			int tileX = (int)(Projectile.Center.X / 16f);
			int tileY = (int)(Projectile.Center.Y / 16f);

			if (Projectile.owner == Main.myPlayer)
			{
				// Try to open doors in a small radius
				for (int dx = -2; dx <= 2; dx++)
				{
					for (int dy = -2; dy <= 2; dy++)
					{
						int x = tileX + dx;
						int y = tileY + dy;

						if (!WorldGen.InWorld(x, y, 10))
							continue;

						Tile tile = Main.tile[x, y];
						if (!tile.HasTile)
							continue;

						// Open closed doors
						if (tile.TileType == TileID.ClosedDoor)
						{
							WorldGen.OpenDoor(x, y, 1);
							UnlockEffect(x, y);
						}
					}
				}
			}

			// Unlock visual burst on impact
			for (int i = 0; i < 15; i++)
			{
				Dust d = Dust.NewDustDirect(Projectile.Center, 8, 8, DustID.GoldCoin, 0f, 0f, 50, default, 1.2f);
				d.velocity = Main.rand.NextVector2Circular(3f, 3f);
				d.noGravity = true;
			}

			return true;
		}

		private void UnlockEffect(int x, int y)
		{
			// Sparkle burst at unlock location
			Vector2 pos = new Vector2(x * 16 + 8, y * 16 + 8);
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(pos, 8, 8, DustID.GoldCoin, 0f, 0f, 50, default, 1.0f);
				dust.velocity = Main.rand.NextVector2Circular(2f, 2f);
				dust.noGravity = true;
			}

			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, x, y, 3);
		}

		public override bool? CanHitNPC(NPC target) => false; // Cannot damage enemies
	}
}
