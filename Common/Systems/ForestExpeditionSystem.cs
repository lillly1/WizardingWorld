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
    /// Forbidden Forest Expedition System -- 4-loop deep forest missions.
    ///
    /// Loop A: Unicorn Glade -- cleanse corrupted traces, protect purity (NOT unicorn hunting)
    /// Loop B: Centaur Skywatch -- stabilize omen stones under time pressure
    /// Loop C: Acromantula Nest -- destroy brood markers in the spider den
    /// Loop D: Thestral Clearing -- activate moonlit beacons in eerie atmosphere (non-hostile)
    ///
    /// Access via Forest Expedition Lantern (from Hagrid). Post-Basilisk + nighttime.
    /// Mod-original expedition structure; canon creature framing preserved.
    /// </summary>
    public class ForestExpeditionSystem : ModSystem
    {
        public static bool expeditionUnlocked;
        public static int expeditionsCompleted;
        public static int currentLoop; // 0=unicorn, 1=centaur, 2=acromantula, 3=thestral

        public static bool missionActive;
        public static int missionTimer;
        public static int objectivesCompleted;

        private const int MISSION_DURATION = 60 * 60 * 3;
        private const int OBJECTIVES_NEEDED = 4;

        private static string Text(string suffix, string fallback, params object[] args) =>
            WizardLocalization.Text($"Mods.WizardingWorld.Forest.{suffix}", fallback, args);

        public override void ClearWorld()
        {
            expeditionUnlocked = false;
            expeditionsCompleted = 0;
            currentLoop = 0;
            missionActive = false;
        }

        public static bool CanUnlock() =>
            DownedBossSystem.downedBasilisk && !expeditionUnlocked;

        public static bool CanStart(Player player) =>
            expeditionUnlocked && !missionActive && !Main.dayTime &&
            player.InModBiome<Content.Biomes.ForbiddenForestBiome>();

        public static void Unlock()
        {
            if (!CanUnlock()) return;
            expeditionUnlocked = true;
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Text("Unlocked", "The Forbidden Forest opens its deeper paths. Hagrid's expeditions are now available."),
                    new Color(80, 120, 80));
                Main.NewText(Text("HagridBriefing", "Hagrid briefs you on safe routes through the deeper forest."),
                    new Color(100, 140, 100));
            }
        }

        public static void StartMission(Player player)
        {
            if (!CanStart(player)) return;
            missionActive = true;
            missionTimer = MISSION_DURATION;
            objectivesCompleted = 0;
            currentLoop = expeditionsCompleted % 4;

            SpawnObjectives(player);

            string startText = currentLoop switch
            {
                0 => Text("UnicornGladeStart", "Unicorn Glade -- cleanse corrupted traces before the dark spreads."),
                1 => Text("SkywatchStart", "Centaur Skywatch -- stabilize omen stones under the night sky."),
                2 => Text("NestStart", "Acromantula Nest -- destroy brood markers before the colony gathers."),
                3 => Text("ThestralStart", "Thestral Clearing -- activate moonlit beacons in the quiet dark."),
                _ => Text("UnicornGladeStart", "Unicorn Glade -- cleanse corrupted traces before the dark spreads."),
            };

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(startText, new Color(80, 140, 80));
                Main.NewText(Text("CompleteObjectives", "Complete {0} objectives before time runs out.",
                    OBJECTIVES_NEEDED), new Color(100, 130, 100));
            }
        }

        public override void PreUpdateWorld()
        {
            if (!missionActive) return;
            missionTimer--;

            // Forest hazards by loop
            if (missionTimer % 1200 == 0 && missionTimer > 0)
            {
                foreach (Player p in Main.player)
                {
                    if (!p.active || p.dead) continue;
                    switch (currentLoop)
                    {
                        case 0: p.AddBuff(BuffID.Darkness, 60); break; // glade: encroaching dark
                        case 1: if (Main.rand.NextBool(2)) p.AddBuff(BuffID.Confused, 60); break; // skywatch: disorienting omens
                        case 2: p.AddBuff(BuffID.Venom, 90); break; // nest: ambient venom
                        case 3: p.AddBuff(BuffID.Slow, 60); p.AddBuff(BuffID.NightOwl, 120); break; // thestral clearing: eerie atmosphere, not hostile
                    }
                }
            }

            if (missionTimer <= 0) { FailMission(); return; }
            if (missionTimer == 60 * 30 && Main.netMode != NetmodeID.Server)
                Main.NewText(Text("TimeWarning", "30 seconds remaining! Hurry!"),
                    new Color(255, 100, 100));
        }

        public static void OnObjectiveCompleted(Player player)
        {
            if (!missionActive) return;
            objectivesCompleted++;
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Text("ObjectiveProgress", "Objectives complete: {0}/{1}",
                    objectivesCompleted, OBJECTIVES_NEEDED), new Color(100, 160, 100));
            if (objectivesCompleted >= OBJECTIVES_NEEDED)
                CompleteMission(player);
        }

        private static void CompleteMission(Player player)
        {
            missionActive = false;
            expeditionsCompleted++;
            CleanupEntities();

            // Loop-specific material
            int materialType = currentLoop switch
            {
                0 => ModContent.ItemType<Content.Items.Consumables.GladeDew>(),
                1 => ModContent.ItemType<Content.Items.Consumables.StarTrace>(),
                2 => ModContent.ItemType<Content.Items.Consumables.BroodVenom>(),
                3 => ModContent.ItemType<Content.Items.Consumables.SpectralEssence>(),
                _ => ModContent.ItemType<Content.Items.Consumables.GladeDew>(),
            };
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), materialType, 4 + expeditionsCompleted);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 10);
            player.AddBuff(ModContent.BuffType<Content.Buffs.ForestWisdomBuff>(), 60 * 60 * 10);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), ItemID.GoldCoin, 2 + expeditionsCompleted);

            string completeText = currentLoop switch
            {
                0 => Text("UnicornGladeComplete", "The glade is quiet again. The corruption recedes."),
                1 => Text("SkywatchComplete", "The omen stones are stable. The centaurs' warnings fade."),
                2 => Text("NestComplete", "The brood markers are destroyed. The colony scatters."),
                3 => Text("ThestralComplete", "The moonlit beacons hold. The clearing feels calm."),
                _ => Text("UnicornGladeComplete", "The glade is quiet again. The corruption recedes."),
            };
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(completeText, new Color(100, 180, 100));

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 15,
                    Text("SourceExpedition", "Forbidden Forest expedition"));
        }

        private static void FailMission()
        {
            missionActive = false;
            CleanupEntities();
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Text("MissionFailed", "The forest overwhelms the expedition. Fall back."),
                    new Color(255, 80, 80));
        }

        private static void SpawnObjectives(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int type = currentLoop switch
            {
                0 => ModContent.NPCType<Content.NPCs.Enemies.CorruptedTrace>(),
                1 => ModContent.NPCType<Content.NPCs.Enemies.OmenStone>(),
                2 => ModContent.NPCType<Content.NPCs.Enemies.BroodMarker>(),
                3 => ModContent.NPCType<Content.NPCs.Enemies.ThestralBeacon>(),
                _ => ModContent.NPCType<Content.NPCs.Enemies.CorruptedTrace>(),
            };
            for (int i = 0; i < OBJECTIVES_NEEDED; i++)
            {
                float x = player.Center.X + Main.rand.Next(-600, 600);
                float y = player.Center.Y + Main.rand.Next(-250, 100);
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, type);
            }
        }

        private static void CleanupEntities()
        {
            int[] types = {
                ModContent.NPCType<Content.NPCs.Enemies.CorruptedTrace>(),
                ModContent.NPCType<Content.NPCs.Enemies.OmenStone>(),
                ModContent.NPCType<Content.NPCs.Enemies.BroodMarker>(),
                ModContent.NPCType<Content.NPCs.Enemies.ThestralBeacon>(),
            };
            foreach (NPC npc in Main.ActiveNPCs)
                foreach (int t in types)
                    if (npc.type == t) npc.active = false;
        }

        public static string GetStatusText()
        {
            if (!expeditionUnlocked)
                return Text("NotUnlocked", "The deeper forest paths are sealed. Defeat the Basilisk first.");
            if (missionActive)
                return Text("ActiveStatus", "Expedition active: {0}/{1} objectives complete, {2} seconds remaining.",
                    objectivesCompleted, OBJECTIVES_NEEDED, missionTimer / 60);
            string nextLoop = (expeditionsCompleted % 4) switch
            {
                0 => "Unicorn Glade", 1 => "Centaur Skywatch", 2 => "Acromantula Nest", 3 => "Thestral Clearing", _ => "Unicorn Glade"
            };
            return Text("ReadyStatus", "Expeditions completed: {0}. Next loop: {1}.",
                expeditionsCompleted, nextLoop);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["fe_unlocked"] = expeditionUnlocked;
            tag["fe_completed"] = expeditionsCompleted;
        }
        public override void LoadWorldData(TagCompound tag)
        {
            expeditionUnlocked = tag.GetBool("fe_unlocked");
            expeditionsCompleted = tag.GetInt("fe_completed");
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(expeditionUnlocked);
            writer.Write(expeditionsCompleted);
        }
        public override void NetReceive(BinaryReader reader)
        {
            expeditionUnlocked = reader.ReadBoolean();
            expeditionsCompleted = reader.ReadInt32();
        }
    }
}
