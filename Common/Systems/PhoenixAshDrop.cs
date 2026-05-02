using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// When a Phoenix minion kills an enemy, there's a 10% chance to drop Phoenix Ash.
	/// This makes the Phoenix summon weapon even more valuable — it produces crafting materials.
	/// Thematically: phoenixes leave ash when their fire consumes things.
	/// </summary>
	public class PhoenixAshDrop : GlobalProjectile
	{
		public override void OnKill(Projectile projectile, int timeLeft)
		{
			// Not relevant — we need OnHitNPC when the NPC dies
		}

		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Only from Phoenix minion
			if (projectile.type != ModContent.ProjectileType<Content.Projectiles.PhoenixMinion>())
				return;

			// Only when the hit kills the enemy
			if (target.life > 0)
				return;

			// 10% chance to drop Phoenix Ash
			if (Main.rand.NextBool(10))
			{
				Item.NewItem(projectile.GetSource_OnHit(target), target.getRect(),
					ModContent.ItemType<Content.Items.Consumables.PhoenixAsh>(), 1);
			}
		}
	}
}
