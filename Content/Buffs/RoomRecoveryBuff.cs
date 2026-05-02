using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Room of Recovery — massive regen, defense, and damage reduction for wounded wizards.</summary>
	public class RoomRecoveryBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.lifeRegen += 12;
			player.statDefense += 15;
			player.endurance += 0.10f;
		}
	}
}
