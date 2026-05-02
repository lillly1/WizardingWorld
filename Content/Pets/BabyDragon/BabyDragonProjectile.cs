using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.BabyDragon
{
	/// <summary>Baby Hungarian Horntail — fiery light pet hatched from a golden egg.</summary>
	public class BabyDragonProjectile : ModProjectile
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
			Projectile.light = 1.2f;
		}

		public override bool PreAI()
		{
			Main.player[Projectile.owner].petFlagFairyQueenPet = false;
			return true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (!player.dead && player.HasBuff(ModContent.BuffType<BabyDragonBuff>()))
				Projectile.timeLeft = 2;

			// Warm fire light
			Terraria.Lighting.AddLight(Projectile.Center, 1.0f, 0.7f, 0.3f);

			// Fire particle trail
			if (Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}
		}
	}
}
