using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.BabyDragon
{
	public class BabyDragonBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.lightPet[Type] = true; // Light pet — baby dragon glows with fire
		}

		public override void Update(Player player, ref int buffIndex)
		{
			bool unused = false;
			player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused,
				ModContent.ProjectileType<BabyDragonProjectile>());
		}
	}
}
