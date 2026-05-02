using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Ward of Hope — a prison-ward sigil that pushes back despair.</summary>
	public class WardOfHopeBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			var wizardPlayer = player.GetModPlayer<Common.Players.WizardPlayer>();
			wizardPlayer.hasWardOfHope = true;

			player.statDefense += 4;
			player.lifeRegen += 2;
			Lighting.AddLight(player.Center, 0.35f, 0.40f, 0.50f);
		}
	}
}
