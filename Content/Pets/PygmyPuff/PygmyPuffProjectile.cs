using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.PygmyPuff
{
	/// <summary>Pygmy Puff — adorable pink puffball pet. Weasleys' Wizard Wheezes best seller.</summary>
	public class PygmyPuffProjectile : ModProjectile
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
			Main.player[Projectile.owner].blackCat = false;
			return true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (!player.dead && player.HasBuff(ModContent.BuffType<PygmyPuffBuff>()))
				Projectile.timeLeft = 2;

			// Pink sparkle — adorable
			if (Main.rand.NextBool(12))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, 0f, 0f, 100, default, 0.4f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
			}
		}
	}
}
