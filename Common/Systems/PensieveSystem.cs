using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Pensieve System — Canon Tier A.
	///
	/// The Pensieve is the mod's lore wrapper and boss replay system.
	/// It solves the mixed-era problem: "All bosses are memories from
	/// different periods of wizarding history, experienced through the
	/// Pensieve found in the Room of Requirement."
	///
	/// Functions:
	/// 1. Boss replay — revisit defeated boss memories
	/// 2. Lore delivery — each memory has era/context description
	/// 3. Timeline explanation — justifies mixed-era content
	/// 4. Future: memory-based challenge modes, lore drops, story chapters
	///
	/// Implementation: Currently a data registry. Future: full UI with
	/// memory selection screen, difficulty modifiers, challenge rewards.
	/// </summary>
	public class PensieveSystem : ModSystem
	{
		public static readonly List<PensieveMemory> Memories = new();

		public override void PostSetupContent()
		{
			// Year 1 Memories
			Memories.Add(new PensieveMemory(
				"The Bathroom Troll",
				"Year One — Halloween Night",
				"A mountain troll was let into the castle by Professor Quirrell as a diversion. Three first-year students faced it alone in the girls' bathroom.",
				ModContent.NPCType<Content.NPCs.Bosses.Troll.TrollBoss>(),
				() => DownedBossSystem.downedTroll
			));

			Memories.Add(new PensieveMemory(
				"The Man with Two Faces",
				"Year One — The Mirror Chamber",
				"Professor Quirrell revealed his terrible secret: the face of the Dark Lord lived on the back of his head, feeding on unicorn blood and seeking the Philosopher's Stone.",
				ModContent.NPCType<Content.NPCs.Bosses.Quirrell.QuirrellBoss>(),
				() => DownedBossSystem.downedQuirrell
			));

			// Year 2
			Memories.Add(new PensieveMemory(
				"The Chamber of Secrets",
				"Year Two — Beneath Hogwarts",
				"The Basilisk, King of Serpents, dwelt beneath the school for a thousand years. Only a Parselmouth could open the Chamber, and only the Sword of Gryffindor could slay the beast.",
				ModContent.NPCType<Content.NPCs.Bosses.Basilisk.BasiliskBoss>(),
				() => DownedBossSystem.downedBasilisk
			));

			// Year 3-era
			Memories.Add(new PensieveMemory(
				"Fenrir's Rage",
				"The Werewolf Wars",
				"Fenrir Greyback, the most savage werewolf alive, terrorized the wizarding world under blood-red moons. Only silver and courage could stand against his fury.",
				ModContent.NPCType<Content.NPCs.Bosses.Fenrir.FenrirBoss>(),
				() => DownedBossSystem.downedFenrir
			));

			// Year 4-era
			Memories.Add(new PensieveMemory(
				"The First Task",
				"Year Four — The Triwizard Tournament",
				"The Hungarian Horntail, most dangerous of all dragons, guarded a golden egg. Champions had to outwit it, outfly it, or outfight it.",
				ModContent.NPCType<Content.NPCs.Bosses.Horntail.HorntailBoss>(),
				() => DownedBossSystem.downedHorntail
			));

			// Forbidden Forest memory
			Memories.Add(new PensieveMemory(
				"Aragog's Lair",
				"The Forbidden Forest",
				"Deep in the Forbidden Forest, the Acromantula colony grew for decades under their patriarch Aragog. 'Follow the spiders,' Hagrid said. He failed to mention what waited at the end.",
				ModContent.NPCType<Content.NPCs.Bosses.Aragog.AragogBoss>(),
				() => DownedBossSystem.downedAragog
			));

			Memories.Add(new PensieveMemory(
				"The Trapdoor Guardian",
				"Year One — The Third Floor Corridor",
				"Fluffy, a three-headed dog, guarded the trapdoor to the Philosopher's Stone. Only music could lull him to sleep.",
				ModContent.NPCType<Content.NPCs.Bosses.Fluffy.FluffyBoss>(),
				() => DownedBossSystem.downedFluffy
			));

			// Year 5-era
			Memories.Add(new PensieveMemory(
				"Educational Decree",
				"Year Five — The Ministry's Interference",
				"Dolores Umbridge seized control of Hogwarts through bureaucracy, cruelty, and Educational Decrees. She didn't fight with power — she fought with authority.",
				ModContent.NPCType<Content.NPCs.Bosses.Umbridge.UmbridgeBoss>(),
				() => DownedBossSystem.downedUmbridge
			));

			// War-era
			Memories.Add(new PensieveMemory(
				"The Faithful Servant",
				"Year Four — The Impostor",
				"Barty Crouch Jr spent a year disguised as Mad-Eye Moody using Polyjuice Potion. His fanatical loyalty to the Dark Lord made him one of the most dangerous Death Eaters alive.",
				ModContent.NPCType<Content.NPCs.Bosses.BartyCrouch.BartyCrouchBoss>(),
				() => DownedBossSystem.downedBartyCrouch
			));

			Memories.Add(new PensieveMemory(
				"The Dark Witch",
				"The Second Wizarding War",
				"Bellatrix Lestrange was the most devoted and dangerous of Voldemort's followers. She killed Sirius Black, tortured the Longbottoms, and feared nothing but her master's displeasure.",
				ModContent.NPCType<Content.NPCs.Bosses.Bellatrix.BellatrixBoss>(),
				() => DownedBossSystem.downedBellatrix
			));

			Memories.Add(new PensieveMemory(
				"Azkaban's Despair",
				"The Dementor Uprising",
				"When the Dementors abandoned the Ministry's control, their concentrated despair manifested as a force of pure darkness. Only the strongest Patronus could pierce its shroud.",
				ModContent.NPCType<Content.NPCs.Bosses.DementorKing.DementorKingBoss>(),
				() => DownedBossSystem.downedDementorKing
			));

			Memories.Add(new PensieveMemory(
				"The Battle's End",
				"The Second Wizarding War — Final Night",
				"Tom Marvolo Riddle, who styled himself Lord Voldemort, fell at last. The Elder Wand would not kill its true master. Love, sacrifice, and courage triumphed over the darkest magic ever known.",
				ModContent.NPCType<Content.NPCs.Bosses.Voldemort.VoldemortBoss>(),
				() => DownedBossSystem.downedVoldemort
			));
		}
	}

	public class PensieveMemory
	{
		public string Title { get; }
		public string Era { get; }
		public string Description { get; }
		public int BossNPCType { get; }
		public System.Func<bool> IsUnlocked { get; }

		public PensieveMemory(string title, string era, string description, int bossType, System.Func<bool> isUnlocked)
		{
			Title = title;
			Era = era;
			Description = description;
			BossNPCType = bossType;
			IsUnlocked = isUnlocked;
		}
	}
}
