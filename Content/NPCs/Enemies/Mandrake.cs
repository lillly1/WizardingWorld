using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Mandrake — screaming plant creature. Debuffs nearby players with Confused.</summary>
	public class Mandrake : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 20;
			NPC.height = 28;
			NPC.damage = 20;
			NPC.defense = 6;
			NPC.lifeMax = 80;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(silver: 10);
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.Zombie;
			AnimationType = NPCID.Zombie;
		}

		public override void AI()
		{
			// Scream aura — confuse nearby players
			NPC.ai[1]++;
			if (NPC.ai[1] >= 180) // Every 3 seconds
			{
				NPC.ai[1] = 0;

				// Green dust scream effect
				for (int i = 0; i < 15; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(4), 8, 8, DustID.JungleSpore, 0f, 0f, 100, default, 1.5f);
					dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
					dust.noGravity = true;
				}

				// Confuse nearby players
				foreach (var player in Main.ActivePlayers)
				{
					if (Vector2.Distance(player.Center, NPC.Center) < 200f)
					{
						player.AddBuff(BuffID.Confused, 120);
					}
				}
			}

			// Leaf dust
			if (Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.JungleSpore, 0f, 0f, 100, default, 0.6f);
				dust.noGravity = true;
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneJungle || spawnInfo.Player.ZoneForest)
				return 0.05f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.JungleSpores, 2, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.Daybloom, 3, 1, 2));
			npcLoot.Add(ItemDropRule.Common(ItemID.HerbBag, 10));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Mandrake"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 6; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.JungleSpore);
		}
	}
}
