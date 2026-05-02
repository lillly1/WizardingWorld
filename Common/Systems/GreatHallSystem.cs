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
    /// Great Hall System -- Hogwarts feast and House Cup ceremony hub.
    ///
    /// Every 7 in-game days, a Great Hall Feast becomes available.
    /// The player can attend by using the Great Hall Bell item.
    /// Attending a feast grants a house-aligned buff based on equipped house armor.
    ///
    /// The House Cup is awarded when any house reaches 1000 cumulative points.
    /// Points persist across sessions (unlike the decaying House Renown combat meter).
    ///
    /// Point sources: boss defeats, Owl Post completions, Azkaban clears,
    /// Snitch catches, class quest completions, and combat renown milestones.
    ///
    /// Mod-original Hogwarts-inspired system.
    /// </summary>
    public class GreatHallSystem : ModSystem
    {
        // Persistent house scores (cumulative, do not decay)
        public static int gryffindorPoints;
        public static int slytherinPoints;
        public static int ravenclawPoints;
        public static int hufflepuffPoints;

        // Feast scheduling
        public static int daysSinceLastFeast;
        public static bool feastAvailable;
        public static bool feastAttendedToday;

        private const int FEAST_INTERVAL_DAYS = 7;
        private const int HOUSE_CUP_THRESHOLD = 1000;

        public override void ClearWorld()
        {
            gryffindorPoints = 0;
            slytherinPoints = 0;
            ravenclawPoints = 0;
            hufflepuffPoints = 0;
            daysSinceLastFeast = 0;
            feastAvailable = false;
            feastAttendedToday = false;
        }

        public override void PreUpdateWorld()
        {
            // Track day transitions
            if (Main.dayTime && Main.time < 100)
            {
                daysSinceLastFeast++;
                feastAttendedToday = false;

                if (daysSinceLastFeast >= FEAST_INTERVAL_DAYS && !feastAvailable)
                {
                    feastAvailable = true;
                    if (Main.netMode != NetmodeID.Server)
                    {
                        string msg = Language.GetTextValue("Mods.WizardingWorld.GreatHall.FeastAvailable");
                        Main.NewText(msg, new Color(255, 215, 100));
                    }
                }
            }
        }

        /// <summary>Award points to a specific house.</summary>
        public static void AwardHousePoints(int house, int amount, string source)
        {
            if (house <= 0 || house > 4 || amount <= 0) return;

            switch (house)
            {
                case 1: gryffindorPoints += amount; break;
                case 2: slytherinPoints += amount; break;
                case 3: ravenclawPoints += amount; break;
                case 4: hufflepuffPoints += amount; break;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                string houseName = GetHouseName(house);
                string msg = Language.GetTextValue("Mods.WizardingWorld.GreatHall.PointsAwarded",
                    amount, houseName, source);
                Main.NewText(msg, GetHouseColor(house));
            }

            // Check for House Cup
            int leader = GetLeadingHouse();
            int leaderPts = GetHouseScore(leader);
            if (leaderPts >= HOUSE_CUP_THRESHOLD)
            {
                AwardHouseCup(leader);
            }
        }

        /// <summary>Attend the Great Hall Feast. Returns the buff type granted.</summary>
        public static int AttendFeast(Player player)
        {
            if (!feastAvailable || feastAttendedToday) return -1;

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            int house = wp.houseSet;
            if (house <= 0 || house > 4) return -1;

            feastAttendedToday = true;
            feastAvailable = false;
            daysSinceLastFeast = 0;

            // Award bonus points for attendance
            AwardHousePoints(house, 25, Language.GetTextValue("Mods.WizardingWorld.GreatHall.FeastAttendance"));

            // Grant house-aligned feast buff (10 minutes)
            int buffType = house switch
            {
                1 => ModContent.BuffType<Content.Buffs.GryffindorFeastBuff>(),
                2 => ModContent.BuffType<Content.Buffs.SlytherinFeastBuff>(),
                3 => ModContent.BuffType<Content.Buffs.RavenclawFeastBuff>(),
                4 => ModContent.BuffType<Content.Buffs.HufflepuffFeastBuff>(),
                _ => -1,
            };

            if (buffType > 0)
                player.AddBuff(buffType, 60 * 60 * 10); // 10 minutes

            // Visual celebration
            for (int i = 0; i < 30; i++)
            {
                int dustType = house switch
                {
                    1 => DustID.Torch,       // Gryffindor: red/gold
                    2 => DustID.GreenTorch,   // Slytherin: green
                    3 => DustID.BlueTorch,    // Ravenclaw: blue
                    4 => DustID.YellowStarDust, // Hufflepuff: yellow
                    _ => DustID.GoldCoin,
                };
                Dust dust = Dust.NewDustDirect(player.Center, 8, 8,
                    dustType, 0f, -2f, 50, default, 1.5f);
                dust.velocity = Main.rand.NextVector2Circular(4f, 6f);
                dust.noGravity = true;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                string houseName = GetHouseName(house);
                string msg = Language.GetTextValue("Mods.WizardingWorld.GreatHall.FeastBlessing", houseName);
                Main.NewText(msg, new Color(255, 215, 100));
            }

            return buffType;
        }

        private static void AwardHouseCup(int house)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                string houseName = GetHouseName(house);
                string msg = Language.GetTextValue("Mods.WizardingWorld.GreatHall.HouseCupWon", houseName);
                Main.NewText(msg, GetHouseColor(house));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.GreatHall.HouseCupCeremony"),
                    new Color(255, 215, 100));
            }

            // Reset all houses for next cycle
            gryffindorPoints = 0;
            slytherinPoints = 0;
            ravenclawPoints = 0;
            hufflepuffPoints = 0;

            // Grant bonus to all players of winning house
            foreach (Player player in Main.player)
            {
                if (!player.active) continue;
                var wp = player.GetModPlayer<Players.WizardPlayer>();
                if (wp.houseSet == house)
                {
                    // House Cup winner gets all feast buffs simultaneously
                    player.AddBuff(ModContent.BuffType<Content.Buffs.GryffindorFeastBuff>(), 60 * 60 * 15);
                    player.AddBuff(ModContent.BuffType<Content.Buffs.SlytherinFeastBuff>(), 60 * 60 * 15);
                    player.AddBuff(ModContent.BuffType<Content.Buffs.RavenclawFeastBuff>(), 60 * 60 * 15);
                    player.AddBuff(ModContent.BuffType<Content.Buffs.HufflepuffFeastBuff>(), 60 * 60 * 15);
                }
            }

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        public static int GetLeadingHouse()
        {
            int max = gryffindorPoints;
            int leader = 1;
            if (slytherinPoints > max) { max = slytherinPoints; leader = 2; }
            if (ravenclawPoints > max) { max = ravenclawPoints; leader = 3; }
            if (hufflepuffPoints > max) { max = hufflepuffPoints; leader = 4; }
            return leader;
        }

        public static int GetHouseScore(int house) => house switch
        {
            1 => gryffindorPoints,
            2 => slytherinPoints,
            3 => ravenclawPoints,
            4 => hufflepuffPoints,
            _ => 0,
        };

        public static string GetHouseName(int house) => house switch
        {
            1 => Language.GetTextValue("Mods.WizardingWorld.Houses.Gryffindor"),
            2 => Language.GetTextValue("Mods.WizardingWorld.Houses.Slytherin"),
            3 => Language.GetTextValue("Mods.WizardingWorld.Houses.Ravenclaw"),
            4 => Language.GetTextValue("Mods.WizardingWorld.Houses.Hufflepuff"),
            _ => Language.GetTextValue("Mods.WizardingWorld.Houses.Unknown"),
        };

        public static Color GetHouseColor(int house) => house switch
        {
            1 => new Color(180, 50, 50),
            2 => new Color(50, 160, 50),
            3 => new Color(50, 80, 180),
            4 => new Color(200, 180, 50),
            _ => Color.White,
        };

        public static string GetStandingsText()
        {
            string leader = GetHouseName(GetLeadingHouse());
            return Language.GetTextValue("Mods.WizardingWorld.GreatHall.Standings",
                gryffindorPoints, slytherinPoints, ravenclawPoints, hufflepuffPoints, leader);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["gh_gryffindor"] = gryffindorPoints;
            tag["gh_slytherin"] = slytherinPoints;
            tag["gh_ravenclaw"] = ravenclawPoints;
            tag["gh_hufflepuff"] = hufflepuffPoints;
            tag["gh_daysSinceFeast"] = daysSinceLastFeast;
            tag["gh_feastAvailable"] = feastAvailable;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            gryffindorPoints = tag.GetInt("gh_gryffindor");
            slytherinPoints = tag.GetInt("gh_slytherin");
            ravenclawPoints = tag.GetInt("gh_ravenclaw");
            hufflepuffPoints = tag.GetInt("gh_hufflepuff");
            daysSinceLastFeast = tag.GetInt("gh_daysSinceFeast");
            feastAvailable = tag.GetBool("gh_feastAvailable");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(gryffindorPoints);
            writer.Write(slytherinPoints);
            writer.Write(ravenclawPoints);
            writer.Write(hufflepuffPoints);
            writer.Write(daysSinceLastFeast);
            writer.Write(feastAvailable);
        }

        public override void NetReceive(BinaryReader reader)
        {
            gryffindorPoints = reader.ReadInt32();
            slytherinPoints = reader.ReadInt32();
            ravenclawPoints = reader.ReadInt32();
            hufflepuffPoints = reader.ReadInt32();
            daysSinceLastFeast = reader.ReadInt32();
            feastAvailable = reader.ReadBoolean();
        }
    }
}
