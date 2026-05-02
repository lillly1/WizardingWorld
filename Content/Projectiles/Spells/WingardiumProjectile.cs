using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Wingardium Leviosa — lifts enemies into the air and drops them for fall damage equivalent.</summary>
	public class WingardiumProjectile : ModProjectile
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
			// White/light blue trail
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror, 0f, 0f, 100, default, 1.0f);
			dust.noGravity = true;
			dust.velocity *= 0.2f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Launch enemy upward
			if (!target.boss && target.knockBackResist > 0)
			{
				target.velocity.Y = -15f;
				target.velocity.X = Main.rand.NextFloat(-3f, 3f);
			}

			// Slow debuff simulating levitation disorientation
			target.AddBuff(BuffID.Slow, 180);
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror, 0f, 0f, 50, default, 1.3f);
				dust.velocity *= 2f;
				dust.noGravity = true;
			}
		}
	}
}
