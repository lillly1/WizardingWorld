using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace WizardingWorld.Common.Systems
{
    /// <summary>
    /// Azkaban's Despair — a late-game event where Dementors escape Azkaban en masse.
    ///
    /// Triggers: Post-Golem, night, via "Azkaban Breach Stone" item or random chance (1/15 per night).
    ///
    /// Mechanics:
    /// - Light suppression: torch/glowstick effectiveness reduced
    /// - Despair debuff: players within the event zone lose life regen unless Patronus is active
    /// - Dementor swarm waves with escalating difficulty
    /// - AzkabanGuard mini-bosses in later waves
    /// - Patronus counterplay: +30% damage to event enemies if Patronus active
    ///
    /// Rewards:
    /// - Dementor's Shroud (crafting material)
    /// - Azkaban Warden's Key (new accessory)
    /// - Essence of Magic
    /// - Patronus-boosting items
    ///
    /// Canon-faithful: Dementors are Azkaban's guards who can escape during dark times.
    /// </summary>
    public class AzkabanDespairEvent : ModSystem
    {
        public static bool eventActive;
        public static int eventProgress;
        public static int eventProgressMax;
        public static int waveTimer;
        public static int anchorsDestroyed;

        private const int WAVE_INTERVAL = 600; // 10 seconds between escalation checks

        public override void PreUpdateWorld()
        {
            if (!eventActive) return;

            // Event only runs at night
            if (Main.dayTime)
            {
                EndEvent();
                return;
            }

            // Progress check
            if (eventProgress >= eventProgressMax)
            {
                EndEvent();
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Azkaban.EventEnd", anchorsDestroyed), new Color(100, 180, 255));
                return;
            }

            // Despair effect on all players
            waveTimer++;
            if (waveTimer % 60 == 0) // every second
            {
                foreach (Player player in Main.player)
                {
                    if (!player.active || player.dead) continue;

                    var wp = player.GetModPlayer<Players.WizardPlayer>();
                    wp.AddDespair(0.030f, "Azkaban's despair");

                    if (wp.patronusActive)
                    {
                        wp.RelieveDespair(0.020f);
                        player.statDefense += 4;
                    }

                    // Despair: suppress life regen unless Patronus is active
                    if (!wp.patronusActive)
                    {
                        player.lifeRegen -= 4; // reduced healing

                        // Apply Darkness periodically
                        if (waveTimer % 300 == 0)
                            player.AddBuff(BuffID.Darkness, 300);
                    }
                }
            }
        }

        public static void StartEvent()
        {
            if (eventActive) return;
            if (!NPC.downedGolemBoss) return; // post-Golem requirement

            eventActive = true;
            eventProgress = 0;
            int activePlayers = 0;
            foreach (Player player in Main.player)
            {
                if (player.active)
                    activePlayers++;
            }
            eventProgressMax = 100 + (activePlayers * 25);
            waveTimer = 0;
            anchorsDestroyed = 0;

            Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Azkaban.EventStart1"), new Color(80, 0, 120));
            Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Azkaban.EventStart2"), new Color(180, 220, 255));

            // Suppress ambient light
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.WorldData);
        }

        public static void EndEvent()
        {
            foreach (Player p in Main.player)
            {
                if (!p.active) continue;
                var wp = p.GetModPlayer<Players.WizardPlayer>();
                if (wp.houseSet > 0)
                    GreatHallSystem.AwardHousePoints(wp.houseSet, 30, Language.GetTextValue("Mods.WizardingWorld.GreatHall.SourceAzkaban"));
            }

            eventActive = false;
            eventProgress = 0;
            waveTimer = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.WorldData);
        }

        public static void OnAnchorDestroyed()
        {
            if (!eventActive) return;
            anchorsDestroyed++;
            eventProgress += 10; // Anchors are worth 10 progress (vs 2 for Dementors)
        }

        public static void OnEnemyKilled(NPC npc)
        {
            if (!eventActive) return;

            // Count event-relevant kills
            if (npc.ModNPC is Content.NPCs.Enemies.Dementor)
                eventProgress += 2;
            else if (npc.ModNPC is Content.NPCs.Enemies.AzkabanGuard)
                eventProgress += 5;
            else if (npc.type == NPCID.Wraith || npc.type == NPCID.Reaper)
                eventProgress += 1; // vanilla dark enemies count too

            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead)
                    continue;

                var wp = player.GetModPlayer<Players.WizardPlayer>();
                if (npc.ModNPC is Content.NPCs.Enemies.AzkabanGuard)
                    wp.RelieveDespair(0.05f);
                else if (npc.ModNPC is Content.NPCs.Enemies.Dementor)
                    wp.RelieveDespair(0.025f);
            }
        }

        public override void ModifyLightingBrightness(ref float scale)
        {
            if (eventActive)
                scale *= 0.6f; // 40% light reduction during event
        }

        public override void SaveWorldData(TagCompound tag)
        {
            // Don't persist active events across sessions
        }

        public override void LoadWorldData(TagCompound tag)
        {
            eventActive = false;
            eventProgress = 0;
        }
    }
}
