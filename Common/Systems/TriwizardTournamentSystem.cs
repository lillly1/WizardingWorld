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
    /// Triwizard Tournament System -- three magical tasks leading to the Triwizard Cup.
    ///
    /// Task 1: Dragon Trial -- survive the Horntail and retrieve the Golden Egg
    /// Task 2: Great Lake Trial -- underwater rescue using Gillyweed, past Merfolk/Grindylows
    /// Task 3: Triwizard Maze -- navigate hazards and reach the Cup
    ///
    /// Access: Use the Goblet of Fire after defeating the Horntail boss.
    /// Progression: complete tasks in order, world-persistent.
    /// Reward: Champion's Trophy accessory + champion buff.
    ///
    /// Mod-original adaptation of the canon Triwizard Tournament structure.
    /// </summary>
    public class TriwizardTournamentSystem : ModSystem
    {
        // Tournament state (world-persistent)
        public static bool tournamentUnlocked;
        public static int currentTask; // 0=not started, 1=dragon, 2=lake, 3=maze, 4=champion
        public static bool task1Complete;
        public static bool task2Complete;
        public static bool task3Complete;
        public static bool championCrowned;

        // Inter-school standings (simulated)
        public static int hogwartsScore;
        public static int durmstrangScore;
        public static int beauxbatonsScore;

        // Active task state (not persisted)
        public static bool taskActive;
        public static int taskTimer;
        public static int taskScore;

        // Task 1: Dragon Trial
        private const int DRAGON_TRIAL_DURATION = 60 * 60 * 3; // 3 minutes
        public static bool goldenEggRetrieved;

        // Task 2: Lake Trial
        private const int LAKE_TRIAL_DURATION = 60 * 60 * 3; // 3 minutes
        public static int rescueTokensCollected;
        private const int RESCUE_TOKENS_NEEDED = 3;

        // Task 3: Maze Trial
        private const int MAZE_TRIAL_DURATION = 60 * 60 * 4; // 4 minutes
        public static int mazeObstaclesCleared;
        private const int MAZE_OBSTACLES_NEEDED = 5;

        private static string Text(string suffix, string fallback, params object[] args) =>
            WizardLocalization.Text($"Mods.WizardingWorld.Triwizard.{suffix}", fallback, args);

        public override void ClearWorld()
        {
            tournamentUnlocked = false;
            currentTask = 0;
            task1Complete = false;
            task2Complete = false;
            task3Complete = false;
            championCrowned = false;
            taskActive = false;
            hogwartsScore = 0;
            durmstrangScore = 0;
            beauxbatonsScore = 0;
        }

        public static bool CanUnlock()
        {
            return DownedBossSystem.downedHorntail && !tournamentUnlocked;
        }

        public static bool CanStartNextTask()
        {
            if (!tournamentUnlocked || taskActive || championCrowned) return false;
            int nextTask = NextTaskToStart();
            return nextTask >= 1 && nextTask <= 3;
        }

        private static int NextTaskToStart()
        {
            if (!task1Complete) return 1;
            if (!task2Complete) return 2;
            if (!task3Complete) return 3;
            return 4;
        }

        public static void UnlockTournament()
        {
            if (!CanUnlock()) return;
            tournamentUnlocked = true;
            currentTask = 1;

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Text("Unlocked", "The Goblet of Fire selects you as Hogwarts champion! The Triwizard Tournament begins!"), new Color(100, 150, 255));
                Main.NewText(Text("Task1Intro", "First Task: Retrieve the Golden Egg from the dragon's nest. Defeat or outsmart the Horntail!"), new Color(255, 200, 100));
                Main.NewText(Text("GobletSelection", "The Goblet of Fire blazes blue! It selects you as the Hogwarts champion! Durmstrang and Beauxbatons have also chosen their champions. The Triwizard Tournament begins!"), new Color(100, 150, 255));
            }
        }

        public static void StartTask(Player player)
        {
            if (!CanStartNextTask()) return;

            currentTask = NextTaskToStart();
            if (currentTask > 3) return;

            taskActive = true;
            taskScore = 0;

            switch (currentTask)
            {
                case 1:
                    taskTimer = DRAGON_TRIAL_DURATION;
                    goldenEggRetrieved = false;
                    if (Main.netMode != NetmodeID.Server)
                        Main.NewText(Text("Task1Start", "FIRST TASK: The Hungarian Horntail guards the Golden Egg! Retrieve it before time runs out!"), new Color(255, 100, 50));
                    break;

                case 2:
                    taskTimer = LAKE_TRIAL_DURATION;
                    rescueTokensCollected = 0;
                    player.AddBuff(ModContent.BuffType<Content.Buffs.GillyweedBuff>(), LAKE_TRIAL_DURATION);
                    if (Main.netMode != NetmodeID.Server)
                        Main.NewText(Text("Task2Start", "SECOND TASK: Dive into the Great Lake! Rescue 3 tokens from the merpeople. Gillyweed granted!"), new Color(50, 150, 255));
                    break;

                case 3:
                    taskTimer = MAZE_TRIAL_DURATION;
                    mazeObstaclesCleared = 0;
                    if (Main.netMode != NetmodeID.Server)
                        Main.NewText(Text("Task3Start", "THIRD TASK: Enter the Triwizard Maze! Clear 5 magical obstacles to reach the Cup!"), new Color(100, 200, 100));
                    break;
            }
        }

        public override void PreUpdateWorld()
        {
            if (!taskActive) return;

            taskTimer--;

            // Task timeout
            if (taskTimer <= 0)
            {
                FailTask();
                return;
            }

            // 30 second warning
            if (taskTimer == 60 * 30 && Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Text("TimeWarning", "30 seconds remaining! Hurry!"), new Color(255, 100, 100));
            }
        }

        /// <summary>Called when the player retrieves a Golden Egg during Task 1.</summary>
        public static void OnGoldenEggRetrieved(Player player)
        {
            if (!taskActive || currentTask != 1) return;
            goldenEggRetrieved = true;
            CompleteTask(player);
        }

        /// <summary>Called when the player rescues a token during Task 2.</summary>
        public static void OnRescueTokenCollected(Player player)
        {
            if (!taskActive || currentTask != 2) return;
            rescueTokensCollected++;

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Text("RescueProgress", "Rescue tokens: {0}/{1}",
                    rescueTokensCollected, RESCUE_TOKENS_NEEDED), new Color(50, 200, 255));
            }

            if (rescueTokensCollected >= RESCUE_TOKENS_NEEDED)
                CompleteTask(player);
        }

        /// <summary>Called when the player clears a maze obstacle during Task 3.</summary>
        public static void OnMazeObstacleCleared(Player player)
        {
            if (!taskActive || currentTask != 3) return;
            mazeObstaclesCleared++;

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Text("MazeProgress", "Obstacles cleared: {0}/{1}",
                    mazeObstaclesCleared, MAZE_OBSTACLES_NEEDED), new Color(100, 255, 100));
            }

            if (mazeObstaclesCleared >= MAZE_OBSTACLES_NEEDED)
                CompleteTask(player);
        }

        private static void CompleteTask(Player player)
        {
            taskActive = false;

            switch (currentTask)
            {
                case 1:
                    task1Complete = true;
                    currentTask = 2;
                    if (Main.netMode != NetmodeID.Server)
                        Main.NewText(Text("Task1Complete", "The Golden Egg is yours! First Task complete!"), new Color(255, 215, 0));
                    break;
                case 2:
                    task2Complete = true;
                    currentTask = 3;
                    if (Main.netMode != NetmodeID.Server)
                        Main.NewText(Text("Task2Complete", "All hostages rescued! Second Task complete!"), new Color(255, 215, 0));
                    break;
                case 3:
                    task3Complete = true;
                    CrownChampion(player);
                    break;
            }

            // Simulated rival school progress
            hogwartsScore += 40; // player completed a task
            durmstrangScore += Main.rand.Next(20, 45);
            beauxbatonsScore += Main.rand.Next(20, 45);

            // Award House Points for task completion
            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 40,
                    Text("SourceTask", "Triwizard Task completed"));
        }

        private static void CrownChampion(Player player)
        {
            championCrowned = true;
            currentTask = 4;

            // Grant champion reward
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Accessories.ChampionsTrophy>());
            player.AddBuff(ModContent.BuffType<Content.Buffs.TriwizardChampionBuff>(), 60 * 60 * 20);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Text("Champion", "You are the Triwizard Champion! Eternal glory is yours!"), new Color(255, 215, 0));
                Main.NewText(Text("ChampionCeremony", "The Great Hall erupts in celebration! A champion worthy of legend!"), new Color(255, 255, 200));
                Main.NewText(Text("FinalStandings", "Final Standings: Hogwarts {0} | Durmstrang {1} | Beauxbatons {2}",
                    hogwartsScore, durmstrangScore, beauxbatonsScore), new Color(255, 215, 100));
            }

            // Celebration dust
            for (int i = 0; i < 60; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center, 8, 8,
                    DustID.GoldFlame, 0f, 0f, 50, default, 2f);
                dust.velocity = Main.rand.NextVector2Circular(8f, 8f);
                dust.noGravity = true;
            }

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        private static void FailTask()
        {
            taskActive = false;
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Text("TaskFailed", "Time's up! The task has failed."), new Color(255, 80, 80));
                Main.NewText(Text("TryAgain", "Use the Goblet of Fire to try again."), new Color(200, 200, 200));
            }
        }

        public static string GetStatusText()
        {
            if (!tournamentUnlocked)
                return Text("NotUnlocked", "The Triwizard Tournament has not yet begun. Defeat the Hungarian Horntail first.");
            if (championCrowned)
                return Text("AlreadyChampion", "You have already been crowned Triwizard Champion!") +
                    $"\nStandings -- Hogwarts: {hogwartsScore} | Durmstrang: {durmstrangScore} | Beauxbatons: {beauxbatonsScore}";
            return Text("Status", "Triwizard Tournament: Task 1: {0} | Task 2: {1} | Task 3: {2}",
                task1Complete ? "Done" : (currentTask == 1 ? "Current" : "Locked"),
                task2Complete ? "Done" : (currentTask == 2 ? "Current" : "Locked"),
                task3Complete ? "Done" : (currentTask == 3 ? "Current" : "Locked")) +
                $"\nStandings -- Hogwarts: {hogwartsScore} | Durmstrang: {durmstrangScore} | Beauxbatons: {beauxbatonsScore}";
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["tw_unlocked"] = tournamentUnlocked;
            tag["tw_task"] = currentTask;
            tag["tw_t1"] = task1Complete;
            tag["tw_t2"] = task2Complete;
            tag["tw_t3"] = task3Complete;
            tag["tw_champion"] = championCrowned;
            tag["tw_hogwarts"] = hogwartsScore;
            tag["tw_durmstrang"] = durmstrangScore;
            tag["tw_beauxbatons"] = beauxbatonsScore;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            tournamentUnlocked = tag.GetBool("tw_unlocked");
            currentTask = tag.GetInt("tw_task");
            task1Complete = tag.GetBool("tw_t1");
            task2Complete = tag.GetBool("tw_t2");
            task3Complete = tag.GetBool("tw_t3");
            championCrowned = tag.GetBool("tw_champion");
            hogwartsScore = tag.GetInt("tw_hogwarts");
            durmstrangScore = tag.GetInt("tw_durmstrang");
            beauxbatonsScore = tag.GetInt("tw_beauxbatons");
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new();
            flags[0] = tournamentUnlocked;
            flags[1] = task1Complete;
            flags[2] = task2Complete;
            flags[3] = task3Complete;
            flags[4] = championCrowned;
            writer.Write(flags);
            writer.Write(currentTask);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            tournamentUnlocked = flags[0];
            task1Complete = flags[1];
            task2Complete = flags[2];
            task3Complete = flags[3];
            championCrowned = flags[4];
            currentTask = reader.ReadInt32();
        }
    }
}
