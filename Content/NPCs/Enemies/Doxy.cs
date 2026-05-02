using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Doxy — tiny venomous fairy. Spawns in swarms. Fast, annoying, and poisonous.</summary>
	public class Doxy : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 14;
			NPC.height = 14;
			NPC.damage = 22;
			NPC.defense = 2;
			NPC.lifeMax = 35;
			NPC.HitSound = SoundID.NPCHit5;
			NPC.DeathSound = SoundID.NPCDeath7;
			NPC.value = Item.buyPrice(silver: 3);
			NPC.knockBackResist = 0.9f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Bat;
			AIType = NPCID.CaveBat;
		}

		public override void AI()
		{
			// Purple/dark fairy dust
			if (Main.rand.NextBool(5))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch, 0f, 0f, 100, default, 0.6f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}

			NPC.spriteDirection = NPC.direction;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Venomous bite
			target.AddBuff(BuffID.Poisoned, 240);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneForest && !Main.dayTime)
				return 0.12f; // Spawns in groups
			if (spawnInfo.Player.ZoneJungle)
				return 0.08f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Stinger, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.PixieDust, 4, 1, 2));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Doxy"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 4; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch);
		}
	}
}
