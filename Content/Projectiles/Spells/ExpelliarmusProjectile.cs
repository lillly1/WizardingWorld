using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>Expelliarmus — white bolt with high knockback to disarm enemies.</summary>
	public class ExpelliarmusProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 2;
			Projectile.timeLeft = 300;
			Projectile.light = 0.5f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			// White/gold dust trail
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 100, default, 1.0f);
			dust.noGravity = true;
			dust.velocity *= 0.2f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			// Extra knockback — the "disarming" effect
			modifiers.Knockback += 8f;
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 12; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 100, default, 1.3f);
				dust.velocity *= 2f;
				dust.noGravity = true;
			}
		}
	}
}
