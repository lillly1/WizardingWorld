using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles
{
	/// <summary>Howler — screaming letter that explodes on contact, dealing AoE damage + confusion.</summary>
	public class HowlerProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 300;
			Projectile.light = 0.3f;
			Projectile.ignoreWater = false;
			Projectile.tileCollide = true;
		}

		public override void AI()
		{
			// Fluttering motion
			Projectile.velocity.Y += 0.05f;
			Projectile.velocity.X *= 0.99f;
			Projectile.rotation += Projectile.velocity.X * 0.05f;

			// Red smoke trail
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}

			// Homing toward nearest enemy after 0.5 seconds
			if (Projectile.ai[0]++ > 30)
			{
				float detectRadius = 250f;
				NPC closest = null;
				float closestDist = detectRadius * detectRadius;

				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.CanBeChasedBy())
					{
						float dist = Vector2.DistanceSquared(npc.Center, Projectile.Center);
						if (dist < closestDist)
						{
							closest = npc;
							closestDist = dist;
						}
					}
				}

				if (closest != null)
				{
					float speed = Projectile.velocity.Length();
					float targetAngle = Projectile.AngleTo(closest.Center);
					Projectile.velocity = Projectile.velocity.ToRotation()
						.AngleTowards(targetAngle, MathHelper.ToRadians(3))
						.ToRotationVector2() * speed;
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
			// HOWLER EXPLOSION — red burst + sound
			float explosionRadius = 100f;

			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 8, 8, DustID.LifeDrain, 0f, 0f, 50, default, 2f);
				dust.velocity = Main.rand.NextVector2Circular(5f, 5f);
				dust.noGravity = true;
			}

			// Confuse nearby enemies
			foreach (var npc in Main.ActiveNPCs)
			{
				if (!npc.friendly && npc.CanBeChasedBy() && Vector2.Distance(npc.Center, Projectile.Center) < explosionRadius)
				{
					npc.AddBuff(BuffID.Confused, 180);
				}
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Confused, 120);
		}
	}
}
