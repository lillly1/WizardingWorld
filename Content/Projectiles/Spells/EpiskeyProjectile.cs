using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Episkey — canon healing spell. Orbits the player and heals over time.
	/// "Episkey is used to heal relatively minor injuries." — Canon Tier A.
	/// Renamed from Reparo (which is object-repair, not flesh-healing).</summary>
	public class EpiskeyProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 480;
			Projectile.light = 0.6f;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			Projectile.ai[0] += 0.06f;
			float radius = 60f;
			Vector2 orbitPos = owner.Center + new Vector2(
				(float)Math.Cos(Projectile.ai[0]) * radius,
				(float)Math.Sin(Projectile.ai[0]) * radius
			);
			Projectile.Center = orbitPos;
			Projectile.velocity = Vector2.Zero;

			// Warm golden healing aura (Episkey is warm light, not green)
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 100, default, 0.8f);
			dust.noGravity = true;
			dust.velocity *= 0.2f;

			Projectile.ai[1]++;
			if (Projectile.ai[1] >= 60)
			{
				Projectile.ai[1] = 0;
				if (owner.statLife < owner.statLifeMax2)
				{
					int healAmount = 5 + (int)(owner.statLifeMax2 * 0.02f);
					owner.statLife = Math.Min(owner.statLife + healAmount, owner.statLifeMax2);
					owner.HealEffect(healAmount);
				}
			}

			Projectile.rotation += 0.1f;
		}
	}
}
