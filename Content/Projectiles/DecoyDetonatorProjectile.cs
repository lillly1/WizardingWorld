using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles
{
	/// <summary>
	/// Decoy Detonator — lands, creates a noise/flash that draws all nearby enemy aggro.
	/// After 3 seconds, explodes dealing damage and applying Jinxed debuff.
	/// Tactical crowd control tool.
	/// </summary>
	public class DecoyDetonatorProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 240; // 4 seconds total
			Projectile.tileCollide = true;
		}

		public override void AI()
		{
			Projectile.ai[0]++;

			if (Projectile.ai[0] < 20)
			{
				// Flying phase — arc through air
				Projectile.velocity.Y += 0.3f;
				Projectile.rotation += 0.2f;
				return;
			}

			// Landed — stop moving
			Projectile.velocity = Vector2.Zero;

			// Decoy phase — draw enemy aggro with noise/flash
			float aggroRadius = 300f;

			// Blinking red light
			if (Projectile.ai[0] % 15 < 8)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(2), 4, 4, DustID.LifeDrain, 0f, 0f, 50, default, 1.0f);
				dust.noGravity = true;
			}

			// Pulse ring effect every second
			if (Projectile.ai[0] % 60 == 0)
			{
				for (int i = 0; i < 20; i++)
				{
					float angle = MathHelper.TwoPi / 20 * i;
					Vector2 pos = Projectile.Center + angle.ToRotationVector2() * 30f;
					Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.GoldCoin, 0f, 0f, 50, default, 0.6f);
					dust.noGravity = true;
					dust.velocity = (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * 2f;
				}
			}

			// Draw nearby enemies toward the decoy
			foreach (var npc in Main.ActiveNPCs)
			{
				if (!npc.friendly && npc.CanBeChasedBy() && Vector2.Distance(npc.Center, Projectile.Center) < aggroRadius)
				{
					// Nudge enemy toward decoy
					Vector2 toDecoy = (Projectile.Center - npc.Center).SafeNormalize(Vector2.Zero) * 0.5f;
					npc.velocity += toDecoy;
				}
			}

			// Explode after 3 seconds of decoy phase
			if (Projectile.ai[0] > 200)
				Projectile.Kill();
		}

		public override void OnKill(int timeLeft)
		{
			// Explosion!
			float explosionRadius = 120f;

			// Big colorful Weasley explosion
			for (int i = 0; i < 40; i++)
			{
				int dustType = Main.rand.Next(new[] { DustID.Torch, DustID.GoldCoin, DustID.LifeDrain, DustID.BlueTorch });
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 8, 8, dustType, 0f, 0f, 50, default, 2f);
				dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
				dust.noGravity = true;
			}

			// Damage and jinx enemies
			if (Projectile.owner == Main.myPlayer)
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (!npc.friendly && npc.CanBeChasedBy() && Vector2.Distance(npc.Center, Projectile.Center) < explosionRadius)
					{
						Player owner = Main.player[Projectile.owner];
						owner.ApplyDamageToNPC(npc, Projectile.damage, 8f, 0, false);
						npc.AddBuff(ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>(), 300);
						npc.AddBuff(BuffID.Confused, 180);
					}
				}
			}
		}
	}
}
