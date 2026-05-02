using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Jobberknoll — a tiny blue speckled bird that is completely silent
	/// until the moment of its death, when it lets out a long scream
	/// consisting of every sound it has ever heard in reverse.
	/// Its feathers are used in Truth Serums (Veritaserum).
	/// Sky critter. Drops Jobberknoll Feather.
	/// </summary>
	public class Jobberknoll : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
			Main.npcCatchable[Type] = true;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 14;
			NPC.height = 12;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 20;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.Item4; // Special death sound — the reverse scream
			NPC.value = Item.buyPrice(silver: 20);
			NPC.knockBackResist = 1f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Bat;
			AIType = NPCID.CaveBat;
			NPC.friendly = true;
		}

		public override void AI()
		{
			// Completely silent — no dust, no effects. Just flies.
			NPC.spriteDirection = NPC.direction;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneSkyHeight || (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime))
				return 0.03f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Feather, 1, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.JobberknollFeather>(), 1, 1, 2));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 1, 2));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Jobberknoll"),
			});
		}
	}
}
