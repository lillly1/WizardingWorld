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
	/// Runespoor — a three-headed orange serpent. Each head serves a purpose:
	/// left head is the planner, middle head is the dreamer, right head is the critic.
	/// Fires three projectiles at once (one per head). Underground enemy.
	/// Drops Runespoor Eggs (valuable potion ingredient).
	/// </summary>
	public class Runespoor : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 20;
			NPC.damage = 40;
			NPC.defense = 14;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = Item.buyPrice(silver: 40);
			NPC.knockBackResist = 0.3f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.DesertScorpionWalk;
		}

		public override void AI()
		{
			// Triple shot every 3 seconds — three heads, three projectiles
			NPC.ai[1]++;
			if (NPC.ai[1] >= 180 && NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.ai[1] = 0;
				Player target = Main.player[NPC.target];
				Vector2 baseDir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 8f;

				// Three spread bolts — one per head
				for (int i = -1; i <= 1; i++)
				{
					Vector2 spreadDir = baseDir.RotatedBy(MathHelper.ToRadians(i * 15));
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, spreadDir,
						ProjectileID.VenomFang, NPC.damage / 3, 0f, Main.myPlayer);
				}
			}

			// Orange scales dust
			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.OrangeTorch, 0f, 0f, 100, default, 0.5f);
				dust.noGravity = true;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Venom, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneRockLayerHeight)
				return 0.04f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.VialofVenom, 3, 2, 5));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Runespoor"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 6; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.OrangeTorch);
		}
	}
}
