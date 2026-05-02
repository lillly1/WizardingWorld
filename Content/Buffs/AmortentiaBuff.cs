using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Amortentia — enemies are less aggressive (reduced aggro range). Boosts NPC happiness.</summary>
	public class AmortentiaBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.aggro -= 500;
			player.lifeRegen += 3;
			player.moveSpeed += 0.05f;

			// Charm aura — pink hearts
			if (Main.rand.NextBool(10))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, Terraria.ID.DustID.PinkTorch, 0f, -1f, 100, default, 0.8f);
				dust.noGravity = true;
			}
		}
	}
}
