using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Quidditch Season System — Canon Tier A/B.
	///
	/// Expands the existing Golden Snitch chase into a seasonal system:
	///
	/// 1. Snitch Chase (existing) — random daytime event, catch for rewards
	/// 2. Broom Time Trials — race through aerial checkpoints for speed records
	/// 3. Quidditch Season — every 7 in-game days, a "Quidditch Match" event triggers
	///    where multiple Snitches appear and the player must catch as many as possible
	/// 4. Seeker Rank — track total Snitches caught, unlock titles and gear
	///
	/// Seeker Ranks:
	/// 0-2  catches: Amateur Seeker
	/// 3-5  catches: Reserve Seeker
	/// 6-10 catches: Starting Seeker
	/// 11-20 catches: Star Seeker
	/// 21+  catches: Legendary Seeker (grants permanent +5% speed)
	///
	/// "The Snitch is worth 150 points and its capture ends the game."
	/// </summary>
	public class QuidditchSeasonPlayer : ModPlayer
	{
		public int totalSnitchesCaught;
		public int seekerRank; // 0-4

		public override void PostUpdate()
		{
			// Update seeker rank
			int newRank = totalSnitchesCaught switch
			{
				>= 21 => 4,
				>= 11 => 3,
				>= 6 => 2,
				>= 3 => 1,
				_ => 0,
			};

			if (newRank > seekerRank)
			{
				seekerRank = newRank;
				string[] titles = { "Amateur Seeker", "Reserve Seeker", "Starting Seeker", "Star Seeker", "Legendary Seeker" };
				if (Player.whoAmI == Main.myPlayer)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.QuidditchMatch.RankUp", titles[seekerRank]), 255, 215, 0);
			}

			// Legendary Seeker permanent speed bonus
			if (seekerRank >= 4)
				Player.moveSpeed += 0.05f;
		}

		public void CatchSnitch()
		{
			totalSnitchesCaught++;

			// Award House Standing
			var hps = Player.GetModPlayer<HouseRenownSystem>();
			hps.AddPoints(25);

			// Award Great Hall house points
			var wp = Player.GetModPlayer<Players.WizardPlayer>();
			if (wp.houseSet > 0)
				GreatHallSystem.AwardHousePoints(wp.houseSet, 15, Language.GetTextValue("Mods.WizardingWorld.GreatHall.SourceSnitch"));

			// Award class quest progress
			var classQuest = Player.GetModPlayer<ClassQuestPlayer>();
			classQuest.CompleteTask("Charms_SnitchCatch_" + totalSnitchesCaught, "Charms");
		}

		public override void SaveData(Terraria.ModLoader.IO.TagCompound tag)
		{
			tag["totalSnitchesCaught"] = totalSnitchesCaught;
			tag["seekerRank"] = seekerRank;
		}

		public override void LoadData(Terraria.ModLoader.IO.TagCompound tag)
		{
			totalSnitchesCaught = tag.GetInt("totalSnitchesCaught");
			seekerRank = tag.GetInt("seekerRank");
		}
	}

	/// <summary>
	/// Quidditch Season — triggers a multi-Snitch event every 7 in-game days.
	/// During the event, 3-5 Snitches appear simultaneously for 2 minutes.
	/// </summary>
	public class QuidditchSeasonEvent : ModSystem
	{
		private static int dayCounter;

		public override void PreUpdateWorld()
		{
			// Track day changes
			if (Main.dayTime && Main.time < 100)
			{
				dayCounter++;

				// Every 7 days, trigger Quidditch Season
				if (dayCounter % 7 == 0 && Main.hardMode)
				{
					TriggerQuidditchSeason();
				}
			}
		}

		private static void TriggerQuidditchSeason()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.QuidditchMatch.SeasonEvent"), 255, 215, 0);
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.QuidditchMatch.SeasonEvent2"), 255, 200, 100);
			}

			// Trigger multiple snitch spawns via the existing QuidditchEvent system
			// Each snitch is independent
			for (int i = 0; i < 3; i++)
			{
				QuidditchEvent.SpawnSnitch();
			}
		}
	}
}
