using System.IO;
using Microsoft.Xna.Framework;
using Terraria; using Terraria.ID; using Terraria.Localization; using Terraria.ModLoader; using Terraria.ModLoader.IO;

namespace WizardingWorld.Common.Systems
{
    /// <summary>
    /// Battle of Hogwarts -- multi-phase castle siege event.
    /// Phase 1: Muster (DA Galleon call, resistance check)
    /// Phase 2: Castle Defense (protect ward anchors, Death Eater waves)
    /// Phase 3: Breach/Chaos (heavy assault, darkness, Hog's Head lifeline)
    /// Phase 4: Resolution (Nagini/Horcrux windows, Voldemort confrontation unlock)
    ///
    /// NOT a generic invasion clone. Objective-focused protective defense.
    /// Mod-original: inspired by the Battle of Hogwarts from Deathly Hallows.
    /// </summary>
    public class BattleOfHogwartsSystem : ModSystem
    {
        public static bool battleUnlocked;
        public static int battlesWon;
        public static bool battleActive;
        public static int battlePhase; // 0=muster, 1=defense, 2=breach, 3=resolution
        public static int phaseTimer;
        public static int objectivesCompleted;
        private static int siegeFlavorTimer;

        private const int DEFENSE_DURATION = 60 * 60 * 3; // 3 min per phase
        private const int DEFENSE_OBJECTIVES = 4;
        private const int BREACH_OBJECTIVES = 3;
        private const int SIEGE_FLAVOR_INTERVAL = 1800; // ~30 seconds

        public override void ClearWorld()
        {
            battleUnlocked = false; battlesWon = 0;
            battleActive = false; battlePhase = 0; siegeFlavorTimer = 0;
        }

        public static bool CanUnlock() =>
            HorcruxHuntSystem.AllCoreHorcruxesDestroyed &&
            HogwartsWardSystem.wardsDefended >= 1 &&
            DownedBossSystem.downedDementorKing &&
            !battleUnlocked;

        public static bool CanStart() =>
            battleUnlocked && !battleActive && !Main.dayTime;

