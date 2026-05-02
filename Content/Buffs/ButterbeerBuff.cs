using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Warm and content — well-fed equivalent with bonus regen.</summary>
	public class ButterbeerBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.wellFed = true;
			player.lifeRegen += 2;
			player.statDefense += 2;
			player.moveSpeed += 0.05f;
		}
	}
}
