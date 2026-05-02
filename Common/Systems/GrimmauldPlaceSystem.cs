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
    /// Grimmauld Place System -- Order of the Phoenix safehouse maintenance.
    ///
    /// The hidden headquarters needs constant upkeep against Doxy infestations,
    /// Walburga's portrait eruptions, and Boggart manifestations.
    ///
    /// Three mission types rotate:
    /// 1. Doxy Sweep: eliminate Doxy nests
    /// 2. Portrait Suppression: silence Walburga curse zones
    /// 3. Boggart Ward: banish fear manifestations
    ///
    /// NOT a public location. Hidden via Fidelius-style concealment.
    /// Mod-original: inspired by the Order's use of 12 Grimmauld Place.
    /// </summary>
    public class GrimmauldPlaceSystem : ModSystem
    {
        public static bool safehouseRevealed;
        public static int maintenanceCompleted;
        public static int currentMissionType; // 0=doxy, 1=portrait, 2=boggart

        public static bool missionActive;
        public static int missionTimer;
        public static int threatsCleared;

        private const int MISSION_DURATION = 60 * 60 * 3;
        private const int THREATS_NEEDED = 4;

        public override void ClearWorld()
        {
            safehouseRevealed = false;
            maintenanceCompleted = 0;
            currentMissionType = 0;
            missionActive = false;
        }

        public static bool CanReveal() =>
            DownedBossSystem.downedDementorKing &&
            ProphecyMissionSystem.missionsCompleted >= 1 &&
            !safehouseRevealed;

        public static bool CanStart() =>
            safehouseRevealed && !missionActive;

        public static void RevealSafehouse()
        {
            if (!CanReveal()) return;
            safehouseRevealed = true;
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Grimmauld.Revealed"),
                    new Color(140, 130, 120));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Grimmauld.Address"),
                    new Color(120, 110, 100));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Grimmauld.Description"),
                    new Color(100, 90, 80));
            }
        }

        public static void StartMission(Player player)
        {
            if (!CanStart()) return;
            missionActive = true;
            missionTimer = MISSION_DURATION;
            threatsCleared = 0;
            currentMissionType = maintenanceCompleted % 3;

            SpawnThreats(player);

            string startKey = currentMissionType switch
            {
                0 => "Mods.WizardingWorld.Grimmauld.DoxySweepStart",
                1 => "Mods.WizardingWorld.Grimmauld.PortraitStart",
                2 => "Mods.WizardingWorld.Grimmauld.BoggartStart",
                _ => "Mods.WizardingWorld.Grimmauld.DoxySweepStart",
            };

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue(startKey), new Color(140, 130, 120));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Grimmauld.ClearThreats",
                    THREATS_NEEDED), new Color(120, 110, 100));
            }
        }

        public override void PreUpdateWorld()
        {
            if (!missionActive) return;
            missionTimer--;

            // House hostility: periodic debuff pulses
            if (missionTimer % 1200 == 0 && missionTimer > 0)
            {
                foreach (Player p in Main.player)
                {
                    if (!p.active || p.dead) continue;
                    switch (currentMissionType)
                    {
                        case 0: // Doxy -- poison nip
                            if (Main.rand.NextBool(2)) p.AddBuff(BuffID.Poisoned, 90);
                            break;
                        case 1: // Portrait -- shrieking: Jinxed
                            if (Main.rand.NextBool(2))
                                p.AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.JinxedDebuff>(), 90);
                            break;
                        case 2: // Boggart -- fear: Darkness + Slow
                            if (Main.rand.NextBool(2))
                            {
                                p.AddBuff(BuffID.Darkness, 120);
                                p.AddBuff(BuffID.Slow, 60);
                            }
                            break;
                    }
                }
            }

            if (missionTimer <= 0) { FailMission(); return; }
            if (missionTimer == 60 * 30 && Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Grimmauld.TimeWarning"),
                    new Color(255, 100, 100));
        }

        public static void OnThreatCleared(Player player)
        {
            if (!missionActive) return;
            threatsCleared++;
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Grimmauld.ThreatProgress",
                    threatsCleared, THREATS_NEEDED), new Color(140, 160, 140));
            if (threatsCleared >= THREATS_NEEDED)
                CompleteMission(player);
        }

        private static void CompleteMission(Player player)
        {
            missionActive = false;
            maintenanceCompleted++;
            CleanupEntities();

            int materialAmount = 3 + maintenanceCompleted;
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.ConcealmentThread>(), materialAmount);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 10);
            player.AddBuff(ModContent.BuffType<Content.Buffs.SafehouseResolveBuff>(), 60 * 60 * 10);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), ItemID.GoldCoin, 2 + maintenanceCompleted);

            string completeKey = currentMissionType switch
            {
                0 => "Mods.WizardingWorld.Grimmauld.DoxySweepComplete",
                1 => "Mods.WizardingWorld.Grimmauld.PortraitComplete",
                2 => "Mods.WizardingWorld.Grimmauld.BoggartComplete",
                _ => "Mods.WizardingWorld.Grimmauld.DoxySweepComplete",
            };

            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue(completeKey), new Color(140, 180, 140));

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 15,
                    Language.GetTextValue("Mods.WizardingWorld.Grimmauld.SourceMaintenance"));
        }

        private static void FailMission()
        {
            missionActive = false;
            CleanupEntities();
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Grimmauld.MissionFailed"),
                    new Color(255, 80, 80));
        }

        private static void SpawnThreats(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int type = currentMissionType switch
            {
                0 => ModContent.NPCType<Content.NPCs.Enemies.DoxyNest>(),
                1 => ModContent.NPCType<Content.NPCs.Enemies.WalburgaPortrait>(),
                2 => ModContent.NPCType<Content.NPCs.Enemies.GrimmauldBoggart>(),
                _ => ModContent.NPCType<Content.NPCs.Enemies.DoxyNest>(),
            };
            for (int i = 0; i < THREATS_NEEDED; i++)
            {
                float x = player.Center.X + Main.rand.Next(-500, 500);
                float y = player.Center.Y + Main.rand.Next(-200, 100);
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, type);
            }
        }

        private static void CleanupEntities()
        {
            int[] types = {
                ModContent.NPCType<Content.NPCs.Enemies.DoxyNest>(),
                ModContent.NPCType<Content.NPCs.Enemies.WalburgaPortrait>(),
                ModContent.NPCType<Content.NPCs.Enemies.GrimmauldBoggart>(),
            };
            foreach (NPC npc in Main.ActiveNPCs)
                foreach (int t in types)
                    if (npc.type == t) npc.active = false;
        }

        public static string GetStatusText()
        {
            if (!safehouseRevealed)
                return Language.GetTextValue("Mods.WizardingWorld.Grimmauld.NotRevealed");
            if (missionActive)
                return Language.GetTextValue("Mods.WizardingWorld.Grimmauld.ActiveStatus",
                    threatsCleared, THREATS_NEEDED, missionTimer / 60);
            string nextType = (maintenanceCompleted % 3) switch
            {
                0 => "Doxy Sweep", 1 => "Portrait Suppression", 2 => "Boggart Ward", _ => "Doxy Sweep"
            };
            return Language.GetTextValue("Mods.WizardingWorld.Grimmauld.ReadyStatus",
                maintenanceCompleted, nextType);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["gp_revealed"] = safehouseRevealed;
            tag["gp_completed"] = maintenanceCompleted;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            safehouseRevealed = tag.GetBool("gp_revealed");
            maintenanceCompleted = tag.GetInt("gp_completed");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(safehouseRevealed);
            writer.Write(maintenanceCompleted);
        }

        public override void NetReceive(BinaryReader reader)
        {
            safehouseRevealed = reader.ReadBoolean();
            maintenanceCompleted = reader.ReadInt32();
        }
    }
}
