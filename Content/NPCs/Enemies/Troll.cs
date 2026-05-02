using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Mountain Troll — slow, tanky brute. Spawns in caverns.</summary>
	public class Troll : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 6;
		}

		public override void SetDefaults()
		{
			NPC.width = 36;
			NPC.height = 56;
			NPC.damage = 45;
			NPC.defense = 22;
			NPC.lifeMax = 600;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = Item.buyPrice(silver: 80);
			NPC.knockBackResist = 0.1f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.Zombie;
			AnimationType = NPCID.Zombie;
		}

		public override void AI()
		{
			// Slow but menacing
			NPC.velocity.X *= 0.85f;

			// Stomp dust when walking
			if (NPC.velocity.Y == 0 && NPC.velocity.X != 0 && Main.rand.NextBool(5))
			{
				Dust.NewDustDirect(NPC.Bottom, NPC.width, 4, DustID.Stone, 0f, -2f, 100, default, 1.5f);
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Devastating hit — broken armor
			target.AddBuff(BuffID.BrokenArmor, 300);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneRockLayerHeight)
				return 0.04f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.StoneBlock, 1, 10, 30));
			npcLoot.Add(ItemDropRule.Common(ItemID.IronBar, 3, 3, 8));
			npcLoot.Add(ItemDropRule.Common(ItemID.GoldBar, 8, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Troll"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Stone);
			}
		}
	}
}
