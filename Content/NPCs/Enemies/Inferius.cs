using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Inferius — dark undead creature. Spawns in dungeon during Hardmode.</summary>
	public class Inferius : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];
		}

		public override void SetDefaults()
		{
			NPC.width = 18;
			NPC.height = 40;
			NPC.damage = 50;
			NPC.defense = 18;
			NPC.lifeMax = 350;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = Item.buyPrice(silver: 40);
			NPC.knockBackResist = 0.3f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.ArmoredSkeleton;
			AnimationType = NPCID.Zombie;
		}

		public override void AI()
		{
			// Shadowflame aura
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.ShadowFlame, 240);
			target.AddBuff(BuffID.Weak, 300);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && spawnInfo.Player.ZoneDungeon)
				return 0.06f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Bone, 1, 5, 15));
			npcLoot.Add(ItemDropRule.Common(ItemID.SoulofNight, 3, 1, 2));
			npcLoot.Add(ItemDropRule.Common(ItemID.Ectoplasm, 5, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Inferius"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame);
			}
		}
	}
}
