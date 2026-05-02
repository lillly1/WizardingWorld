using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Crucio — the Cruciatus Curse. Red beam that deals damage over time.</summary>
	public class CrucioProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = -1;
			Projectile.timeLeft = 180; // 3 seconds
			Projectile.light = 0.7f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 15; // Hits every quarter second
			Projectile.extraUpdates = 2;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			// Beam stays connected to player — extends outward
			Vector2 toMouse = Main.MouseWorld - owner.Center;
			if (toMouse.Length() > 0)
				Projectile.velocity = toMouse.SafeNormalize(Vector2.UnitX) * 16f;

			Projectile.Center = owner.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * Projectile.ai[0];
			Projectile.ai[0] = MathHelper.Clamp(Projectile.ai[0] + 4f, 0, 300f);

			// Red lightning dust
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain, 0f, 0f, 50, default, 1.2f);
				dust.noGravity = true;
				dust.velocity *= 0.5f;
			}

			// Crackling secondary particles
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(6, 6), 4, 4, DustID.RedTorch, 0f, 0f, 0, default, 0.5f);
				dust.noGravity = true;
			}

			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Pain — Confused + On Fire (the Cruciatus Curse causes agony)
			target.AddBuff(BuffID.Confused, 60);
			target.AddBuff(BuffID.OnFire3, 120);

			// CORRUPTION — Cruciatus is an Unforgivable Curse. Canon Tier A.
			// "You have to really WANT to cause pain." — Bellatrix
			if (Projectile.owner >= 0 && Projectile.owner < Main.maxPlayers)
			{
				var darkPlayer = Main.player[Projectile.owner].GetModPlayer<Common.Players.DarkArtsCorruptionPlayer>();
				darkPlayer.AddCorruption(0.015f, "Cruciatus Curse"); // Less than AK but still significant
			}
		}
	}
}
