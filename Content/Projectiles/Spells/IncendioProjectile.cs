using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Incendio — fire stream that sets enemies ablaze.</summary>
	public class IncendioProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 3;
			Projectile.timeLeft = 180;
			Projectile.light = 0.8f;
			Projectile.ignoreWater = false;
			Projectile.tileCollide = true;
		}

		public override void AI()
		{
			// Fire trail
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
				dust.noGravity = true;
				dust.velocity = Projectile.velocity * 0.1f;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			// Slight gravity for arc
			Projectile.velocity.Y += 0.05f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.OnFire, 300); // 5 seconds on fire
		}

		public override void OnKill(int timeLeft)
		{
			// Fire burst
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
				dust.velocity *= 3f;
				dust.noGravity = true;
			}
		}
	}
}
