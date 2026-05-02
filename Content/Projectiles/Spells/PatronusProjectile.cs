using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Expecto Patronum — summons a silver guardian that homes into enemies and provides defense.</summary>
	public class PatronusProjectile : ModProjectile
	{
		private NPC HomingTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set => Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = -1;
			Projectile.timeLeft = 600;
			Projectile.light = 1.2f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 30;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			// Grant defense buff to owner
			var wizPlayer = owner.GetModPlayer<Common.Players.WizardPlayer>();
			wizPlayer.patronusActive = true;
			wizPlayer.patronusTimer = 10;
			wizPlayer.RelieveDespair(0.0025f);
			owner.statDefense += 8;

			// Silver dust trail
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 100, default, 1.2f);
			dust.noGravity = true;
			dust.velocity *= 0.3f;

			// Homing behavior
			float maxDetectRadius = 500f;
			int unicornType = ModContent.ItemType<Content.Items.Weapons.Wands.UnicornHairWand>();
			int masteryLevel = owner.GetModPlayer<Common.Players.WizardPlayer>().GetWandMasteryLevel(unicornType);
			if (masteryLevel >= 2)
				maxDetectRadius += 120f;

			if (HomingTarget == null || !HomingTarget.CanBeChasedBy())
				HomingTarget = FindClosestNPC(maxDetectRadius);

			if (HomingTarget != null)
			{
				// Home toward target
				float speed = 10f;
				float inertia = 20f;
				Vector2 direction = (HomingTarget.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
			}
			else
			{
				// Orbit around player
				float orbitRadius = 100f;
				Projectile.ai[1] += 0.05f;
				Vector2 orbitPos = owner.Center + new Vector2(
					(float)Math.Cos(Projectile.ai[1]) * orbitRadius,
					(float)Math.Sin(Projectile.ai[1]) * orbitRadius
				);
				Vector2 toOrbit = orbitPos - Projectile.Center;
				Projectile.velocity = toOrbit * 0.1f;
			}

			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		private NPC FindClosestNPC(float maxDetectDistance)
		{
			NPC closestNPC = null;
			float sqrMaxDist = maxDetectDistance * maxDetectDistance;

			foreach (var target in Main.ActiveNPCs)
			{
				if (target.CanBeChasedBy())
				{
					float sqrDist = Vector2.DistanceSquared(target.Center, Projectile.Center);
					if (target.type == ModContent.NPCType<Content.NPCs.Enemies.Dementor>()
						|| target.type == ModContent.NPCType<Content.NPCs.Enemies.AzkabanGuard>()
						|| target.type == ModContent.NPCType<Content.NPCs.Enemies.Lethifold>()
						|| target.type == ModContent.NPCType<Content.NPCs.Bosses.DementorKing.DementorKingBoss>())
						sqrDist *= 0.25f;
					if (sqrDist < sqrMaxDist)
					{
						sqrMaxDist = sqrDist;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			if (target.type == ModContent.NPCType<Content.NPCs.Enemies.Dementor>()
				|| target.type == ModContent.NPCType<Content.NPCs.Enemies.AzkabanGuard>()
				|| target.type == ModContent.NPCType<Content.NPCs.Enemies.Lethifold>())
				modifiers.FinalDamage *= 2f;
			else if (target.type == ModContent.NPCType<Content.NPCs.Bosses.DementorKing.DementorKingBoss>())
				modifiers.FinalDamage *= 1.5f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Main.player[Projectile.owner].GetModPlayer<Common.Players.WizardPlayer>().RelieveDespair(0.05f);
			target.AddBuff(BuffID.Confused, 60);
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 50, default, 1.5f);
				dust.velocity *= 3f;
				dust.noGravity = true;
			}
		}
	}
}
