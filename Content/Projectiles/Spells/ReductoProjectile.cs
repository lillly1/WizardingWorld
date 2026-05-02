using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Reducto — explosive blue bolt that deals AoE damage on impact.</summary>
	public class ReductoProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 1;
			Projectile.timeLeft = 240;
			Projectile.light = 0.6f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			// Blue bolt trail
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch, 0f, 0f, 100, default, 1.2f);
			dust.noGravity = true;
			dust.velocity *= 0.2f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void OnKill(int timeLeft)
		{
			// Explosion — deal AoE damage
			float explosionRadius = 80f;

			// Visual explosion
			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 8, 8, DustID.BlueTorch, 0f, 0f, 50, default, 2f);
				dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
				dust.noGravity = true;
			}

			for (int i = 0; i < 15; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 8, 8, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
				dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
			}

			// Damage nearby enemies
			if (Projectile.owner == Main.myPlayer)
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (!npc.friendly && npc.CanBeChasedBy() && Vector2.Distance(npc.Center, Projectile.Center) < explosionRadius)
					{
						Player owner = Main.player[Projectile.owner];
						owner.ApplyDamageToNPC(npc, Projectile.damage, 8f, (npc.Center.X > Projectile.Center.X) ? 1 : -1, false);
					}
				}
			}
		}
	}
}
