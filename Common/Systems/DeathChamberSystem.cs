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
    /// Death Chamber / Veil System -- Department of Mysteries survival mission.
    ///
    /// Objective: Seal 3 Veil Fractures near the Veil while surviving pull forces and despair.
    /// The Veil is central, dangerous, and NOT a resurrection device.
    /// Hazards: pull toward Veil, heavy despair, ghostly Veil Echoes, blackout pulses.
    /// Reward: Veil Thread material + protective buff.
    ///
    /// Unlocked after completing at least one Time Chamber mission.
    /// Mod-original: inspired by the Death Chamber from Order of the Phoenix.
    /// </summary>
    public class DeathChamberSystem : ModSystem
    {
        public static bool chamberUnlocked;
        public static int runsCompleted;

        public static bool missionActive;
        public static int missionTimer;
        public static int fracturesSealed;

        // Veil pull anchor
        public static Vector2 veilPosition;

        private const int MISSION_DURATION = 60 * 60 * 3; // 3 minutes
        private const int FRACTURES_NEEDED = 3;

        public override void ClearWorld()
        {
            chamberUnlocked = false;
            runsCompleted = 0;
            missionActive = false;
        }

        public static bool CanUnlock() =>
            TimeChamberSystem.runsCompleted >= 1 && !chamberUnlocked;

        public static bool CanStart() =>
            chamberUnlocked && !missionActive &&
            !ProphecyMissionSystem.missionActive &&
            !TimeChamberSystem.missionActive;

        public static void Unlock()
        {
            if (!CanUnlock()) return;
            chamberUnlocked = true;
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.DeathChamberUnlocked"),
                    new Color(140, 120, 160));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.VeilWarning"),
                    new Color(180, 150, 180));
            }
        }

        public static void StartMission(Player player)
        {
            if (!CanStart()) return;
            missionActive = true;
            missionTimer = MISSION_DURATION;
            fracturesSealed = 0;
            veilPosition = player.Center + new Vector2(0, -100);

            SpawnVeilFractures(player);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.DeathChamberStart"),
                    new Color(140, 120, 180));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.SealFractures",
                    FRACTURES_NEEDED), new Color(180, 160, 200));
            }
        }

        public override void PreUpdateWorld()
        {
            if (!missionActive) return;
            missionTimer--;

            // Veil pull force + despair on all players
            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead) continue;

                float dist = Vector2.Distance(player.Center, veilPosition);
                if (dist < 600f)
                {
                    // Pull toward Veil
                    Vector2 pull = (veilPosition - player.Center).SafeNormalize(Vector2.Zero);
                    float strength = (1f - dist / 600f) * 1.5f;
                    player.velocity += pull * strength * 0.02f;

                    // Despair buildup near the Veil
                    player.GetModPlayer<Players.WizardPlayer>().AddDespair(
                        0.02f * (1f - dist / 600f), "the Veil's whispers");
                }

                // Periodic blackout pulses
                if (missionTimer % 360 == 0)
                    player.AddBuff(BuffID.Darkness, 180);
            }

            // Veil visual: dark dust cloud at anchor
            if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(veilPosition - new Vector2(20, 30), 40, 60,
                    DustID.Shadowflame, 0f, -1f, 100, default, 1.5f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }

            if (missionTimer <= 0) { FailMission(); return; }
            if (missionTimer == 60 * 30 && Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.TimeWarning"),
                    new Color(255, 100, 100));
        }

        public static void OnFractureSealed(Player player)
        {
            if (!missionActive) return;
            fracturesSealed++;
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.FractureProgress",
                    fracturesSealed, FRACTURES_NEEDED), new Color(180, 160, 200));
            if (fracturesSealed >= FRACTURES_NEEDED)
                CompleteMission(player);
        }

        private static void CompleteMission(Player player)
        {
            missionActive = false;
            runsCompleted++;
            CleanupEntities();

            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.VeilThread>(), 3 + runsCompleted);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 15);
            player.AddBuff(ModContent.BuffType<Content.Buffs.VeilWardBuff>(), 60 * 60 * 10);

            // Relieve despair as reward for surviving
            player.GetModPlayer<Players.WizardPlayer>().RelieveDespair(0.20f);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.DeathChamberComplete"),
                    new Color(180, 200, 255));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.VeilSealed"),
                    new Color(200, 180, 220));
            }

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 30,
                    Language.GetTextValue("Mods.WizardingWorld.Ministry.SourceDeathChamber"));
        }

        private static void FailMission()
        {
            missionActive = false;
            CleanupEntities();
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Ministry.DeathChamberFailed"),
                    new Color(255, 80, 80));
        }

        private static void SpawnVeilFractures(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int type = ModContent.NPCType<Content.NPCs.Enemies.VeilFracture>();
            for (int i = 0; i < FRACTURES_NEEDED; i++)
            {
                float angle = i * (System.MathF.PI * 2f / FRACTURES_NEEDED);
                float x = veilPosition.X + System.MathF.Cos(angle) * 250f;
                float y = veilPosition.Y + System.MathF.Sin(angle) * 150f;
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, type);
            }
        }

        private static void CleanupEntities()
        {
            int type = ModContent.NPCType<Content.NPCs.Enemies.VeilFracture>();
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.type == type) npc.active = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["dc_unlocked"] = chamberUnlocked;
            tag["dc_runs"] = runsCompleted;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            chamberUnlocked = tag.GetBool("dc_unlocked");
            runsCompleted = tag.GetInt("dc_runs");
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
