using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Protego — shield barrier that blocks hostile projectiles near the player.</summary>
	public class ProtegoProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 300; // 5 seconds
			Projectile.light = 0.6f;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Default;
			Projectile.damage = 0;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			// Stick to player
			Projectile.Center = owner.Center;
			Projectile.velocity = Vector2.Zero;

			// Shield visual — blue barrier dust
			for (int i = 0; i < 3; i++)
			{
				float angle = Main.rand.NextFloat() * MathHelper.TwoPi;
				Vector2 pos = owner.Center + angle.ToRotationVector2() * 40f;
				Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.BlueTorch, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
				dust.velocity = (owner.Center - pos).SafeNormalize(Vector2.Zero) * 0.5f;
			}

			// Destroy hostile projectiles within range
			float shieldRadius = 50f;
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile proj = Main.projectile[i];
				if (proj.active && proj.hostile && !proj.friendly)
				{
					if (Vector2.Distance(proj.Center, owner.Center) < shieldRadius)
					{
						proj.Kill();

						// Deflection effect
						for (int j = 0; j < 5; j++)
						{
							Dust dust = Dust.NewDustDirect(proj.position, proj.width, proj.height, DustID.BlueTorch, 0f, 0f, 50, default, 1.2f);
							dust.noGravity = true;
							dust.velocity *= 2f;
						}
					}
				}
			}

			Projectile.rotation += 0.02f;
		}

		public override bool? CanHitNPC(NPC target) => false;
	}
}
