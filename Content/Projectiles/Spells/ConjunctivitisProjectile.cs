using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>
	/// Conjunctivitis Curse — targets the eyes. Reduces enemy damage and accuracy.
	/// Particularly effective against bosses (reduces their attack frequency).
	/// The curse that Krum used against the Chinese Fireball.
	/// </summary>
	public class ConjunctivitisProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 1;
			Projectile.timeLeft = 300;
			Projectile.light = 0.5f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 2;
		}

		public override void AI()
		{
			// Yellow-orange eye-targeting bolt
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, 0f, 0f, 100, default, 1.0f);
			dust.noGravity = true;
			dust.velocity *= 0.15f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Blind the target — Darkness equivalent for NPCs
			target.AddBuff(BuffID.Confused, 300); // 5 seconds confused

			// The real power: reduce the enemy's damage significantly
			// This is handled by the Jinxed debuff
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>(), 600); // 10 seconds jinxed

			// Visual: eyes hurt effect
			for (int i = 0; i < 10; i++)
			{
				Dust d = Dust.NewDustDirect(target.Center + new Vector2(0, -target.height / 3f), 8, 8, DustID.YellowStarDust, 0f, 0f, 50, default, 1.2f);
				d.noGravity = true;
				d.velocity = Main.rand.NextVector2Circular(2f, 2f);
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 12; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, 0f, 0f, 50, default, 1.3f);
				dust.velocity *= 2f;
				dust.noGravity = true;
			}
		}
	}
}
