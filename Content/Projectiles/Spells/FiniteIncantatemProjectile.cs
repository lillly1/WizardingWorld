using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>
	/// Finite Incantatem — the counter-spell. Clears ALL debuffs from the player.
	/// Creates a golden cleansing wave centered on the caster.
	/// Also removes debuffs from nearby allies in multiplayer.
	/// The most important defensive utility spell in the wizarding world.
	/// </summary>
	public class FiniteIncantatemProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 80;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 30; // Very short — just a burst
			Projectile.light = 1.5f;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			Projectile.Center = owner.Center;
			Projectile.velocity = Vector2.Zero;

			// On first frame: cleanse all debuffs
			if (Projectile.ai[0] == 0)
			{
				Projectile.ai[0] = 1;

				// Remove ALL debuffs from the caster
				for (int i = 0; i < Player.MaxBuffs; i++)
				{
					if (owner.buffType[i] > 0 && Main.debuff[owner.buffType[i]])
					{
						owner.DelBuff(i);
						i--; // Recheck this index since array shifted
					}
				}

				// Also cleanse nearby players in multiplayer (200px radius)
				foreach (var player in Main.ActivePlayers)
				{
					if (player.whoAmI != owner.whoAmI && Vector2.Distance(player.Center, owner.Center) < 200f)
					{
						for (int i = 0; i < Player.MaxBuffs; i++)
						{
							if (player.buffType[i] > 0 && Main.debuff[player.buffType[i]])
							{
								player.DelBuff(i);
								i--;
							}
						}
					}
				}
			}

			// Expanding golden cleansing ring
			float radius = (30 - Projectile.timeLeft) * 4f;
			for (int i = 0; i < 8; i++)
			{
				float angle = MathHelper.TwoPi / 8 * i + Main.GameUpdateCount * 0.1f;
				Vector2 pos = owner.Center + angle.ToRotationVector2() * radius;
				Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.GoldCoin, 0f, 0f, 50, default, 1.5f);
				dust.noGravity = true;
				dust.velocity = (pos - owner.Center).SafeNormalize(Vector2.Zero) * 2f;
			}

			// Central golden sparkle burst
			if (Projectile.timeLeft > 20)
			{
				for (int i = 0; i < 3; i++)
				{
					Dust dust = Dust.NewDustDirect(owner.Center + Main.rand.NextVector2Circular(20, 20), 4, 4, DustID.GoldCoin, 0f, -2f, 50, default, 1.2f);
					dust.noGravity = true;
				}
			}

			Projectile.scale = 1f + (30 - Projectile.timeLeft) * 0.05f;
		}

		public override bool? CanHitNPC(NPC target) => false;
	}
}
