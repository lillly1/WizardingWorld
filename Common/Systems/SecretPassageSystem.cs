using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Secret Passage Network -- Hogwarts hidden route system.
	///
	/// Unlocked passages:
	/// 1. Whomping Willow -> Shrieking Shack (via ShriekingShackSystem)
	/// 2. One-Eyed Witch -> Honeydukes cellar (via Diagon Alley visit)
	/// 3. Room of Requirement escape route (via RoR system)
	///
	/// Using a passage: cooldown-based covert transit that teleports the player
	/// to spawn or a marked safe location. Reduced cooldown with Marauder's Map.
	///
	/// Mod-original: inspired by the Marauder's Map's passage knowledge.
	/// </summary>
	public class SecretPassageSystem : ModSystem
	{
		public static bool passageNetworkUnlocked;
		public static int passagesDiscovered; // 0-3
		public static int passageCooldown; // ticks

		// Staged transition state
		private static bool transitionActive;
		private static int transitionTimer;
		private static Player transitionPlayer;
		private const int TRANSITION_DARKNESS_TICKS = 60; // 1 second darkness phase

		private const int BASE_COOLDOWN = 60 * 60 * 5; // 5 minutes
		private const int MAP_COOLDOWN = 60 * 60 * 3; // 3 minutes with Map

		public override void ClearWorld()
		{
			passageNetworkUnlocked = false;
			passagesDiscovered = 0;
			passageCooldown = 0;
		}

		public override void PreUpdateWorld()
		{
			if (passageCooldown > 0) passageCooldown--;

			// Staged transition tick
			if (transitionActive && transitionPlayer != null)
			{
				transitionTimer--;

				if (transitionTimer <= 0)
				{
					CompleteTransition(transitionPlayer);
				}
			}
		}

		public static void DiscoverPassage(string passageName)
		{
			passagesDiscovered++;
			if (!passageNetworkUnlocked && passagesDiscovered >= 2)
			{
				passageNetworkUnlocked = true;
				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Passages.NetworkUnlocked"),
						new Color(180, 160, 140));
			}

			if (Main.netMode != NetmodeID.Server)
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Passages.Discovered", passageName),
					new Color(160, 140, 120));
		}

		public static bool CanUsePassage(Player player)
		{
			return passageNetworkUnlocked && passageCooldown <= 0;
		}

		public static void UsePassage(Player player)
		{
			if (!CanUsePassage(player)) return;
			if (transitionActive) return; // already transitioning

			var wp = player.GetModPlayer<Players.WizardPlayer>();
			passageCooldown = wp.hasMaraudersMap ? MAP_COOLDOWN : BASE_COOLDOWN;

			// Stage 1: Entry text and wraith dust at entry point
			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Transit.PassageFade"),
					new Color(140, 120, 100));

				// Wraith dust at entry
				for (int i = 0; i < 20; i++)
				{
					Dust dust = Dust.NewDustDirect(player.Center, 8, 8,
						DustID.Wraith, 0f, -1f, 80, default, 1.0f);
					dust.noGravity = true;
					dust.velocity = Main.rand.NextVector2Circular(3f, 4f);
				}
			}

			// Stage 2: Darkness buff for brief transition feeling
			player.AddBuff(BuffID.Darkness, TRANSITION_DARKNESS_TICKS);

			// Stage 3: Travel text
			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Transit.PassageTravel"),
					new Color(120, 100, 80));
			}

			// Begin transition timer -- teleport happens after darkness wears off
			transitionActive = true;
			transitionTimer = TRANSITION_DARKNESS_TICKS;
			transitionPlayer = player;
		}

		private static void CompleteTransition(Player player)
		{
			transitionActive = false;
			transitionPlayer = null;

			// Stage 4: Teleport to spawn
			player.Teleport(new Vector2(
				Main.spawnTileX * 16, (Main.spawnTileY - 3) * 16));

			if (Main.netMode != NetmodeID.Server)
			{
				// Stage 5: Arrival text
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Transit.PassageArrive"),
					new Color(160, 140, 120));

				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Passages.Used"),
					new Color(140, 120, 100));

				// Wraith dust at exit
				for (int i = 0; i < 20; i++)
				{
					Dust dust = Dust.NewDustDirect(player.Center, 8, 8,
						DustID.Wraith, 0f, -1f, 80, default, 1.0f);
					dust.noGravity = true;
					dust.velocity = Main.rand.NextVector2Circular(3f, 4f);
				}
			}
		}

		public static string GetStatusText()
		{
			if (!passageNetworkUnlocked)
				return Language.GetTextValue("Mods.WizardingWorld.Passages.NotUnlocked",
					passagesDiscovered);
			if (passageCooldown > 0)
				return Language.GetTextValue("Mods.WizardingWorld.Passages.OnCooldown",
					passageCooldown / 60);
			return Language.GetTextValue("Mods.WizardingWorld.Passages.Ready", passagesDiscovered);
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["sp_unlocked"] = passageNetworkUnlocked;
			tag["sp_discovered"] = passagesDiscovered;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			passageNetworkUnlocked = tag.GetBool("sp_unlocked");
			passagesDiscovered = tag.GetInt("sp_discovered");
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(passageNetworkUnlocked);
			writer.Write(passagesDiscovered);
		}

		public override void NetReceive(BinaryReader reader)
		{
			passageNetworkUnlocked = reader.ReadBoolean();
			passagesDiscovered = reader.ReadInt32();
		}
	}
}
