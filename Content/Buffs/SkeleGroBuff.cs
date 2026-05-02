using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Skele-Gro effect — massive life regeneration. Painful but effective.</summary>
	public class SkeleGroBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			// Massive regen — regrows bones!
			player.lifeRegen += 20;

			// But painful — reduced speed while bones regrow
			player.moveSpeed -= 0.10f;

			// Bone particles (the bones are growing back)
			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, Terraria.ID.DustID.Bone, 0f, 0f, 100, default, 0.5f);
				dust.noGravity = true;
			}
		}
	}
}
