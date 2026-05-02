using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Polyjuice — grants stealth and reduced aggro (disguise effect).</summary>
	public class PolyjuiceBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.invis = true;
			player.aggro -= 300;
			player.moveSpeed += 0.10f;
		}
	}
}
