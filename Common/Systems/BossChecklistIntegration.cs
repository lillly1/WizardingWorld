using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Integration with the BossChecklist mod (if installed).
	/// Registers our 3 bosses + Nagini mini-boss with BossChecklist
	/// so they appear in the boss progression list.
	/// Uses weak references — won't crash if BossChecklist isn't installed.
	/// </summary>
	public class BossChecklistIntegration : ModSystem
	{
		public override void PostSetupContent()
		{
			if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist))
				return;

			// Mountain Troll — Pre-Hardmode intro boss (difficulty ~1.5)
			bossChecklist.Call("LogBoss", Mod, nameof(Content.NPCs.Bosses.Troll.TrollBoss), 1.5f,
				() => DownedBossSystem.downedTroll, ModContent.NPCType<Content.NPCs.Bosses.Troll.TrollBoss>(),
				new System.Collections.Generic.Dictionary<string, object> {
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.TrollSummonItem>(),
					["displayName"] = "Mountain Troll", ["spawnInfo"] = "Use a Smelly Sock.",
				});

			// Basilisk — Pre-Hardmode, after Skeletron (difficulty ~4.5)
			bossChecklist.Call(
				"LogBoss",
				Mod,
				nameof(Content.NPCs.Bosses.Basilisk.BasiliskBoss),
				4.5f,
				() => DownedBossSystem.downedBasilisk,
				ModContent.NPCType<Content.NPCs.Bosses.Basilisk.BasiliskBoss>(),
				new System.Collections.Generic.Dictionary<string, object>
				{
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.BasiliskSummonItem>(),
					["displayName"] = "Basilisk",
					["spawnInfo"] = "Use a Serpent's Diary. Crafted at the Enchanting Table.",
				}
			);

			// Hungarian Horntail — Hardmode, after Mech Bosses (difficulty ~8)
			bossChecklist.Call(
				"LogBoss",
				Mod,
				nameof(Content.NPCs.Bosses.Horntail.HorntailBoss),
				8f,
				() => DownedBossSystem.downedHorntail,
				ModContent.NPCType<Content.NPCs.Bosses.Horntail.HorntailBoss>(),
				new System.Collections.Generic.Dictionary<string, object>
				{
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.HorntailSummonItem>(),
					["displayName"] = "Hungarian Horntail",
					["spawnInfo"] = "Use a Cracked Dragon Egg. Available from Hagrid or crafted.",
				}
			);

			// Lord Voldemort — Post-Cultist, TRUE FINAL BOSS (difficulty ~17)
			bossChecklist.Call(
				"LogBoss",
				Mod,
				nameof(Content.NPCs.Bosses.Voldemort.VoldemortBoss),
				17f, // Post-Cultist — TRUE FINAL BOSS (was 14f post-Golem)
				() => DownedBossSystem.downedVoldemort,
				ModContent.NPCType<Content.NPCs.Bosses.Voldemort.VoldemortBoss>(),
				new System.Collections.Generic.Dictionary<string, object>
				{
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.VoldemortSummonItem>(),
					["displayName"] = "Lord Voldemort",
					["spawnInfo"] = "Use a Dark Mark. Post-Cultist, night only. DESTROY HORCRUXES FIRST to weaken him (100%→40% power).",
				}
			);

			// Professor Quirrell — Pre-Hardmode, after Eye of Cthulhu (difficulty ~3.5)
			bossChecklist.Call(
				"LogBoss",
				Mod,
				nameof(Content.NPCs.Bosses.Quirrell.QuirrellBoss),
				3.5f,
				() => DownedBossSystem.downedQuirrell,
				ModContent.NPCType<Content.NPCs.Bosses.Quirrell.QuirrellBoss>(),
				new System.Collections.Generic.Dictionary<string, object>
				{
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.QuirrellSummonItem>(),
					["displayName"] = "Professor Quirrell",
					["spawnInfo"] = "Use a Suspicious Turban. Crafted at the Enchanting Table. Requires Eye of Cthulhu defeated.",
				}
			);

			// Dolores Umbridge — Hardmode, after Mech Bosses (difficulty ~9)
			bossChecklist.Call(
				"LogBoss",
				Mod,
				nameof(Content.NPCs.Bosses.Umbridge.UmbridgeBoss),
				9f,
				() => DownedBossSystem.downedUmbridge,
				ModContent.NPCType<Content.NPCs.Bosses.Umbridge.UmbridgeBoss>(),
				new System.Collections.Generic.Dictionary<string, object>
				{
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.UmbridgeSummonItem>(),
					["displayName"] = "Dolores Umbridge",
					["spawnInfo"] = "Use an Educational Decree. Crafted at the Enchanting Table. Requires a mechanical boss defeated.",
				}
			);

			// Aragog — Early Hardmode (difficulty ~6)
			bossChecklist.Call("LogBoss", Mod, nameof(Content.NPCs.Bosses.Aragog.AragogBoss), 6f,
				() => DownedBossSystem.downedAragog, ModContent.NPCType<Content.NPCs.Bosses.Aragog.AragogBoss>(),
				new System.Collections.Generic.Dictionary<string, object> {
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.AragogSummonItem>(),
					["displayName"] = "Aragog", ["spawnInfo"] = "Use an Acromantula Egg. Hardmode only.",
				});

			// Fluffy — Post-Mech Bosses (difficulty ~7.5)
			bossChecklist.Call("LogBoss", Mod, nameof(Content.NPCs.Bosses.Fluffy.FluffyBoss), 7.5f,
				() => DownedBossSystem.downedFluffy, ModContent.NPCType<Content.NPCs.Bosses.Fluffy.FluffyBoss>(),
				new System.Collections.Generic.Dictionary<string, object> {
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.FluffySummonItem>(),
					["displayName"] = "Fluffy", ["spawnInfo"] = "Use an Enchanted Flute. Requires mech boss defeated.",
				});

			// Fenrir Greyback — Blood Moon special (difficulty ~9.5)
			bossChecklist.Call("LogBoss", Mod, nameof(Content.NPCs.Bosses.Fenrir.FenrirBoss), 9.5f,
				() => DownedBossSystem.downedFenrir, ModContent.NPCType<Content.NPCs.Bosses.Fenrir.FenrirBoss>(),
				new System.Collections.Generic.Dictionary<string, object> {
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.FenrirSummonItem>(),
					["displayName"] = "Fenrir Greyback", ["spawnInfo"] = "Use a Bloodied Claw during Blood Moon only.",
				});

			// Bellatrix Lestrange — Post-Plantera (difficulty ~11)
			bossChecklist.Call("LogBoss", Mod, nameof(Content.NPCs.Bosses.Bellatrix.BellatrixBoss), 11f,
				() => DownedBossSystem.downedBellatrix, ModContent.NPCType<Content.NPCs.Bosses.Bellatrix.BellatrixBoss>(),
				new System.Collections.Generic.Dictionary<string, object> {
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.BellatrixSummonItem>(),
					["displayName"] = "Bellatrix Lestrange", ["spawnInfo"] = "Use an Azkaban Prisoner Tag. Requires Plantera defeated.",
				});

			// Barty Crouch Jr — Post-Plantera (difficulty ~12)
			bossChecklist.Call("LogBoss", Mod, nameof(Content.NPCs.Bosses.BartyCrouch.BartyCrouchBoss), 12f,
				() => DownedBossSystem.downedBartyCrouch, ModContent.NPCType<Content.NPCs.Bosses.BartyCrouch.BartyCrouchBoss>(),
				new System.Collections.Generic.Dictionary<string, object> {
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.BartyCrouchSummonItem>(),
					["displayName"] = "Barty Crouch Jr", ["spawnInfo"] = "Use a Suspicious Flask. Requires Plantera defeated.",
				});

			// Azkaban's Despair — Post-Golem (difficulty ~13, MOVED from post-Cultist)
			bossChecklist.Call("LogBoss", Mod, nameof(Content.NPCs.Bosses.DementorKing.DementorKingBoss), 13f,
				() => DownedBossSystem.downedDementorKing, ModContent.NPCType<Content.NPCs.Bosses.DementorKing.DementorKingBoss>(),
				new System.Collections.Generic.Dictionary<string, object> {
					["spawnItems"] = ModContent.ItemType<Content.Items.Consumables.DementorKingSummonItem>(),
					["displayName"] = "Azkaban's Despair", ["spawnInfo"] = "Use a Frozen Soul Lantern. Post-Golem, night only. The penultimate wizard boss.",
				});

			// Nagini — Mini-boss, registered as miniboss
			bossChecklist.Call(
				"LogMiniBoss",
				Mod,
				nameof(Content.NPCs.Enemies.Nagini),
				10f,
				() => DownedBossSystem.downedVoldemort, // Tied to Voldemort progression
				ModContent.NPCType<Content.NPCs.Enemies.Nagini>(),
				new System.Collections.Generic.Dictionary<string, object>
				{
					["displayName"] = "Nagini",
					["spawnInfo"] = "Rare spawn at night in Hardmode. Also appears during Death Eater Invasions.",
				}
			);
		}
	}
}
