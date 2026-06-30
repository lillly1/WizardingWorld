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
    /// St Mungo's Triage System -- magical injury treatment missions.
    ///
    /// Three ward types rotate:
    /// Ward 1 (Spell Damage): Stabilize Hex Residue nodes
    /// Ward 2 (Creature Injuries): Treat Venom Wound nodes
    /// Ward 3 (Potion Accidents): Neutralize Cauldron Spill nodes
    ///
    /// Each mission: stabilize 3 nodes within 3 minutes.
    /// Rewards: Healer's Salts material + treatment buff + gold + House Points.
    ///
    /// NOT a resurrection service. NOT a miracle cure. Triage and stabilization.
    /// Mod-original: inspired by St Mungo's Hospital for Magical Maladies.
    /// </summary>
    public class StMungosTriageSystem : ModSystem
    {
        public static bool hospitalUnlocked;
        public static int missionsCompleted;
        public static int currentWardType; // 0=spell, 1=creature, 2=potion

        public static bool missionActive;
        public static int missionTimer;
        public static int nodesStabilized;

        private const int MISSION_DURATION = 60 * 60 * 3;
        private const int NODES_NEEDED = 3;

        private static string Text(string suffix, string fallback, params object[] args) =>
            WizardLocalization.Text($"Mods.WizardingWorld.StMungos.{suffix}", fallback, args);

        public override void ClearWorld()
        {
            hospitalUnlocked = false;
            missionsCompleted = 0;
            currentWardType = 0;
            missionActive = false;
        }

        public static bool CanUnlock() =>
            DownedBossSystem.downedHorntail && !hospitalUnlocked;

        public static bool CanStart() =>
            hospitalUnlocked && !missionActive;

        public static void Unlock()
        {
            if (!CanUnlock()) return;
            hospitalUnlocked = true;
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Text("Unlocked", "St Mungo's Hospital for Magical Maladies and Injuries is now accessible. Speak to a Healer or use the Visitor Pass."),
                    new Color(200, 220, 200));
                Main.NewText(Text("Entrance", "The wards are open. Healers need triage assistance."),
                    new Color(180, 200, 180));
            }
        }

        public static void StartMission(Player player)
        {
            if (!CanStart()) return;
            missionActive = true;
            missionTimer = MISSION_DURATION;
            nodesStabilized = 0;

            // Rotate ward type
            currentWardType = missionsCompleted % 3;

            SpawnWardNodes(player);

            string wardText = currentWardType switch
            {
                0 => Text("SpellWardStart", "Spell Damage Ward -- hex residue nodes are destabilizing. Stabilize them."),
                1 => Text("CreatureWardStart", "Creature Injuries Ward -- venom wound nodes are spreading toxins. Stabilize them."),
                2 => Text("PotionWardStart", "Potion Accidents Ward -- cauldron spill nodes are leaking fumes. Neutralize them."),
                _ => Text("SpellWardStart", "Spell Damage Ward -- hex residue nodes are destabilizing. Stabilize them."),
            };

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(wardText, new Color(200, 220, 200));
                Main.NewText(Text("StabilizeNodes", "Stabilize {0} nodes before time runs out.",
                    NODES_NEEDED), new Color(180, 200, 180));
            }
        }

        public override void PreUpdateWorld()
        {
            if (!missionActive) return;
            missionTimer--;

            // Ward-specific hazards
            if (missionTimer % 900 == 0 && missionTimer > 0)
            {
                foreach (Player p in Main.player)
                {
                    if (!p.active || p.dead) continue;
                    switch (currentWardType)
                    {
                        case 0: // Spell Damage -- mana burn pulses
                            if (Main.rand.NextBool(2))
                                p.AddBuff(BuffID.ManaSickness, 120);
                            break;
                        case 1: // Creature Injuries -- bleed/venom
                            if (Main.rand.NextBool(2))
                                p.AddBuff(BuffID.Venom, 90);
                            break;
                        case 2: // Potion Accidents -- confusion/steam
                            if (Main.rand.NextBool(2))
                                p.AddBuff(BuffID.Confused, 60);
                            break;
                    }
                }
            }

            if (missionTimer <= 0) { FailMission(); return; }
            if (missionTimer == 60 * 30 && Main.netMode != NetmodeID.Server)
                Main.NewText(Text("TimeWarning", "30 seconds remaining! Hurry!"),
                    new Color(255, 100, 100));
        }

        public static void OnNodeStabilized(Player player)
        {
            if (!missionActive) return;
            nodesStabilized++;
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Text("NodeProgress", "Nodes stabilized: {0}/{1}",
                    nodesStabilized, NODES_NEEDED), new Color(180, 220, 180));
            if (nodesStabilized >= NODES_NEEDED)
                CompleteMission(player);
        }

        private static void CompleteMission(Player player)
        {
            missionActive = false;
            missionsCompleted++;
            CleanupEntities();

            int saltsAmount = 4 + missionsCompleted;
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.HealersSalts>(), saltsAmount);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 10);
            player.AddBuff(ModContent.BuffType<Content.Buffs.TriageResolvedBuff>(), 60 * 60 * 10);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), ItemID.GoldCoin, 2 + missionsCompleted);

            string completeText = currentWardType switch
            {
                0 => Text("SpellWardComplete", "Spell Damage Ward stabilized. The hex residue is contained."),
                1 => Text("CreatureWardComplete", "Creature Injuries Ward stabilized. The venom is neutralized."),
                2 => Text("PotionWardComplete", "Potion Accidents Ward stabilized. The fumes have cleared."),
                _ => Text("SpellWardComplete", "Spell Damage Ward stabilized. The hex residue is contained."),
            };

            if (Main.netMode != NetmodeID.Server)
                Main.NewText(completeText, new Color(100, 200, 100));

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 15,
                    Text("SourceTriage", "St Mungo's triage"));
        }

        private static void FailMission()
        {
            missionActive = false;
            CleanupEntities();
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Text("MissionFailed", "The ward is overwhelmed. Triage failed."),
                    new Color(255, 80, 80));
        }

        private static void SpawnWardNodes(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int nodeType = currentWardType switch
            {
                0 => ModContent.NPCType<Content.NPCs.Enemies.HexResidueNode>(),
                1 => ModContent.NPCType<Content.NPCs.Enemies.VenomWoundNode>(),
                2 => ModContent.NPCType<Content.NPCs.Enemies.CauldronSpillNode>(),
                _ => ModContent.NPCType<Content.NPCs.Enemies.HexResidueNode>(),
            };
            for (int i = 0; i < NODES_NEEDED; i++)
            {
                float x = player.Center.X + Main.rand.Next(-500, 500);
                float y = player.Center.Y + Main.rand.Next(-200, 100);
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, nodeType);
            }
        }

        private static void CleanupEntities()
        {
            int[] types = {
                ModContent.NPCType<Content.NPCs.Enemies.HexResidueNode>(),
                ModContent.NPCType<Content.NPCs.Enemies.VenomWoundNode>(),
                ModContent.NPCType<Content.NPCs.Enemies.CauldronSpillNode>(),
            };
            foreach (NPC npc in Main.ActiveNPCs)
                foreach (int t in types)
                    if (npc.type == t) npc.active = false;
        }

        public static string GetStatusText()
        {
            if (!hospitalUnlocked)
                return Text("NotUnlocked", "St Mungo's access requires defeating the Hungarian Horntail first.");
            if (missionActive)
                return Text("ActiveStatus", "Triage active: {0}/{1} nodes stabilized, {2} seconds remaining.",
                    nodesStabilized, NODES_NEEDED, missionTimer / 60);
            string nextWard = (missionsCompleted % 3) switch
            {
                0 => "Spell Damage", 1 => "Creature Injuries", 2 => "Potion Accidents", _ => "Spell Damage"
            };
            return Text("ReadyStatus", "Missions completed: {0}. Next ward: {1}.",
                missionsCompleted, nextWard);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["sm_unlocked"] = hospitalUnlocked;
            tag["sm_missions"] = missionsCompleted;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            hospitalUnlocked = tag.GetBool("sm_unlocked");
            missionsCompleted = tag.GetInt("sm_missions");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(hospitalUnlocked);
            writer.Write(missionsCompleted);
        }

        public override void NetReceive(BinaryReader reader)
        {
            hospitalUnlocked = reader.ReadBoolean();
            missionsCompleted = reader.ReadInt32();
        }
    }
}
