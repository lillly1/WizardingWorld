using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles
{
	/// <summary>Peruvian Instant Darkness Powder — creates a cloud that blinds enemies and reduces their accuracy.</summary>
	public class DarknessPowderProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 300; // 5 seconds cloud
			Projectile.light = 0f;
			Projectile.ignoreWater = false;
			Projectile.tileCollide = true;
		}

		public override void AI()
		{
			// Phase 1: flying (first 30 ticks)
			if (Projectile.ai[0] < 30)
			{
				Projectile.ai[0]++;
				Projectile.velocity.Y += 0.2f;
				return;
			}

			// Phase 2: stationary cloud
			Projectile.velocity = Vector2.Zero;

			// Dark cloud particles
			float cloudRadius = 80f;
			for (int i = 0; i < 5; i++)
			{
				Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(cloudRadius, cloudRadius);
				Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.Smoke, 0f, 0f, 200, new Color(20, 20, 30), 2f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}

			// Blind enemies in the cloud
			foreach (var npc in Main.ActiveNPCs)
			{
				if (!npc.friendly && Vector2.Distance(npc.Center, Projectile.Center) < cloudRadius)
				{
					npc.AddBuff(BuffID.Confused, 30); // Continuous confusion while in cloud
					npc.velocity *= 0.95f; // Slow them down
				}
			}

			// Reduce light in the area
			Terraria.Lighting.AddLight(Projectile.Center, -0.5f, -0.5f, -0.5f);
		}

		public override bool? CanHitNPC(NPC target) => false; // Doesn't do direct damage
	}
}
