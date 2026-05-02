using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>
	/// Fenrir's Wolfsbane — upgraded wolfsbane effect using Fenrir's own pelt.
	/// Night: +25% melee, +25% speed, +15 defense, +10% crit.
	/// Day: +10% melee, +10% speed.
	/// Significantly stronger than regular Wolfsbane.
	/// </summary>
	public class FenrirsWolfsbaneBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			if (!Main.dayTime)
			{
				player.GetDamage(DamageClass.Melee) += 0.25f;
				player.moveSpeed += 0.25f;
				player.statDefense += 15;
				player.GetCritChance(DamageClass.Generic) += 10;
				player.jumpSpeedBoost += 3f;
			}
			else
			{
				player.GetDamage(DamageClass.Melee) += 0.10f;
				player.moveSpeed += 0.10f;
				player.statDefense += 5;
			}

			// Feral particles at night
			if (!Main.dayTime && Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Smoke, 0f, 0f, 150, default, 0.4f);
				dust.noGravity = true;
			}
		}
	}
}
