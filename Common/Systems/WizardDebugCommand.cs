using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
    /// <summary>
    /// Developer diagnostics — /wwdebug command.
    /// Reports real runtime state for world progression, Hallows, Horcruxes, mastery, biomes, and bosses.
    /// </summary>
    public class WizardDebugCommand : ModCommand
    {
        public override string Command => "wwdebug";
        public override string Usage => "/wwdebug <summary|hallows|horcruxes|mastery|bosses|biome>";
        public override string Description => "Wizarding World developer diagnostics";
        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0 || args[0] == "summary")
            {
                PrintSummary(caller);
                return;
            }

            switch (args[0].ToLower())
            {
                case "hallows": PrintHallows(caller); break;
                case "horcruxes": PrintHorcruxes(caller); break;
                case "mastery": PrintMastery(caller); break;
                case "bosses": PrintBosses(caller); break;
                case "biome": PrintBiome(caller); break;
                default:
                    caller.Reply("Unknown subcommand. Use: summary, hallows, horcruxes, mastery, bosses, biome", Color.Red);
                    break;
            }
        }

        private void PrintSummary(CommandCaller caller)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Wizarding World Debug Summary]");

            // Boss progress
            int bossCount = 0;
            if (DownedBossSystem.downedTroll) bossCount++;
            if (DownedBossSystem.downedQuirrell) bossCount++;
            if (DownedBossSystem.downedBasilisk) bossCount++;
            if (DownedBossSystem.downedAragog) bossCount++;
            if (DownedBossSystem.downedFluffy) bossCount++;
            if (DownedBossSystem.downedHorntail) bossCount++;
            if (DownedBossSystem.downedUmbridge) bossCount++;
            if (DownedBossSystem.downedFenrir) bossCount++;
            if (DownedBossSystem.downedBellatrix) bossCount++;
            if (DownedBossSystem.downedBartyCrouch) bossCount++;
            if (DownedBossSystem.downedDementorKing) bossCount++;
            if (DownedBossSystem.downedVoldemort) bossCount++;
            sb.AppendLine($"  Bosses defeated: {bossCount}/12");

            // Horcruxes
            sb.AppendLine($"  Horcruxes destroyed: {HorcruxHuntSystem.horcruxesDestroyed}/4");
            sb.AppendLine($"  Nagini defeated: {HorcruxHuntSystem.naginiDefeated}");
            sb.AppendLine($"  Voldemort power: {(int)(HorcruxHuntSystem.GetVoldemortPowerMultiplier() * 100)}%");

            // Hallows
            sb.AppendLine($"  Cloak claimed: {HallowsSystem.invisibilityCloakClaimed}");
            sb.AppendLine($"  Stone awakened: {HallowsSystem.resurrectionStoneAwakened}");
            sb.AppendLine($"  Hallows attuned: {HallowsSystem.hallowsAttuned}");

            // Player state
            var wp = caller.Player.GetModPlayer<Players.WizardPlayer>();
            sb.AppendLine($"  House set: {wp.houseSet}");
            sb.AppendLine($"  Despair: {wp.despair:F2}");
            sb.AppendLine($"  Patronus active: {wp.patronusActive}");
            sb.AppendLine($"  Has Deathly Hallows: {wp.hasDeathlyHallows}");

            caller.Reply(sb.ToString(), Color.Gold);
        }

        private void PrintHallows(CommandCaller caller)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Deathly Hallows State]");
            sb.AppendLine($"  Cloak claimed from Dumbledore: {HallowsSystem.invisibilityCloakClaimed}");
            sb.AppendLine($"  Resurrection Stone awakened: {HallowsSystem.resurrectionStoneAwakened}");
            sb.AppendLine($"  Master of Death attuned: {HallowsSystem.hallowsAttuned}");

            var wp = caller.Player.GetModPlayer<Players.WizardPlayer>();
            sb.AppendLine($"  [Player] hasElderWand: {wp.hasElderWand}");
            sb.AppendLine($"  [Player] hasInvisibilityCloak: {wp.hasInvisibilityCloak}");
            sb.AppendLine($"  [Player] hasResurrectionStone: {wp.hasResurrectionStone}");
            sb.AppendLine($"  [Player] hasGauntsRing: {wp.hasGauntsRing}");
            sb.AppendLine($"  [Player] hasDemiguiseCloak: {wp.hasDemiguiseCloak}");
            sb.AppendLine($"  [Player] hasDeathlyHallows: {wp.hasDeathlyHallows}");

            sb.AppendLine($"  Can claim cloak: {HallowsSystem.CanClaimInvisibilityCloak}");
            sb.AppendLine($"  Can purify ring: {HallowsSystem.CanPurifyGauntsRing(caller.Player)}");

            string guidance = HallowsSystem.GetDumbledoreGuidance(caller.Player);
            sb.AppendLine($"  Dumbledore says: \"{guidance}\"");

            caller.Reply(sb.ToString(), new Color(210, 210, 255));
        }

        private void PrintHorcruxes(CommandCaller caller)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Horcrux Hunt State]");
            sb.AppendLine($"  Diary destroyed: {HorcruxHuntSystem.diaryDestroyed}");
            sb.AppendLine($"  Locket destroyed: {HorcruxHuntSystem.locketDestroyed}");
            sb.AppendLine($"  Cup destroyed: {HorcruxHuntSystem.cupDestroyed}");
            sb.AppendLine($"  Diadem destroyed: {HorcruxHuntSystem.diademDestroyed}");
            sb.AppendLine($"  All core destroyed: {HorcruxHuntSystem.AllCoreHorcruxesDestroyed}");
            sb.AppendLine($"  Nagini defeated: {HorcruxHuntSystem.naginiDefeated}");
            sb.AppendLine($"  Total destroyed: {HorcruxHuntSystem.horcruxesDestroyed}/4");
            sb.AppendLine($"  Voldemort power multiplier: {HorcruxHuntSystem.GetVoldemortPowerMultiplier():F2}");

            int prep = HorcruxHuntSystem.GetPreparationScore();
            sb.AppendLine($"  Preparation score: {prep}");

            caller.Reply(sb.ToString(), new Color(255, 100, 100));
        }

        private void PrintMastery(CommandCaller caller)
        {
            var wp = caller.Player.GetModPlayer<Players.WizardPlayer>();
            var sb = new StringBuilder();
            sb.AppendLine("[Wand Mastery Progress]");

            if (wp.wandMasteryXP == null || wp.wandMasteryXP.Count == 0)
            {
                sb.AppendLine("  No wand mastery progress yet.");
            }
            else
            {
                foreach (var kvp in wp.wandMasteryXP)
                {
                    int level = wp.GetWandMasteryLevel(kvp.Key);
                    string levelName = level switch
                    {
                        0 => "New",
                        1 => "Familiar",
                        2 => "Attuned",
                        3 => "Mastered",
                        _ => "Unknown"
                    };
                    // Try to get item name
                    string itemName = $"ItemType#{kvp.Key}";
                    try
                    {
                        var item = new Item();
                        item.SetDefaults(kvp.Key);
                        if (!string.IsNullOrEmpty(item.Name))
                            itemName = item.Name;
                    }
                    catch { }
                    sb.AppendLine($"  {itemName}: {kvp.Value} XP (Level {level} - {levelName})");
                }
            }

            caller.Reply(sb.ToString(), new Color(255, 215, 0));
        }

        private void PrintBosses(CommandCaller caller)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Boss Defeat Flags]");
            sb.AppendLine($"  1. Mountain Troll: {DownedBossSystem.downedTroll}");
            sb.AppendLine($"  2. Quirrell: {DownedBossSystem.downedQuirrell}");
            sb.AppendLine($"  3. Basilisk: {DownedBossSystem.downedBasilisk}");
            sb.AppendLine($"  4. Aragog: {DownedBossSystem.downedAragog}");
            sb.AppendLine($"  5. Fluffy: {DownedBossSystem.downedFluffy}");
            sb.AppendLine($"  6. Horntail: {DownedBossSystem.downedHorntail}");
            sb.AppendLine($"  7. Umbridge: {DownedBossSystem.downedUmbridge}");
            sb.AppendLine($"  8. Fenrir: {DownedBossSystem.downedFenrir}");
            sb.AppendLine($"  9. Bellatrix: {DownedBossSystem.downedBellatrix}");
            sb.AppendLine($"  10. Barty Crouch: {DownedBossSystem.downedBartyCrouch}");
            sb.AppendLine($"  11. Dementor King: {DownedBossSystem.downedDementorKing}");
            sb.AppendLine($"  12. Voldemort: {DownedBossSystem.downedVoldemort}");

            // Vanilla progression gates
            sb.AppendLine($"  [Vanilla] Golem: {NPC.downedGolemBoss}");
            sb.AppendLine($"  [Vanilla] Lunatic Cultist: {NPC.downedAncientCultist}");
            sb.AppendLine($"  [Vanilla] Moon Lord: {NPC.downedMoonlord}");
            sb.AppendLine($"  [Vanilla] Hardmode: {Main.hardMode}");

            caller.Reply(sb.ToString(), new Color(180, 100, 255));
        }

        private void PrintBiome(CommandCaller caller)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Biome & Event State]");

            // Biome checks
            sb.AppendLine($"  In Forbidden Forest: {caller.Player.InModBiome<Content.Biomes.ForbiddenForestBiome>()}");
            sb.AppendLine($"  Zone Dungeon: {caller.Player.ZoneDungeon}");

            // Events
            sb.AppendLine($"  Blood Moon: {Main.bloodMoon}");
            sb.AppendLine($"  Death Eater Invasion: {DeathEaterInvasion.invasionActive}");
            sb.AppendLine($"  Azkaban Despair Event: {AzkabanDespairEvent.eventActive}");
            if (AzkabanDespairEvent.eventActive)
            {
                sb.AppendLine($"    Progress: {AzkabanDespairEvent.eventProgress}/{AzkabanDespairEvent.eventProgressMax}");
            }

            // Dementor spawn eligibility
            bool dementorCanSpawn = Main.hardMode && (
                Main.bloodMoon ||
                DeathEaterInvasion.invasionActive ||
                AzkabanDespairEvent.eventActive ||
                (!Main.dayTime && caller.Player.InModBiome<Content.Biomes.ForbiddenForestBiome>()));
            sb.AppendLine($"  Dementor eligible to spawn: {dementorCanSpawn}");

            sb.AppendLine($"  Day/Night: {(Main.dayTime ? "Day" : "Night")}");

            // Despair
            var wp = caller.Player.GetModPlayer<Players.WizardPlayer>();
            sb.AppendLine($"  Player despair: {wp.despair:F3}");
            sb.AppendLine($"  Patronus active: {wp.patronusActive}");

            caller.Reply(sb.ToString(), new Color(100, 200, 100));
        }
    }
}
