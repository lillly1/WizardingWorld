using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Chain Stupefy — upgraded stun bolt that chains to up to 3 nearby enemies on hit.</summary>
	public class ChainStupefyProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 4;
			Projectile.timeLeft = 300;
			Projectile.light = 0.7f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1; // One hit per NPC
		}

		public override void AI()
		{
			// Bright red chain-lightning trail
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain, 0f, 0f, 100, default, 1.3f);
				dust.noGravity = true;
				dust.velocity *= 0.15f;
			}

			// Electric arc particles
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 4, 4, DustID.RedTorch, 0f, 0f, 0, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 1f;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Stun the hit target
			target.AddBuff(BuffID.Confused, 180);

			// Chain to nearest un-hit enemy
			if (Projectile.penetrate > 1)
			{
				float chainRange = 250f;
				NPC closest = null;
				float closestDist = chainRange * chainRange;

				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.whoAmI != target.whoAmI && npc.CanBeChasedBy()
						&& Projectile.localNPCImmunity[npc.whoAmI] == 0)
					{
						float dist = Vector2.DistanceSquared(npc.Center, target.Center);
						if (dist < closestDist)
						{
							closest = npc;
							closestDist = dist;
						}
					}
				}

				if (closest != null)
				{
					// Redirect toward next target
					Projectile.Center = target.Center;
					Projectile.velocity = (closest.Center - target.Center).SafeNormalize(Vector2.UnitY) * Projectile.velocity.Length();
					Projectile.netUpdate = true;

					// Chain visual — lightning arc between targets
					for (int i = 0; i < 15; i++)
					{
						Vector2 pos = Vector2.Lerp(target.Center, closest.Center, i / 15f);
						pos += Main.rand.NextVector2Circular(4, 4);
						Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.LifeDrain, 0f, 0f, 50, default, 0.8f);
						dust.noGravity = true;
					}
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 15; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain, 0f, 0f, 50, default, 1.5f);
				dust.velocity *= 2.5f;
				dust.noGravity = true;
			}
		}
	}
}
