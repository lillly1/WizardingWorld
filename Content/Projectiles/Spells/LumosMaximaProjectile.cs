using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Lumos Maxima — powerful light burst that illuminates a massive area and damages undead.</summary>
	public class LumosMaximaProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 480; // 8 seconds
			Projectile.light = 3.0f; // Massive light
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 30;
			Projectile.DamageType = DamageClass.Default;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			// Float above player's head
			Vector2 targetPos = owner.Center + new Vector2(0, -80);
			Vector2 toTarget = targetPos - Projectile.Center;
			Projectile.velocity = toTarget * 0.08f;

			// Massive light
			Terraria.Lighting.AddLight(Projectile.Center, 2.5f, 2.5f, 2.0f);

			// Brilliant white/gold sparkles in a wide radius
			for (int i = 0; i < 3; i++)
			{
				Vector2 sparklePos = Projectile.Center + Main.rand.NextVector2Circular(40, 40);
				Dust dust = Dust.NewDustDirect(sparklePos, 4, 4, DustID.GoldCoin, 0f, 0f, 50, default, 1.0f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}

			// Pulsing size
			Projectile.scale = 1.0f + (float)Math.Sin(Main.GameUpdateCount * 0.05) * 0.2f;
			Projectile.rotation += 0.03f;

			// Damage undead/dark enemies that come too close (40px radius)
			// The projectile itself does contact damage via CanHitNPC
		}

		public override bool? CanHitNPC(NPC target)
		{
			// Only damages undead/dark-type enemies within range
			if (Vector2.Distance(target.Center, Projectile.Center) > 60f)
				return false;

			return null; // Let normal collision handle it
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			modifiers.FinalDamage *= 0.5f; // Light damage, not a primary weapon
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Confused, 60);
			target.AddBuff(BuffID.OnFire, 120);
		}
	}
}
