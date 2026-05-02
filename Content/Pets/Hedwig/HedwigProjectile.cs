using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.Hedwig
{
	/// <summary>Hedwig — snowy owl light pet. Provides bright illumination.</summary>
	public class HedwigProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 4;
			Main.projPet[Type] = true;
			ProjectileID.Sets.LightPet[Type] = true;
			ProjectileID.Sets.CharacterPreviewAnimations[Type] =
				ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Type], 6)
					.WithOffset(-10, -20f)
					.WithSpriteDirection(-1)
					.WithCode(DelegateMethods.CharacterPreview.Float);
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.FairyQueenPet);
			AIType = ProjectileID.FairyQueenPet;
			Projectile.light = 1.5f;
		}

		public override bool PreAI()
		{
			Player player = Main.player[Projectile.owner];
			player.petFlagFairyQueenPet = false;
			return true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (!player.dead && player.HasBuff(ModContent.BuffType<HedwigBuff>()))
				Projectile.timeLeft = 2;

			// Soft white glow
			Terraria.Lighting.AddLight(Projectile.Center, 1.2f, 1.2f, 1.1f);

			// Occasional feather dust
			if (Main.rand.NextBool(10))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 100, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}
		}
	}
}
