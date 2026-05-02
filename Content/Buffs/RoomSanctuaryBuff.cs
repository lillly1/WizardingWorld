using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Room of Sanctuary — balanced defense, regen, and mana recovery for all wizards.</summary>
	public class RoomSanctuaryBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.statDefense += 8;
			player.lifeRegen += 4;
			player.manaRegen += 50;
			player.endurance += 0.05f;
		}
	}
}
