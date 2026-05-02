using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Aguamenti — water stream that pierces enemies and extinguishes fire debuffs on the player.</summary>
	public class AguamentiProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 5;
			Projectile.timeLeft = 120;
			Projectile.light = 0.3f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		public override void AI()
		{
			// Water stream dust
			for (int i = 0; i < 3; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
				dust.velocity = Projectile.velocity * 0.1f + Main.rand.NextVector2Circular(1f, 1f);
			}

			// Slight gravity for natural water arc
			Projectile.velocity.Y += 0.08f;
			Projectile.rotation = Projectile.velocity.ToRotation();

			// Extinguish fire on owner
			Player owner = Main.player[Projectile.owner];
			if (Projectile.ai[0] == 0)
			{
				owner.ClearBuff(BuffID.OnFire);
				owner.ClearBuff(BuffID.OnFire3);
				owner.ClearBuff(BuffID.Burning);
				Projectile.ai[0] = 1; // Only once
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Water extinguishes fire enemies for bonus damage, slows others
			target.AddBuff(BuffID.Wet, 300);
			target.AddBuff(BuffID.Slow, 120);
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, 0f, 50, default, 1.2f);
				dust.velocity *= 2f;
			}
		}
	}
}
