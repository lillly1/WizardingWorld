using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Fwooper — brightly colored magical bird whose song drives the listener insane.
	/// Spawns in jungles and the Forbidden Forest. Applies Confused + Jinxed on hit.
	/// Colorful and beautiful but dangerous.
	/// </summary>
	public class Fwooper : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 20;
			NPC.height = 18;
			NPC.damage = 28;
			NPC.defense = 6;
			NPC.lifeMax = 100;
			NPC.HitSound = SoundID.NPCHit5;
			NPC.DeathSound = SoundID.NPCDeath7;
			NPC.value = Item.buyPrice(silver: 15);
			NPC.knockBackResist = 0.6f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Bat;
			AIType = NPCID.CaveBat;
		}

		public override void AI()
		{
			// Colorful feather dust trail
			int[] colors = { DustID.PinkTorch, DustID.YellowStarDust, DustID.BlueTorch, DustID.GreenTorch };
			if (Main.rand.NextBool(5))
			{
				int dustType = colors[Main.rand.Next(colors.Length)];
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 0.6f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}

			NPC.spriteDirection = NPC.direction;

			// Insanity aura — confuse nearby players periodically
			NPC.ai[1]++;
			if (NPC.ai[1] >= 240) // Every 4 seconds
			{
				NPC.ai[1] = 0;
				foreach (var player in Main.ActivePlayers)
				{
					if (Vector2.Distance(player.Center, NPC.Center) < 200f)
					{
						player.AddBuff(BuffID.Confused, 90); // 1.5 second confusion
					}
				}
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Confused, 180);
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>(), 120);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneJungle && Main.dayTime)
				return 0.06f;
			if (spawnInfo.Player.InModBiome<Biomes.ForbiddenForestBiome>())
				return 0.04f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Feather, 1, 2, 4));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 1, 2));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Fwooper"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 6; i++)
			{
				int dustType = new[] { DustID.PinkTorch, DustID.YellowStarDust, DustID.BlueTorch }[Main.rand.Next(3)];
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);
			}
		}
	}
}
