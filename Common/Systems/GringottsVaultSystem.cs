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
    /// Gringotts Vault System -- sanctioned vault retrieval missions.
    ///
    /// The player receives a goblin-issued recovery contract to retrieve
    /// cursed or misplaced items from Gringotts vaults.
    /// NOT framed as robbery -- this is authorized bank work.
    ///
    /// Objective: Collect 4 Vault Relics while avoiding security jinxes.
    /// Hazards: Anti-theft jinx bursts, cursed treasure decoys, vault seals.
    /// Reward: Galleon Dust (crafting material) + vault completion buff.
    ///
    /// Mod-original: inspired by Gringotts' high-security vault architecture.
    /// </summary>
    public class GringottsVaultSystem : ModSystem
    {
        public static bool vaultAccessUnlocked;
        public static int runsCompleted;

        public static bool missionActive;
        public static int missionTimer;
        public static int relicsCollected;

        private const int MISSION_DURATION = 60 * 60 * 3; // 3 minutes
        private const int RELICS_NEEDED = 4;

        public override void ClearWorld()
        {
            vaultAccessUnlocked = false;
            runsCompleted = 0;
            missionActive = false;
        }

        public static bool CanUnlock()
        {
            return DownedBossSystem.downedBasilisk && !vaultAccessUnlocked;
        }

        public static bool CanStart()
        {
            return vaultAccessUnlocked && !missionActive;
        }

        public static void Unlock()
        {
            if (!CanUnlock()) return;
            vaultAccessUnlocked = true;
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Gringotts.Unlocked"),
                    new Color(200, 180, 100));
            }
        }

        public static void StartMission(Player player)
        {
            if (!CanStart()) return;
            missionActive = true;
            missionTimer = MISSION_DURATION;
            relicsCollected = 0;

            // Gringotts cart descent framing
            DiagonAlleySystem.CartDescentFraming(player);

            SpawnVaultRelics(player);
            SpawnSecurityHazards(player);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Gringotts.MissionStart"),
                    new Color(200, 180, 100));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Gringotts.CollectRelics",
                    RELICS_NEEDED), new Color(220, 200, 120));
            }
        }

        public override void PreUpdateWorld()
        {
            if (!missionActive) return;
            missionTimer--;

            // Anti-theft jinx bursts every 15 seconds
            if (missionTimer % 900 == 0 && missionTimer > 0)
            {
                foreach (Player p in Main.player)
                {
                    if (!p.active || p.dead) continue;
                    // Random jinx damage burst near player
                    if (Main.rand.NextBool(2))
                    {
                        p.AddBuff(BuffID.Confused, 60); // 1 second confusion
                        if (Main.netMode != NetmodeID.Server)
                            Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Gringotts.JinxWarning"),
                                new Color(255, 150, 50));
                    }
                }
            }

            if (missionTimer <= 0) { FailMission(); return; }
            if (missionTimer == 60 * 30 && Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Gringotts.TimeWarning"),
                    new Color(255, 100, 100));
        }

        public static void OnRelicCollected(Player player)
        {
            if (!missionActive) return;
            relicsCollected++;
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Gringotts.RelicProgress",
                    relicsCollected, RELICS_NEEDED), new Color(220, 200, 120));
            if (relicsCollected >= RELICS_NEEDED)
                CompleteMission(player);
        }

        private static void CompleteMission(Player player)
        {
            missionActive = false;
            runsCompleted++;
            CleanupEntities();

            int dustAmount = 6 + runsCompleted * 2;
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.GalleonDust>(), dustAmount);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 12);
            player.AddBuff(ModContent.BuffType<Content.Buffs.VaultFortuneBuff>(), 60 * 60 * 8);

            // Gold coin bonus
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), ItemID.GoldCoin,
                3 + runsCompleted);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Gringotts.MissionComplete"),
                    new Color(220, 200, 100));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Gringotts.GoblinThanks"),
                    new Color(180, 160, 100));
            }

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 15,
                    Language.GetTextValue("Mods.WizardingWorld.Gringotts.SourceVault"));
        }

        private static void FailMission()
        {
            missionActive = false;
            CleanupEntities();
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Gringotts.MissionFailed"),
                    new Color(255, 80, 80));
        }

        private static void SpawnVaultRelics(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int type = ModContent.NPCType<Content.NPCs.Enemies.VaultRelic>();
            for (int i = 0; i < RELICS_NEEDED; i++)
            {
                float x = player.Center.X + Main.rand.Next(-500, 500);
                float y = player.Center.Y + Main.rand.Next(-150, 150);
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, type);
            }
        }

        private static void SpawnSecurityHazards(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int type = ModContent.NPCType<Content.NPCs.Enemies.CursedTreasure>();
            for (int i = 0; i < 2; i++)
            {
                float x = player.Center.X + Main.rand.Next(-400, 400);
                float y = player.Center.Y + Main.rand.Next(-100, 100);
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, type);
            }
        }

        private static void CleanupEntities()
        {
            int relicType = ModContent.NPCType<Content.NPCs.Enemies.VaultRelic>();
            int treasureType = ModContent.NPCType<Content.NPCs.Enemies.CursedTreasure>();
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.type == relicType || npc.type == treasureType)
                    npc.active = false;
        }

        public static string GetStatusText()
        {
            if (!vaultAccessUnlocked)
                return Language.GetTextValue("Mods.WizardingWorld.Gringotts.NotUnlocked");
            if (missionActive)
                return Language.GetTextValue("Mods.WizardingWorld.Gringotts.ActiveStatus",
                    relicsCollected, RELICS_NEEDED, missionTimer / 60);
            return Language.GetTextValue("Mods.WizardingWorld.Gringotts.ReadyStatus", runsCompleted);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["gv_unlocked"] = vaultAccessUnlocked;
            tag["gv_runs"] = runsCompleted;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            vaultAccessUnlocked = tag.GetBool("gv_unlocked");
            runsCompleted = tag.GetInt("gv_runs");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(vaultAccessUnlocked);
            writer.Write(runsCompleted);
        }

        public override void NetReceive(BinaryReader reader)
        {
            vaultAccessUnlocked = reader.ReadBoolean();
            runsCompleted = reader.ReadInt32();
        }
    }
}
