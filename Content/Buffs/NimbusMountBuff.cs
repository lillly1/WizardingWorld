using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	public class NimbusMountBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.mount.SetMount(ModContent.MountType<Mounts.NimbusMount>(), player);
			player.buffTime[buffIndex] = 10;
		}
	}
}
