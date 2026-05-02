using System;
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
    /// Quidditch Cup System -- inter-house Quidditch season with match flow.
    ///
    /// Separate from the House Cup (which tracks House Points from general activities).
    /// The Quidditch Cup tracks match wins across a season of inter-house matches.
    ///
    /// Match flow:
    /// 1. Player uses Quidditch Whistle to start a match (daytime, on broom, post-Basilisk)
    /// 2. Opposing house is rotated from the remaining 3 houses
    /// 3. Scoring phase: player scores goals (Quaffle throws at targets), opponent scores over time
    /// 4. Bludger hazards appear as projectile dangers
    /// 5. Golden Snitch spawns after scoring phase -- catching it ends the match (+150)
    /// 6. Final score determines winner; standings updated
    /// 7. After playing all 3 opponents, season ends -- Quidditch Cup awarded
    ///
    /// Mod-original Hogwarts-inspired system.
    /// </summary>
    public class QuidditchCupSystem : ModSystem
    {
        // Season standings (match wins per house)
        public static int[] houseMatchWins = new int[5]; // index 1-4

        // Season progress
        public static int matchesPlayedThisSeason;
        public static bool quidditchCupAwarded;
        public static int lastCupWinner; // 0 = none

        // Active match state (not persisted)
        public static bool matchActive;
        public static int matchPlayerHouse;
        public static int matchOpponentHouse;
        public static int playerScore;
        public static int opponentScore;
        public static int matchTimer; // ticks remaining
        public static int matchPhase; // 0=scoring, 1=snitch

        // Pitch boundary center and dimensions (set when match starts)
        private static Vector2 pitchCenter;
        private const float PITCH_HALF_WIDTH = 600f;
        private const float PITCH_HALF_HEIGHT = 350f;

        // Commentary timer
        private static int commentaryCooldown;
        private const int COMMENTARY_INTERVAL = 60 * 15; // every 15 seconds

        private const int SCORING_PHASE_DURATION = 60 * 60 * 2; // 2 minutes
        private const int MATCHES_PER_SEASON = 3; // play each other house once

        public override void ClearWorld()
        {
            houseMatchWins = new int[5];
            matchesPlayedThisSeason = 0;
            quidditchCupAwarded = false;
            lastCupWinner = 0;
            matchActive = false;
        }

        public override void PreUpdateWorld()
        {
            if (!matchActive) return;

            matchTimer--;

            // Pitch boundary visual markers -- gold dust rectangle
            if (Main.netMode != NetmodeID.Server && matchTimer % 6 == 0)
            {
                SpawnBoundaryParticles();
            }

            // Lightweight commentary at intervals
            commentaryCooldown--;
            if (commentaryCooldown <= 0 && Main.netMode != NetmodeID.Server)
            {
                commentaryCooldown = COMMENTARY_INTERVAL;
                int pick = Main.rand.Next(3);
                string key = pick switch
                {
                    0 => "Mods.WizardingWorld.QuidditchMatch.Commentary1",
                    1 => "Mods.WizardingWorld.QuidditchMatch.Commentary2",
                    _ => "Mods.WizardingWorld.QuidditchMatch.Commentary3",
                };
                Main.NewText(Language.GetTextValue(key), new Color(255, 230, 150));
            }

            // Opponent scoring (simulated pressure)
            if (matchPhase == 0 && matchTimer % 600 == 0) // every 10 seconds
            {
                int opGoal = Main.rand.Next(0, 3); // 0-2 goals
                opponentScore += opGoal * 10;
                if (opGoal > 0 && Main.netMode != NetmodeID.Server)
                {
                    // Floating score popup for opponent goals
                    Player local = Main.LocalPlayer;
                    CombatText.NewText(local.Hitbox, GreatHallSystem.GetHouseColor(matchOpponentHouse),
                        Language.GetTextValue("Mods.WizardingWorld.QuidditchMatch.GoalScored", opGoal * 10));

                    string msg = Language.GetTextValue("Mods.WizardingWorld.Quidditch.OpponentScores",
                        GreatHallSystem.GetHouseName(matchOpponentHouse), opGoal * 10);
                    Main.NewText(msg, GreatHallSystem.GetHouseColor(matchOpponentHouse));

                    // Score update line
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.QuidditchMatch.ScoreUpdate",
                        playerScore, opponentScore), new Color(255, 255, 200));
                }
            }

            // Transition to Snitch phase
            if (matchPhase == 0 && matchTimer <= 0)
            {
                matchPhase = 1;
                matchTimer = 60 * 60; // 1 minute to catch Snitch

                // Spawn the Snitch using existing system
                QuidditchEvent.SpawnSnitch();

                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.SnitchReleased"),
                        new Color(255, 215, 0));
                    string scores = Language.GetTextValue("Mods.WizardingWorld.Quidditch.CurrentScore",
                        playerScore, opponentScore);
                    Main.NewText(scores, new Color(255, 255, 200));
                }
            }

            // Snitch phase timeout -- opponent catches Snitch
            if (matchPhase == 1 && matchTimer <= 0 && !QuidditchEvent.snitchActive)
            {
                EndMatch(false); // Snitch already caught by player via hook
            }
            else if (matchPhase == 1 && matchTimer <= 0)
            {
                // Opponent Seeker catches the Snitch
                opponentScore += 150;
                QuidditchEvent.snitchActive = false;
                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.OpponentCatchesSnitch",
                        GreatHallSystem.GetHouseName(matchOpponentHouse)),
                        GreatHallSystem.GetHouseColor(matchOpponentHouse));
                }
                EndMatch(false);
            }
        }

        public static bool CanStartMatch(Player player)
        {
            if (matchActive) return false;
            if (quidditchCupAwarded) return false;
            if (!Main.dayTime) return false;
            if (!DownedBossSystem.downedBasilisk) return false;

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet <= 0 || wp.houseSet > 4) return false;

            // Must be on a mount (broom)
            if (!player.mount.Active) return false;

            return matchesPlayedThisSeason < MATCHES_PER_SEASON;
        }

        public static void StartMatch(Player player)
        {
            if (!CanStartMatch(player)) return;

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            matchPlayerHouse = wp.houseSet;

            // Rotate opponent: pick next house the player hasn't played
            matchOpponentHouse = GetNextOpponent(matchPlayerHouse);
            if (matchOpponentHouse <= 0) return;

            matchActive = true;
            playerScore = 0;
            opponentScore = 0;
            matchPhase = 0;
            matchTimer = SCORING_PHASE_DURATION;
            pitchCenter = player.Center;
            commentaryCooldown = COMMENTARY_INTERVAL;

            if (Main.netMode != NetmodeID.Server)
            {
                // Pitch entry announcement
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.QuidditchMatch.PitchEntry"),
                    new Color(255, 215, 0));

                string playerName = GreatHallSystem.GetHouseName(matchPlayerHouse);
                string oppName = GreatHallSystem.GetHouseName(matchOpponentHouse);
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.MatchStart",
                    playerName, oppName), new Color(255, 215, 100));

                // Match start flavor from QuidditchMatch keys
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.QuidditchMatch.MatchStart"),
                    new Color(255, 230, 150));

                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.ScoreGoals"),
                    new Color(200, 200, 255));
            }

            // Spawn pitch elements
            SpawnPitchElements(player);
        }

        private static void SpawnPitchElements(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            // Spawn 3 hoops at varying heights, 400px ahead of the player
            int hoopType = ModContent.NPCType<Content.NPCs.Enemies.QuidditchHoop>();
            float baseX = player.Center.X + 400f * player.direction;
            for (int i = 0; i < 3; i++)
            {
                float y = player.Center.Y - 200 + (i * 100); // stacked vertically
                NPC.NewNPC(player.GetSource_FromThis(), (int)baseX, (int)y, hoopType);
            }

            // Spawn 2 Bludgers
            int bludgerType = ModContent.NPCType<Content.NPCs.Enemies.Bludger>();
            NPC.NewNPC(player.GetSource_FromThis(),
                (int)(player.Center.X + 200), (int)(player.Center.Y - 100), bludgerType);
            NPC.NewNPC(player.GetSource_FromThis(),
                (int)(player.Center.X - 200), (int)(player.Center.Y - 100), bludgerType);
        }

        /// <summary>Spawns gold dust particles in a rectangle around the pitch boundary.</summary>
        private static void SpawnBoundaryParticles()
        {
            // Spawn particles along each edge of the rectangular pitch
            for (int i = 0; i < 4; i++)
            {
                float t = Main.rand.NextFloat();
                float px, py;
                switch (i)
                {
                    case 0: // Top edge
                        px = pitchCenter.X - PITCH_HALF_WIDTH + t * PITCH_HALF_WIDTH * 2f;
                        py = pitchCenter.Y - PITCH_HALF_HEIGHT;
                        break;
                    case 1: // Bottom edge
                        px = pitchCenter.X - PITCH_HALF_WIDTH + t * PITCH_HALF_WIDTH * 2f;
                        py = pitchCenter.Y + PITCH_HALF_HEIGHT;
                        break;
                    case 2: // Left edge
                        px = pitchCenter.X - PITCH_HALF_WIDTH;
                        py = pitchCenter.Y - PITCH_HALF_HEIGHT + t * PITCH_HALF_HEIGHT * 2f;
                        break;
                    default: // Right edge
                        px = pitchCenter.X + PITCH_HALF_WIDTH;
                        py = pitchCenter.Y - PITCH_HALF_HEIGHT + t * PITCH_HALF_HEIGHT * 2f;
                        break;
                }
                Dust dust = Dust.NewDustDirect(new Vector2(px, py), 4, 4,
                    DustID.GoldCoin, 0f, 0f, 80, default, 0.7f);
                dust.noGravity = true;
                dust.velocity *= 0.1f;
            }
        }

        /// <summary>Called when the player scores a Quaffle goal during a match.</summary>
        public static void OnPlayerGoal()
        {
            if (!matchActive || matchPhase != 0) return;
            playerScore += 10;

            if (Main.netMode != NetmodeID.Server)
            {
                // Floating score popup above the player
                Player p = Main.LocalPlayer;
                CombatText.NewText(p.Hitbox, new Color(255, 215, 0),
                    Language.GetTextValue("Mods.WizardingWorld.QuidditchMatch.HoopHit"), true);

                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.PlayerScores",
                    GreatHallSystem.GetHouseName(matchPlayerHouse), playerScore),
                    GreatHallSystem.GetHouseColor(matchPlayerHouse));

                // Score update line
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.QuidditchMatch.ScoreUpdate",
                    playerScore, opponentScore), new Color(255, 255, 200));
            }
        }

        /// <summary>Called when the player catches the Snitch during a match.</summary>
        public static void OnPlayerCatchesSnitch()
        {
            if (!matchActive || matchPhase != 1) return;
            playerScore += 150;

            if (Main.netMode != NetmodeID.Server)
            {
                // Dramatic floating +150 popup
                Player p = Main.LocalPlayer;
                CombatText.NewText(p.Hitbox, new Color(255, 215, 0),
                    Language.GetTextValue("Mods.WizardingWorld.QuidditchMatch.GoalScored", 150), true);

                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.PlayerCatchesSnitch"),
                    new Color(255, 215, 0));
            }

            EndMatch(true);
        }

        private static void CleanupPitchElements()
        {
            int hoopType = ModContent.NPCType<Content.NPCs.Enemies.QuidditchHoop>();
            int bludgerType = ModContent.NPCType<Content.NPCs.Enemies.Bludger>();
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == hoopType || npc.type == bludgerType)
                    npc.active = false;
            }
        }

        private static void EndMatch(bool playerCaughtSnitch)
        {
            CleanupPitchElements();
            matchActive = false;
            matchesPlayedThisSeason++;

            bool playerWins = playerScore > opponentScore;

            if (playerWins)
                houseMatchWins[matchPlayerHouse]++;
            else
                houseMatchWins[matchOpponentHouse]++;

            if (Main.netMode != NetmodeID.Server)
            {
                // Final score line from QuidditchMatch keys
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.QuidditchMatch.MatchEnd",
                    playerScore, opponentScore), new Color(255, 255, 200));

                string result = playerWins
                    ? Language.GetTextValue("Mods.WizardingWorld.Quidditch.MatchWin",
                        GreatHallSystem.GetHouseName(matchPlayerHouse), playerScore, opponentScore)
                    : Language.GetTextValue("Mods.WizardingWorld.Quidditch.MatchLoss",
                        GreatHallSystem.GetHouseName(matchOpponentHouse), opponentScore, playerScore);
                Main.NewText(result, playerWins
                    ? GreatHallSystem.GetHouseColor(matchPlayerHouse)
                    : GreatHallSystem.GetHouseColor(matchOpponentHouse));
            }

            // Award House Points for participation
            GreatHallSystem.AwardHousePoints(matchPlayerHouse,
                playerWins ? 20 : 5,
                Language.GetTextValue("Mods.WizardingWorld.Quidditch.SourceMatch"));

            // Check season completion
            if (matchesPlayedThisSeason >= MATCHES_PER_SEASON)
            {
                AwardQuidditchCup();
            }
        }

        private static void AwardQuidditchCup()
        {
            int winner = 1;
            int maxWins = houseMatchWins[1];
            for (int i = 2; i <= 4; i++)
            {
                if (houseMatchWins[i] > maxWins)
                {
                    maxWins = houseMatchWins[i];
                    winner = i;
                }
            }

            lastCupWinner = winner;
            quidditchCupAwarded = true;

            if (Main.netMode != NetmodeID.Server)
            {
                string winnerName = GreatHallSystem.GetHouseName(winner);
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.CupWon", winnerName),
                    GreatHallSystem.GetHouseColor(winner));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.CupCeremony"),
                    new Color(255, 215, 100));
            }

            // Reward winning house players
            foreach (Player p in Main.player)
            {
                if (!p.active) continue;
                var wp = p.GetModPlayer<Players.WizardPlayer>();
                if (wp.houseSet == winner)
                {
                    p.AddBuff(ModContent.BuffType<Content.Buffs.QuidditchCupBuff>(), 60 * 60 * 15);
                }
            }

            // Also give Great Hall points for the ceremony
            GreatHallSystem.AwardHousePoints(winner, 50,
                Language.GetTextValue("Mods.WizardingWorld.Quidditch.SourceCup"));
        }

        public static void ResetSeason()
        {
            houseMatchWins = new int[5];
            matchesPlayedThisSeason = 0;
            quidditchCupAwarded = false;
            lastCupWinner = 0;
        }

        private static int GetNextOpponent(int playerHouse)
        {
            // Simple rotation: play houses in order, skipping own house
            int played = matchesPlayedThisSeason;
            int count = 0;
            for (int h = 1; h <= 4; h++)
            {
                if (h == playerHouse) continue;
                if (count == played) return h;
                count++;
            }
            return 0;
        }

        public static string GetStandingsText()
        {
            return Language.GetTextValue("Mods.WizardingWorld.Quidditch.Standings",
                houseMatchWins[1], houseMatchWins[2], houseMatchWins[3], houseMatchWins[4],
                matchesPlayedThisSeason, MATCHES_PER_SEASON);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["qc_wins"] = houseMatchWins;
            tag["qc_played"] = matchesPlayedThisSeason;
            tag["qc_awarded"] = quidditchCupAwarded;
            tag["qc_winner"] = lastCupWinner;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            houseMatchWins = tag.Get<int[]>("qc_wins") ?? new int[5];
            if (houseMatchWins.Length < 5) houseMatchWins = new int[5];
            matchesPlayedThisSeason = tag.GetInt("qc_played");
            quidditchCupAwarded = tag.GetBool("qc_awarded");
            lastCupWinner = tag.GetInt("qc_winner");
        }

        public override void NetSend(BinaryWriter writer)
        {
            for (int i = 0; i < 5; i++) writer.Write(houseMatchWins[i]);
            writer.Write(matchesPlayedThisSeason);
            writer.Write(quidditchCupAwarded);
            writer.Write(lastCupWinner);
        }

        public override void NetReceive(BinaryReader reader)
        {
            for (int i = 0; i < 5; i++) houseMatchWins[i] = reader.ReadInt32();
            matchesPlayedThisSeason = reader.ReadInt32();
            quidditchCupAwarded = reader.ReadBoolean();
            lastCupWinner = reader.ReadInt32();
        }
    }
}
