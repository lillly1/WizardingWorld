using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Impedimenta — AoE jinx that slows all enemies in a wide radius on impact.</summary>
	public class ImpedimentaProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 1;
			Projectile.timeLeft = 240;
			Projectile.light = 0.5f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			// Purple/blue trail
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 100, default, 1.0f);
			dust.noGravity = true;
			dust.velocity *= 0.15f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void OnKill(int timeLeft)
		{
			// AoE slow explosion
			float radius = 150f;

			// Big purple burst
			for (int i = 0; i < 40; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 8, 8, DustID.PurpleTorch, 0f, 0f, 50, default, 1.8f);
				dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
				dust.noGravity = true;
			}

			// Slow all enemies in radius
			foreach (var npc in Main.ActiveNPCs)
			{
				if (!npc.friendly && npc.CanBeChasedBy() && Vector2.Distance(npc.Center, Projectile.Center) < radius)
				{
					npc.AddBuff(BuffID.Slow, 300); // 5 seconds
					npc.velocity *= 0.3f; // Immediate slow
				}
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Slow, 300);
		}
	}
}
