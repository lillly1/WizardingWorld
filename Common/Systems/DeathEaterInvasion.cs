using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Death Eater Invasion — custom event system.
	/// Triggers randomly at night in Hardmode after defeating the Basilisk,
	/// or can be triggered with a Dark Mark item.
	/// Spawns waves of Death Eaters, Dementors, and Inferi.
	/// Ends after enough enemies are defeated.
	/// </summary>
	public class DeathEaterInvasion : ModSystem
	{
		public static bool invasionActive;
		public static int invasionProgress;
		public static int invasionProgressMax = 100;
		public static int invasionProgressWave;

		private static string Text(string suffix, string fallback, params object[] args) =>
			WizardLocalization.Text($"Mods.WizardingWorld.Events.DeathEaterInvasion.{suffix}", fallback, args);

		private static bool CanRandomStart()
		{
			return Main.hardMode
				&& !Main.dayTime
				&& DownedBossSystem.downedBasilisk
				&& !TriwizardTournamentSystem.taskActive
				&& !StMungosTriageSystem.missionActive
				&& !ForestExpeditionSystem.missionActive;
		}

		public override void PreUpdateWorld()
		{
			if (!invasionActive)
			{
				// Random chance to start at night in Hardmode after Basilisk
				if (CanRandomStart() && Main.rand.NextBool(18000)) // ~1 in 5 minutes average
				{
					StartInvasion();
				}
				return;
			}

			// Check if invasion should end
			if (invasionProgress >= invasionProgressMax)
			{
				EndInvasion();
				return;
			}

			// End if day comes
			if (Main.dayTime)
			{
				EndInvasion();
			}
		}

		public static void StartInvasion()
		{
			if (invasionActive)
				return;

			invasionActive = true;
			invasionProgress = 0;
			invasionProgressWave = 1;

			// Scale difficulty to player count
			invasionProgressMax = 80 + (Main.CurrentFrameFlags.ActivePlayersCount * 20);

			if (Main.netMode != NetmodeID.Server)
			{
				// Dark sky message
				Main.NewText(Text("Start", "The Dark Mark rises... Death Eaters are attacking!"), 175, 50, 200);
			}

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);
		}

		public static void EndInvasion()
		{
			if (!invasionActive)
				return;

			invasionActive = false;
			invasionProgress = 0;

			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(Text("End", "The Death Eaters have been driven back. The night is safe once more."), 50, 200, 50);
			}

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);
		}

		public static void ReportKill()
		{
			if (!invasionActive)
				return;

			invasionProgress++;

			// Wave progression messages
			if (invasionProgress == invasionProgressMax / 3 && invasionProgressWave == 1)
			{
				invasionProgressWave = 2;
				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Text("Wave2", "More Dark creatures emerge from the shadows!"), 175, 50, 200);
			}
			else if (invasionProgress == invasionProgressMax * 2 / 3 && invasionProgressWave == 2)
			{
				invasionProgressWave = 3;
				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Text("Wave3", "The Dark Lord's most dangerous servants have arrived!"), 200, 30, 30);
			}
		}
	}

	/// <summary>Hooks into enemy kills to track invasion progress.</summary>
	public class DeathEaterInvasionGlobalNPC : GlobalNPC
	{
		public override void OnKill(NPC npc)
		{
			// Track kills for Azkaban's Despair event
			AzkabanDespairEvent.OnEnemyKilled(npc);

			if (!DeathEaterInvasion.invasionActive)
				return;

			// Count kills of invasion-relevant enemies
			if (npc.type == ModContent.NPCType<Content.NPCs.Enemies.DeathEater>()
				|| npc.type == ModContent.NPCType<Content.NPCs.Enemies.Dementor>()
				|| npc.type == ModContent.NPCType<Content.NPCs.Enemies.Inferius>())
			{
				DeathEaterInvasion.ReportKill();
			}
		}

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (DeathEaterInvasion.invasionActive)
			{
				// Increase spawn rate dramatically during invasion
				spawnRate = (int)(spawnRate * 0.3f);
				maxSpawns = (int)(maxSpawns * 2.5f);
			}
		}

		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			if (!DeathEaterInvasion.invasionActive)
				return;

			if (!spawnInfo.Player.ZoneOverworldHeight)
				return;

			// Clear normal spawns and replace with invasion enemies
			pool.Clear();

			// Wave 1: Death Eaters and Dementors
			pool[ModContent.NPCType<Content.NPCs.Enemies.DeathEater>()] = 1f;
			pool[ModContent.NPCType<Content.NPCs.Enemies.Dementor>()] = 0.7f;

			// Wave 2: Add Inferi
			if (DeathEaterInvasion.invasionProgressWave >= 2)
			{
				pool[ModContent.NPCType<Content.NPCs.Enemies.Inferius>()] = 0.8f;
			}

			// Wave 3: Add Nagini as rare spawn
			if (DeathEaterInvasion.invasionProgressWave >= 3)
			{
				pool[ModContent.NPCType<Content.NPCs.Enemies.Nagini>()] = 0.1f;
			}
		}
	}
}
