using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Puffskein — a round, fluffy custard-coloured creature.
	/// Completely harmless, docile, and loved by wizard children.
	/// Hums contentedly when happy. Catchable critter.
	/// The ancestor of the Pygmy Puff.
	/// </summary>
	public class Puffskein : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 2;
			Main.npcCatchable[Type] = true;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 16;
			NPC.height = 14;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 15;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(silver: 10);
			NPC.knockBackResist = 1f;
			NPC.aiStyle = NPCAIStyleID.Passive;
			AIType = NPCID.Bunny;
			AnimationType = NPCID.Bunny;
			NPC.friendly = true;
		}

		public override void AI()
		{
			// Custard-coloured happy particles
			if (Main.rand.NextBool(20))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.YellowStarDust, 0f, -0.5f, 100, default, 0.3f);
				dust.noGravity = true;
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneForest && Main.dayTime)
				return 0.05f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.PinkGel, 2, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Puffskein"),
			});
		}
	}
}
