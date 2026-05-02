using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>
	/// Accio — the Summoning Charm. Pulls all dropped items toward the player.
	/// Creates a magnetic field that attracts items in a huge radius (600px).
	/// Essential utility for wizard players — no more losing drops in caves.
	/// </summary>
	public class AccioProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 180; // 3 seconds of attraction
			Projectile.light = 0.5f;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			Projectile.Center = owner.Center;
			Projectile.velocity = Vector2.Zero;

			float attractRadius = 600f;
			float pullStrength = 15f;

			// Pull all dropped items toward the player
			for (int i = 0; i < Main.maxItems; i++)
			{
				Item item = Main.item[i];
				if (item.active && !item.beingGrabbed && item.noGrabDelay == 0)
				{
					float dist = Vector2.Distance(item.Center, owner.Center);
					if (dist < attractRadius && dist > 20f)
					{
						Vector2 pullDir = (owner.Center - item.Center).SafeNormalize(Vector2.Zero);
						item.velocity = pullDir * pullStrength;

						// Sparkle trail on attracted items
						if (Main.rand.NextBool(3))
						{
							Dust dust = Dust.NewDustDirect(item.position, item.width, item.height, DustID.MagicMirror, 0f, 0f, 100, default, 0.6f);
							dust.noGravity = true;
						}
					}
				}
			}

			// Visual: swirling attraction aura
			for (int i = 0; i < 3; i++)
			{
				float angle = Main.GameUpdateCount * 0.08f + MathHelper.TwoPi / 3 * i;
				float radius = 40f + (float)Math.Sin(Main.GameUpdateCount * 0.05f) * 10f;
				Vector2 pos = owner.Center + angle.ToRotationVector2() * radius;
				Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.MagicMirror, 0f, 0f, 50, default, 0.8f);
				dust.noGravity = true;
				dust.velocity = (owner.Center - pos).SafeNormalize(Vector2.Zero) * 1f;
			}

			Projectile.rotation += 0.05f;
		}

		public override bool? CanHitNPC(NPC target) => false;
	}
}
