using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.Niffler
{
	public class NifflerProjectile : ModProjectile
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

			if (!player.dead && player.HasBuff(ModContent.BuffType<NifflerBuff>()))
				Projectile.timeLeft = 2;

			// Sparkle dust — attracted to shiny things
			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 100, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}
		}
	}
}
