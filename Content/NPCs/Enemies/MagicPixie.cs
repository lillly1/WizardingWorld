using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Cornish Pixie — small, fast, annoying flyer. Spawns on the surface.</summary>
	public class MagicPixie : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 18;
			NPC.height = 20;
			NPC.damage = 18;
			NPC.defense = 4;
			NPC.lifeMax = 50;
			NPC.HitSound = SoundID.NPCHit5;
			NPC.DeathSound = SoundID.NPCDeath7;
			NPC.value = Item.buyPrice(silver: 5);
			NPC.knockBackResist = 0.8f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Bat;
			AIType = NPCID.CaveBat;
		}

		public override void AI()
		{
			// Blue sparkle dust
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BlueTorch, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
			}

			NPC.spriteDirection = NPC.direction;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Confused, 120);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (!Main.dayTime && spawnInfo.Player.ZoneOverworldHeight)
				return 0.1f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.PixieDust, 2, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.FallenStar, 5));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.MagicPixie"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 6; i++)
			{
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BlueTorch);
			}
		}
	}
}
