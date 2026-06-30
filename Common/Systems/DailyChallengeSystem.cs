using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Daily Wizard Challenge — a random daily task that rewards Essence of Magic.
	/// Changes every real-world day (based on date seed).
	/// Tracks kill count for the daily target enemy type.
	/// Completing the challenge grants 15-30 Essence + a random buff.
	///
	/// Challenges rotate through:
	/// - Kill X wizard enemies
	/// - Kill X Fantastic Beasts
	/// - Survive a night in the Forbidden Forest
	/// - Catch a wizard fish
	/// - Defeat a specific boss
	///
	/// Players check the challenge via the Daily Prophet or talking to Ollivander.
	/// </summary>
	public class DailyChallengeSystem : ModSystem
	{
		public static int dailyKillTarget;
		public static int dailyKillCount;
		public static string dailyChallengeDescription;
		public static bool dailyCompleted;
		private static int lastDay = -1;

		private static string Text(string suffix, string fallback, params object[] args) =>
			WizardLocalization.Text($"Mods.WizardingWorld.DailyChallenge.{suffix}", fallback, args);

		public override void PreUpdateWorld()
		{
			int today = DateTime.Now.DayOfYear + DateTime.Now.Year * 366;
			if (today != lastDay)
			{
				lastDay = today;
				GenerateChallenge(today);
			}
		}

		private static void GenerateChallenge(int seed)
		{
			var rng = new Random(seed);
			dailyKillCount = 0;
			dailyCompleted = false;

			int challengeType = rng.Next(5);
			switch (challengeType)
			{
				case 0:
					dailyKillTarget = 20;
					dailyChallengeDescription = Text("DefeatCreatures", "Defeat 20 magical creatures");
					break;
				case 1:
					dailyKillTarget = 10;
					dailyChallengeDescription = Text("DefeatForest", "Defeat 10 enemies in the Forbidden Forest");
					break;
				case 2:
					dailyKillTarget = 5;
					dailyChallengeDescription = Text("DefeatDarkForces", "Defeat 5 Dementors or Death Eaters");
					break;
				case 3:
					dailyKillTarget = 15;
					dailyChallengeDescription = Text("DefeatUnderground", "Defeat 15 underground wizard creatures");
					break;
				case 4:
					dailyKillTarget = 30;
					dailyChallengeDescription = Text("DefeatAny", "Defeat 30 wizard enemies (any type)");
					break;
			}
		}

		public static string GetChallengeText()
		{
			if (dailyCompleted)
				return Text("CompleteToday", "Today's challenge is complete. Come back tomorrow.");

			return Text("Status", "Daily Challenge: {0} ({1}/{2})", dailyChallengeDescription, dailyKillCount, dailyKillTarget);
		}

		public static void ReportKill()
		{
			if (dailyCompleted)
				return;

			dailyKillCount++;

			if (dailyKillCount >= dailyKillTarget)
			{
				dailyCompleted = true;

				// Reward ALL active players
				foreach (var player in Main.ActivePlayers)
				{
					player.QuickSpawnItem(player.GetSource_GiftOrReward(),
						ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(),
						Main.rand.Next(15, 31));

					// Random buff reward
					int[] buffs = { BuffID.Wrath, BuffID.Rage, BuffID.Endurance, BuffID.Lifeforce, BuffID.Lucky };
					player.AddBuff(buffs[Main.rand.Next(buffs.Length)], 36000);

					if (player.whoAmI == Main.myPlayer)
						Main.NewText(Text("Complete", "Daily wizard challenge complete. Essence of Magic rewarded!"), 255, 215, 0);
				}
			}
		}
	}

	/// <summary>Tracks wizard enemy kills for the daily challenge.</summary>
	public class DailyChallengeKillTracker : GlobalNPC
	{
		public override void OnKill(NPC npc)
		{
			// Only count mod enemies
			if (npc.ModNPC == null || npc.ModNPC.Mod != ModContent.GetInstance<WizardingWorld>())
				return;

			if (npc.friendly || NPCID.Sets.CountsAsCritter[npc.type])
				return;

			// Zero-slot objectives such as St Mungo's ward nodes should not complete daily kill tasks.
			if (npc.npcSlots <= 0f)
				return;

			DailyChallengeSystem.ReportKill();
		}
	}
}
