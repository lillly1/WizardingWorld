using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Shield Charm — Protego in a bottle. +10 defense, +8% DR.</summary>
	public class ShieldCharmBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.statDefense += 10;
			player.endurance += 0.08f;
		}
	}
}
