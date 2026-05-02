using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Modifies spawn pools when the Forbidden Forest biome is active.
	/// Replaces normal forest spawns with wizard creatures.
	/// Spawn diversity increases as more wizard bosses are defeated.
	/// </summary>
	public class ForbiddenForestSpawns : GlobalNPC
	{
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (player.InModBiome<Content.Biomes.ForbiddenForestBiome>())
			{
				// More spawns in the Forbidden Forest
				spawnRate = (int)(spawnRate * 0.6f);
				maxSpawns = (int)(maxSpawns * 1.8f);
			}
		}

		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			if (!spawnInfo.Player.InModBiome<Content.Biomes.ForbiddenForestBiome>())
				return;

			// Replace most normal spawns with wizard creatures
			// Keep some vanilla spawns for variety (don't clear completely)

			// Core Forbidden Forest fauna
			pool[ModContent.NPCType<Content.NPCs.Enemies.Acromantula>()] = 1.0f;
			pool[ModContent.NPCType<Content.NPCs.Enemies.Thestral>()] = 0.6f;
			pool[ModContent.NPCType<Content.NPCs.Enemies.Boggart>()] = 0.4f;
			pool[ModContent.NPCType<Content.NPCs.Enemies.Bowtruckle>()] = 0.3f;

			// Post-Horntail: more dangerous creatures
			if (DownedBossSystem.downedHorntail)
			{
				pool[ModContent.NPCType<Content.NPCs.Enemies.Dementor>()] = 0.5f;
				pool[ModContent.NPCType<Content.NPCs.Enemies.Werewolf>()] = 0.3f;
			}

			// Post-Voldemort: the darkest creatures
			if (DownedBossSystem.downedVoldemort)
			{
				pool[ModContent.NPCType<Content.NPCs.Enemies.DeathEater>()] = 0.3f;
				pool[ModContent.NPCType<Content.NPCs.Enemies.AzkabanGuard>()] = 0.1f;
			}

			// Reduce vanilla spawns
			foreach (var key in new List<int>(pool.Keys))
			{
				if (key < Terraria.ID.NPCID.Count) // Vanilla NPC
					pool[key] *= 0.3f; // Reduce by 70%
			}
		}
	}
}
