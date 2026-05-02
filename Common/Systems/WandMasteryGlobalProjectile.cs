using Terraria;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Tracks wand spell hits for mastery XP gain.
	/// Each enemy hit by a spell projectile grants 1 mastery XP for the source wand.
	/// </summary>
	public class WandMasteryGlobalProjectile : GlobalProjectile
	{
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
		{
			// Only apply to mod spell projectiles
			return entity.ModProjectile != null &&
				   entity.DamageType == ModContent.GetInstance<SpellDamage>();
		}

		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers) return;

			Player player = Main.player[projectile.owner];
			if (player == null || !player.active) return;

			var wp = player.GetModPlayer<Players.WizardPlayer>();

			// Find the held wand that produced this projectile
			Item heldItem = player.HeldItem;
			if (heldItem != null && heldItem.active &&
				heldItem.DamageType == ModContent.GetInstance<SpellDamage>() &&
				heldItem.damage > 1)
			{
				wp.AddWandMasteryXP(heldItem.type, 1);

				// Mastered wand visual — subtle golden sparkle on hit
				if (wp.GetWandMasteryLevel(heldItem.type) >= 3 && Main.rand.NextBool(4))
				{
					for (int i = 0; i < 3; i++)
					{
						Dust dust = Dust.NewDustDirect(target.position, target.width, target.height,
							Terraria.ID.DustID.GoldCoin, 0f, 0f, 100, default, 0.6f);
						dust.noGravity = true;
						dust.velocity *= 1.5f;
					}
				}
			}
		}
	}
}
