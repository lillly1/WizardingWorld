using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Gillyweed — full underwater breathing, swim speed, and no movement penalty in water.</summary>
	public class GillyweedBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.gills = true;
			player.ignoreWater = true;
			player.accFlipper = true;
			player.moveSpeed += 0.10f;

			// Bubble particles while in water
			if (player.wet && Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, Terraria.ID.DustID.Water, 0f, -2f, 100, default, 0.6f);
				dust.noGravity = true;
			}
		}
	}
}
