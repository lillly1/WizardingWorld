using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles
{
	/// <summary>House-Elf minion — fights with snap-magic, teleports around enemies.</summary>
	public class HouseElfMinion : ModProjectile
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
			Projectile.width = 20;
			Projectile.height = 30;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.minionSlots = 1f;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
		}

		public override bool MinionContactDamage() => true;
		public override bool? CanCutTiles() => false;

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			if (owner.dead || !owner.active)
			{
				owner.ClearBuff(ModContent.BuffType<Buffs.HouseElfMinionBuff>());
				return;
			}

			if (owner.HasBuff(ModContent.BuffType<Buffs.HouseElfMinionBuff>()))
				Projectile.timeLeft = 2;

			// Snap-magic sparkle
			if (Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror, 0f, 0f, 100, default, 0.7f);
				dust.noGravity = true;
			}

			// Find target
			NPC closestTarget = null;
			float closestDist = 500f * 500f;

			if (owner.HasMinionAttackTargetNPC)
			{
				NPC manual = Main.npc[owner.MinionAttackTargetNPC];
				if (manual.CanBeChasedBy())
				{
					float dist = Vector2.DistanceSquared(manual.Center, Projectile.Center);
					if (dist < closestDist)
					{
						closestTarget = manual;
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
				// Teleport near enemy periodically
				Projectile.ai[0]++;
				if (Projectile.ai[0] >= 60)
				{
					Projectile.ai[0] = 0;
					Vector2 teleportPos = closestTarget.Center + Main.rand.NextVector2CircularEdge(60, 60);
					Projectile.Center = teleportPos;

					// Snap effect
					for (int i = 0; i < 8; i++)
					{
						Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror, 0f, 0f, 50, default, 1.2f);
						dust.noGravity = true;
						dust.velocity *= 2f;
					}
				}

				// Chase between teleports
				float speed = 10f;
				float inertia = 15f;
				Vector2 dir = (closestTarget.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + dir) / inertia;
			}
			else
			{
				// Hover near player
				Vector2 idlePos = owner.Center + new Vector2(-40 * owner.direction, -30);
				Vector2 toIdle = idlePos - Projectile.Center;
				if (toIdle.Length() > 300f)
					Projectile.Center = idlePos;
				else if (toIdle.Length() > 15f)
					Projectile.velocity = toIdle.SafeNormalize(Vector2.Zero) * 5f;
				else
					Projectile.velocity *= 0.9f;
			}

			Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;

			// Animate
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 6)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
			}
		}
	}
}
