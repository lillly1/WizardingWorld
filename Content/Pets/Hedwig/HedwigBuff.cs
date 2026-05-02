using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.Hedwig
{
	/// <summary>Hedwig — light pet owl that illuminates the area.</summary>
	public class HedwigBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.lightPet[Type] = true; // Light pet, not vanity pet
		}

		public override void Update(Player player, ref int buffIndex)
		{
			bool unused = false;
			player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused,
				ModContent.ProjectileType<HedwigProjectile>());
		}
	}
}
