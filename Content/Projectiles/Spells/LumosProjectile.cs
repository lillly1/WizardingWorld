using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Lumos — bright light orb that follows the player and illuminates the area.</summary>
	public class LumosProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 600;
			Projectile.light = 2.0f;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			// Float near the player's head
			Vector2 targetPos = owner.Center + new Vector2(0, -60);
			Vector2 direction = targetPos - Projectile.Center;
			Projectile.velocity = direction * 0.1f;

			// Sparkle dust
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TreasureSparkle, 0f, 0f, 150, default, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}

			Projectile.rotation += 0.05f;
		}
	}
}
