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
    /// Knockturn Alley System -- dark-object containment / appraisal missions.
    ///
    /// Adjacent to the Diagon Alley package. Accessed through a shady side-street.
    /// The player takes Ministry-sanctioned cursed-object containment contracts
    /// from Borgin and Burkes, inspecting and securing dangerous dark artifacts.
    ///
    /// Framing: NOT glamorizing dark arts. The player handles cursed objects
    /// carefully under contract, not collecting evil trophies.
    ///
    /// Mod-original: inspired by Knockturn Alley and Borgin and Burkes.
    /// </summary>
    public class KnockturnAlleySystem : ModSystem
    {
        public static bool knockturnUnlocked;
        public static int contractsCompleted;

        public static bool missionActive;
        public static int missionTimer;
        public static int objectsContained;

        private const int MISSION_DURATION = 60 * 60 * 3; // 3 minutes
        private const int OBJECTS_NEEDED = 3;

        public override void ClearWorld()
        {
            knockturnUnlocked = false;
            contractsCompleted = 0;
            missionActive = false;
        }

        public static bool CanUnlock()
        {
            return DiagonAlleySystem.diagonAlleyVisited &&
                   DownedBossSystem.downedBellatrix && !knockturnUnlocked;
        }

        public static bool CanStart()
        {
            return knockturnUnlocked && !missionActive &&
                   !GringottsVaultSystem.missionActive;
        }

        public static void Unlock()
        {
            if (!CanUnlock()) return;
            knockturnUnlocked = true;
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Knockturn.Unlocked"),
                    new Color(120, 100, 140));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Knockturn.Warning"),
                    new Color(160, 130, 160));
            }
        }

        public static void StartMission(Player player)
        {
            if (!CanStart()) return;
            missionActive = true;
            missionTimer = MISSION_DURATION;
            objectsContained = 0;

            SpawnCursedObjects(player);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Knockturn.MissionStart"),
                    new Color(140, 110, 160));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Knockturn.ContainObjects",
                    OBJECTS_NEEDED), new Color(160, 130, 170));
            }

            // Dark Arts corruption risk
            var darkPlayer = player.GetModPlayer<Players.DarkArtsCorruptionPlayer>();
            darkPlayer.AddCorruption(0.03f);
        }

        public override void PreUpdateWorld()
        {
            if (!missionActive) return;
            missionTimer--;

            // Cursed atmosphere -- periodic jinx pulses
            if (missionTimer % 1200 == 0 && missionTimer > 0)
            {
                foreach (Player p in Main.player)
                {
                    if (!p.active || p.dead) continue;
                    if (Main.rand.NextBool(3))
                    {
                        p.AddBuff(BuffID.Darkness, 90);
                        p.GetModPlayer<Players.WizardPlayer>().AddDespair(0.02f, "Knockturn Alley's dark aura");
                    }
                }
            }

            if (missionTimer <= 0) { FailMission(); return; }
            if (missionTimer == 60 * 30 && Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Knockturn.TimeWarning"),
                    new Color(255, 100, 100));
        }

        public static void OnObjectContained(Player player)
        {
            if (!missionActive) return;
            objectsContained++;
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Knockturn.ContainProgress",
                    objectsContained, OBJECTS_NEEDED), new Color(160, 140, 180));
            if (objectsContained >= OBJECTS_NEEDED)
                CompleteMission(player);
        }

        private static void CompleteMission(Player player)
        {
            missionActive = false;
            contractsCompleted++;
            CleanupEntities();

            int dustAmount = 4 + contractsCompleted;
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.CursedResidue>(), dustAmount);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 10);
            player.AddBuff(ModContent.BuffType<Content.Buffs.DarkAppraiserBuff>(), 60 * 60 * 8);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), ItemID.GoldCoin, 2 + contractsCompleted);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Knockturn.MissionComplete"),
                    new Color(160, 140, 180));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Knockturn.BorginPays"),
                    new Color(140, 120, 150));
            }

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 10,
                    Language.GetTextValue("Mods.WizardingWorld.Knockturn.SourceContract"));
        }

        private static void FailMission()
        {
            missionActive = false;
            CleanupEntities();
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Knockturn.MissionFailed"),
                    new Color(255, 80, 80));
        }

        private static void SpawnCursedObjects(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            // Mix of object types for variety
            int opalType = ModContent.NPCType<Content.NPCs.Enemies.CursedOpalDisplay>();
            int cabinetType = ModContent.NPCType<Content.NPCs.Enemies.VanishingCabinetEcho>();

            // 2 opals + 1 cabinet
            for (int i = 0; i < 2; i++)
            {
                float x = player.Center.X + Main.rand.Next(-500, 500);
                float y = player.Center.Y + Main.rand.Next(-200, 100);
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, opalType);
            }
            float cx = player.Center.X + Main.rand.Next(-400, 400);
            float cy = player.Center.Y + Main.rand.Next(-150, 100);
            NPC.NewNPC(player.GetSource_FromThis(), (int)cx, (int)cy, cabinetType);
        }

        private static void CleanupEntities()
        {
            int t1 = ModContent.NPCType<Content.NPCs.Enemies.CursedOpalDisplay>();
            int t2 = ModContent.NPCType<Content.NPCs.Enemies.VanishingCabinetEcho>();
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.type == t1 || npc.type == t2) npc.active = false;
        }

        public static string GetStatusText()
        {
            if (!knockturnUnlocked)
                return Language.GetTextValue("Mods.WizardingWorld.Knockturn.NotUnlocked");
            if (missionActive)
                return Language.GetTextValue("Mods.WizardingWorld.Knockturn.ActiveStatus",
                    objectsContained, OBJECTS_NEEDED, missionTimer / 60);
            return Language.GetTextValue("Mods.WizardingWorld.Knockturn.ReadyStatus", contractsCompleted);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["kn_unlocked"] = knockturnUnlocked;
            tag["kn_contracts"] = contractsCompleted;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            knockturnUnlocked = tag.GetBool("kn_unlocked");
            contractsCompleted = tag.GetInt("kn_contracts");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(knockturnUnlocked);
            writer.Write(contractsCompleted);
        }

        public override void NetReceive(BinaryReader reader)
        {
            knockturnUnlocked = reader.ReadBoolean();
            contractsCompleted = reader.ReadInt32();
        }
    }
}
