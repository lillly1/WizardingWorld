using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Bowtruckle — tiny peaceful tree-guardian creature. Critter behavior.
	/// Doesn't attack unless provoked. Found in forests near trees.
	/// Drops wood and occasionally wand-crafting materials.
	/// </summary>
	public class Bowtruckle : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
			Main.npcCatchable[Type] = true; // Can be caught with a bug net!
			NPCID.Sets.CountsAsCritter[Type] = true;
			NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 12;
			NPC.height = 16;
			NPC.damage = 5;
			NPC.defense = 0;
			NPC.lifeMax = 20;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(silver: 5);
			NPC.knockBackResist = 0.8f;
			NPC.aiStyle = NPCAIStyleID.Passive;
			AIType = NPCID.Bunny; // Passive critter AI
			AnimationType = NPCID.Bunny;
			NPC.catchItem = ModContent.ItemType<Items.Consumables.BowtruckleCatch>();
			NPC.friendly = true;
		}

		public override void AI()
		{
			// Leaf dust occasionally
			if (Main.rand.NextBool(20))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.JungleSpore, 0f, -0.5f, 100, default, 0.4f);
				dust.noGravity = true;
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.dayTime && spawnInfo.Player.ZoneForest)
				return 0.08f;
			if (spawnInfo.Player.ZoneJungle)
				return 0.05f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Wood, 1, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.FallenStar, 10));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Bowtruckle"),
			});
		}
	}
}
