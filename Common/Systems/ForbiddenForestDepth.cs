using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Forbidden Forest Depth System — Canon Tier A.
	///
	/// Expands the Forbidden Forest with a "depth" mechanic:
	/// the deeper into the forest you go (further from spawn), the more
	/// dangerous it becomes. Three zones:
	///
	/// Zone 1 — Forest Edge (post-Basilisk): Bowtruckles, Thestrals, Centaurs
	/// Zone 2 — Deep Forest (post-Horntail): Acromantulas, Werewolves, Dementors
	/// Zone 3 — Heart of Darkness (post-Bellatrix): Azkaban escapees, Lethifolds, Nundu
	///
	/// Depth is determined by distance from world center.
	/// Each zone has progressively harder enemies and better Unicorn Blood drop rates.
	///
	/// "Students are forbidden from entering the forest. For good reason."
	/// </summary>
	public class ForbiddenForestDepth : ModPlayer
	{
		public int forestDepthZone; // 0=not in forest, 1=edge, 2=deep, 3=heart

		public override void PostUpdate()
		{
			if (!Player.InModBiome<Content.Biomes.ForbiddenForestBiome>())
			{
				forestDepthZone = 0;
				return;
			}

			// Calculate depth based on distance from world center
			float distFromCenter = System.Math.Abs(Player.Center.X / 16f - Main.maxTilesX / 2f);
			float maxDist = Main.maxTilesX / 2f;
			float depthRatio = distFromCenter / maxDist;

			if (depthRatio > 0.6f)
				forestDepthZone = 3; // Heart of Darkness
			else if (depthRatio > 0.4f)
				forestDepthZone = 2; // Deep Forest
			else
				forestDepthZone = 1; // Forest Edge

			// Zone entry messages
			if (Player.whoAmI == Main.myPlayer)
			{
				// Ambient effects scale with depth
				if (forestDepthZone >= 2 && Main.rand.NextBool(30))
				{
					// Darker particles in deep forest
					Dust dust = Dust.NewDustDirect(
						Player.Center + Main.rand.NextVector2Circular(300, 150),
						4, 4, DustID.Shadowflame, 0f, -0.2f, 200, default, 0.3f);
					dust.noGravity = true;
				}

				if (forestDepthZone >= 3 && Main.rand.NextBool(20))
				{
					// Heart of Darkness — oppressive dark mist
					Dust dust = Dust.NewDustDirect(
						Player.Center + Main.rand.NextVector2Circular(200, 100),
						8, 8, DustID.Smoke, Main.rand.NextFloat(-0.5f, 0.5f), -0.1f, 220, default, 0.5f);
					dust.noGravity = true;
				}
			}
		}
	}
}
