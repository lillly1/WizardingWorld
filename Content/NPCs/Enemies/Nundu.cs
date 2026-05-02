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
	/// Nundu — the most dangerous magical beast in existence.
	/// A massive leopard whose breath is toxic and can destroy entire villages.
	/// Requires at least 100 wizards working together to subdue one.
	/// Extremely rare, extremely lethal. Hardmode jungle exclusive.
	/// Applies multiple debuffs on hit, has a toxic breath AoE aura.
	/// </summary>
	public class Nundu : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 6;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 54;
			NPC.height = 36;
			NPC.damage = 90;
			NPC.defense = 30;
			NPC.lifeMax = 1500;
			NPC.HitSound = SoundID.NPCHit6;
			NPC.DeathSound = SoundID.NPCDeath8;
			NPC.value = Item.buyPrice(gold: 5);
			NPC.knockBackResist = 0.02f;
			NPC.aiStyle = -1;
			NPC.npcSlots = 5f;
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			NPC.ai[0]++;

			// Ground movement with lunges
			if (NPC.velocity.Y < 10f)
				NPC.velocity.Y += 0.3f; // Gravity

			float speed = 5f + (1f - (float)NPC.life / NPC.lifeMax) * 4f;
			Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
			NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, dir.X * speed, 0.08f);

			// Lunge every 3 seconds
			if (NPC.ai[0] % 180 == 0 && NPC.velocity.Y == 0)
			{
				NPC.velocity = dir * 18f;
				NPC.velocity.Y = -8f;
				NPC.netUpdate = true;
			}

			// Toxic breath aura — poisons nearby players passively
			foreach (var player in Main.ActivePlayers)
			{
				if (Vector2.Distance(player.Center, NPC.Center) < 200f)
				{
					player.AddBuff(BuffID.Poisoned, 60);
					if (NPC.ai[0] % 60 == 0) // Every second
						player.AddBuff(BuffID.Venom, 30);
				}
			}

			// Toxic green cloud particles
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position + Main.rand.NextVector2Circular(NPC.width, NPC.height), 4, 4, DustID.GreenTorch, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 0f), 150, default, 0.6f);
				dust.noGravity = true;
			}

			NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Venom, 600);
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.DarkCurseDebuff>(), 180);
			target.AddBuff(BuffID.BrokenArmor, 300);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && spawnInfo.Player.ZoneJungle && !NPC.AnyNPCs(Type))
				return 0.004f; // Extremely rare
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 10, 20));
			npcLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 5, 10));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Accessories.DarkLordsBane>(), 25));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Nundu"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 12; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GreenTorch);
		}
	}
}
