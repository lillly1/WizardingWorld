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
    /// Shrieking Shack System -- Whomping Willow / tunnel / shack missions.
    ///
    /// Three rotating loops:
    /// 0. Willow Suppression -- destroy branch knots while enduring knockback pulses
    /// 1. Tunnel Stabilization -- destroy tunnel wards under Darkness + Slow
    /// 2. Shack Containment -- seal moon-ward fractures under Jinxed + despair
    ///
    /// Unlock: post-Fenrir + night. Access via WillowPassageToken.
    /// Mod-original expedition structure; canon location framing preserved.
    /// </summary>
    public class ShriekingShackSystem : ModSystem
    {
        public static bool shackUnlocked;
        public static int completions;
        public static int currentLoop; // 0=willow, 1=tunnel, 2=shack

        public static bool missionActive;
        public static int missionTimer;
        public static int objectivesCompleted;

        private const int MISSION_DURATION = 60 * 60 * 3; // 3 minutes
        private static int ObjectivesNeeded => currentLoop switch
        {
            0 => 3,
            1 => 4,
            2 => 3,
            _ => 3,
        };

        public override void ClearWorld()
        {
            shackUnlocked = false;
            completions = 0;
            currentLoop = 0;
            missionActive = false;
        }

        public static bool CanUnlock() =>
            DownedBossSystem.downedFenrir && !shackUnlocked;

        public static bool CanStart() =>
            shackUnlocked && !missionActive && !Main.dayTime;

        public static void Unlock()
        {
            if (!CanUnlock()) return;
            shackUnlocked = true;
            SecretPassageSystem.DiscoverPassage("Whomping Willow");
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Shack.Unlocked"),
                    new Color(140, 130, 160));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Shack.Briefing"),
                    new Color(120, 110, 140));
            }
        }

        public static void StartMission(Player player)
        {
            if (!CanStart()) return;
            missionActive = true;
            missionTimer = MISSION_DURATION;
            objectivesCompleted = 0;
            currentLoop = completions % 3;

            SpawnObjectives(player);

            // Transit framing text for Willow / Tunnel entry
            if (Main.netMode != NetmodeID.Server)
            {
                if (currentLoop == 0 || currentLoop == 1)
                {
                    // Entering via the Whomping Willow passage
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Transit.WillowEntry"),
                        new Color(130, 120, 100));
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Transit.WillowTunnel"),
                        new Color(110, 100, 80));
                }
            }

            bool isFullMoon = Main.moonPhase == 0;
            if (isFullMoon && Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Shack.FullMoonWarning"),
                    new Color(200, 180, 255));

            string startKey = currentLoop switch
            {
                0 => "Mods.WizardingWorld.Shack.WillowStart",
                1 => "Mods.WizardingWorld.Shack.TunnelStart",
                2 => "Mods.WizardingWorld.Shack.ContainmentStart",
                _ => "Mods.WizardingWorld.Shack.WillowStart",
            };

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue(startKey), new Color(140, 130, 160));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Shack.CompleteObjectives",
                    ObjectivesNeeded), new Color(120, 110, 140));
            }
        }

        public override void PreUpdateWorld()
        {
            if (!missionActive) return;
            missionTimer--;

            // Loop hazards every 20 seconds (doubled during full moon)
            if (missionTimer % 1200 == 0 && missionTimer > 0)
            {
                int durationMult = Main.moonPhase == 0 ? 2 : 1;
                foreach (Player p in Main.player)
                {
                    if (!p.active || p.dead) continue;
                    switch (currentLoop)
                    {
                        case 0: // Willow: knockback pulse
                            p.velocity.Y -= 6f;
                            p.velocity.X += Main.rand.NextFloat(-4f, 4f);
                            break;
                        case 1: // Tunnel: Darkness + Slow
                            p.AddBuff(BuffID.Darkness, 120 * durationMult);
                            p.AddBuff(BuffID.Slow, 90 * durationMult);
                            break;
                        case 2: // Shack: Jinxed + despair
                            p.AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.JinxedDebuff>(), 120 * durationMult);
                            var wp = p.GetModPlayer<Players.WizardPlayer>();
                            wp.despair = System.Math.Min(wp.despair + (Main.moonPhase == 0 ? 0.25f : 0.15f), 1f);
                            break;
                    }
                }
            }

            if (missionTimer <= 0) { FailMission(); return; }
            if (missionTimer == 60 * 30 && Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Shack.TimeWarning"),
                    new Color(255, 100, 100));
        }

        public static void OnObjectiveCompleted(Player player)
        {
            if (!missionActive) return;
            objectivesCompleted++;
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Shack.ObjectiveProgress",
                    objectivesCompleted, ObjectivesNeeded), new Color(140, 160, 160));
            if (objectivesCompleted >= ObjectivesNeeded)
                CompleteMission(player);
        }

        private static void CompleteMission(Player player)
        {
            missionActive = false;
            completions++;
            CleanupEntities();

            // Reward: MoonsilverThread
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.MoonsilverThread>(), 3 + completions);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 10);
            player.AddBuff(ModContent.BuffType<Content.Buffs.MoonwardBuff>(), 60 * 60 * 10);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), ItemID.GoldCoin, 2 + completions);

            // Full moon bonus: extra MoonsilverThread
            if (Main.moonPhase == 0)
                player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                    ModContent.ItemType<Content.Items.Consumables.MoonsilverThread>(), 2);

            string completeKey = currentLoop switch
            {
                0 => "Mods.WizardingWorld.Shack.WillowComplete",
                1 => "Mods.WizardingWorld.Shack.TunnelComplete",
                2 => "Mods.WizardingWorld.Shack.ContainmentComplete",
                _ => "Mods.WizardingWorld.Shack.WillowComplete",
            };
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue(completeKey), new Color(140, 180, 160));

            // 15 House Renown
            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 15,
                    Language.GetTextValue("Mods.WizardingWorld.Shack.SourceShack"));
        }

        private static void FailMission()
        {
            missionActive = false;
            CleanupEntities();
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Shack.MissionFailed"),
                    new Color(255, 80, 80));
        }

        private static void SpawnObjectives(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int type = currentLoop switch
            {
                0 => ModContent.NPCType<Content.NPCs.Enemies.WillowKnot>(),
                1 => ModContent.NPCType<Content.NPCs.Enemies.TunnelWard>(),
                2 => ModContent.NPCType<Content.NPCs.Enemies.MoonWardFracture>(),
                _ => ModContent.NPCType<Content.NPCs.Enemies.WillowKnot>(),
            };
            for (int i = 0; i < ObjectivesNeeded; i++)
            {
                float x = player.Center.X + Main.rand.Next(-600, 600);
                float y = player.Center.Y + Main.rand.Next(-250, 100);
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, type);
            }
        }

        private static void CleanupEntities()
        {
            int[] types = {
                ModContent.NPCType<Content.NPCs.Enemies.WillowKnot>(),
                ModContent.NPCType<Content.NPCs.Enemies.TunnelWard>(),
                ModContent.NPCType<Content.NPCs.Enemies.MoonWardFracture>(),
            };
            foreach (NPC npc in Main.ActiveNPCs)
                foreach (int t in types)
                    if (npc.type == t) npc.active = false;
        }

        public static string GetStatusText()
        {
            if (!shackUnlocked)
                return Language.GetTextValue("Mods.WizardingWorld.Shack.NotUnlocked");
            if (missionActive)
                return Language.GetTextValue("Mods.WizardingWorld.Shack.ActiveStatus",
                    objectivesCompleted, ObjectivesNeeded, missionTimer / 60);
            string nextLoop = (completions % 3) switch
            {
                0 => "Willow Suppression", 1 => "Tunnel Stabilization", 2 => "Shack Containment", _ => "Willow Suppression"
            };
            return Language.GetTextValue("Mods.WizardingWorld.Shack.ReadyStatus",
                completions, nextLoop);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["ss_unlocked"] = shackUnlocked;
            tag["ss_completed"] = completions;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            shackUnlocked = tag.GetBool("ss_unlocked");
            completions = tag.GetInt("ss_completed");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(shackUnlocked);
            writer.Write(completions);
        }

        public override void NetReceive(BinaryReader reader)
        {
            shackUnlocked = reader.ReadBoolean();
            completions = reader.ReadInt32();
        }
    }
}
