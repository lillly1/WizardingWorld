using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Avada Kedavra — the Killing Curse. Green bolt, extreme damage, no piercing.</summary>
	public class AvadaKedavraProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 1;
			Projectile.timeLeft = 300;
			Projectile.light = 1.0f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 3;
		}

		public override void AI()
		{
			// Bright green trail
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, 0f, 100, default, 1.5f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
			}

			// Lightning-like secondary particles
			if (Main.rand.NextBool(3))
			{
				Dust dust2 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 4, 4, DustID.CursedTorch, 0f, 0f, 0, default, 0.6f);
				dust2.noGravity = true;
				dust2.velocity *= 1.5f;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			// Killing Curse ignores armor
			modifiers.ArmorPenetration += 50;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Chance to instantly kill non-boss enemies below 15% HP
			if (!target.boss && target.life < target.lifeMax * 0.15f)
			{
				target.life = 0;
				target.checkDead();
			}

			// CORRUPTION COST — Canon Tier A. The Killing Curse requires genuine intent to kill.
			// Using it marks the caster's soul. "You have to mean it." — Bellatrix
			if (Projectile.owner >= 0 && Projectile.owner < Main.maxPlayers)
			{
				Player owner = Main.player[Projectile.owner];
				owner.AddBuff(BuffID.ManaSickness, 300); // 5 seconds mana sickness

				// Dark Arts Corruption — the real cost
				var darkPlayer = owner.GetModPlayer<Common.Players.DarkArtsCorruptionPlayer>();
				darkPlayer.AddCorruption(0.03f, "Killing Curse"); // Significant corruption per cast
			}
		}

		public override void OnKill(int timeLeft)
		{
			// Massive green explosion
			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, 0f, 50, default, 2f);
				dust.velocity *= 4f;
				dust.noGravity = true;
			}
		}
	}
}
