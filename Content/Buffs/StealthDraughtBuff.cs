using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>
	/// Stealth Draught — brewed from Dugbog Hide.
	/// Grants invisibility, -500 aggro, +15% speed for 4 minutes.
	/// The potion equivalent of hiding in plain sight.
	/// </summary>
	public class StealthDraughtBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.invis = true;
			player.aggro -= 500;
			player.moveSpeed += 0.15f;
		}
	}
}
