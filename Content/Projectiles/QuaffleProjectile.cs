using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles
{
	/// <summary>Quaffle — bouncing ball projectile that returns to the player.</summary>
	public class QuaffleProjectile : ModProjectile
	{
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// During a Quidditch match, only Hoop targets count as goals
			if (Common.Systems.QuidditchCupSystem.matchActive && Common.Systems.QuidditchCupSystem.matchPhase == 0)
			{
				if (target.type == ModContent.NPCType<NPCs.Enemies.QuidditchHoop>())
				{
					Common.Systems.QuidditchCupSystem.OnPlayerGoal();

					// Goal celebration visual
					for (int i = 0; i < 15; i++)
					{
						Dust dust = Dust.NewDustDirect(target.Center, 8, 8,
							DustID.GoldCoin, 0f, -2f, 50, default, 1.5f);
						dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
						dust.noGravity = true;
					}
				}
				// Non-hoop NPC hits during matches do NOT score
			}
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 3;
			Projectile.timeLeft = 300;
			Projectile.light = 0.2f;
			Projectile.ignoreWater = false;
			Projectile.tileCollide = true;
		}

		public override void AI()
		{
			// Gravity
			Projectile.velocity.Y += 0.2f;
			if (Projectile.velocity.Y > 14f)
				Projectile.velocity.Y = 14f;

			Projectile.rotation += Projectile.velocity.Length() * 0.05f;

			// After 1 second, start returning to player
			Projectile.ai[0]++;
			if (Projectile.ai[0] > 60)
			{
				Player owner = Main.player[Projectile.owner];
				Vector2 returnDir = (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12f;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, returnDir, 0.05f);
				Projectile.tileCollide = false;

				// Kill if close to player
				if (Vector2.Distance(Projectile.Center, owner.Center) < 30f)
					Projectile.Kill();
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.penetrate--;
			if (Projectile.penetrate <= 0)
			{
				Projectile.Kill();
				return false;
			}

			// Bounce
			if (Projectile.velocity.X != oldVelocity.X)
				Projectile.velocity.X = -oldVelocity.X * 0.8f;
			if (Projectile.velocity.Y != oldVelocity.Y)
				Projectile.velocity.Y = -oldVelocity.Y * 0.8f;

			return false;
		}
	}
}
