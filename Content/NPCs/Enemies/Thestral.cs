using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Thestral — skeletal winged horse. Only spawns after a boss has been killed. Invisible-themed.</summary>
	public class Thestral : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 44;
			NPC.damage = 40;
			NPC.defense = 16;
			NPC.lifeMax = 300;
			NPC.HitSound = SoundID.NPCHit36;
			NPC.DeathSound = SoundID.NPCDeath39;
			NPC.value = Item.buyPrice(silver: 40);
			NPC.knockBackResist = 0.3f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Bat;
			AIType = NPCID.GiantBat;
			NPC.alpha = 100; // Semi-transparent — visible only to those who've seen death
		}

		public override void AI()
		{
			// Dark mist trail
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 150, default, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}

			NPC.spriteDirection = NPC.direction;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			// Only spawns once you've killed a boss (seen death)
			if (!Main.dayTime && spawnInfo.Player.ZoneOverworldHeight
				&& (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3))
				return 0.04f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Feather, 2, 2, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.SoulofFlight, 4, 1, 2));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Thestral"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 8; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Smoke);
		}
	}
}
