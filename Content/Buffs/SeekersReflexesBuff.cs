using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>
	/// Seeker's Reflexes — lightning-fast reflexes of a Quidditch Seeker.
	/// +15% movement speed, +10% dodge chance (simulated via endurance),
	/// +10% attack speed. The buff that lets you catch the Snitch.
	/// </summary>
	public class SeekersReflexesBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.moveSpeed += 0.15f;
			player.maxRunSpeed += 1.5f;
			player.endurance += 0.10f; // Simulates dodge via damage reduction
			player.GetAttackSpeed(DamageClass.Generic) += 0.10f;
		}
	}
}
