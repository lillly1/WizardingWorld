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
    /// Time Chamber System -- Department of Mysteries temporal stabilization mission.
    ///
    /// Objective: Collect 4 Unstable Hourglasses before time collapses.
    /// Hazards: Time distortion zones slow/haste the player, temporal burst damage.
    /// Reward: Chronal Sand material + time-themed buff.
    ///
    /// Unlocked after completing at least one Prophecy mission.
    /// Mod-original: inspired by the Time Room with its clocks and Time-Turners.
    /// </summary>
    public class TimeChamberSystem : ModSystem
    {
        public static bool chamberUnlocked;
        public static int runsCompleted;

        public static bool missionActive;
        public static int missionTimer;
        public static int hourglassesCollected;

        private const int MISSION_DURATION = 60 * 60 * 2; // 2 minutes (tighter than Prophecy)
        private const int HOURGLASSES_NEEDED = 4;

        public override void ClearWorld()
        {
            chamberUnlocked = false;
            runsCompleted = 0;
            missionActive = false;
        }

        public static bool CanUnlock() =>
            ProphecyMissionSystem.missionsCompleted >= 1 && !chamberUnlocked;

        public static bool CanStart() =>
            chamberUnlocked && !missionActive &&
            !ProphecyMissionSystem.missionActive &&
            !DeathChamberSystem.missionActive;

        public static void Unlock()
        {
            if (!CanUnlock()) return;
            chamberUnlocked = true;
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.TimeChamberUnlocked"),
                    new Color(200, 180, 100));
            }
        }

        public static void StartMission(Player player)
        {
            if (!CanStart()) return;
            missionActive = true;
            missionTimer = MISSION_DURATION;
            hourglassesCollected = 0;

            SpawnHourglasses(player);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.TimeChamberStart"),
                    new Color(220, 200, 100));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.CollectHourglasses",
                    HOURGLASSES_NEEDED), new Color(200, 200, 150));
            }
        }

        public override void PreUpdateWorld()
        {
            if (!missionActive) return;
            missionTimer--;

            // Time distortion effect on players: alternate slow/haste every 10 seconds
            if (missionTimer % 600 < 300)
            {
                foreach (Player p in Main.player)
                {
                    if (!p.active || p.dead) continue;
                    p.AddBuff(BuffID.Slow, 10);
                }
            }

            if (missionTimer <= 0) { FailMission(); return; }
            if (missionTimer == 60 * 20 && Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.TimeWarning"),
                    new Color(255, 100, 100));
        }

        public static void OnHourglassCollected(Player player)
        {
            if (!missionActive) return;
            hourglassesCollected++;
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.HourglassProgress",
                    hourglassesCollected, HOURGLASSES_NEEDED), new Color(220, 200, 100));
            if (hourglassesCollected >= HOURGLASSES_NEEDED)
                CompleteMission(player);
        }

        private static void CompleteMission(Player player)
        {
            missionActive = false;
            runsCompleted++;
            CleanupEntities();

            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.ChronalSand>(), 4 + runsCompleted);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 10);
            player.AddBuff(ModContent.BuffType<Content.Buffs.TemporalInsightBuff>(), 60 * 60 * 8);

            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.TimeChamberComplete"),
                    new Color(220, 200, 100));

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 20,
                    Language.GetTextValue("Mods.WizardingWorld.Ministry.SourceTimeChamber"));
        }

        private static void FailMission()
        {
            missionActive = false;
            CleanupEntities();
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.TimeChamberFailed"),
                    new Color(255, 100, 100));
        }

        private static void SpawnHourglasses(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int type = ModContent.NPCType<Content.NPCs.Enemies.UnstableHourglass>();
            for (int i = 0; i < HOURGLASSES_NEEDED; i++)
            {
                float x = player.Center.X + Main.rand.Next(-500, 500);
                float y = player.Center.Y + Main.rand.Next(-200, 100);
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, type);
            }
        }

        private static void CleanupEntities()
        {
            int type = ModContent.NPCType<Content.NPCs.Enemies.UnstableHourglass>();
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.type == type) npc.active = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["tc_unlocked"] = chamberUnlocked;
            tag["tc_runs"] = runsCompleted;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            chamberUnlocked = tag.GetBool("tc_unlocked");
            runsCompleted = tag.GetInt("tc_runs");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(chamberUnlocked);
            writer.Write(runsCompleted);
        }

        public override void NetReceive(BinaryReader reader)
        {
            chamberUnlocked = reader.ReadBoolean();
            runsCompleted = reader.ReadInt32();
        }
    }
}
