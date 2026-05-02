using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Sectumsempra — dark slashing bolt that causes bleeding (Ichor-like defense reduction).</summary>
	public class SectumsempraProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 2;
			Projectile.timeLeft = 300;
			Projectile.light = 0.4f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 2;
		}

		public override void AI()
		{
			// Dark purple/crimson trail
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CrimsonTorch, 0f, 0f, 100, default, 1.0f);
			dust.noGravity = true;
			dust.velocity *= 0.1f;

			// Secondary dark dust
			if (Main.rand.NextBool(2))
			{
				Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 100, default, 0.8f);
				dust2.noGravity = true;
				dust2.velocity *= 0.1f;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Bleeding — Ichor debuff (defense reduction) + Shadowflame
			target.AddBuff(BuffID.Ichor, 300);
			target.AddBuff(BuffID.ShadowFlame, 180);

			// Minor corruption — Sectumsempra is dark magic but not Unforgivable
			// "For enemies" — Snape's annotation in the Half-Blood Prince's book
			if (Projectile.owner >= 0 && Projectile.owner < Main.maxPlayers)
			{
				var darkPlayer = Main.player[Projectile.owner].GetModPlayer<Common.Players.DarkArtsCorruptionPlayer>();
				darkPlayer.AddCorruption(0.005f, "dark curse"); // Mild — it's a borderline spell
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 15; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CrimsonTorch, 0f, 0f, 100, default, 1.5f);
				dust.velocity *= 2.5f;
				dust.noGravity = true;
			}
		}
	}
}
