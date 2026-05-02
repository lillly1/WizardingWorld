using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Biomes
{
	/// <summary>
	/// Forbidden Forest — custom ModBiome.
	/// Activates when: player is in a forest, it's nighttime, and the Basilisk has been defeated.
	/// When active:
	/// - Unique wizard enemy spawn pool (Acromantula, Thestral, Bowtruckle, Centaur-area enemies)
	/// - Darker lighting with green-tinted fog
	/// - Custom water color (dark green)
	/// - Increased enemy spawn rate
	/// The deeper into the wizarding world the player gets, the more the forest comes alive.
	/// </summary>
	public class ForbiddenForestBiome : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;

		public override int Music => MusicID.Eerie;

		public override string BestiaryIcon => "WizardingWorld/Content/Biomes/ForbiddenForestBiome_Icon";

		public override Color? BackgroundColor => new Color(20, 40, 20);

		public override bool IsBiomeActive(Player player)
		{
			// Activates in forest at night after Basilisk is defeated
			bool inForest = player.ZoneForest && player.ZoneOverworldHeight;
			bool isNight = !Main.dayTime;
			bool basiliskDowned = Common.Systems.DownedBossSystem.downedBasilisk;

			return inForest && isNight && basiliskDowned;
		}

		public override void SpecialVisuals(Player player, bool isActive)
		{
			if (isActive)
			{
				// Darker, more ominous lighting
				player.ManageSpecialBiomeVisuals("WizardingWorld:ForbiddenForest", isActive);
			}
		}

		public override float GetWeight(Player player)
		{
			// Higher weight when deeper into the mod's progression
			float weight = 0.5f;
			if (Common.Systems.DownedBossSystem.downedHorntail)
				weight += 0.2f;
			if (Common.Systems.DownedBossSystem.downedVoldemort)
				weight += 0.3f;
			return weight;
		}
	}
}
