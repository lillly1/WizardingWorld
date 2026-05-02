using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Liquid Luck — increased luck, crit chance, and coin drops.</summary>
	public class FelixFelicisBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.luck += 0.5f;
			player.GetCritChance(DamageClass.Generic) += 10;
			player.goldRing = true;
		}
	}
}
