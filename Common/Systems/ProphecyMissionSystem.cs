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
    /// Prophecy Mission System -- Department of Mysteries retrieval loop.
    ///
    /// Access: Use Ministry Visitor Badge after defeating Bellatrix (post-Plantera).
    /// Mission: Navigate the Hall of Prophecy, retrieve 3 Prophecy Orbs while
    /// surviving Death Eater ambushes and Ministry ward hazards.
    ///
    /// Rewards: Prophecy Dust (crafting material), Order commendation.
    /// Repeatable with escalating difficulty on subsequent runs.
    ///
    /// Mod-original: inspired by the Battle of the Department of Mysteries.
    /// The player is on an Order-sanctioned retrieval mission, not replaying
    /// Harry's specific prophecy story.
    /// </summary>
    public class ProphecyMissionSystem : ModSystem
    {
        // Persistent state
        public static bool missionUnlocked;
        public static int missionsCompleted;

        // Active mission state (not persisted)
        public static bool missionActive;
        public static int missionTimer;
        public static int orbsCollected;
        public static int deathEatersDefeated;

        private const int MISSION_DURATION = 60 * 60 * 3; // 3 minutes
        private const int ORBS_NEEDED = 3;

        public override void ClearWorld()
        {
            missionUnlocked = false;
            missionsCompleted = 0;
            missionActive = false;
        }

        public static bool CanUnlock()
        {
            return DownedBossSystem.downedBellatrix && !missionUnlocked;
        }

        public static bool CanStartMission()
        {
            return missionUnlocked && !missionActive;
        }

        public static void UnlockMission()
        {
            if (!CanUnlock()) return;
            missionUnlocked = true;

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.Unlocked"),
                    new Color(80, 100, 180));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.OrderBriefing"),
                    new Color(180, 160, 120));
            }
        }

        public static void StartMission(Player player)
        {
            if (!CanStartMission()) return;

            missionActive = true;
            missionTimer = MISSION_DURATION;
            orbsCollected = 0;
            deathEatersDefeated = 0;

            // Spawn initial prophecy orb markers
            SpawnProphecyOrbs(player);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.MissionStart"),
                    new Color(100, 120, 200));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.CollectOrbs",
                    ORBS_NEEDED), new Color(200, 180, 255));
            }
        }

        public override void PreUpdateWorld()
        {
            if (!missionActive) return;

            missionTimer--;

            // Death Eater ambush waves
            if (missionTimer % (60 * 20) == 0 && missionTimer > 0) // every 20 seconds
            {
                SpawnDeathEaterAmbush();
            }

            // Timeout
            if (missionTimer <= 0)
            {
                FailMission();
                return;
            }

            // 30 second warning
            if (missionTimer == 60 * 30 && Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.TimeWarning"),
                    new Color(255, 100, 100));
            }
        }

        /// <summary>Called when a Prophecy Orb is collected.</summary>
        public static void OnOrbCollected(Player player)
        {
            if (!missionActive) return;
            orbsCollected++;

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.OrbProgress",
                    orbsCollected, ORBS_NEEDED), new Color(200, 180, 255));
            }

            if (orbsCollected >= ORBS_NEEDED)
                CompleteMission(player);
        }

        /// <summary>Called when a Death Eater is killed during the mission.</summary>
        public static void OnDeathEaterKilled()
        {
            if (!missionActive) return;
            deathEatersDefeated++;
        }

        private static void CompleteMission(Player player)
        {
            missionActive = false;
            missionsCompleted++;

            // Rewards
            int dustAmount = 5 + missionsCompleted * 2; // scales with completions
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.ProphecyDust>(), dustAmount);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 15);

            // Completion buff
            player.AddBuff(ModContent.BuffType<Content.Buffs.OrderCommendationBuff>(), 60 * 60 * 10);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.MissionComplete",
                    deathEatersDefeated), new Color(100, 200, 100));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.OrderReward"),
                    new Color(180, 160, 120));
            }

            // Award House Points
            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 25,
                    Language.GetTextValue("Mods.WizardingWorld.Ministry.SourceMission"));

            CleanupMissionEntities();

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        private static void FailMission()
        {
            missionActive = false;
            CleanupMissionEntities();

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.MissionFailed"),
                    new Color(255, 80, 80));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.TryAgain"),
                    new Color(200, 200, 200));
            }
        }

        private static void SpawnProphecyOrbs(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int orbType = ModContent.NPCType<Content.NPCs.Enemies.ProphecyOrb>();
            for (int i = 0; i < ORBS_NEEDED; i++)
            {
                float x = player.Center.X + Main.rand.Next(-400, 400);
                float y = player.Center.Y + Main.rand.Next(-200, 100);
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, orbType);
            }
        }

        private static void SpawnDeathEaterAmbush()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            Player target = Main.player[Player.FindClosest(
                new Vector2(Main.maxTilesX * 8, Main.maxTilesY * 4), 1, 1)];
            if (target == null || !target.active) return;

            int deType = ModContent.NPCType<Content.NPCs.Enemies.DeathEater>();
            int count = 1 + missionsCompleted / 2; // escalates
            for (int i = 0; i < System.Math.Min(count, 3); i++)
            {
                float x = target.Center.X + Main.rand.Next(-300, 300);
                float y = target.Center.Y - 200 + Main.rand.Next(-100, 100);
                NPC.NewNPC(target.GetSource_FromThis(), (int)x, (int)y, deType);
            }
        }

        private static void CleanupMissionEntities()
        {
            int orbType = ModContent.NPCType<Content.NPCs.Enemies.ProphecyOrb>();
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == orbType)
                    npc.active = false;
            }
        }

        public static string GetStatusText()
        {
            if (!missionUnlocked)
                return Language.GetTextValue("Mods.WizardingWorld.Ministry.NotUnlocked");
            if (missionActive)
                return Language.GetTextValue("Mods.WizardingWorld.Ministry.ActiveStatus",
                    orbsCollected, ORBS_NEEDED, missionTimer / 60);
            return Language.GetTextValue("Mods.WizardingWorld.Ministry.ReadyStatus", missionsCompleted);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["pm_unlocked"] = missionUnlocked;
            tag["pm_completed"] = missionsCompleted;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            missionUnlocked = tag.GetBool("pm_unlocked");
            missionsCompleted = tag.GetInt("pm_completed");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(missionUnlocked);
            writer.Write(missionsCompleted);
        }

        public override void NetReceive(BinaryReader reader)
        {
            missionUnlocked = reader.ReadBoolean();
            missionsCompleted = reader.ReadInt32();
        }
    }
}
