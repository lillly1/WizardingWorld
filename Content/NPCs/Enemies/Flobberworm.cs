using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Flobberworm — the most boring magical creature in existence.
	/// Passive underground critter. Drops Flobberworm Mucus (potion ingredient).
	/// </summary>
	public class Flobberworm : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 2;
			Main.npcCatchable[Type] = true;
			NPCID.Sets.CountsAsCritter[Type] = true;
			NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 16;
			NPC.height = 8;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 5;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(copper: 50);
			NPC.knockBackResist = 1f;
			NPC.aiStyle = NPCAIStyleID.Passive;
			AIType = NPCID.Worm; // Worm-like passive AI
			NPC.friendly = true;
			NPC.npcSlots = 0.1f;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneDirtLayerHeight || spawnInfo.Player.ZoneRockLayerHeight)
				return 0.06f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Worm, 1, 1, 2));
			npcLoot.Add(ItemDropRule.Common(ItemID.GlowingMushroom, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Flobberworm"),
			});
		}
	}
}
