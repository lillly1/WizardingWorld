using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>
	/// Bombarda — explosive curse that destroys tiles in an area on impact.
	/// The spell Hermione used to break Sirius out of the tower.
	/// Combines combat damage with terrain modification utility.
	/// </summary>
	public class BombardaProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 1;
			Projectile.timeLeft = 240;
			Projectile.light = 0.6f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			// Orange-white explosive trail
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.2f);
			dust.noGravity = true;
			dust.velocity *= 0.15f;

			if (Main.rand.NextBool(3))
			{
				Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 0.8f);
				dust2.noGravity = true;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void OnKill(int timeLeft)
		{
			// EXPLOSION — destroy tiles in a 4-tile radius
			int explosionRadius = 4;

			// Visual explosion
			for (int i = 0; i < 50; i++)
			{
				int dustType = Main.rand.NextBool() ? DustID.Torch : DustID.Smoke;
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 8, 8, dustType, 0f, 0f, 50, default, 2.5f);
				dust.velocity = Main.rand.NextVector2Circular(8f, 8f);
				dust.noGravity = dustType == DustID.Torch;
			}

			// Destroy tiles (only if owner is the local player, and only breakable tiles)
			if (Projectile.owner == Main.myPlayer)
			{
				int tileX = (int)(Projectile.Center.X / 16f);
				int tileY = (int)(Projectile.Center.Y / 16f);

				for (int x = tileX - explosionRadius; x <= tileX + explosionRadius; x++)
				{
					for (int y = tileY - explosionRadius; y <= tileY + explosionRadius; y++)
					{
						float dist = Vector2.Distance(new Vector2(x, y), new Vector2(tileX, tileY));
						if (dist <= explosionRadius && WorldGen.InWorld(x, y))
						{
							Tile tile = Main.tile[x, y];
							if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileDungeon[tile.TileType])
							{
								// Don't destroy dungeon bricks, but most other tiles are fair game
								WorldGen.KillTile(x, y, false, false, false);
							}

							// Also break walls
							if (tile.WallType > WallID.None)
								WorldGen.KillWall(x, y, false);
						}
					}
				}

				// Sync in multiplayer
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, tileX - explosionRadius, tileY - explosionRadius,
						explosionRadius * 2 + 1, explosionRadius * 2 + 1);
				}
			}

			// Damage nearby enemies
			if (Projectile.owner == Main.myPlayer)
			{
				float damageRadius = explosionRadius * 16f + 32f;
				foreach (var npc in Main.ActiveNPCs)
				{
					if (!npc.friendly && npc.CanBeChasedBy() && Vector2.Distance(npc.Center, Projectile.Center) < damageRadius)
					{
						Player owner = Main.player[Projectile.owner];
						owner.ApplyDamageToNPC(npc, Projectile.damage, 10f, 0, false);
					}
				}
			}
		}
	}
}
