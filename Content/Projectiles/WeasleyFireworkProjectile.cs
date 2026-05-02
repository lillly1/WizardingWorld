using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles
{
	/// <summary>Weasley Firework — flies upward then explodes in colorful AoE burst.</summary>
	public class WeasleyFireworkProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 120;
			Projectile.light = 0.4f;
			Projectile.ignoreWater = false;
			Projectile.tileCollide = true;
		}

		public override void AI()
		{
			// Fly upward then arc toward enemies
			Projectile.velocity.Y -= 0.15f; // Upward thrust
			Projectile.velocity.X *= 0.98f;

			// Sparkle trail — random colors
			int dustType = Main.rand.Next(new[] { DustID.RedTorch, DustID.BlueTorch, DustID.GreenTorch, DustID.YellowStarDust, DustID.PurpleTorch });
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 1.0f);
			dust.noGravity = true;
			dust.velocity *= 0.3f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			// After 1 second, start homing toward nearest enemy
			Projectile.ai[0]++;
			if (Projectile.ai[0] > 60)
			{
				float detectRadius = 300f;
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
					float turnSpeed = MathHelper.ToRadians(4);
					float targetAngle = Projectile.AngleTo(closest.Center);
					Projectile.velocity = Projectile.velocity.ToRotation()
						.AngleTowards(targetAngle, turnSpeed)
						.ToRotationVector2() * Projectile.velocity.Length();
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
			// FIREWORK EXPLOSION — colorful burst
			float explosionRadius = 120f;

			int[] colors = { DustID.RedTorch, DustID.BlueTorch, DustID.GreenTorch, DustID.YellowStarDust, DustID.PurpleTorch, DustID.PinkTorch };
			for (int i = 0; i < 50; i++)
			{
				int dustType = colors[Main.rand.Next(colors.Length)];
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 8, 8, dustType, 0f, 0f, 50, default, 2f);
				dust.velocity = Main.rand.NextVector2Circular(7f, 7f);
				dust.noGravity = true;
			}

			// Damage nearby enemies
			if (Projectile.owner == Main.myPlayer)
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (!npc.friendly && npc.CanBeChasedBy() && Vector2.Distance(npc.Center, Projectile.Center) < explosionRadius)
					{
						Player owner = Main.player[Projectile.owner];
						owner.ApplyDamageToNPC(npc, Projectile.damage, 6f, (npc.Center.X > Projectile.Center.X) ? 1 : -1, false);
						npc.AddBuff(BuffID.Confused, 120);
						npc.AddBuff(BuffID.OnFire, 180);
					}
				}
			}
		}
	}
}
