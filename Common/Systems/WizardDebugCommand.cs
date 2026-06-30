#if DEBUG
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Players;

namespace WizardingWorld.Common.Systems
{
    /// <summary>
    /// Developer diagnostics — /wwdebug command.
    /// Reports real runtime state for world progression, Hallows, Horcruxes, mastery, biomes, and bosses.
    /// </summary>
	public class WizardDebugCommand : ModCommand
	{
		public override string Command => "wwdebug";
		public override string Usage => "/wwdebug <summary|early|mid|posthorntail|umbridge|fenrir|bellatrix|barty|dementor|voldemort|kit|god|vanilla|bossflag|battle|audio|hallows|horcruxes|mastery|bosses|biome>";
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
				case "early": PrintEarlyRoute(caller); break;
				case "mid": PrintMidRoute(caller); break;
				case "post":
				case "posthorntail": PrintPostHorntail(caller); break;
				case "umbridge": PrintUmbridge(caller); break;
				case "fenrir": PrintFenrir(caller); break;
				case "bellatrix": PrintBellatrix(caller); break;
				case "barty": PrintBarty(caller); break;
				case "dementor":
				case "dementorking": PrintDementor(caller); break;
				case "voldemort":
				case "final":
				case "finale": PrintVoldemort(caller); break;
				case "kit": GiveQaKit(caller, args); break;
				case "god": SetDebugGodMode(caller, args); break;
				case "vanilla": SetVanillaGate(caller, args); break;
				case "bossflag": SetWizardBossFlag(caller, args); break;
				case "battle": SetBattleGate(caller, args); break;
				case "audio": QueueAudioQa(caller, args); break;
				case "hallows": HandleHallows(caller, args); break;
				case "horcruxes": HandleHorcruxes(caller, args); break;
				case "mastery": PrintMastery(caller); break;
				case "bosses": PrintBosses(caller); break;
				case "biome": PrintBiome(caller); break;
				default:
					caller.Reply("Unknown subcommand. Use: summary, early, mid, posthorntail, umbridge, fenrir, bellatrix, barty, dementor, voldemort, kit, god, vanilla, bossflag, battle, audio, hallows, horcruxes, mastery, bosses, biome", Color.Red);
					break;
			}
		}

		private void PrintEarlyRoute(CommandCaller caller)
		{
			Player player = caller.Player;
			var letter = player.GetModPlayer<HogwartsLetterSystem>();
			var sb = new StringBuilder();
			sb.AppendLine("[Wizarding World Early QA]");

			bool earlyVanillaBossDowned = NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedSlimeKing;
			bool letterReady = player.statLifeMax >= 120 || earlyVanillaBossDowned;
			bool hasSpellWand = HasSpellWand(player);
			bool hasOakWand = HasItem(player, ModContent.ItemType<Content.Items.Weapons.Wands.OakWand>());
			bool hasLetter = HasItem(player, ModContent.ItemType<Content.Items.Consumables.HogwartsLetter>());
			bool hasEnchantingTable = HasItem(player, ModContent.ItemType<Content.Items.Placeable.EnchantingTableItem>());
			bool hasTrollSummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.TrollSummonItem>());
			bool hasQuirrellSummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.QuirrellSummonItem>());
			bool hasBasiliskSummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.BasiliskSummonItem>());
			bool introBypassed = !letter.receivedLetter && (hasLetter || hasOakWand || hasEnchantingTable);

			int wood = CountAnyWood(player);
			int stars = CountItem(player, ItemID.FallenStar);
			int torches = CountItem(player, ItemID.Torch);
			int amethyst = CountItem(player, ItemID.Amethyst);
			int stone = CountItem(player, ItemID.StoneBlock);
			int silk = CountItem(player, ItemID.Silk);
			int evilChunks = CountEvilChunks(player);
			int books = CountItem(player, ItemID.Book);
			int bones = CountItem(player, ItemID.Bone);
			int cobweb = CountItem(player, ItemID.Cobweb);
			int townNpcs = CountTownNpcs();

			bool canCraftTableGemRoute = wood >= 20 && stars >= 5 && torches >= 5 && amethyst >= 2;
			bool canCraftTableStoneRoute = wood >= 30 && stars >= 8 && torches >= 10 && stone >= 25;
			bool canCraftTable = canCraftTableGemRoute || canCraftTableStoneRoute;
			bool canCraftTrollSummon = silk >= 1 && evilChunks >= 1 && stars >= 1;
			bool canCraftQuirrellSummon = silk >= 10 && amethyst >= 3 && stars >= 5;
			bool canCraftBasiliskSummon = books >= 1 && bones >= 10 && cobweb >= 20;
			bool quirrellUseGate = DownedBossSystem.downedTroll && NPC.downedBoss1;
			bool basiliskUseGate = DownedBossSystem.downedQuirrell && NPC.downedBoss3;

			sb.AppendLine("  Intro:");
			AppendCheck(sb, "Hogwarts Letter trigger", letterReady,
				$"life {player.statLifeMax}/120, early vanilla boss downed: {earlyVanillaBossDowned}");
			AppendCheck(sb, "Letter delivered", letter.receivedLetter,
				$"letter item: {hasLetter}, Oak Wand: {hasOakWand}");
			if (introBypassed)
				sb.AppendLine("    [INFO] Intro manually bypassed - QA kit/manual items present; natural owl delivery not verified.");
			AppendCheck(sb, "Any spell wand in inventory", hasSpellWand,
				"Ollivander can move in from this, or from 3 town NPCs.");

			sb.AppendLine("  Crafting:");
			AppendCheck(sb, "Enchanting Table route A", canCraftTableGemRoute,
				$"wood {wood}/20, stars {stars}/5, torches {torches}/5, amethyst {amethyst}/2");
			AppendCheck(sb, "Enchanting Table route B", canCraftTableStoneRoute,
				$"wood {wood}/30, stars {stars}/8, torches {torches}/10, stone {stone}/25");
			AppendCheck(sb, "Enchanting Table item", hasEnchantingTable,
				"Also sold by Ollivander once he has moved in.");
			AppendCheck(sb, "Smelly Sock materials", canCraftTrollSummon || hasTrollSummon,
				$"summon item: {hasTrollSummon}, silk {silk}/1, evil chunk {evilChunks}/1, stars {stars}/1; station: Enchanting Table");
			AppendCheck(sb, "Suspicious Turban materials", canCraftQuirrellSummon || hasQuirrellSummon,
				$"summon item: {hasQuirrellSummon}, silk {silk}/10, amethyst {amethyst}/3, stars {stars}/5; station: Enchanting Table");
			AppendCheck(sb, "Serpent's Diary materials", canCraftBasiliskSummon || hasBasiliskSummon,
				$"summon item: {hasBasiliskSummon}, book {books}/1, bone {bones}/10, cobweb {cobweb}/20; station: Enchanting Table");

			sb.AppendLine("  Progression gates:");
			AppendCheck(sb, "Mountain Troll defeated", DownedBossSystem.downedTroll,
				"First Wizarding World boss.");
			AppendCheck(sb, "Eye of Cthulhu defeated", NPC.downedBoss1,
				"Required before Quirrell can be summoned.");
			AppendCheck(sb, "Quirrell summon usable", quirrellUseGate,
				$"Troll: {DownedBossSystem.downedTroll}, Eye: {NPC.downedBoss1}");
			AppendCheck(sb, "Quirrell defeated", DownedBossSystem.downedQuirrell,
				"Required before Basilisk can be summoned.");
			AppendCheck(sb, "Skeletron defeated", NPC.downedBoss3,
				"Required before Basilisk can be summoned.");
			AppendCheck(sb, "Basilisk summon usable", basiliskUseGate,
				$"Quirrell: {DownedBossSystem.downedQuirrell}, Skeletron: {NPC.downedBoss3}");
			AppendCheck(sb, "Basilisk defeated", DownedBossSystem.downedBasilisk,
				"Completes the first major Wizarding World arc.");

			sb.AppendLine("  NPCs and shops:");
			AppendCheck(sb, "Ollivander can move in", hasSpellWand || townNpcs >= 3,
				$"spell wand: {hasSpellWand}, town NPCs: {townNpcs}/3");
			AppendCheck(sb, "Hagrid can move in", DownedBossSystem.downedTroll || DownedBossSystem.downedBasilisk || townNpcs >= 5,
				$"Troll: {DownedBossSystem.downedTroll}, Basilisk: {DownedBossSystem.downedBasilisk}, town NPCs: {townNpcs}/5");
			sb.AppendLine($"    Ollivander: Oak/Willow/Table always; Ash after Eye: {YesNo(NPC.downedBoss1)}; Rowan after Troll: {YesNo(DownedBossSystem.downedTroll)}; Holly after Eye/Quirrell: {YesNo(NPC.downedBoss1 || DownedBossSystem.downedQuirrell)}; Birch after Basilisk/Hardmode: {YesNo(DownedBossSystem.downedBasilisk || Main.hardMode)}; Red Oak after Basilisk: {YesNo(DownedBossSystem.downedBasilisk)}");
			sb.AppendLine($"    Hagrid summons: Troll always; Quirrell after Troll+Eye: {YesNo(quirrellUseGate)}; Basilisk after Quirrell+Skeletron: {YesNo(basiliskUseGate)}");

			sb.AppendLine("  Suggested next step:");
			sb.AppendLine($"    {GetEarlyNextStep(letter, player, hasLetter, hasOakWand, hasEnchantingTable, canCraftTable, hasTrollSummon, canCraftTrollSummon, hasQuirrellSummon, canCraftQuirrellSummon, hasBasiliskSummon, canCraftBasiliskSummon)}");

			caller.Reply(sb.ToString(), new Color(120, 220, 255));
		}

		private void PrintMidRoute(CommandCaller caller)
		{
			Player player = caller.Player;
			var sb = new StringBuilder();
			sb.AppendLine("[Wizarding World Mid QA]");

			bool anyMech = WizardConditions.AnyMechBossDowned;
			bool aragogActive = NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.Bosses.Aragog.AragogBoss>());
			bool fluffyActive = NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.Bosses.Fluffy.FluffyBoss>());
			bool horntailActive = NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.Bosses.Horntail.HorntailBoss>());
			bool aragogSummonUsable = Main.hardMode && !aragogActive;
			bool fluffySummonUsable = anyMech && !fluffyActive;
			bool horntailSummonUsable = anyMech && !horntailActive;
			bool hasEnchantingTable = HasItem(player, ModContent.ItemType<Content.Items.Placeable.EnchantingTableItem>());
			bool hasAragogSummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.AragogSummonItem>());
			bool hasFluffySummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.FluffySummonItem>());
			bool hasHorntailSummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.HorntailSummonItem>());

			int cobweb = CountItem(player, ItemID.Cobweb);
			int spiderFang = CountItem(player, ItemID.SpiderFang);
			int essence = CountItem(player, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>());
			int goldBars = CountItem(player, ItemID.GoldBar);
			int soulsSight = CountItem(player, ItemID.SoulofSight);
			int hellstoneBars = CountItem(player, ItemID.HellstoneBar);
			int soulsMight = CountItem(player, ItemID.SoulofMight);
			int soulsFlight = CountItem(player, ItemID.SoulofFlight);
			int dragonScales = CountItem(player, ModContent.ItemType<Content.Items.Consumables.DragonScale>());

			bool canCraftAragog = cobweb >= 30 && spiderFang >= 5 && essence >= 10;
			bool canCraftFluffy = goldBars >= 10 && soulsSight >= 5 && essence >= 10;
			bool canCraftHorntailFirst = hellstoneBars >= 15 && soulsMight >= 10 && soulsFlight >= 5;
			bool canCraftHorntailRepeat = hellstoneBars >= 10 && soulsMight >= 5 && dragonScales >= 3;

			sb.AppendLine("  World gates:");
			AppendCheck(sb, "Early arc complete", DownedBossSystem.downedBasilisk,
				$"Basilisk: {DownedBossSystem.downedBasilisk}. Recommended before mid QA.");
			AppendCheck(sb, "Hardmode", Main.hardMode,
				"Required before Aragog, Fluffy, and Horntail checks.");
			AppendCheck(sb, "Mechanical boss defeated", anyMech,
				$"Any: {NPC.downedMechBossAny}, Destroyer: {NPC.downedMechBoss1}, Twins: {NPC.downedMechBoss2}, Skeletron Prime: {NPC.downedMechBoss3}");

			sb.AppendLine("  Crafting and summon items:");
			AppendCheck(sb, "Enchanting Table item", hasEnchantingTable,
				"Used for all three mid summon recipes.");
			AppendCheck(sb, "Acromantula Egg", hasAragogSummon || canCraftAragog,
				$"item: {hasAragogSummon}, cobweb {cobweb}/30, spider fang {spiderFang}/5, essence {essence}/10");
			AppendCheck(sb, "Enchanted Flute", hasFluffySummon || canCraftFluffy,
				$"item: {hasFluffySummon}, gold bars {goldBars}/10, soul of sight {soulsSight}/5, essence {essence}/10");
			AppendCheck(sb, "Dragon Egg (Cracked)", hasHorntailSummon || canCraftHorntailFirst || canCraftHorntailRepeat,
				$"item: {hasHorntailSummon}, first route: hellstone {hellstoneBars}/15, might {soulsMight}/10, flight {soulsFlight}/5; repeat route: hellstone {hellstoneBars}/10, might {soulsMight}/5, dragon scale {dragonScales}/3");

			sb.AppendLine("  Progression gates:");
			AppendCheck(sb, "Aragog summon usable", aragogSummonUsable,
				$"Hardmode: {Main.hardMode}, active Aragog: {aragogActive}");
			AppendCheck(sb, "Aragog defeated", DownedBossSystem.downedAragog,
				"Recommended before Fluffy in the main QA route.");
			AppendCheck(sb, "Fluffy summon usable", fluffySummonUsable,
				$"mechanical boss: {anyMech}, active Fluffy: {fluffyActive}");
			AppendCheck(sb, "Fluffy defeated", DownedBossSystem.downedFluffy,
				"Recommended before Horntail in the main QA route.");
			AppendCheck(sb, "Horntail summon usable", horntailSummonUsable,
				$"mechanical boss: {anyMech}, active Horntail: {horntailActive}");
			AppendCheck(sb, "Horntail defeated", DownedBossSystem.downedHorntail,
				"Completes the mid entrance QA arc.");

			sb.AppendLine("  Hagrid shop gates:");
			sb.AppendLine($"    Aragog after Hardmode: {YesNo(Main.hardMode)}; Fluffy after mech boss: {YesNo(anyMech)}; Horntail after mech boss: {YesNo(anyMech)}");

			sb.AppendLine("  Suggested next step:");
			sb.AppendLine($"    {GetMidNextStep(aragogActive, fluffyActive, horntailActive, hasAragogSummon, canCraftAragog, hasFluffySummon, canCraftFluffy, hasHorntailSummon, canCraftHorntailFirst || canCraftHorntailRepeat)}");

			caller.Reply(sb.ToString(), new Color(120, 220, 255));
		}

		private void PrintPostHorntail(CommandCaller caller)
		{
			Player player = caller.Player;
			var sb = new StringBuilder();
			sb.AppendLine("[Wizarding World Post-Horntail QA]");

			bool hasGoblet = HasItem(player, ModContent.ItemType<Content.Items.Placeable.GobletOfFire>());
			bool hasStMungosPass = HasItem(player, ModContent.ItemType<Content.Items.Consumables.StMungosPass>());
			bool hasForestLantern = HasItem(player, ModContent.ItemType<Content.Items.Consumables.ForestLantern>());
			bool hasBossCompass = HasItem(player, ModContent.ItemType<Content.Items.Accessories.BossCompass>());
			bool hasAlmanac = HasItem(player, ModContent.ItemType<Content.Items.Consumables.WizardsAlmanac>());

			sb.AppendLine("  Core gate:");
			AppendCheck(sb, "Horntail defeated", DownedBossSystem.downedHorntail,
				"Required for Triwizard and St Mungo's unlocks.");

			sb.AppendLine("  Triwizard:");
			AppendCheck(sb, "Goblet of Fire item", hasGoblet,
				"Use it after Horntail to unlock/start the tournament.");
			AppendCheck(sb, "Tournament can unlock", TriwizardTournamentSystem.CanUnlock(),
				$"Horntail: {DownedBossSystem.downedHorntail}, unlocked: {TriwizardTournamentSystem.tournamentUnlocked}");
			AppendCheck(sb, "Tournament unlocked", TriwizardTournamentSystem.tournamentUnlocked,
				$"task {TriwizardTournamentSystem.currentTask}, active: {TriwizardTournamentSystem.taskActive}, champion: {TriwizardTournamentSystem.championCrowned}");
			sb.AppendLine($"    Tasks: T1={TriwizardTournamentSystem.task1Complete}, T2={TriwizardTournamentSystem.task2Complete}, T3={TriwizardTournamentSystem.task3Complete}; can start next: {YesNo(TriwizardTournamentSystem.CanStartNextTask())}");

			sb.AppendLine("  St Mungo's:");
			AppendCheck(sb, "St Mungo's Pass item", hasStMungosPass,
				"Use it after Horntail to unlock the hospital, then start triage missions.");
			AppendCheck(sb, "Hospital can unlock", StMungosTriageSystem.CanUnlock(),
				$"Horntail: {DownedBossSystem.downedHorntail}, unlocked: {StMungosTriageSystem.hospitalUnlocked}");
			AppendCheck(sb, "Hospital unlocked / Healer can move in", StMungosTriageSystem.hospitalUnlocked,
				$"missions: {StMungosTriageSystem.missionsCompleted}, active: {StMungosTriageSystem.missionActive}, can start: {YesNo(StMungosTriageSystem.CanStart())}");

			sb.AppendLine("  Forest and shops:");
			AppendCheck(sb, "Forest Lantern item", hasForestLantern,
				"Forest expeditions are post-Basilisk; post-Horntail changes forest danger/messaging.");
			AppendCheck(sb, "Forbidden Forest expedition unlocked", ForestExpeditionSystem.expeditionUnlocked,
				$"can unlock: {YesNo(ForestExpeditionSystem.CanUnlock())}, can start now: {YesNo(ForestExpeditionSystem.CanStart(player))}, completed: {ForestExpeditionSystem.expeditionsCompleted}");
			sb.AppendLine($"    In Forbidden Forest: {YesNo(player.InModBiome<Content.Biomes.ForbiddenForestBiome>())}; night: {YesNo(!Main.dayTime)}; post-Horntail forest state: {YesNo(DownedBossSystem.downedHorntail)}");
			sb.AppendLine($"    Hagrid post-Horntail items: DragonScaleRing {YesNo(DownedBossSystem.downedHorntail)}, BeastHuntersCharm {YesNo(Main.hardMode && DownedBossSystem.downedHorntail)}");
			sb.AppendLine($"    Potions Master post-Horntail item: DraconisElixir {YesNo(Main.hardMode && DownedBossSystem.downedHorntail)}");

			sb.AppendLine("  Navigation:");
			AppendCheck(sb, "Boss Compass item", hasBossCompass,
				"After Horntail, next boss guidance should point to Umbridge.");
			AppendCheck(sb, "Wizard's Almanac item", hasAlmanac,
				"Sold by Dumbledore; use it to inspect systems/boss pages.");
			sb.AppendLine($"    Expected next boss after Horntail: {(DownedBossSystem.downedUmbridge ? "after Umbridge path" : "Umbridge")}");

			sb.AppendLine("  Suggested next step:");
			sb.AppendLine($"    {GetPostHorntailNextStep(hasGoblet, hasStMungosPass, hasForestLantern, hasBossCompass, hasAlmanac)}");

			caller.Reply(sb.ToString(), new Color(120, 220, 255));
		}

		private void PrintUmbridge(CommandCaller caller)
		{
			Player player = caller.Player;
			var sb = new StringBuilder();
			sb.AppendLine("[Wizarding World Umbridge QA]");

			bool active = NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.Bosses.Umbridge.UmbridgeBoss>());
			bool gateOpen = WizardConditions.UmbridgeGateOpen;
			bool hasSummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.UmbridgeSummonItem>());
			bool hasBossCompass = HasItem(player, ModContent.ItemType<Content.Items.Accessories.BossCompass>());
			bool hasAlmanac = HasItem(player, ModContent.ItemType<Content.Items.Consumables.WizardsAlmanac>());
			bool hasQaWeapon = HasItem(player, ModContent.ItemType<Content.Items.Weapons.Wands.QATestWand>());
			bool hasEnchantingTable = HasItem(player, ModContent.ItemType<Content.Items.Placeable.EnchantingTableItem>());

			int books = CountItem(player, ItemID.Book);
			int pinkGel = CountItem(player, ItemID.PinkGel);
			int essence = CountItem(player, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>());
			bool canCraft = books >= 3 && pinkGel >= 10 && essence >= 10;
			bool summonUsable = gateOpen && !active;

			sb.AppendLine("  Core gate:");
			AppendCheck(sb, "Hardmode", Main.hardMode,
				"Umbridge is a late-hardmode route check, not an early hardmode shortcut.");
			AppendCheck(sb, "Mechanical boss defeated", WizardConditions.AnyMechBossDowned,
				"Matches the vanilla post-mech side of the gate.");
			AppendCheck(sb, "Horntail defeated", DownedBossSystem.downedHorntail,
				"Keeps the main Wizarding World order: Horntail -> Umbridge.");
			AppendCheck(sb, "Umbridge gate open", gateOpen,
				$"Hardmode={YesNo(Main.hardMode)}, mech={YesNo(WizardConditions.AnyMechBossDowned)}, Horntail={YesNo(DownedBossSystem.downedHorntail)}");

			sb.AppendLine("  Summon route:");
			AppendCheck(sb, "Educational Decree item", hasSummon,
				"Use it to summon Dolores Umbridge.");
			AppendCheck(sb, "Educational Decree materials", hasSummon || canCraft,
				$"Book {books}/3, Pink Gel {pinkGel}/10, Essence of Magic {essence}/10; Enchanting Table item in inventory: {YesNo(hasEnchantingTable)}");
			AppendCheck(sb, "Hagrid shop gate", gateOpen,
				"Hagrid should sell Educational Decree only after Horntail plus a mechanical boss.");
			AppendCheck(sb, "Summon usable now", summonUsable,
				active ? "Umbridge is already active." : "Requires Hardmode, any mechanical boss, Horntail defeated, and no active Umbridge.");

			sb.AppendLine("  Fight and handoff:");
			AppendCheck(sb, "QA test weapon", hasQaWeapon,
				"Use /wwdebug kit umbridge or /wwdebug kit weapon if combat validation is too slow.");
			AppendCheck(sb, "Umbridge active", active,
				"Only expected while the fight is running.");
			AppendCheck(sb, "Umbridge defeated", DownedBossSystem.downedUmbridge,
				"After the kill, this flag should persist after Save & Exit.");
			sb.AppendLine($"    Expected next boss after Umbridge: {(DownedBossSystem.downedFenrir ? "after Fenrir path" : "Fenrir Greyback")}");

			sb.AppendLine("  Navigation:");
			AppendCheck(sb, "Boss Compass item", hasBossCompass,
				DownedBossSystem.downedUmbridge ? "After Umbridge, compass should point to Fenrir." : "Before Umbridge, compass should point to Dolores Umbridge.");
			AppendCheck(sb, "Wizard's Almanac item", hasAlmanac,
				"Boss page should list Educational Decree as post-Horntail + Mech.");

			sb.AppendLine("  Suggested next step:");
			sb.AppendLine($"    {GetUmbridgeNextStep(active, hasSummon, canCraft)}");

			caller.Reply(sb.ToString(), new Color(255, 160, 220));
		}

		private void PrintFenrir(CommandCaller caller)
		{
			Player player = caller.Player;
			var sb = new StringBuilder();
			sb.AppendLine("[Wizarding World Fenrir QA]");

			bool active = NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.Bosses.Fenrir.FenrirBoss>());
			bool gateOpen = WizardConditions.FenrirGateOpen;
			bool hasSummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.FenrirSummonItem>());
			bool hasBossCompass = HasItem(player, ModContent.ItemType<Content.Items.Accessories.BossCompass>());
			bool hasAlmanac = HasItem(player, ModContent.ItemType<Content.Items.Consumables.WizardsAlmanac>());
			bool hasQaWeapon = HasItem(player, ModContent.ItemType<Content.Items.Weapons.Wands.QATestWand>());
			bool hasEnchantingTable = HasItem(player, ModContent.ItemType<Content.Items.Placeable.EnchantingTableItem>());

			int moonCharm = CountItem(player, ItemID.MoonCharm);
			int soulsNight = CountItem(player, ItemID.SoulofNight);
			int essence = CountItem(player, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>());
			bool canCraft = moonCharm >= 1 && soulsNight >= 10 && essence >= 10;
			bool summonUsable = gateOpen && Main.bloodMoon && !active;

			sb.AppendLine("  Core gate:");
			AppendCheck(sb, "Hardmode", Main.hardMode,
				"Fenrir is a late-hardmode Blood Moon boss.");
			AppendCheck(sb, "Umbridge defeated", DownedBossSystem.downedUmbridge,
				"Keeps the main route: Umbridge -> Fenrir.");
			AppendCheck(sb, "Fenrir gate open", gateOpen,
				$"Hardmode={YesNo(Main.hardMode)}, Umbridge={YesNo(DownedBossSystem.downedUmbridge)}");
			AppendCheck(sb, "Blood Moon active", Main.bloodMoon,
				"Use /wwdebug vanilla bloodmoon in a disposable QA world if you need to force this gate.");
			sb.AppendLine($"    Day/Night: {(Main.dayTime ? "Day" : "Night")}; moon phase: {Main.moonPhase}");

			sb.AppendLine("  Summon route:");
			AppendCheck(sb, "Bloodied Claw item", hasSummon,
				"Use it during a Blood Moon to summon Fenrir Greyback.");
			AppendCheck(sb, "Bloodied Claw materials", hasSummon || canCraft,
				$"Moon Charm {moonCharm}/1, Souls of Night {soulsNight}/10, Essence of Magic {essence}/10; Enchanting Table item in inventory: {YesNo(hasEnchantingTable)}");
			AppendCheck(sb, "Hagrid shop gate", gateOpen,
				"Hagrid should sell Bloodied Claw only after Umbridge.");
			AppendCheck(sb, "Summon usable now", summonUsable,
				active ? "Fenrir is already active." : "Requires Hardmode, Umbridge defeated, Blood Moon, and no active Fenrir.");

			sb.AppendLine("  Fight and handoff:");
			AppendCheck(sb, "QA test weapon", hasQaWeapon,
				"Use /wwdebug kit fenrir or /wwdebug kit weapon if combat validation is too slow.");
			AppendCheck(sb, "Fenrir active", active,
				"Only expected while the fight is running.");
			AppendCheck(sb, "Fenrir defeated", DownedBossSystem.downedFenrir,
				"After the kill, this flag should persist after Save & Exit.");
			sb.AppendLine($"    Expected next boss after Fenrir: {(DownedBossSystem.downedBellatrix ? "after Bellatrix path" : "Bellatrix Lestrange")}");

			sb.AppendLine("  Navigation:");
			AppendCheck(sb, "Boss Compass item", hasBossCompass,
				DownedBossSystem.downedFenrir ? "After Fenrir, compass should point to Bellatrix." : "Before Fenrir, compass should point to Fenrir Greyback.");
			AppendCheck(sb, "Wizard's Almanac item", hasAlmanac,
				"Boss page should list Bloodied Claw as post-Umbridge + Blood Moon.");

			sb.AppendLine("  Suggested next step:");
			sb.AppendLine($"    {GetFenrirNextStep(active, hasSummon, canCraft)}");

			caller.Reply(sb.ToString(), new Color(180, 120, 255));
		}

		private void PrintBellatrix(CommandCaller caller)
		{
			Player player = caller.Player;
			var sb = new StringBuilder();
			sb.AppendLine("[Wizarding World Bellatrix QA]");

			bool active = NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.Bosses.Bellatrix.BellatrixBoss>());
			bool gateOpen = WizardConditions.BellatrixGateOpen;
			bool hasSummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.BellatrixSummonItem>());
			bool hasBossCompass = HasItem(player, ModContent.ItemType<Content.Items.Accessories.BossCompass>());
			bool hasAlmanac = HasItem(player, ModContent.ItemType<Content.Items.Consumables.WizardsAlmanac>());
			bool hasQaWeapon = HasItem(player, ModContent.ItemType<Content.Items.Weapons.Wands.QATestWand>());
			bool hasEnchantingTable = HasItem(player, ModContent.ItemType<Content.Items.Placeable.EnchantingTableItem>());

			int bones = CountItem(player, ItemID.Bone);
			int soulsNight = CountItem(player, ItemID.SoulofNight);
			int darkShards = CountItem(player, ItemID.DarkShard);
			int essence = CountItem(player, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>());
			bool canCraft = bones >= 30 && soulsNight >= 10 && darkShards >= 2 && essence >= 15;
			bool summonUsable = gateOpen && !active;

			sb.AppendLine("  Core gate:");
			AppendCheck(sb, "Hardmode", Main.hardMode,
				"Bellatrix is a late-hardmode duel boss.");
			AppendCheck(sb, "Fenrir defeated", DownedBossSystem.downedFenrir,
				"Keeps the main route: Fenrir -> Bellatrix.");
			AppendCheck(sb, "Plantera defeated", NPC.downedPlantBoss,
				"Use /wwdebug vanilla plantera in a disposable QA world if you need to force this gate.");
			AppendCheck(sb, "Bellatrix gate open", gateOpen,
				$"Hardmode={YesNo(Main.hardMode)}, Fenrir={YesNo(DownedBossSystem.downedFenrir)}, Plantera={YesNo(NPC.downedPlantBoss)}");

			sb.AppendLine("  Summon route:");
			AppendCheck(sb, "Azkaban Prisoner Tag item", hasSummon,
				"Use it to summon Bellatrix Lestrange.");
			AppendCheck(sb, "Azkaban Prisoner Tag materials", hasSummon || canCraft,
				$"Bone {bones}/30, Souls of Night {soulsNight}/10, Dark Shard {darkShards}/2, Essence of Magic {essence}/15; Enchanting Table item in inventory: {YesNo(hasEnchantingTable)}");
			AppendCheck(sb, "Hagrid shop gate", gateOpen,
				"Hagrid should sell Azkaban Prisoner Tag only after Fenrir plus Plantera.");
			AppendCheck(sb, "Summon usable now", summonUsable,
				active ? "Bellatrix is already active." : "Requires Hardmode, Fenrir defeated, Plantera defeated, and no active Bellatrix.");

			sb.AppendLine("  Fight and handoff:");
			AppendCheck(sb, "QA test weapon", hasQaWeapon,
				"Use /wwdebug kit bellatrix or /wwdebug kit weapon if combat validation is too slow.");
			AppendCheck(sb, "Bellatrix active", active,
				"Only expected while the fight is running.");
			AppendCheck(sb, "Bellatrix defeated", DownedBossSystem.downedBellatrix,
				"After the kill, this flag should persist after Save & Exit.");
			sb.AppendLine($"    Expected next boss after Bellatrix: {(DownedBossSystem.downedBartyCrouch ? "after Barty path" : "Barty Crouch Jr")}");

			sb.AppendLine("  Navigation:");
			AppendCheck(sb, "Boss Compass item", hasBossCompass,
				DownedBossSystem.downedBellatrix ? "After Bellatrix, compass should point to Barty Crouch Jr." : "Before Bellatrix, compass should point to Bellatrix Lestrange.");
			AppendCheck(sb, "Wizard's Almanac item", hasAlmanac,
				"Boss page should list Azkaban Prisoner Tag as post-Fenrir + Plantera.");

			sb.AppendLine("  Suggested next step:");
			sb.AppendLine($"    {GetBellatrixNextStep(active, hasSummon, canCraft)}");

			caller.Reply(sb.ToString(), new Color(255, 120, 210));
		}

		private void PrintBarty(CommandCaller caller)
		{
			Player player = caller.Player;
			var sb = new StringBuilder();
			sb.AppendLine("[Wizarding World Barty QA]");

			bool active = NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.Bosses.BartyCrouch.BartyCrouchBoss>());
			bool gateOpen = WizardConditions.BartyGateOpen;
			bool hasSummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.BartyCrouchSummonItem>());
			bool hasBossCompass = HasItem(player, ModContent.ItemType<Content.Items.Accessories.BossCompass>());
			bool hasAlmanac = HasItem(player, ModContent.ItemType<Content.Items.Consumables.WizardsAlmanac>());
			bool hasQaWeapon = HasItem(player, ModContent.ItemType<Content.Items.Weapons.Wands.QATestWand>());
			bool hasEnchantingTable = HasItem(player, ModContent.ItemType<Content.Items.Placeable.EnchantingTableItem>());

			int bottles = CountItem(player, ItemID.Bottle);
			int deathweed = CountItem(player, ItemID.Deathweed);
			int soulsNight = CountItem(player, ItemID.SoulofNight);
			int essence = CountItem(player, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>());
			bool canCraft = bottles >= 1 && deathweed >= 5 && soulsNight >= 5 && essence >= 12;
			bool summonUsable = gateOpen && !active;

			sb.AppendLine("  Core gate:");
			AppendCheck(sb, "Hardmode", Main.hardMode,
				"Barty is a late-hardmode shapeshifter boss.");
			AppendCheck(sb, "Bellatrix defeated", DownedBossSystem.downedBellatrix,
				"Keeps the main route: Bellatrix -> Barty Crouch Jr.");
			AppendCheck(sb, "Plantera defeated", NPC.downedPlantBoss,
				"Use /wwdebug vanilla plantera in a disposable QA world if you need to force this gate.");
			AppendCheck(sb, "Barty gate open", gateOpen,
				$"Hardmode={YesNo(Main.hardMode)}, Bellatrix={YesNo(DownedBossSystem.downedBellatrix)}, Plantera={YesNo(NPC.downedPlantBoss)}");

			sb.AppendLine("  Summon route:");
			AppendCheck(sb, "Suspicious Flask item", hasSummon,
				"Use it to summon Barty Crouch Jr.");
			AppendCheck(sb, "Suspicious Flask materials", hasSummon || canCraft,
				$"Bottle {bottles}/1, Deathweed {deathweed}/5, Souls of Night {soulsNight}/5, Essence of Magic {essence}/12; Enchanting Table item in inventory: {YesNo(hasEnchantingTable)}");
			AppendCheck(sb, "Hagrid shop gate", gateOpen,
				"Hagrid should sell Suspicious Flask only after Bellatrix plus Plantera.");
			AppendCheck(sb, "Summon usable now", summonUsable,
				active ? "Barty is already active." : "Requires Hardmode, Bellatrix defeated, Plantera defeated, and no active Barty.");

			sb.AppendLine("  Fight and handoff:");
			AppendCheck(sb, "QA test weapon", hasQaWeapon,
				"Use /wwdebug kit barty or /wwdebug kit weapon if combat validation is too slow.");
			AppendCheck(sb, "Barty active", active,
				"Only expected while the fight is running.");
			AppendCheck(sb, "Barty defeated", DownedBossSystem.downedBartyCrouch,
				"After the kill, this flag should persist after Save & Exit.");
			sb.AppendLine($"    Expected next boss after Barty: {(DownedBossSystem.downedDementorKing ? "after Dementor King path" : "Dementor King / Azkaban's Despair")}");

			sb.AppendLine("  Navigation:");
			AppendCheck(sb, "Boss Compass item", hasBossCompass,
				DownedBossSystem.downedBartyCrouch ? "After Barty, compass should point to Dementor King." : "Before Barty, compass should point to Barty Crouch Jr.");
			AppendCheck(sb, "Wizard's Almanac item", hasAlmanac,
				"Boss page should list Suspicious Flask as post-Bellatrix + Plantera.");

			sb.AppendLine("  Suggested next step:");
			sb.AppendLine($"    {GetBartyNextStep(active, hasSummon, canCraft)}");

			caller.Reply(sb.ToString(), new Color(210, 120, 255));
		}

		private void PrintDementor(CommandCaller caller)
		{
			Player player = caller.Player;
			var sb = new StringBuilder();
			sb.AppendLine("[Wizarding World Dementor King QA]");

			bool active = NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.Bosses.DementorKing.DementorKingBoss>());
			bool gateOpen = WizardConditions.DementorGateOpen;
			bool hasSummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.DementorKingSummonItem>());
			bool hasBossCompass = HasItem(player, ModContent.ItemType<Content.Items.Accessories.BossCompass>());
			bool hasAlmanac = HasItem(player, ModContent.ItemType<Content.Items.Consumables.WizardsAlmanac>());
			bool hasQaWeapon = HasItem(player, ModContent.ItemType<Content.Items.Weapons.Wands.QATestWand>());
			bool hasEnchantingTable = HasItem(player, ModContent.ItemType<Content.Items.Placeable.EnchantingTableItem>());

			int soulsNight = CountItem(player, ItemID.SoulofNight);
			int ectoplasm = CountItem(player, ItemID.Ectoplasm);
			int essence = CountItem(player, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>());
			int unicornBlood = CountItem(player, ModContent.ItemType<Content.Items.Consumables.UnicornBlood>());
			bool canCraft = soulsNight >= 15 && ectoplasm >= 10 && essence >= 25 && unicornBlood >= 3;
			bool summonUsable = gateOpen && !Main.dayTime && !active;

			sb.AppendLine("  Core gate:");
			AppendCheck(sb, "Hardmode", Main.hardMode,
				"Azkaban's Despair is a late-hardmode boss.");
			AppendCheck(sb, "Barty defeated", DownedBossSystem.downedBartyCrouch,
				"Keeps the main route: Barty -> Dementor King.");
			AppendCheck(sb, "Golem defeated", NPC.downedGolemBoss,
				"Use /wwdebug vanilla golem in a disposable QA world if you need to force this gate.");
			AppendCheck(sb, "Night", !Main.dayTime,
				"Use /wwdebug vanilla night to force night without starting a Blood Moon.");
			AppendCheck(sb, "Dementor gate open", gateOpen,
				$"Hardmode={YesNo(Main.hardMode)}, Barty={YesNo(DownedBossSystem.downedBartyCrouch)}, Golem={YesNo(NPC.downedGolemBoss)}");

			sb.AppendLine("  Summon route:");
			AppendCheck(sb, "Frozen Soul Lantern item", hasSummon,
				"Use it at night to summon Azkaban's Despair.");
			AppendCheck(sb, "Frozen Soul Lantern materials", hasSummon || canCraft,
				$"Souls of Night {soulsNight}/15, Ectoplasm {ectoplasm}/10, Essence of Magic {essence}/25, Unicorn Blood {unicornBlood}/3; Enchanting Table item in inventory: {YesNo(hasEnchantingTable)}");
			AppendCheck(sb, "Hagrid shop gate", gateOpen,
				"Hagrid should sell Frozen Soul Lantern only after Barty plus Golem.");
			AppendCheck(sb, "Summon usable now", summonUsable,
				active ? "Azkaban's Despair is already active." : "Requires Hardmode, Barty defeated, Golem defeated, night, and no active Dementor King.");

			sb.AppendLine("  Fight and handoff:");
			AppendCheck(sb, "QA test weapon", hasQaWeapon,
				"Use /wwdebug kit dementor or /wwdebug kit weapon if combat validation is too slow.");
			AppendCheck(sb, "Dementor King active", active,
				"Only expected while the fight is running.");
			AppendCheck(sb, "Dementor King defeated", DownedBossSystem.downedDementorKing,
				"After the kill, this flag should persist after Save & Exit.");
			sb.AppendLine($"    Expected next boss after Dementor King: {(DownedBossSystem.downedVoldemort ? "route complete" : "Lord Voldemort")}");

			sb.AppendLine("  Navigation:");
			AppendCheck(sb, "Boss Compass item", hasBossCompass,
				DownedBossSystem.downedDementorKing ? "After Dementor King, compass should point to Voldemort readiness." : "Before Dementor King, compass should point to Azkaban's Despair.");
			AppendCheck(sb, "Wizard's Almanac item", hasAlmanac,
				"Boss page should list Frozen Soul Lantern as post-Barty + Golem + night.");
			sb.AppendLine($"    Azkaban event active: {YesNo(AzkabanDespairEvent.eventActive)}; event progress: {AzkabanDespairEvent.eventProgress}/{AzkabanDespairEvent.eventProgressMax}");

			sb.AppendLine("  Suggested next step:");
			sb.AppendLine($"    {GetDementorNextStep(active, hasSummon, canCraft)}");

			caller.Reply(sb.ToString(), new Color(160, 180, 255));
		}

		private void PrintVoldemort(CommandCaller caller)
		{
			Player player = caller.Player;
			var sb = new StringBuilder();
			sb.AppendLine("[Wizarding World Voldemort QA]");

			bool active = NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.Bosses.Voldemort.VoldemortBoss>());
			bool technicalSummonGate = NPC.downedAncientCultist && !Main.dayTime;
			bool recommendedReady = DownedBossSystem.downedDementorKing
				&& HorcruxHuntSystem.AllCoreHorcruxesDestroyed
				&& HorcruxHuntSystem.naginiDefeated
				&& BattleOfHogwartsSystem.battlesWon > 0
				&& technicalSummonGate;
			bool hasSummon = HasItem(player, ModContent.ItemType<Content.Items.Consumables.VoldemortSummonItem>());
			bool hasDeathEaterMark = HasItem(player, ModContent.ItemType<Content.Items.Consumables.DarkMarkSummon>());
			bool hasBossCompass = HasItem(player, ModContent.ItemType<Content.Items.Accessories.BossCompass>());
			bool hasAlmanac = HasItem(player, ModContent.ItemType<Content.Items.Consumables.WizardsAlmanac>());
			bool hasHorcruxTracker = HasItem(player, ModContent.ItemType<Content.Items.Consumables.HorcruxTracker>());
			bool hasQaWeapon = HasItem(player, ModContent.ItemType<Content.Items.Weapons.Wands.QATestWand>());
			bool hasEnchantingTable = HasItem(player, ModContent.ItemType<Content.Items.Placeable.EnchantingTableItem>());

			int bones = CountItem(player, ItemID.Bone);
			int soulsNight = CountItem(player, ItemID.SoulofNight);
			int ectoplasm = CountItem(player, ItemID.Ectoplasm);
			int lunarFragments = CountItem(player, ItemID.LunarTabletFragment);
			bool canCraft = bones >= 30 && soulsNight >= 15 && ectoplasm >= 10 && lunarFragments >= 5;
			bool summonUsable = technicalSummonGate && !active;

			sb.AppendLine("  Core route:");
			AppendCheck(sb, "Dementor King defeated", DownedBossSystem.downedDementorKing,
				"Main route handoff into Voldemort readiness.");
			AppendCheck(sb, "Core Horcruxes destroyed", HorcruxHuntSystem.AllCoreHorcruxesDestroyed,
				$"Diary={YesNo(HorcruxHuntSystem.diaryDestroyed)}, Locket={YesNo(HorcruxHuntSystem.locketDestroyed)}, Cup={YesNo(HorcruxHuntSystem.cupDestroyed)}, Diadem={YesNo(HorcruxHuntSystem.diademDestroyed)}; power={HorcruxHuntSystem.GetVoldemortPowerMultiplier():P0}");
			AppendCheck(sb, "Nagini defeated", HorcruxHuntSystem.naginiDefeated,
				"Normally resolved by winning the Battle of Hogwarts; for QA use /wwdebug battle win.");
			AppendCheck(sb, "Battle of Hogwarts won", BattleOfHogwartsSystem.battlesWon > 0,
				$"unlocked={YesNo(BattleOfHogwartsSystem.battleUnlocked)}, active={YesNo(BattleOfHogwartsSystem.battleActive)}, wins={BattleOfHogwartsSystem.battlesWon}, wards defended={HogwartsWardSystem.wardsDefended}");
			AppendCheck(sb, "Lunatic Cultist defeated", NPC.downedAncientCultist,
				"Use /wwdebug vanilla cultist in a disposable QA world if you need to force this gate.");
			AppendCheck(sb, "Night", !Main.dayTime,
				"Use /wwdebug vanilla night to force night for the Dark Mark summon.");
			AppendCheck(sb, "Recommended readiness", recommendedReady,
				"Story route ready = Dementor + core Horcruxes + Nagini/Battle + Cultist + night.");

			sb.AppendLine("  Summon route:");
			AppendCheck(sb, "Dark Mark boss item", hasSummon,
				"This is VoldemortSummonItem. Dark Mark (Morsmordre) is the Death Eater event item, not the boss summon.");
			AppendCheck(sb, "Death Eater Dark Mark present", hasDeathEaterMark,
				"Only informational; this item starts the Death Eater invasion.");
			AppendCheck(sb, "Dark Mark materials", hasSummon || canCraft,
				$"Bone {bones}/30, Souls of Night {soulsNight}/15, Ectoplasm {ectoplasm}/10, Lunar Tablet Fragment {lunarFragments}/5; Enchanting Table item in inventory: {YesNo(hasEnchantingTable)}");
			AppendCheck(sb, "Hagrid shop gate", NPC.downedAncientCultist,
				"Hagrid should sell the boss Dark Mark after Lunatic Cultist.");
			AppendCheck(sb, "Summon usable now", summonUsable,
				active ? "Lord Voldemort is already active." : "Requires Lunatic Cultist defeated, night, and no active Voldemort.");

			sb.AppendLine("  Navigation:");
			AppendCheck(sb, "Boss Compass item", hasBossCompass,
				HorcruxHuntSystem.AllCoreHorcruxesDestroyed ? "After Dementor and core Horcruxes, compass should point to Voldemort." : "Before core Horcruxes, compass should ask for more Horcrux destruction.");
			AppendCheck(sb, "Wizard's Almanac item", hasAlmanac,
				"Boss page should list Dark Mark as post-Cultist + night.");
			AppendCheck(sb, "Horcrux Tracker item", hasHorcruxTracker,
				"Use it to inspect Horcrux/Battle state and naturally unlock/start the Battle of Hogwarts.");

			sb.AppendLine("  Fight and persistence:");
			AppendCheck(sb, "QA test weapon", hasQaWeapon,
				"Use /wwdebug kit voldemort or /wwdebug kit weapon if combat validation is too slow.");
			AppendCheck(sb, "Voldemort active", active,
				"Only expected while the fight is running.");
			AppendCheck(sb, "Voldemort defeated", DownedBossSystem.downedVoldemort,
				"After the kill, this flag should persist after Save & Exit.");

			sb.AppendLine("  Suggested next step:");
			sb.AppendLine($"    {GetVoldemortNextStep(active, hasSummon, canCraft)}");

			caller.Reply(sb.ToString(), new Color(230, 110, 170));
		}

		private void GiveQaKit(CommandCaller caller, string[] args)
		{
			if (args.Length < 2)
			{
				caller.Reply("Usage: /wwdebug kit <intro|troll|quirrell|basilisk|aragog|fluffy|horntail|posthorntail|umbridge|fenrir|bellatrix|barty|dementor|voldemort|weapon>. Use these in a disposable single-player QA world.", Color.Yellow);
				return;
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				caller.Reply("QA kits are disabled on multiplayer clients.", Color.Red);
				return;
			}

			string kit = args[1].ToLowerInvariant();
			switch (kit)
			{
				case "intro":
					GrantIntroKit(caller);
					break;
				case "troll":
					GrantIntroKit(caller);
					GrantBasicCombatPrep(caller, 160);
					SetDebugGodMode(caller.Player, true);
					GrantItem(caller, ModContent.ItemType<Content.Items.Weapons.Wands.WillowWand>(), 1);
					GrantItem(caller, ItemID.Silk, 1);
					GrantItem(caller, ItemID.RottenChunk, 1);
					GrantItem(caller, ItemID.FallenStar, 1);
					GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.TrollSummonItem>(), 1);
					break;
				case "quirrell":
					GrantIntroKit(caller);
					GrantItem(caller, ItemID.Silk, 10);
					GrantItem(caller, ItemID.Amethyst, 3);
					GrantItem(caller, ItemID.FallenStar, 5);
					GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.QuirrellSummonItem>(), 1);
					break;
				case "basilisk":
					GrantIntroKit(caller);
					GrantQaTestWeapon(caller);
					GrantItem(caller, ItemID.Book, 1);
					GrantItem(caller, ItemID.Bone, 10);
					GrantItem(caller, ItemID.Cobweb, 20);
					GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.BasiliskSummonItem>(), 1);
					break;
				case "aragog":
					GrantMidBaseKit(caller);
					GrantItem(caller, ItemID.Cobweb, 30);
					GrantItem(caller, ItemID.SpiderFang, 5);
					GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.AragogSummonItem>(), 1);
					break;
				case "fluffy":
					GrantMidBaseKit(caller);
					GrantItem(caller, ItemID.GoldBar, 10);
					GrantItem(caller, ItemID.SoulofSight, 5);
					GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.FluffySummonItem>(), 1);
					break;
				case "horntail":
					GrantMidBaseKit(caller);
					GrantItem(caller, ItemID.HellstoneBar, 15);
					GrantItem(caller, ItemID.SoulofMight, 10);
					GrantItem(caller, ItemID.SoulofFlight, 5);
					GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.HorntailSummonItem>(), 1);
					break;
				case "post":
				case "posthorntail":
					GrantPostHorntailKit(caller);
					break;
				case "umbridge":
					GrantUmbridgeKit(caller);
					break;
				case "fenrir":
					GrantFenrirKit(caller);
					break;
				case "bellatrix":
					GrantBellatrixKit(caller);
					break;
				case "barty":
					GrantBartyKit(caller);
					break;
				case "dementor":
				case "dementorking":
					GrantDementorKit(caller);
					break;
				case "voldemort":
				case "final":
				case "finale":
					GrantVoldemortKit(caller);
					break;
				case "weapon":
					GrantQaTestWeapon(caller);
					break;
				default:
					caller.Reply("Unknown QA kit. Use: intro, troll, quirrell, basilisk, aragog, fluffy, horntail, posthorntail, umbridge, fenrir, bellatrix, barty, dementor, voldemort, weapon", Color.Red);
					return;
			}

			string godNote = kit == "troll" ? " Debug god mode was enabled for flow validation; use /wwdebug god off to disable it." : "";
			caller.Reply($"Granted QA kit: {kit}. Boss and vanilla progression flags were not changed; run the matching /wwdebug route command to see remaining gates.{godNote}", new Color(120, 255, 120));
		}

		private void SetDebugGodMode(CommandCaller caller, string[] args)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				caller.Reply("Debug god mode is disabled on multiplayer clients.", Color.Red);
				return;
			}

			if (args.Length < 2 || args[1].Equals("status", System.StringComparison.OrdinalIgnoreCase))
			{
				bool current = caller.Player.GetModPlayer<WizardPlayer>().debugGodMode;
				caller.Reply($"Debug god mode is currently {(current ? "ON" : "OFF")}. Use /wwdebug god <on|off|toggle>.", Color.Yellow);
				return;
			}

			string mode = args[1].ToLowerInvariant();
			bool enabled;
			switch (mode)
			{
				case "on":
					enabled = true;
					break;
				case "off":
					enabled = false;
					break;
				case "toggle":
					enabled = !caller.Player.GetModPlayer<WizardPlayer>().debugGodMode;
					break;
				default:
					caller.Reply("Usage: /wwdebug god <on|off|toggle|status>", Color.Yellow);
					return;
			}

			SetDebugGodMode(caller.Player, enabled);
			caller.Reply($"Debug god mode {(enabled ? "enabled" : "disabled")}. This only affects the current test session and does not change boss or progression flags.", enabled ? new Color(120, 255, 120) : Color.Yellow);
		}

		private void SetVanillaGate(CommandCaller caller, string[] args)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				caller.Reply("Vanilla QA gates are disabled on multiplayer clients.", Color.Red);
				return;
			}

			if (args.Length < 2 || args[1].Equals("status", System.StringComparison.OrdinalIgnoreCase))
			{
				caller.Reply($"Vanilla QA gates: Eye of Cthulhu={NPC.downedBoss1}, Skeletron={NPC.downedBoss3}, Hardmode={Main.hardMode}, MechAny={WizardConditions.AnyMechBossDowned}, Plantera={NPC.downedPlantBoss}, Golem={NPC.downedGolemBoss}, Cultist={NPC.downedAncientCultist}, Night={!Main.dayTime}, BloodMoon={Main.bloodMoon}. Use /wwdebug vanilla <eye|skeletron|hardmode|mech|plantera|golem|cultist|night|bloodmoon|mid|all>.", Color.Yellow);
				return;
			}

			string gate = args[1].ToLowerInvariant();
			switch (gate)
			{
				case "eye":
				case "eoc":
					NPC.downedBoss1 = true;
					break;
				case "skeletron":
					NPC.downedBoss3 = true;
					break;
				case "hardmode":
				case "hm":
					Main.hardMode = true;
					break;
				case "mech":
				case "mechanical":
					Main.hardMode = true;
					NPC.downedMechBossAny = true;
					NPC.downedMechBoss1 = true;
					break;
				case "bloodmoon":
				case "blood":
					Main.dayTime = false;
					Main.time = 0;
					Main.bloodMoon = true;
					break;
				case "plantera":
				case "plant":
					Main.hardMode = true;
					NPC.downedMechBossAny = true;
					NPC.downedMechBoss1 = true;
					NPC.downedPlantBoss = true;
					break;
				case "golem":
					Main.hardMode = true;
					NPC.downedMechBossAny = true;
					NPC.downedMechBoss1 = true;
					NPC.downedPlantBoss = true;
					NPC.downedGolemBoss = true;
					break;
				case "cultist":
				case "lunatic":
				case "lc":
					Main.hardMode = true;
					NPC.downedMechBossAny = true;
					NPC.downedMechBoss1 = true;
					NPC.downedPlantBoss = true;
					NPC.downedGolemBoss = true;
					NPC.downedAncientCultist = true;
					break;
				case "night":
					Main.dayTime = false;
					Main.time = 0;
					Main.bloodMoon = false;
					break;
				case "mid":
					Main.hardMode = true;
					NPC.downedMechBossAny = true;
					NPC.downedMechBoss1 = true;
					break;
				case "all":
					NPC.downedBoss1 = true;
					NPC.downedBoss3 = true;
					Main.hardMode = true;
					NPC.downedMechBossAny = true;
					NPC.downedMechBoss1 = true;
					NPC.downedPlantBoss = true;
					NPC.downedGolemBoss = true;
					NPC.downedAncientCultist = true;
					break;
				default:
					caller.Reply("Usage: /wwdebug vanilla <eye|eoc|skeletron|hardmode|hm|mech|plantera|plant|golem|cultist|lunatic|night|bloodmoon|blood|mid|all|status>", Color.Yellow);
					return;
			}

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);

			caller.Reply($"Granted vanilla QA gate: {gate}. Eye of Cthulhu={NPC.downedBoss1}, Skeletron={NPC.downedBoss3}, Hardmode={Main.hardMode}, MechAny={WizardConditions.AnyMechBossDowned}, Plantera={NPC.downedPlantBoss}, Golem={NPC.downedGolemBoss}, Cultist={NPC.downedAncientCultist}, Night={!Main.dayTime}, BloodMoon={Main.bloodMoon}. Wizarding World boss flags were not changed; run the matching /wwdebug route command to see remaining gates.", new Color(120, 255, 120));
		}

		private void SetWizardBossFlag(CommandCaller caller, string[] args)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				caller.Reply("Wizarding World boss QA flags are disabled on multiplayer clients.", Color.Red);
				return;
			}

			if (args.Length < 2 || args[1].Equals("status", System.StringComparison.OrdinalIgnoreCase))
			{
				caller.Reply($"Wizarding World boss QA flags: Troll={DownedBossSystem.downedTroll}, Quirrell={DownedBossSystem.downedQuirrell}, Basilisk={DownedBossSystem.downedBasilisk}, Aragog={DownedBossSystem.downedAragog}, Fluffy={DownedBossSystem.downedFluffy}, Horntail={DownedBossSystem.downedHorntail}, Umbridge={DownedBossSystem.downedUmbridge}, Fenrir={DownedBossSystem.downedFenrir}, Bellatrix={DownedBossSystem.downedBellatrix}, Barty={DownedBossSystem.downedBartyCrouch}, Dementor={DownedBossSystem.downedDementorKing}, Voldemort={DownedBossSystem.downedVoldemort}. Use /wwdebug bossflag <troll|quirrell|basilisk|aragog|fluffy|horntail|umbridge|fenrir|bellatrix|barty|dementor|voldemort|early|mid>.", Color.Yellow);
				return;
			}

			string boss = args[1].ToLowerInvariant();
			switch (boss)
			{
				case "troll":
					DownedBossSystem.downedTroll = true;
					break;
				case "quirrell":
					DownedBossSystem.downedQuirrell = true;
					break;
				case "basilisk":
					DownedBossSystem.downedBasilisk = true;
					break;
				case "aragog":
					DownedBossSystem.downedAragog = true;
					break;
				case "fluffy":
					DownedBossSystem.downedFluffy = true;
					break;
				case "horntail":
					DownedBossSystem.downedHorntail = true;
					break;
				case "umbridge":
					DownedBossSystem.downedUmbridge = true;
					break;
				case "fenrir":
					DownedBossSystem.downedFenrir = true;
					break;
				case "bellatrix":
					DownedBossSystem.downedBellatrix = true;
					break;
				case "barty":
				case "bartycrouch":
					DownedBossSystem.downedBartyCrouch = true;
					break;
				case "dementor":
				case "dementorking":
					DownedBossSystem.downedDementorKing = true;
					break;
				case "voldemort":
				case "final":
					DownedBossSystem.downedVoldemort = true;
					break;
				case "early":
					DownedBossSystem.downedTroll = true;
					DownedBossSystem.downedQuirrell = true;
					DownedBossSystem.downedBasilisk = true;
					break;
				case "mid":
					DownedBossSystem.downedAragog = true;
					DownedBossSystem.downedFluffy = true;
					DownedBossSystem.downedHorntail = true;
					break;
				default:
					caller.Reply("Usage: /wwdebug bossflag <troll|quirrell|basilisk|aragog|fluffy|horntail|umbridge|fenrir|bellatrix|barty|dementor|voldemort|early|mid|status>", Color.Yellow);
					return;
			}

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);

			caller.Reply($"Granted Wizarding World boss QA flag: {boss}. Troll={DownedBossSystem.downedTroll}, Quirrell={DownedBossSystem.downedQuirrell}, Basilisk={DownedBossSystem.downedBasilisk}, Aragog={DownedBossSystem.downedAragog}, Fluffy={DownedBossSystem.downedFluffy}, Horntail={DownedBossSystem.downedHorntail}, Umbridge={DownedBossSystem.downedUmbridge}, Fenrir={DownedBossSystem.downedFenrir}, Bellatrix={DownedBossSystem.downedBellatrix}, Barty={DownedBossSystem.downedBartyCrouch}, Dementor={DownedBossSystem.downedDementorKing}, Voldemort={DownedBossSystem.downedVoldemort}. Vanilla progression flags were not changed; run the matching /wwdebug route command to see remaining gates.", new Color(120, 255, 120));
		}

		private void SetBattleGate(CommandCaller caller, string[] args)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				caller.Reply("Battle QA gates are disabled on multiplayer clients.", Color.Red);
				return;
			}

			if (args.Length < 2 || args[1].Equals("status", System.StringComparison.OrdinalIgnoreCase))
			{
				caller.Reply($"Battle of Hogwarts QA state: wardsDefended={HogwartsWardSystem.wardsDefended}, unlocked={BattleOfHogwartsSystem.battleUnlocked}, active={BattleOfHogwartsSystem.battleActive}, wins={BattleOfHogwartsSystem.battlesWon}, Nagini={HorcruxHuntSystem.naginiDefeated}. Use /wwdebug battle <unlock|win>.", Color.Yellow);
				return;
			}

			string gate = args[1].ToLowerInvariant();
			switch (gate)
			{
				case "unlock":
				case "unlocked":
					HogwartsWardSystem.wardsDefended = System.Math.Max(HogwartsWardSystem.wardsDefended, 1);
					BattleOfHogwartsSystem.battleUnlocked = true;
					break;
				case "win":
				case "won":
				case "ready":
					HogwartsWardSystem.wardsDefended = System.Math.Max(HogwartsWardSystem.wardsDefended, 1);
					BattleOfHogwartsSystem.battleUnlocked = true;
					BattleOfHogwartsSystem.battlesWon = System.Math.Max(BattleOfHogwartsSystem.battlesWon, 1);
					BattleOfHogwartsSystem.battleActive = false;
					BattleOfHogwartsSystem.battlePhase = 3;
					BattleOfHogwartsSystem.phaseTimer = 0;
					BattleOfHogwartsSystem.objectivesCompleted = 0;
					HorcruxHuntSystem.naginiDefeated = true;
					break;
				default:
					caller.Reply("Usage: /wwdebug battle <unlock|win|status>", Color.Yellow);
					return;
			}

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);

			caller.Reply($"Granted Battle of Hogwarts QA gate: {gate}. wardsDefended={HogwartsWardSystem.wardsDefended}, unlocked={BattleOfHogwartsSystem.battleUnlocked}, wins={BattleOfHogwartsSystem.battlesWon}, Nagini={HorcruxHuntSystem.naginiDefeated}. Run /wwdebug voldemort to see remaining gates.", new Color(120, 255, 120));
		}

		private void QueueAudioQa(CommandCaller caller, string[] args)
		{
			if (args.Length < 2)
			{
				caller.Reply("Usage: /wwdebug audio <new|spells|ambient|enemies|all>", Color.Yellow);
				return;
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				caller.Reply("Audio QA is disabled on multiplayer clients.", Color.Red);
				return;
			}

			string group = args[1].ToLowerInvariant();
			var cues = new List<AudioQaCue>();

			switch (group)
			{
				case "new":
					AddNewAudioCues(cues);
					break;
				case "spells":
					AddSpellAudioCues(cues);
					break;
				case "ambient":
					AddAmbientAudioCues(cues);
					break;
				case "enemies":
					AddEnemyAudioCues(cues);
					break;
				case "all":
					AddAmbientAudioCues(cues);
					AddSpellAudioCues(cues);
					AddEnemyAudioCues(cues);
					break;
				default:
					caller.Reply("Unknown audio group. Use: new, spells, ambient, enemies, all", Color.Red);
					return;
			}

			WizardAudioDebugSystem.Queue(cues);
			caller.Reply($"Queued audio QA group '{group}' with {cues.Count} sounds. Stand still and listen; each cue is labeled in chat.", new Color(120, 220, 255));
		}

		private static void AddNewAudioCues(List<AudioQaCue> cues)
		{
			cues.Add(new("Alohomora", WizardSoundStyles.Alohomora));
			cues.Add(new("Conjunctivitis", WizardSoundStyles.Conjunctivitis));
			cues.Add(new("Sectumsempra", WizardSoundStyles.Sectumsempra));
			cues.Add(new("Reparo", WizardSoundStyles.Reparo));
			cues.Add(new("Wingardium", WizardSoundStyles.Wingardium));
			cues.Add(new("Aguamenti", WizardSoundStyles.Aguamenti));
			cues.Add(new("MagicHum", WizardSoundStyles.MagicHum));
			cues.Add(new("ForestWind", WizardSoundStyles.ForestWind));
			cues.Add(new("CauldronBubble", WizardSoundStyles.CauldronBubble));
		}

		private static void AddSpellAudioCues(List<AudioQaCue> cues)
		{
			cues.Add(new("Accio", WizardSoundStyles.Accio));
			cues.Add(new("Aguamenti", WizardSoundStyles.Aguamenti));
			cues.Add(new("Alohomora", WizardSoundStyles.Alohomora));
			cues.Add(new("Apparition", WizardSoundStyles.Apparition));
			cues.Add(new("AvadaKedavra", WizardSoundStyles.AvadaKedavra));
			cues.Add(new("Bombarda", WizardSoundStyles.Bombarda));
			cues.Add(new("Conjunctivitis", WizardSoundStyles.Conjunctivitis));
			cues.Add(new("Crucio", WizardSoundStyles.Crucio));
			cues.Add(new("ExpectoPatronum", WizardSoundStyles.ExpectoPatronum));
			cues.Add(new("Expelliarmus", WizardSoundStyles.Expelliarmus));
			cues.Add(new("Fiendfyre", WizardSoundStyles.Fiendfyre));
			cues.Add(new("FiniteIncantatem", WizardSoundStyles.FiniteIncantatem));
			cues.Add(new("Impedimenta", WizardSoundStyles.Impedimenta));
			cues.Add(new("Incendio", WizardSoundStyles.Incendio));
			cues.Add(new("Levicorpus", WizardSoundStyles.Levicorpus));
			cues.Add(new("Lumos", WizardSoundStyles.Lumos));
			cues.Add(new("Protego", WizardSoundStyles.Protego));
			cues.Add(new("Reducto", WizardSoundStyles.Reducto));
			cues.Add(new("Reparo", WizardSoundStyles.Reparo));
			cues.Add(new("Riddikulus", WizardSoundStyles.Riddikulus));
			cues.Add(new("Sectumsempra", WizardSoundStyles.Sectumsempra));
			cues.Add(new("Stupefy", WizardSoundStyles.Stupefy));
			cues.Add(new("Wingardium", WizardSoundStyles.Wingardium));
		}

		private static void AddAmbientAudioCues(List<AudioQaCue> cues)
		{
			cues.Add(new("OwlHoot", WizardSoundStyles.OwlHoot));
			cues.Add(new("MagicHum", WizardSoundStyles.MagicHum));
			cues.Add(new("ForestWind", WizardSoundStyles.ForestWind));
			cues.Add(new("CauldronBubble", WizardSoundStyles.CauldronBubble));
		}

		private static void AddEnemyAudioCues(List<AudioQaCue> cues)
		{
			cues.Add(new("TrollRoar", WizardSoundStyles.TrollRoar));
			cues.Add(new("SpiderHiss", WizardSoundStyles.SpiderHiss));
			cues.Add(new("DragonRoar", WizardSoundStyles.DragonRoar));
			cues.Add(new("WerewolfHowl", WizardSoundStyles.WerewolfHowl));
			cues.Add(new("DementorScream", WizardSoundStyles.DementorScream));
			cues.Add(new("GhostWail", WizardSoundStyles.GhostWail));
		}

		private static void GrantIntroKit(CommandCaller caller)
		{
			caller.Player.GetModPlayer<HogwartsLetterSystem>().receivedLetter = true;
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.HogwartsLetter>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Weapons.Wands.OakWand>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Placeable.EnchantingTableItem>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 10);
			GrantItem(caller, ItemID.Wood, 30);
			GrantItem(caller, ItemID.FallenStar, 8);
			GrantItem(caller, ItemID.Torch, 10);
			GrantItem(caller, ItemID.Amethyst, 3);
			GrantItem(caller, ItemID.StoneBlock, 25);
		}

		private static void GrantMidBaseKit(CommandCaller caller)
		{
			GrantIntroKit(caller);
			GrantQaTestWeapon(caller);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 10);
		}

		private static void GrantPostHorntailKit(CommandCaller caller)
		{
			GrantMidBaseKit(caller);
			GrantItem(caller, ModContent.ItemType<Content.Items.Placeable.GobletOfFire>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.StMungosPass>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.ForestLantern>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Accessories.BossCompass>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.WizardsAlmanac>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.GoldenEgg>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.UnicornBlood>(), 2);
		}

		private static void GrantUmbridgeKit(CommandCaller caller)
		{
			GrantPostHorntailKit(caller);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.UmbridgeSummonItem>(), 2);
			GrantItem(caller, ItemID.Book, 3);
			GrantItem(caller, ItemID.PinkGel, 10);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 10);
		}

		private static void GrantFenrirKit(CommandCaller caller)
		{
			GrantUmbridgeKit(caller);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.FenrirSummonItem>(), 2);
			GrantItem(caller, ItemID.MoonCharm, 1);
			GrantItem(caller, ItemID.SoulofNight, 10);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 10);
		}

		private static void GrantBellatrixKit(CommandCaller caller)
		{
			GrantFenrirKit(caller);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.BellatrixSummonItem>(), 2);
			GrantItem(caller, ItemID.Bone, 30);
			GrantItem(caller, ItemID.SoulofNight, 10);
			GrantItem(caller, ItemID.DarkShard, 2);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 15);
		}

		private static void GrantBartyKit(CommandCaller caller)
		{
			GrantBellatrixKit(caller);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.BartyCrouchSummonItem>(), 2);
			GrantItem(caller, ItemID.Bottle, 1);
			GrantItem(caller, ItemID.Deathweed, 5);
			GrantItem(caller, ItemID.SoulofNight, 5);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 12);
		}

		private static void GrantDementorKit(CommandCaller caller)
		{
			GrantBartyKit(caller);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.DementorKingSummonItem>(), 2);
			GrantItem(caller, ItemID.SoulofNight, 15);
			GrantItem(caller, ItemID.Ectoplasm, 10);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 25);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.UnicornBlood>(), 3);
		}

		private static void GrantVoldemortKit(CommandCaller caller)
		{
			GrantDementorKit(caller);
			GrantQaTestWeapon(caller);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.VoldemortSummonItem>(), 2);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.HorcruxTracker>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Accessories.BossCompass>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Consumables.WizardsAlmanac>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Accessories.BasiliskFang>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Accessories.RiddlesDiary>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Accessories.SlytherinsLocket>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Accessories.HufflepuffsCup>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Accessories.DiademOfRavenclaw>(), 1);
			GrantItem(caller, ItemID.Bone, 30);
			GrantItem(caller, ItemID.SoulofNight, 15);
			GrantItem(caller, ItemID.Ectoplasm, 10);
			GrantItem(caller, ItemID.LunarTabletFragment, 5);
		}

		private static void GrantHallowsKit(CommandCaller caller)
		{
			HorcruxHuntSystem.diaryDestroyed = true;
			HorcruxHuntSystem.locketDestroyed = true;
			HorcruxHuntSystem.cupDestroyed = true;
			HorcruxHuntSystem.diademDestroyed = true;
			HorcruxHuntSystem.horcruxesDestroyed = 4;
			HorcruxHuntSystem.naginiDefeated = true;
			DownedBossSystem.downedDementorKing = true;
			DownedBossSystem.downedVoldemort = true;
			HallowsSystem.invisibilityCloakClaimed = true;
			HallowsSystem.resurrectionStoneAwakened = true;

			GrantItem(caller, ModContent.ItemType<Content.Items.Weapons.Wands.ElderWand>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Accessories.InvisibilityCloak>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Accessories.ResurrectionStone>(), 1);
			GrantItem(caller, ModContent.ItemType<Content.Items.Accessories.GauntsRing>(), 1);

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);
		}

		private static void CompleteHallowsQa(CommandCaller caller)
		{
			GrantHallowsKit(caller);
			HallowsSystem.hallowsAttuned = true;

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);
		}

		private static void GrantQaTestWeapon(CommandCaller caller)
		{
			GrantItem(caller, ModContent.ItemType<Content.Items.Weapons.Wands.QATestWand>(), 1);
		}

		private static void SetDebugGodMode(Player player, bool enabled)
		{
			player.GetModPlayer<WizardPlayer>().debugGodMode = enabled;
			if (!enabled)
				return;

			player.statLife = player.statLifeMax2;
			player.statMana = player.statManaMax2;
			player.SetImmuneTimeForAllTypes(60);
		}

		private static void GrantBasicCombatPrep(CommandCaller caller, int minimumLifeMax)
		{
			Player player = caller.Player;
			if (player.statLifeMax < minimumLifeMax)
				player.statLifeMax = minimumLifeMax;

			player.statLifeMax2 = System.Math.Max(player.statLifeMax2, minimumLifeMax);
			player.statLife = player.statLifeMax2;
			if (player.statManaMax < 80)
				player.statManaMax = 80;

			player.statManaMax2 = System.Math.Max(player.statManaMax2, 80);
			player.statMana = player.statManaMax2;

			EquipIfEmpty(caller, 0, ItemID.IronHelmet);
			EquipIfEmpty(caller, 1, ItemID.IronChainmail);
			EquipIfEmpty(caller, 2, ItemID.IronGreaves);
			GrantItem(caller, ItemID.HealingPotion, 15);
			GrantItem(caller, ItemID.IronskinPotion, 3);
			GrantItem(caller, ItemID.RegenerationPotion, 3);
			GrantItem(caller, ItemID.SwiftnessPotion, 3);
			GrantItem(caller, ItemID.ManaRegenerationPotion, 2);
			GrantItem(caller, ItemID.LesserManaPotion, 20);
			GrantItem(caller, ItemID.WoodPlatform, 120);
			GrantItem(caller, ItemID.Campfire, 1);
		}

		private static void EquipIfEmpty(CommandCaller caller, int armorSlot, int itemType)
		{
			Player player = caller.Player;
			if (player.armor[armorSlot].IsAir)
				player.armor[armorSlot].SetDefaults(itemType);
			else
				GrantItem(caller, itemType, 1);
		}

		private static void GrantItem(CommandCaller caller, int itemType, int stack)
		{
			Player player = caller.Player;
			player.QuickSpawnItem(player.GetSource_GiftOrReward(), itemType, stack);
		}

		private static void AppendCheck(StringBuilder sb, string label, bool passed, string detail)
		{
			sb.AppendLine($"    [{(passed ? "OK" : "WAIT")}] {label} - {detail}");
		}

		private static string YesNo(bool value)
		{
			return value ? "yes" : "no";
		}

		private static bool HasItem(Player player, int itemType)
		{
			return CountItem(player, itemType) > 0;
		}

		private static int CountItem(Player player, int itemType)
		{
			int count = 0;
			foreach (Item item in player.inventory)
			{
				if (!item.IsAir && item.type == itemType)
					count += item.stack;
			}

			return count;
		}

		private static int CountAnyWood(Player player)
		{
			return CountItem(player, ItemID.Wood)
				+ CountItem(player, ItemID.BorealWood)
				+ CountItem(player, ItemID.RichMahogany)
				+ CountItem(player, ItemID.Ebonwood)
				+ CountItem(player, ItemID.Shadewood)
				+ CountItem(player, ItemID.Pearlwood)
				+ CountItem(player, ItemID.SpookyWood)
				+ CountItem(player, ItemID.DynastyWood)
				+ CountItem(player, ItemID.PalmWood)
				+ CountItem(player, ItemID.AshWood);
		}

		private static int CountEvilChunks(Player player)
		{
			return CountItem(player, ItemID.RottenChunk) + CountItem(player, ItemID.Vertebrae);
		}

		private static bool HasSpellWand(Player player)
		{
			DamageClass spellDamage = ModContent.GetInstance<Content.DamageClasses.SpellDamage>();
			foreach (Item item in player.inventory)
			{
				if (!item.IsAir && item.DamageType == spellDamage)
					return true;
			}

			return false;
		}

		private static int CountTownNpcs()
		{
			int count = 0;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.active && npc.townNPC && npc.type != NPCID.OldMan)
					count++;
			}

			return count;
		}

		private static string GetEarlyNextStep(
			HogwartsLetterSystem letter,
			Player player,
			bool hasLetter,
			bool hasOakWand,
			bool hasEnchantingTable,
			bool canCraftTable,
			bool hasTrollSummon,
			bool canCraftTrollSummon,
			bool hasQuirrellSummon,
			bool canCraftQuirrellSummon,
			bool hasBasiliskSummon,
			bool canCraftBasiliskSummon)
		{
			bool earlyVanillaBossDowned = NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedSlimeKing;

			bool introBypassed = hasLetter || hasOakWand || hasEnchantingTable;
			if (!letter.receivedLetter && !introBypassed)
			{
				if (player.statLifeMax < 120 && !earlyVanillaBossDowned)
					return "Reach 120 max life with a Life Crystal, or defeat an early vanilla boss, to trigger the Hogwarts Letter.";

				return "The Hogwarts Letter trigger is satisfied; wait a moment in-world for the owl delivery update.";
			}

			if (!hasOakWand)
				return "The starter Oak Wand is missing; check inventory, or use /wwdebug kit intro in a disposable QA world.";

			if (!hasEnchantingTable)
			{
				if (canCraftTable)
					return "Craft and place the Enchanting Table. If one is already placed, continue to the Troll summon check.";

				return "Gather wood, Fallen Stars, torches, and either Amethyst or Stone Blocks for the Enchanting Table.";
			}

			if (!DownedBossSystem.downedTroll)
			{
				if (hasTrollSummon)
					return "Use the Smelly Sock and verify the Mountain Troll fight, loot, and Hagrid/Ollivander unlocks afterward.";

				if (canCraftTrollSummon)
					return "Craft a Smelly Sock at the Enchanting Table, then fight the Mountain Troll.";

				return "Gather Silk, Rotten Chunk or Vertebra, and one Fallen Star for the Smelly Sock.";
			}

			if (!NPC.downedBoss1)
				return "Defeat Eye of Cthulhu; it is now the vanilla gate before Quirrell.";

			if (!DownedBossSystem.downedQuirrell)
			{
				if (hasQuirrellSummon)
					return "Use the Suspicious Turban and verify Quirrell's fight, loot, and Basilisk handoff.";

				if (canCraftQuirrellSummon)
					return "Craft a Suspicious Turban at the Enchanting Table, then fight Quirrell.";

				return "Gather Silk, Amethyst, and Fallen Stars for the Suspicious Turban.";
			}

			if (!NPC.downedBoss3)
				return "Defeat Skeletron; it is now the vanilla gate before Basilisk.";

			if (!DownedBossSystem.downedBasilisk)
			{
				if (hasBasiliskSummon)
					return "Use the Serpent's Diary and verify the Basilisk fight plus post-Basilisk shop unlocks.";

				if (canCraftBasiliskSummon)
					return "Craft a Serpent's Diary at the Enchanting Table, then fight Basilisk.";

				return "Gather a Book, Bones, and Cobwebs for the Serpent's Diary.";
			}

			return "The early arc is complete; move QA focus to Aragog, Fluffy, Horntail, and hardmode pacing.";
		}

		private static string GetMidNextStep(
			bool aragogActive,
			bool fluffyActive,
			bool horntailActive,
			bool hasAragogSummon,
			bool canCraftAragog,
			bool hasFluffySummon,
			bool canCraftFluffy,
			bool hasHorntailSummon,
			bool canCraftHorntail)
		{
			bool anyMech = WizardConditions.AnyMechBossDowned;

			if (!DownedBossSystem.downedBasilisk)
				return "Finish or QA-flag the early arc first; Aragog is the next recommended Wizarding World boss.";

			if (!Main.hardMode)
				return "Enter Hardmode, then verify Aragog and the Hardmode Hagrid/Ollivander shop updates.";

			if (!DownedBossSystem.downedAragog)
			{
				if (aragogActive)
					return "Aragog is already active; finish the current fight, then rerun /wwdebug mid.";

				if (hasAragogSummon)
					return "Use the Acromantula Egg and verify Aragog's fight, loot, and Hagrid dialogue afterward.";

				if (canCraftAragog)
					return "Craft an Acromantula Egg at the Enchanting Table, then fight Aragog.";

				return "Gather Cobwebs, Spider Fangs, and Essence of Magic for the Acromantula Egg.";
			}

			if (!anyMech)
				return "Defeat one mechanical boss; this opens the Fluffy and Horntail summon gates.";

			if (!DownedBossSystem.downedFluffy)
			{
				if (fluffyActive)
					return "Fluffy is already active; finish the current fight, then rerun /wwdebug mid.";

				if (hasFluffySummon)
					return "Use the Enchanted Flute and verify Fluffy's fight, loot, and post-mech shop state.";

				if (canCraftFluffy)
					return "Craft an Enchanted Flute at the Enchanting Table, then fight Fluffy.";

				return "Gather Gold Bars, Souls of Sight, and Essence of Magic for the Enchanted Flute.";
			}

			if (!DownedBossSystem.downedHorntail)
			{
				if (horntailActive)
					return "Horntail is already active; finish the current fight, then rerun /wwdebug mid.";

				if (hasHorntailSummon)
					return "Use the Dragon Egg (Cracked) and verify Horntail's fight, loot, and post-Horntail unlocks.";

				if (canCraftHorntail)
					return "Craft a Dragon Egg (Cracked) at the Enchanting Table, then fight Horntail.";

				return "Gather Hellstone Bars, Souls of Might, and Souls of Flight for the first Dragon Egg (Cracked).";
			}

			return "The mid entrance arc is complete; next QA focus should move to post-Horntail systems and late Hardmode pacing.";
		}

		private static string GetPostHorntailNextStep(
			bool hasGoblet,
			bool hasStMungosPass,
			bool hasForestLantern,
			bool hasBossCompass,
			bool hasAlmanac)
		{
			if (!DownedBossSystem.downedHorntail)
				return "Defeat or QA-flag Horntail first; this view is for post-Horntail unlock checks.";

			if (!hasGoblet || !hasStMungosPass || !hasForestLantern || !hasBossCompass || !hasAlmanac)
				return "Use /wwdebug kit posthorntail in a disposable QA world, then rerun /wwdebug posthorntail.";

			if (TriwizardTournamentSystem.CanUnlock())
				return "Use the Goblet of Fire and verify the Triwizard unlock text plus task 1 handoff.";

			if (!TriwizardTournamentSystem.tournamentUnlocked)
				return "Triwizard is not unlocked, but CanUnlock is false; inspect Goblet use feedback.";

			if (StMungosTriageSystem.CanUnlock())
				return "Use the St Mungo's Pass and verify hospital unlock text plus Healer move-in eligibility.";

			if (!StMungosTriageSystem.hospitalUnlocked)
				return "St Mungo's is not unlocked, but CanUnlock is false; inspect pass use feedback.";

			if (ForestExpeditionSystem.CanUnlock())
				return "Use the Forest Lantern once to unlock expeditions, then rerun this check.";

			if (!ForestExpeditionSystem.expeditionUnlocked)
				return "Forest expeditions are not unlocked yet; use the Forest Lantern to verify the post-Basilisk forest entry.";

			if (TriwizardTournamentSystem.taskActive && TriwizardTournamentSystem.currentTask == 1)
				return "Use the Golden Egg to complete Triwizard Task 1, then rerun /wwdebug posthorntail.";

			if (!TriwizardTournamentSystem.task1Complete)
				return "Use the Goblet of Fire to start Triwizard Task 1, then use the Golden Egg to complete it.";

			if (StMungosTriageSystem.missionActive)
				return "Complete the active St Mungo's triage mission by defeating the ward nodes.";

			if (StMungosTriageSystem.missionsCompleted == 0)
				return "Use the St Mungo's Pass to start and verify the first triage mission.";

			return "Post-Horntail thin-slice QA is complete; next discussion can move to Umbridge and late-hardmode pacing.";
		}

		private static string GetUmbridgeNextStep(bool active, bool hasSummon, bool canCraft)
		{
			if (!Main.hardMode)
				return "Enable or reach Hardmode first; Umbridge should not be available before Hardmode.";

			if (!WizardConditions.AnyMechBossDowned)
				return "Defeat or QA-gate one mechanical boss; this is the vanilla side of the Umbridge gate.";

			if (!DownedBossSystem.downedHorntail)
				return "Finish Horntail first; Umbridge is the next Wizarding World boss after Horntail.";

			if (DownedBossSystem.downedUmbridge)
				return "Umbridge is defeated and the route now hands off to Fenrir. If this is after Save & Exit, persistence is verified and this QA slice is complete.";

			if (active)
				return "Umbridge is active; finish the fight and verify the defeat flag plus loot.";

			if (hasSummon)
				return "Use the Educational Decree and verify Umbridge summon, phase text, defeat flag, and loot.";

			if (canCraft)
				return "Craft an Educational Decree at the Enchanting Table, then use it to summon Umbridge.";

			return "Use /wwdebug kit umbridge in a disposable QA world, or gather Book x3, Pink Gel x10, and Essence of Magic x10.";
		}

		private static string GetFenrirNextStep(bool active, bool hasSummon, bool canCraft)
		{
			if (!Main.hardMode)
				return "Enable or reach Hardmode first; Fenrir should not be available before Hardmode.";

			if (!DownedBossSystem.downedUmbridge)
				return "Finish Umbridge first; Fenrir is the next Wizarding World boss after Umbridge.";

			if (DownedBossSystem.downedFenrir)
				return "Fenrir is defeated and the route now hands off to Bellatrix. If this is after Save & Exit, persistence is verified and this QA slice is complete.";

			if (active)
				return "Fenrir is active; finish the fight and verify the defeat flag plus loot.";

			if (!Main.bloodMoon)
				return "Trigger or wait for a Blood Moon. For QA, use /wwdebug vanilla bloodmoon, then rerun /wwdebug fenrir.";

			if (hasSummon)
				return "Use the Bloodied Claw and verify Fenrir summon, phase behavior, defeat flag, and loot.";

			if (canCraft)
				return "Craft a Bloodied Claw at the Enchanting Table, then use it during the Blood Moon.";

			return "Use /wwdebug kit fenrir in a disposable QA world, or gather Moon Charm x1, Souls of Night x10, and Essence of Magic x10.";
		}

		private static string GetBellatrixNextStep(bool active, bool hasSummon, bool canCraft)
		{
			if (!Main.hardMode)
				return "Enable or reach Hardmode first; Bellatrix should not be available before Hardmode.";

			if (!DownedBossSystem.downedFenrir)
				return "Finish Fenrir first; Bellatrix is the next Wizarding World boss after Fenrir.";

			if (!NPC.downedPlantBoss)
				return "Defeat or QA-gate Plantera; for QA use /wwdebug vanilla plantera, then rerun /wwdebug bellatrix.";

			if (DownedBossSystem.downedBellatrix)
				return "Bellatrix is defeated and the route now hands off to Barty Crouch Jr. If this is after Save & Exit, persistence is verified and this QA slice is complete.";

			if (active)
				return "Bellatrix is active; finish the fight and verify the defeat flag plus loot.";

			if (hasSummon)
				return "Use the Azkaban Prisoner Tag and verify Bellatrix summon, phase text, defeat flag, and loot.";

			if (canCraft)
				return "Craft an Azkaban Prisoner Tag at the Enchanting Table, then use it to summon Bellatrix.";

			return "Use /wwdebug kit bellatrix in a disposable QA world, or gather Bone x30, Souls of Night x10, Dark Shard x2, and Essence of Magic x15.";
		}

		private static string GetBartyNextStep(bool active, bool hasSummon, bool canCraft)
		{
			if (!Main.hardMode)
				return "Enable or reach Hardmode first; Barty should not be available before Hardmode.";

			if (!DownedBossSystem.downedBellatrix)
				return "Finish Bellatrix first; Barty Crouch Jr is the next Wizarding World boss after Bellatrix.";

			if (!NPC.downedPlantBoss)
				return "Defeat or QA-gate Plantera; for QA use /wwdebug vanilla plantera, then rerun /wwdebug barty.";

			if (DownedBossSystem.downedBartyCrouch)
				return "Barty is defeated and the route now hands off to Dementor King / Azkaban's Despair. If this is after Save & Exit, persistence is verified and this QA slice is complete.";

			if (active)
				return "Barty is active; finish the fight and verify the defeat flag plus loot.";

			if (hasSummon)
				return "Use the Suspicious Flask and verify Barty summon, phase behavior, defeat flag, and loot.";

			if (canCraft)
				return "Craft a Suspicious Flask at the Enchanting Table, then use it to summon Barty.";

			return "Use /wwdebug kit barty in a disposable QA world, or gather Bottle x1, Deathweed x5, Souls of Night x5, and Essence of Magic x12.";
		}

		private static string GetDementorNextStep(bool active, bool hasSummon, bool canCraft)
		{
			if (!Main.hardMode)
				return "Enable or reach Hardmode first; Azkaban's Despair should not be available before Hardmode.";

			if (!DownedBossSystem.downedBartyCrouch)
				return "Finish Barty first; Dementor King is the next Wizarding World boss after Barty.";

			if (!NPC.downedGolemBoss)
				return "Defeat or QA-gate Golem; for QA use /wwdebug vanilla golem, then rerun /wwdebug dementor.";

			if (DownedBossSystem.downedDementorKing)
				return "Dementor King is defeated and the route now hands off to Voldemort readiness. If this is after Save & Exit, persistence is verified and this QA slice is complete.";

			if (active)
				return "Azkaban's Despair is active; finish the fight and verify the defeat flag plus loot.";

			if (Main.dayTime)
				return "Wait until night, or use /wwdebug vanilla night, then rerun /wwdebug dementor.";

			if (hasSummon)
				return "Use the Frozen Soul Lantern and verify Dementor King summon, despair behavior, defeat flag, and loot.";

			if (canCraft)
				return "Craft a Frozen Soul Lantern at the Enchanting Table, then use it at night.";

			return "Use /wwdebug kit dementor in a disposable QA world, or gather Souls of Night x15, Ectoplasm x10, Essence of Magic x25, and Unicorn Blood x3.";
		}

		private static string GetVoldemortNextStep(bool active, bool hasSummon, bool canCraft)
		{
			if (!DownedBossSystem.downedDementorKing)
				return "Finish Dementor King first; Voldemort readiness starts after Azkaban's Despair.";

			if (!HorcruxHuntSystem.AllCoreHorcruxesDestroyed)
				return "Destroy all four core Horcruxes, or for QA use /wwdebug horcruxes core, then rerun /wwdebug voldemort.";

			if (BattleOfHogwartsSystem.battlesWon <= 0 || !HorcruxHuntSystem.naginiDefeated)
				return "Win the Battle of Hogwarts to resolve Nagini, or for QA use /wwdebug battle win, then rerun /wwdebug voldemort.";

			if (!NPC.downedAncientCultist)
				return "Defeat or QA-gate Lunatic Cultist; for QA use /wwdebug vanilla cultist, then rerun /wwdebug voldemort.";

			if (DownedBossSystem.downedVoldemort)
				return "Voldemort is defeated. If this is after Save & Exit, persistence is verified and this QA slice is complete.";

			if (active)
				return "Lord Voldemort is active; finish the fight and verify the defeat flag plus loot.";

			if (Main.dayTime)
				return "Wait until night, or use /wwdebug vanilla night, then rerun /wwdebug voldemort.";

			if (hasSummon)
				return "Use the Dark Mark boss item and verify Voldemort summon, phase behavior, defeat flag, and loot.";

			if (canCraft)
				return "Craft a Dark Mark at the Enchanting Table, then use it at night.";

			return "Use /wwdebug kit voldemort in a disposable QA world, or gather Bone x30, Souls of Night x15, Ectoplasm x10, and Lunar Tablet Fragment x5.";
		}

		private static string GetHallowsNextStep(Player player)
		{
			if (!HorcruxHuntSystem.AllCoreHorcruxesDestroyed)
				return "Destroy all four core Horcruxes before the Hallows route can continue.";

			if (!DownedBossSystem.downedDementorKing)
				return "Defeat Dementor King first; Dumbledore should not entrust the true Cloak before Azkaban's Despair falls.";

			if (HallowsSystem.CanClaimInvisibilityCloak)
				return "Talk to Dumbledore and click Receive Cloak, then rerun /wwdebug hallows.";

			if (!DownedBossSystem.downedVoldemort)
				return "Defeat Voldemort before purifying Gaunt's Ring into the Resurrection Stone.";

			if (!HallowsSystem.resurrectionStoneAwakened && !HallowsSystem.PlayerHasGauntsRing(player))
				return "Carry Gaunt's Ring, then talk to Dumbledore to purify it.";

			if (HallowsSystem.CanPurifyGauntsRing(player))
				return "Talk to Dumbledore and click Purify Ring, then rerun /wwdebug hallows.";

			if (!HallowsSystem.hallowsAttuned)
				return "Keep Elder Wand in inventory and equip the true Invisibility Cloak plus Resurrection Stone to trigger Master of Death.";

			return "Hallows route is complete. If this is after Save & Exit, persistence is verified and this QA slice is complete.";
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
            Player player = caller.Player;
            var sb = new StringBuilder();
            sb.AppendLine("[Deathly Hallows State]");
            sb.AppendLine($"  Cloak claimed from Dumbledore: {HallowsSystem.invisibilityCloakClaimed}");
            sb.AppendLine($"  Resurrection Stone awakened: {HallowsSystem.resurrectionStoneAwakened}");
            sb.AppendLine($"  Master of Death attuned: {HallowsSystem.hallowsAttuned}");
            sb.AppendLine($"  Dumbledore present: {NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.Town.Dumbledore>())}");

            sb.AppendLine("  Inventory / equipment:");
            AppendCheck(sb, "Elder Wand in inventory", HasItem(player, ModContent.ItemType<Content.Items.Weapons.Wands.ElderWand>()),
                "Elder Wand drops from Voldemort. It sets the Hallow flag while in inventory.");
            AppendCheck(sb, "True Invisibility Cloak in inventory", HasItem(player, ModContent.ItemType<Content.Items.Accessories.InvisibilityCloak>()),
                "Claim from Dumbledore after Dementor King plus core Horcruxes.");
            AppendCheck(sb, "Resurrection Stone in inventory", HasItem(player, ModContent.ItemType<Content.Items.Accessories.ResurrectionStone>()),
                "Purify Gaunt's Ring at Dumbledore after Voldemort.");
            AppendCheck(sb, "Gaunt's Ring carried/equipped", HallowsSystem.PlayerHasGauntsRing(player),
                "Required before Dumbledore can purify it into the Resurrection Stone.");

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            sb.AppendLine("  Active player flags:");
            sb.AppendLine($"  [Player] hasElderWand: {wp.hasElderWand}");
            sb.AppendLine($"  [Player] hasInvisibilityCloak: {wp.hasInvisibilityCloak}");
            sb.AppendLine($"  [Player] hasResurrectionStone: {wp.hasResurrectionStone}");
            sb.AppendLine($"  [Player] hasGauntsRing: {wp.hasGauntsRing}");
            sb.AppendLine($"  [Player] hasDemiguiseCloak: {wp.hasDemiguiseCloak}");
            sb.AppendLine($"  [Player] hasDeathlyHallows: {wp.hasDeathlyHallows}");

            sb.AppendLine($"  Can claim cloak: {HallowsSystem.CanClaimInvisibilityCloak}");
            sb.AppendLine($"  Can purify ring: {HallowsSystem.CanPurifyGauntsRing(player)}");

            string guidance = HallowsSystem.GetDumbledoreGuidance(player);
            sb.AppendLine($"  Dumbledore says: \"{guidance}\"");
            sb.AppendLine("  Suggested next step:");
            sb.AppendLine($"    {GetHallowsNextStep(player)}");

            caller.Reply(sb.ToString(), new Color(210, 210, 255));
        }

        private void HandleHallows(CommandCaller caller, string[] args)
        {
            if (args.Length < 2 || args[1].Equals("status", System.StringComparison.OrdinalIgnoreCase))
            {
                PrintHallows(caller);
                return;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                caller.Reply("Hallows QA shortcuts are disabled on multiplayer clients.", Color.Red);
                return;
            }

            Player player = caller.Player;
            string action = args[1].ToLowerInvariant();
            switch (action)
            {
                case "claim":
                case "cloak":
                    if (!HallowsSystem.TryClaimInvisibilityCloak(player))
                    {
                        caller.Reply("Cannot claim the true Invisibility Cloak yet. Run /wwdebug hallows for the missing gate.", Color.Yellow);
                        return;
                    }
                    caller.Reply("Claimed QA Hallows reward: true Invisibility Cloak. Run /wwdebug hallows.", new Color(120, 255, 120));
                    break;
                case "purify":
                case "stone":
                    HallowsSystem.resurrectionStoneAwakened = true;
                    GrantItem(caller, ModContent.ItemType<Content.Items.Accessories.ResurrectionStone>(), 1);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                    caller.Reply("Purified QA Hallows reward: Resurrection Stone. This shortcut bypasses Gaunt's Ring detection; run /wwdebug hallows.", new Color(120, 255, 120));
                    break;
                case "kit":
                    GrantHallowsKit(caller);
                    caller.Reply("Granted QA Hallows kit: Elder Wand, true Invisibility Cloak, Resurrection Stone, and Gaunt's Ring fallback. Equip the Cloak and Stone, keep Elder Wand in inventory, then run /wwdebug hallows.", new Color(120, 255, 120));
                    break;
                case "complete":
                case "attune":
                case "done":
                    CompleteHallowsQa(caller);
                    caller.Reply("Completed QA Hallows route: Master of Death attuned. Note: [Player] hasDeathlyHallows is still a live equipment flag and may be false unless the trio is equipped.", new Color(120, 255, 120));
                    break;
                default:
                    caller.Reply("Usage: /wwdebug hallows <status|claim|purify|kit|complete>", Color.Yellow);
                    break;
            }
        }

        private void HandleHorcruxes(CommandCaller caller, string[] args)
        {
            if (args.Length < 2 || args[1].Equals("status", System.StringComparison.OrdinalIgnoreCase))
            {
                PrintHorcruxes(caller);
                return;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                caller.Reply("Horcrux QA gates are disabled on multiplayer clients.", Color.Red);
                return;
            }

            string gate = args[1].ToLowerInvariant();
            switch (gate)
            {
                case "core":
                case "cores":
                    HorcruxHuntSystem.diaryDestroyed = true;
                    HorcruxHuntSystem.locketDestroyed = true;
                    HorcruxHuntSystem.cupDestroyed = true;
                    HorcruxHuntSystem.diademDestroyed = true;
                    HorcruxHuntSystem.horcruxesDestroyed = 4;
                    break;
                case "all":
                case "ready":
                    HorcruxHuntSystem.diaryDestroyed = true;
                    HorcruxHuntSystem.locketDestroyed = true;
                    HorcruxHuntSystem.cupDestroyed = true;
                    HorcruxHuntSystem.diademDestroyed = true;
                    HorcruxHuntSystem.horcruxesDestroyed = 4;
                    HorcruxHuntSystem.naginiDefeated = true;
                    break;
                case "nagini":
                    HorcruxHuntSystem.naginiDefeated = true;
                    break;
                default:
                    caller.Reply("Usage: /wwdebug horcruxes <core|all|nagini|status>", Color.Yellow);
                    return;
            }

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);

            caller.Reply($"Granted Horcrux QA gate: {gate}. Core destroyed={HorcruxHuntSystem.AllCoreHorcruxesDestroyed}, Nagini={HorcruxHuntSystem.naginiDefeated}, power={(int)(HorcruxHuntSystem.GetVoldemortPowerMultiplier() * 100)}%. Run /wwdebug voldemort to see remaining gates.", new Color(120, 255, 120));
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
            sb.AppendLine($"  [Vanilla] Eye of Cthulhu: {NPC.downedBoss1}");
            sb.AppendLine($"  [Vanilla] Skeletron: {NPC.downedBoss3}");
            sb.AppendLine($"  [Vanilla] Mechanical boss: {WizardConditions.AnyMechBossDowned}");
            sb.AppendLine($"  [Vanilla] Plantera: {NPC.downedPlantBoss}");
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

	public readonly struct AudioQaCue
	{
		public AudioQaCue(string label, SoundStyle sound)
		{
			Label = label;
			Sound = sound;
		}

		public string Label { get; }
		public SoundStyle Sound { get; }
	}

	public class WizardAudioDebugSystem : ModSystem
	{
		private struct ScheduledCue
		{
			public ScheduledCue(int timer, AudioQaCue cue)
			{
				Timer = timer;
				Cue = cue;
			}

			public int Timer;
			public AudioQaCue Cue;
		}

		private static readonly List<ScheduledCue> ScheduledCues = new();

		public static void Queue(List<AudioQaCue> cues)
		{
			ScheduledCues.Clear();

			int timer = 30;
			foreach (AudioQaCue cue in cues)
			{
				ScheduledCues.Add(new ScheduledCue(timer, cue));
				timer += 100;
			}
		}

		public override void PostUpdateWorld()
		{
			if (Main.dedServ || Main.gameMenu || ScheduledCues.Count == 0)
				return;

			Player player = Main.LocalPlayer;
			if (player == null || !player.active || player.dead)
				return;

			for (int i = ScheduledCues.Count - 1; i >= 0; i--)
			{
				ScheduledCue scheduled = ScheduledCues[i];
				scheduled.Timer--;

				if (scheduled.Timer > 0)
				{
					ScheduledCues[i] = scheduled;
					continue;
				}

				SoundEngine.PlaySound(scheduled.Cue.Sound, player.Center);
				Main.NewText($"Audio QA: {scheduled.Cue.Label}", 120, 220, 255);
				ScheduledCues.RemoveAt(i);
			}
		}
	}
}
#endif
