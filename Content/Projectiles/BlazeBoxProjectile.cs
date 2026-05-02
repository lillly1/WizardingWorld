using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles
{
	/// <summary>
	/// Weasley's Basic Blaze Box — AoE fire burst on impact.
	/// Creates a ring of fire that damages and ignites enemies.
	/// </summary>
	public class BlazeBoxProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 180;
			Projectile.light = 0.4f;
			Projectile.tileCollide = true;
		}

		public override void AI()
		{
			Projectile.velocity.Y += 0.25f;
			Projectile.rotation += 0.15f;

			// Fire trail
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.0f);
			dust.noGravity = true;
			dust.velocity *= 0.2f;
		}

		public override void OnKill(int timeLeft)
		{
			// Fire ring explosion
			float radius = 100f;

			for (int i = 0; i < 40; i++)
			{
				float angle = MathHelper.TwoPi / 40 * i;
				Vector2 pos = Projectile.Center + angle.ToRotationVector2() * (radius * 0.5f);
				Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.Torch, 0f, 0f, 50, default, 2.5f);
				dust.velocity = angle.ToRotationVector2() * 3f;
				dust.noGravity = true;
			}

			// Inner fire burst
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 8, 8, DustID.Torch, 0f, 0f, 50, default, 2f);
				dust.velocity = Main.rand.NextVector2Circular(5f, 5f);
				dust.noGravity = true;
			}

			// Damage and ignite enemies in radius
			if (Projectile.owner == Main.myPlayer)
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (!npc.friendly && npc.CanBeChasedBy() && Vector2.Distance(npc.Center, Projectile.Center) < radius)
					{
						Player owner = Main.player[Projectile.owner];
						owner.ApplyDamageToNPC(npc, Projectile.damage, 6f, 0, false);
						npc.AddBuff(BuffID.OnFire3, 300); // 5 seconds Hellfire
					}
				}
			}
		}
	}
}
