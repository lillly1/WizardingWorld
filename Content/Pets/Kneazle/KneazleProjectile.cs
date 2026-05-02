using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.Kneazle
{
	/// <summary>Kneazle — magical cat-like creature. Intelligent, detects enemies.</summary>
	public class KneazleProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 4;
			Main.projPet[Type] = true;
			ProjectileID.Sets.CharacterPreviewAnimations[Type] =
				ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Type], 6)
					.WithOffset(-10, -20f)
					.WithSpriteDirection(-1)
					.WithCode(DelegateMethods.CharacterPreview.Float);
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.BlackCat);
			AIType = ProjectileID.BlackCat;
		}

		public override bool PreAI()
		{
			Player player = Main.player[Projectile.owner];
			player.blackCat = false;
			return true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (!player.dead && player.HasBuff(ModContent.BuffType<KneazleBuff>()))
				Projectile.timeLeft = 2;

			// Occasional purr particles (warm glow)
			if (Main.rand.NextBool(15))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, 0f, 0f, 100, default, 0.4f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
			}
		}
	}
}
