using System;
using Terraria;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Implements the Soul Fragment's life steal mechanic.
	/// When the player has the Soul Fragment equipped, spell damage heals them for 5% of damage dealt.
	/// This is done via GlobalProjectile to catch all spell projectile hits.
	/// </summary>
	public class SoulFragmentLifeSteal : GlobalProjectile
	{
		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Only apply to player-owned spell damage projectiles
			if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
				return;

			Player owner = Main.player[projectile.owner];
			if (!owner.active || owner.dead)
				return;

			// Check if projectile is spell damage
			if (projectile.DamageType != ModContent.GetInstance<SpellDamage>())
				return;

			// Check if player has Soul Fragment equipped
			bool hasSoulFragment = false;
			for (int i = 3; i < 3 + owner.extraAccessorySlots + 7; i++)
			{
				if (i < owner.armor.Length && owner.armor[i].active
					&& owner.armor[i].type == ModContent.ItemType<Content.Items.BossLoot.Voldemort.SoulFragment>())
				{
					hasSoulFragment = true;
					break;
				}
			}

			if (!hasSoulFragment)
				return;

			// Life steal: 5% of damage dealt
			int healAmount = Math.Max(1, damageDone / 20);
			healAmount = Math.Min(healAmount, 10); // Cap at 10 HP per hit

			owner.statLife += healAmount;
			if (owner.statLife > owner.statLifeMax2)
				owner.statLife = owner.statLifeMax2;

			owner.HealEffect(healAmount);
		}
	}
}
