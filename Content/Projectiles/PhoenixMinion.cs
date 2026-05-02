using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles
{
	/// <summary>Phoenix minion — fiery bird that attacks enemies and heals the player passively.</summary>
	public class PhoenixMinion : ModProjectile
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
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.minionSlots = 1f;
			Projectile.penetrate = -1;
			Projectile.light = 0.5f;
		}

		public override bool MinionContactDamage() => true;
		public override bool? CanCutTiles() => false;

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			if (owner.dead || !owner.active)
			{
				owner.ClearBuff(ModContent.BuffType<Buffs.PhoenixMinionBuff>());
				return;
			}

			if (owner.HasBuff(ModContent.BuffType<Buffs.PhoenixMinionBuff>()))
				Projectile.timeLeft = 2;

			// Passive heal — 1 HP every 3 seconds
			Projectile.ai[1]++;
			if (Projectile.ai[1] >= 180)
			{
				Projectile.ai[1] = 0;
				if (owner.statLife < owner.statLifeMax2)
				{
					owner.statLife += 1;
					owner.HealEffect(1);
				}
			}

			// Fire dust
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}

			// Find target
			float maxRange = 600f;
			NPC closestTarget = null;
			float closestDist = maxRange * maxRange;

			// Check for manual target
			if (owner.HasMinionAttackTargetNPC)
			{
				NPC manualTarget = Main.npc[owner.MinionAttackTargetNPC];
				if (manualTarget.CanBeChasedBy())
				{
					float dist = Vector2.DistanceSquared(manualTarget.Center, Projectile.Center);
					if (dist < closestDist)
					{
						closestTarget = manualTarget;
						closestDist = dist;
					}
				}
			}

			if (closestTarget == null)
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.CanBeChasedBy())
					{
						float dist = Vector2.DistanceSquared(npc.Center, Projectile.Center);
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
				// Chase enemy
				float speed = 12f;
				float inertia = 15f;
				Vector2 dir = (closestTarget.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + dir) / inertia;
			}
			else
			{
				// Hover near player
				Vector2 idlePos = owner.Center + new Vector2(0, -60);
				Vector2 toIdle = idlePos - Projectile.Center;
				float dist = toIdle.Length();
				if (dist > 300f)
				{
					Projectile.Center = idlePos;
					Projectile.velocity = Vector2.Zero;
				}
				else if (dist > 20f)
				{
					Projectile.velocity = toIdle.SafeNormalize(Vector2.Zero) * 6f;
				}
				else
				{
					Projectile.velocity *= 0.9f;
				}
			}

			Projectile.rotation = Projectile.velocity.X * 0.05f;
			Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;

			// Animate
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 6)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.OnFire3, 180);
		}
	}
}
