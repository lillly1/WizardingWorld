using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Firewhiskey buzz — damage boost but slightly tipsy.</summary>
	public class FirewhiskeyBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(DamageClass.Generic) += 0.12f;
			player.GetCritChance(DamageClass.Generic) += 8;

			// Tipsy side effect — slight random movement wobble
			if (Main.rand.NextBool(300))
				player.AddBuff(BuffID.Confused, 30); // Very brief confusion

			// Fire breath particles
			if (Main.rand.NextBool(10))
			{
				Dust dust = Dust.NewDustDirect(
					player.position + new Vector2(player.direction > 0 ? player.width : -4, 10),
					4, 4, DustID.Torch, player.direction * 1f, 0f, 100, default, 0.5f);
				dust.noGravity = true;
			}
		}
	}
}
