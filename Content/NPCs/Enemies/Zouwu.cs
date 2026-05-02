using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Zouwu — a massive, elephant-sized magical cat from Fantastic Beasts.
	/// Incredibly fast and powerful. Lunges across the screen.
	/// Can be calmed with a toy bell (reference to the movie).
	/// Rare hardmode surface spawn. Drops valuable Zouwu Mane (crafting material).
	/// </summary>
	public class Zouwu : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 6;
		}

		public override void SetDefaults()
		{
			NPC.width = 60;
			NPC.height = 40;
			NPC.damage = 70;
			NPC.defense = 24;
			NPC.lifeMax = 800;
			NPC.HitSound = SoundID.NPCHit6;
			NPC.DeathSound = SoundID.NPCDeath8;
			NPC.value = Item.buyPrice(gold: 3);
			NPC.knockBackResist = 0.05f;
			NPC.aiStyle = -1;
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			NPC.ai[0]++;

			// Phase: prowl → LUNGE → recover → repeat
			if (NPC.ai[0] < 120)
			{
				// Prowl — slow approach
				float speed = 3f;
				Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * speed;
				NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, dir.X, 0.05f);

				// Apply gravity
				if (NPC.velocity.Y < 10f)
					NPC.velocity.Y += 0.3f;

				NPC.damage = NPC.defDamage / 2;
			}
			else if (NPC.ai[0] == 120)
			{
				// LUNGE — massive leap at the player
				Vector2 lungeDir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
				NPC.velocity = lungeDir * 22f;
				NPC.velocity.Y -= 8f; // Arc upward
				NPC.damage = NPC.defDamage * 2;
				NPC.netUpdate = true;

				// Dust burst on launch
				for (int i = 0; i < 15; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, 0f, 100, default, 1.2f);
					dust.velocity = Main.rand.NextVector2Circular(4f, 3f);
					dust.noGravity = true;
				}
			}
			else if (NPC.ai[0] < 160)
			{
				// Mid-lunge — gravity kicks in
				NPC.velocity.Y += 0.2f;
			}
			else
			{
				// Recovery — slow down
				NPC.velocity *= 0.95f;
				if (NPC.ai[0] > 200)
				{
					NPC.ai[0] = 0;
					NPC.netUpdate = true;
				}
			}

			// Colorful mane particles
			if (Main.rand.NextBool(4))
			{
				int[] colors = { DustID.RedTorch, DustID.GoldCoin, DustID.BlueTorch };
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height / 2, colors[Main.rand.Next(3)], 0f, 0f, 100, default, 0.6f);
				dust.noGravity = true;
				dust.velocity = -NPC.velocity * 0.1f;
			}

			NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && !NPC.AnyNPCs(Type))
				return 0.006f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 5, 10));
			npcLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 3, 8));
			npcLoot.Add(ItemDropRule.Common(ItemID.Feather, 1, 5, 10));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Zouwu"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 10; i++)
			{
				int dustType = new[] { DustID.RedTorch, DustID.GoldCoin }[Main.rand.Next(2)];
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);
			}
		}
	}
}
