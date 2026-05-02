using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.GoldenSnitch
{
	public class GoldenSnitchProjectile : ModProjectile
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
			Projectile.CloneDefaults(ProjectileID.ZephyrFish);
			AIType = ProjectileID.ZephyrFish;
			Projectile.light = 0.3f;
		}

		public override bool PreAI()
		{
			Player player = Main.player[Projectile.owner];
			player.zephyrfish = false;
			return true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (!player.dead && player.HasBuff(ModContent.BuffType<GoldenSnitchBuff>()))
				Projectile.timeLeft = 2;

			// Golden sparkle dust
			if (Main.rand.NextBool(5))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 100, default, 0.6f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}
	}
}
