using Terraria;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Wolfsbane — grants werewolf-like power boost at night (damage, speed, defense).</summary>
	public class WolfsbaneBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			if (!Main.dayTime)
			{
				// Night bonus — werewolf power
				player.GetDamage(DamageClass.Generic) += 0.15f;
				player.moveSpeed += 0.20f;
				player.statDefense += 8;
				player.jumpSpeedBoost += 2f;
			}
			else
			{
				// Day — mild benefit
				player.statDefense += 3;
				player.moveSpeed += 0.05f;
			}
		}
	}
}
