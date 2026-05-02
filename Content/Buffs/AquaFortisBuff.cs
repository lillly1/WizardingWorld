using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Aqua Fortis — water warrior buff. Bonus damage while wet/underwater.</summary>
	public class AquaFortisBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.gills = true;
			player.ignoreWater = true;

			if (player.wet)
			{
				player.GetDamage(DamageClass.Generic) += 0.20f;
				player.statDefense += 8;
				player.moveSpeed += 0.20f;
			}
			else
			{
				player.GetDamage(DamageClass.Generic) += 0.05f;
			}

			// Water bubble particles
			if (player.wet && Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Water, 0f, -1f, 100, default, 0.6f);
				dust.noGravity = true;
			}
		}
	}
}
