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

		public override void PreUpdateWorld()
		{
			if (!invasionActive)
			{
				// Random chance to start at night in Hardmode after Basilisk
				if (Main.hardMode && !Main.dayTime && DownedBossSystem.downedBasilisk
					&& Main.rand.NextBool(18000)) // ~1 in 5 minutes average
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
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Events.DeathEaterInvasion.Start"), 175, 50, 200);
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
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Events.DeathEaterInvasion.End"), 50, 200, 50);
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
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Events.DeathEaterInvasion.Wave2"), 175, 50, 200);
			}
			else if (invasionProgress == invasionProgressMax * 2 / 3 && invasionProgressWave == 2)
			{
				invasionProgressWave = 3;
				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Events.DeathEaterInvasion.Wave3"), 200, 30, 30);
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