        public static void Unlock()
        {
            if (!CanUnlock()) return;
            battleUnlocked = true;
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.Unlocked"), new Color(200, 180, 140));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.Preparation"), new Color(180, 160, 120));
            }
        }

        public static void StartBattle(Player player)
        {
            if (!CanStart()) return;
            battleActive = true; battlePhase = 1; phaseTimer = DEFENSE_DURATION; objectivesCompleted = 0;
            siegeFlavorTimer = SIEGE_FLAVOR_INTERVAL;
            SpawnDefenseObjectives(player);

            // Castle defense buff -- animated armor and statues fighting alongside the player
            player.AddBuff(ModContent.BuffType<Content.Buffs.CastleDefenseBuff>(), DEFENSE_DURATION * 2);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BattleEndgame.McGonagallRally"), new Color(180, 200, 255));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BattleEndgame.CastleDefenders"), new Color(160, 180, 200));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.Phase1Start"), new Color(200, 100, 80));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.DefendCastle", DEFENSE_OBJECTIVES), new Color(180, 120, 100));
            }
        }

        public override void PreUpdateWorld()
        {
            if (!battleActive) return;
            phaseTimer--;
            siegeFlavorTimer--;

            // Death Eater waves during defense/breach
            if ((battlePhase == 1 || battlePhase == 2) && phaseTimer % 600 == 0 && phaseTimer > 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Player target = Main.player[Player.FindClosest(new Vector2(Main.maxTilesX * 8, Main.maxTilesY * 4), 1, 1)];
                if (target != null && target.active)
                {
                    int deType = ModContent.NPCType<Content.NPCs.Enemies.DeathEater>();
                    int count = battlePhase == 2 ? 3 : 2;
                    for (int i = 0; i < count; i++)
                        NPC.NewNPC(target.GetSource_FromThis(), (int)(target.Center.X + Main.rand.Next(-400, 400)), (int)(target.Center.Y - 200), deType);
                }
            }

            // Breach-phase despair/darkness
            if (battlePhase == 2 && phaseTimer % 900 == 0)
            {
                foreach (Player p in Main.player)
                {
                    if (!p.active || p.dead) continue;
                    p.AddBuff(BuffID.Darkness, 180);
                    p.GetModPlayer<Players.WizardPlayer>().AddDespair(0.03f, "the castle breach");
                }
            }

            // Siege flavor messages every ~30 seconds during active phases
            if (siegeFlavorTimer <= 0 && (battlePhase == 1 || battlePhase == 2) && Main.netMode != NetmodeID.Server)
            {
                siegeFlavorTimer = SIEGE_FLAVOR_INTERVAL;
                string key = Main.rand.NextBool()
                    ? "Mods.WizardingWorld.BattleEndgame.SiegeIntensity"
                    : "Mods.WizardingWorld.BattleEndgame.DefenderFlavor";
                Main.NewText(Language.GetTextValue(key), new Color(170, 160, 180));
            }

            // Animated armor dust near ward nodes -- stone-colored particles suggesting castle constructs
            if (Main.netMode != NetmodeID.Server && phaseTimer % 40 == 0)
            {
                int wardType = ModContent.NPCType<Content.NPCs.Enemies.CastleWardNode>();
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.type != wardType || !npc.active) continue;
                    Dust d = Dust.NewDustDirect(
                        npc.position + new Vector2(Main.rand.Next(-24, 24), Main.rand.Next(-16, 16)),
                        4, 4, DustID.Stone, 0f, -0.4f, 120, default, 0.7f);
                    d.noGravity = true;
                    d.velocity *= 0.2f;
                }
            }

            if (phaseTimer <= 0) AdvanceOrFail();
            if (phaseTimer == 60 * 30 && Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.TimeWarning"), new Color(255, 100, 100));
        }

        public static void OnObjectiveCompleted(Player player)
        {
            if (!battleActive) return;
            objectivesCompleted++;
            int needed = battlePhase == 1 ? DEFENSE_OBJECTIVES : BREACH_OBJECTIVES;
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.ObjectiveProgress", objectivesCompleted, needed), new Color(180, 200, 180));
            if (objectivesCompleted >= needed) AdvancePhase(player);
        }

        private static void AdvancePhase(Player player)
        {
            CleanupEntities();
            if (battlePhase == 1)
            {
                battlePhase = 2; phaseTimer = DEFENSE_DURATION; objectivesCompleted = 0;
                SpawnBreachObjectives(player);
                if (Main.netMode != NetmodeID.Server)
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.Phase2Start"), new Color(200, 100, 80));
            }
            else if (battlePhase == 2)
            {
                battlePhase = 3; // Resolution
                CompleteBattle(player);
            }
        }

        private static void AdvanceOrFail()
        {
            battleActive = false; CleanupEntities(); RemoveCastleDefenseBuff();
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.Failed"), new Color(255, 80, 80));
        }

        private static void CompleteBattle(Player player)
        {
            battleActive = false; battlesWon++; CleanupEntities(); RemoveCastleDefenseBuff();
            int matAmount = 5 + battlesWon * 2;
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), ModContent.ItemType<Content.Items.Consumables.CastleDefenseRune>(), matAmount);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 25);
            player.AddBuff(ModContent.BuffType<Content.Buffs.CastleVictoryBuff>(), 60 * 60 * 15);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), ItemID.GoldCoin, 5 + battlesWon * 2);

            // Dramatic Nagini resolution subphase -- the final Horcrux
            if (!HorcruxHuntSystem.naginiDefeated)
            {
                // Dramatic flavor text heralding the encounter
                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BattleEndgame.NaginiPhase"), new Color(200, 80, 80));
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BattleEndgame.NaginiStrike"), new Color(255, 200, 100));
                }

                // CursedTorch dust -- sickly green soul-fragment energy (Horcrux destruction visual)
                for (int i = 0; i < 35; i++)
                {
                    Dust dust = Dust.NewDustDirect(player.Center, 8, 8, DustID.CursedTorch, 0f, -2f, 50, default, 1.6f);
                    dust.velocity = Main.rand.NextVector2Circular(5f, 7f);
                    dust.velocity.Y -= 3f;
                    dust.noGravity = true;
                }

                // Shadowflame burst -- dark energy dissipating as the Horcrux is destroyed
                for (int i = 0; i < 40; i++)
                {
                    Dust dust = Dust.NewDustDirect(player.Center, 8, 8, DustID.Shadowflame, 0f, 0f, 50, default, 1.8f);
                    dust.velocity = Main.rand.NextVector2Circular(7f, 7f);
                    dust.noGravity = true;
                }

                // Now mark Nagini defeated and announce
                HorcruxHuntSystem.naginiDefeated = true;
                if (Main.netMode != NetmodeID.Server)
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.NaginiSlain"), new Color(255, 215, 0));
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.Victory"), new Color(100, 200, 100));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.VoldemortReady"), new Color(255, 215, 100));
            }

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 50, Language.GetTextValue("Mods.WizardingWorld.Battle.SourceBattle"));
        }

        private static void SpawnDefenseObjectives(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int type = ModContent.NPCType<Content.NPCs.Enemies.CastleWardNode>();
            for (int i = 0; i < DEFENSE_OBJECTIVES; i++)
            {
                float x = player.Center.X + Main.rand.Next(-600, 600);
                float y = player.Center.Y + Main.rand.Next(-200, 100);
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, type);
            }
        }

        private static void SpawnBreachObjectives(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int type = ModContent.NPCType<Content.NPCs.Enemies.BreachSeal>();
            for (int i = 0; i < BREACH_OBJECTIVES; i++)
            {
                float x = player.Center.X + Main.rand.Next(-500, 500);
                float y = player.Center.Y + Main.rand.Next(-150, 100);
                NPC.NewNPC(player.GetSource_FromThis(), (int)x, (int)y, type);
            }
        }

        private static void CleanupEntities()
        {
            int[] types = { ModContent.NPCType<Content.NPCs.Enemies.CastleWardNode>(), ModContent.NPCType<Content.NPCs.Enemies.BreachSeal>() };
            foreach (NPC npc in Main.ActiveNPCs) foreach (int t in types) if (npc.type == t) npc.active = false;
        }

        private static void RemoveCastleDefenseBuff()
        {
            int buffType = ModContent.BuffType<Content.Buffs.CastleDefenseBuff>();
            foreach (Player p in Main.player)
            {
                if (!p.active) continue;
                p.ClearBuff(buffType);
            }
        }

        public static string GetStatusText()
        {
            if (!battleUnlocked) return Language.GetTextValue("Mods.WizardingWorld.Battle.NotUnlocked");
            if (battleActive) return Language.GetTextValue("Mods.WizardingWorld.Battle.ActiveStatus", battlePhase, objectivesCompleted, phaseTimer / 60);
            return Language.GetTextValue("Mods.WizardingWorld.Battle.ReadyStatus", battlesWon);
        }

        public override void SaveWorldData(TagCompound tag) { tag["bh_unlocked"] = battleUnlocked; tag["bh_won"] = battlesWon; }
        public override void LoadWorldData(TagCompound tag) { battleUnlocked = tag.GetBool("bh_unlocked"); battlesWon = tag.GetInt("bh_won"); }
        public override void NetSend(BinaryWriter writer) { writer.Write(battleUnlocked); writer.Write(battlesWon); }
        public override void NetReceive(BinaryReader reader) { battleUnlocked = reader.ReadBoolean(); battlesWon = reader.ReadInt32(); }
    }
}
