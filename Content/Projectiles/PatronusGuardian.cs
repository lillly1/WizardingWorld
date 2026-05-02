using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles
{
	/// <summary>
	/// Patronus Guardian — a powerful silver stag minion.
	/// Homes into enemies, deals magic damage, provides passive defense to owner.
	/// The embodiment of hope and positive energy.
	/// </summary>
	public class PatronusGuardian : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 4;
			ProjectileID.Sets.MinionTargettingFeature[Type] = true;
			Main.projPet[Type] = true;
			ProjectileID.Sets.MinionSacrificable[Type] = true;
			ProjectileID.Sets.CultistIsResistantTo[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 36;
			Projectile.height = 36;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.minionSlots = 2f; // Costs 2 slots — it's powerful
			Projectile.penetrate = -1;
			Projectile.light = 1.0f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 15;
		}

		public override bool MinionContactDamage() => true;
		public override bool? CanCutTiles() => false;

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			if (owner.dead || !owner.active)
			{
				owner.ClearBuff(ModContent.BuffType<Buffs.PatronusCharmBuff>());
				return;
			}

			if (owner.HasBuff(ModContent.BuffType<Buffs.PatronusCharmBuff>()))
				Projectile.timeLeft = 2;

			Projectile.localAI[0]++;
			if (Projectile.localAI[0] >= 30)
			{
				Projectile.localAI[0] = 0;
				owner.GetModPlayer<Common.Players.WizardPlayer>().RelieveDespair(0.03f);
			}

			// CORRUPTION CHECK — Dark Arts weaken the Patronus. Canon Tier A.
			// "A Patronus is the embodiment of positive feelings. Darkness diminishes it."
			var darkPlayer = owner.GetModPlayer<Common.Players.DarkArtsCorruptionPlayer>();
			float patronusEffectiveness = darkPlayer.GetPatronusEffectiveness();

			// Scale damage with corruption (lower corruption = stronger Patronus)
			Projectile.damage = (int)(Projectile.originalDamage * patronusEffectiveness);

			// If corruption is too high, Patronus flickers and weakens visually
			if (patronusEffectiveness < 0.5f && Main.rand.NextBool(10))
			{
				// Flickering — the Patronus is unstable
				Projectile.alpha = Main.rand.Next(50, 150);
			}
			else
			{
				Projectile.alpha = 0;
			}

			// Brilliant silver light (dimmer with corruption)
			float lightStrength = 0.8f * patronusEffectiveness;
			Lighting.AddLight(Projectile.Center, lightStrength, lightStrength, lightStrength + 0.2f);

			// Silver sparkle trail
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 50, default, 1.0f);
				dust.noGravity = true;
				dust.velocity = Projectile.velocity * -0.2f + Main.rand.NextVector2Circular(1f, 1f);
			}

			// Find target — prioritize Dementors and dark enemies
			NPC closestTarget = null;
			float closestDist = 700f * 700f;

			if (owner.HasMinionAttackTargetNPC)
			{
				NPC manual = Main.npc[owner.MinionAttackTargetNPC];
				if (manual.CanBeChasedBy())
				{
					closestTarget = manual;
					closestDist = Vector2.DistanceSquared(manual.Center, Projectile.Center);
				}
			}

			if (closestTarget == null)
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.CanBeChasedBy())
					{
						float dist = Vector2.DistanceSquared(npc.Center, Projectile.Center);

						// Prioritize Dementors and prison horrors — Patronus is their natural counter
						if (npc.type == ModContent.NPCType<Content.NPCs.Enemies.Dementor>()
							|| npc.type == ModContent.NPCType<Content.NPCs.Enemies.AzkabanGuard>()
							|| npc.type == ModContent.NPCType<Content.NPCs.Enemies.Lethifold>()
							|| npc.type == ModContent.NPCType<Content.NPCs.Bosses.DementorKing.DementorKingBoss>())
							dist *= 0.3f; // Effectively triples detection range for Dementors

						if (dist < closestDist)
						{
							closestTarget = npc;
							closestDist = dist;
						}
					}
				}
			}

			if (closestTarget != null)
			{
				// Aggressive homing
				float speed = 14f;
				float inertia = 12f;
				Vector2 dir = (closestTarget.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + dir) / inertia;
			}
			else
			{
				// Orbit owner gracefully
				Projectile.ai[0] += 0.04f;
				float orbitRadius = 80f;
				Vector2 orbitPos = owner.Center + new Vector2(
					(float)Math.Cos(Projectile.ai[0]) * orbitRadius,
					(float)Math.Sin(Projectile.ai[0]) * orbitRadius - 30f
				);
				Vector2 toOrbit = orbitPos - Projectile.Center;
				if (toOrbit.Length() > 300f)
					Projectile.Center = orbitPos;
				else
					Projectile.velocity = toOrbit * 0.08f;
			}

			Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
			Projectile.rotation = Projectile.velocity.X * 0.03f;

			// Animate
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Extra damage to Dementors — Patronus is their hard counter
			if (target.type == ModContent.NPCType<Content.NPCs.Enemies.Dementor>()
				|| target.type == ModContent.NPCType<Content.NPCs.Enemies.AzkabanGuard>()
				|| target.type == ModContent.NPCType<Content.NPCs.Enemies.Lethifold>())
			{
				target.life -= damageDone; // Double damage
				target.HitEffect(hit);
			}

			if (target.type == ModContent.NPCType<Content.NPCs.Bosses.DementorKing.DementorKingBoss>())
			{
				target.life -= damageDone / 2; // 50% bonus damage vs the boss of despair
				target.HitEffect(hit);
			}

			Main.player[Projectile.owner].GetModPlayer<Common.Players.WizardPlayer>().RelieveDespair(0.05f);

			// Stun dark enemies
			target.AddBuff(BuffID.Confused, 60);
		}
	}
}
