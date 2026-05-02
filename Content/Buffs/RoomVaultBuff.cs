using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Come and Go Room — luck boost, treasure sense, and coin attraction for wealth-seekers.</summary>
	public class RoomVaultBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.luck += 0.3f;
			player.findTreasure = true;
			player.goldRing = true;
		}
	}
}
