using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Fiendfyre — cursed flame that grows, pierces infinitely, and leaves a fire trail.</summary>
	public class FiendfyreProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = -1;
			Projectile.timeLeft = 300;
			Projectile.light = 1.0f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
			Projectile.scale = 0.5f;
		}

		public override void AI()
		{
			// Grow over time — Fiendfyre is alive and hungry
			if (Projectile.scale < 2.0f)
				Projectile.scale += 0.008f;

			// INSTABILITY — Canon Tier A. Fiendfyre is notoriously difficult to control.
			// "It was cursed fire — Fiendfyre — one of the substances that can destroy a Horcrux,
			// but I would never, ever have dared use it, it was too dangerous." — Hermione
			if (Main.rand.NextBool(30)) // ~3% chance per frame to swerve
			{
				Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(25));
			}

			// Small chance to damage the caster if too close (the fire turns on you)
			if (Projectile.owner >= 0 && Projectile.owner < Main.maxPlayers)
			{
				Player owner = Main.player[Projectile.owner];
				if (Vector2.Distance(owner.Center, Projectile.Center) < 60f && Main.rand.NextBool(60))
				{
					owner.Hurt(Terraria.DataStructures.PlayerDeathReason.LegacyDefault(), 10, 0);
				}

				// Corruption from using cursed fire
				if (Projectile.ai[0] == 0) // Only once on spawn
				{
					Projectile.ai[0] = 1;
					var darkPlayer = owner.GetModPlayer<Common.Players.DarkArtsCorruptionPlayer>();
					darkPlayer.AddCorruption(0.02f, "Fiendfyre");
				}
			}

			// Slight homing toward nearest enemy
			float detectRadius = 300f;
			NPC closest = null;
			float closestDist = detectRadius * detectRadius;

			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.CanBeChasedBy())
				{
					float dist = Vector2.DistanceSquared(npc.Center, Projectile.Center);
					if (dist < closestDist)
					{
						closest = npc;
						closestDist = dist;
					}
				}
			}

			if (closest != null)
			{
				float turnSpeed = MathHelper.ToRadians(2);
				float targetAngle = Projectile.AngleTo(closest.Center);
				float currentAngle = Projectile.velocity.ToRotation();
				float newAngle = currentAngle.AngleTowards(targetAngle, turnSpeed);
				Projectile.velocity = newAngle.ToRotationVector2() * Projectile.velocity.Length();
			}

			// Slow down slightly over time
			Projectile.velocity *= 0.998f;

			// Intense fire trail
			for (int i = 0; i < 4; i++)
			{
				Dust dust = Dust.NewDustDirect(
					Projectile.position - Projectile.velocity * 0.5f,
					Projectile.width, Projectile.height,
					DustID.Torch, 0f, 0f, 50, default, Projectile.scale * 1.5f);
				dust.noGravity = true;
				dust.velocity = Projectile.velocity * -0.2f + Main.rand.NextVector2Circular(2f, 2f);
			}

			// Shadowflame particles for the cursed feel
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(10, 10),
					4, 4, DustID.Shadowflame, 0f, 0f, 0, default, Projectile.scale);
				dust.noGravity = true;
			}

			Projectile.rotation += 0.15f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.OnFire3, 600); // 10 seconds Hellfire
			target.AddBuff(BuffID.ShadowFlame, 300);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			// Scales with projectile size
			modifiers.FinalDamage *= Projectile.scale;
		}
	}
}
