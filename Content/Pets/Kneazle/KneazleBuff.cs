using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.Kneazle
{
	/// <summary>Kneazle pet buff — magical cat that detects untrustworthy people (enemy detection).</summary>
	public class KneazleBuff : ModBuff
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
				ModContent.ProjectileType<KneazleProjectile>());

			// Kneazle bonus: detects enemies (knows who to trust)
			player.detectCreature = true;
		}
	}
}
