using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.Niffler
{
	public class NifflerBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			bool unused = false;
			player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused,
				ModContent.ProjectileType<NifflerProjectile>());

			// Niffler bonus: increased coin drops
			player.goldRing = true;
		}
	}
}
