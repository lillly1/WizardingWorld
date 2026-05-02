using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Veritaserum — reveals all enemies, traps, and treasure on the map.</summary>
	public class VeritaserumBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.detectCreature = true;
			player.dangerSense = true;
			player.findTreasure = true;
			player.nightVision = true;
		}
	}
}
