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
	/// Erumpent — a massive rhinoceros-like magical beast from Fantastic Beasts.
	/// Its horn contains explosive fluid that detonates on impact.
	/// Charges at the player and EXPLODES on contact, dealing massive AoE damage.
	/// Rare, dangerous, hardmode jungle/savanna enemy.
	/// </summary>
	public class Erumpent : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 50;
			NPC.height = 36;
			NPC.damage = 80;
			NPC.defense = 25;
			NPC.lifeMax = 700;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath10;
			NPC.value = Item.buyPrice(gold: 2);
			NPC.knockBackResist = 0f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.GraniteGolem;
			AnimationType = NPCID.GraniteGolem;
		}

		public override void AI()
		{
			// Charge toward player aggressively
			if (NPC.HasValidTarget)
			{
				Player target = Main.player[NPC.target];
				float dist = Vector2.Distance(NPC.Center, target.Center);

				// When close, speed up for charge
				if (dist < 300f)
					NPC.velocity.X *= 1.05f;
			}

			// Orange glow from the horn
			if (Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(NPC.position + new Vector2(NPC.direction > 0 ? NPC.width : 0, 8), 8, 8, DustID.Torch, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// EXPLODE on contact!
			for (int i = 0; i < 40; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.Center, 8, 8, DustID.Torch, 0f, 0f, 50, default, 2.5f);
				dust.velocity = Main.rand.NextVector2Circular(8f, 8f);
				dust.noGravity = true;
			}

			// AoE damage to other nearby enemies too
			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.whoAmI != NPC.whoAmI && !npc.friendly && Vector2.Distance(npc.Center, NPC.Center) < 150f)
				{
					npc.SimpleStrikeNPC(NPC.damage, 0, false, 0f);
				}
			}

			target.AddBuff(BuffID.OnFire3, 300);

			// The Erumpent dies in the explosion
			NPC.life = 0;
			NPC.checkDead();
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && spawnInfo.Player.ZoneJungle && !NPC.AnyNPCs(Type))
				return 0.015f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.ExplosivePowder, 1, 5, 10));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 3, 6));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.DragonScale>(), 5));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Erumpent"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 8; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch);
		}
	}
}
