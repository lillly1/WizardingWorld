using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>
	/// Levicorpus — hoists enemies into the air by their ankles.
	/// Launches enemies high upward, leaving them helpless. Non-boss enemies take fall damage.
	/// Snape's signature jinx from the Half-Blood Prince's textbook.
	/// </summary>
	public class LevicorpusProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 3;
			Projectile.timeLeft = 240;
			Projectile.light = 0.4f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			// Purple levitation trail
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 100, default, 0.9f);
			dust.noGravity = true;
			dust.velocity *= 0.15f;

			// Upward-spiraling particles
			if (Main.rand.NextBool(3))
			{
				Dust dust2 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(6, 6), 4, 4, DustID.PurpleTorch, 0f, -2f, 50, default, 0.6f);
				dust2.noGravity = true;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Hoist the enemy skyward!
			if (!target.boss && target.knockBackResist > 0)
			{
				target.velocity.Y = -20f; // Massive upward launch
				target.velocity.X = Main.rand.NextFloat(-2f, 2f);
			}
			else if (target.boss)
			{
				// Bosses resist but still get nudged
				target.velocity.Y -= 5f;
			}

			// Jinxed debuff while airborne
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>(), 180);

			// Visual: purple lifting aura around enemy
			for (int i = 0; i < 12; i++)
			{
				Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.PurpleTorch, 0f, -3f, 50, default, 1.2f);
				dust.noGravity = true;
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 50, default, 1.3f);
				dust.velocity *= 2f;
				dust.noGravity = true;
			}
		}
	}
}
