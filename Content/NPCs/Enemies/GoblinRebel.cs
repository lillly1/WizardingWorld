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
	/// Goblin Rebel — a rogue Gringotts goblin armed with enchanted weapons.
	/// Underground fighter that throws gold coins as projectiles (painful irony).
	/// Drops extra gold. Represents the goblin rebellions from wizarding history.
	/// Pre-hardmode underground enemy.
	/// </summary>
	public class GoblinRebel : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 20;
			NPC.height = 34;
			NPC.damage = 30;
			NPC.defense = 12;
			NPC.lifeMax = 150;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(gold: 1); // Goblins carry lots of gold
			NPC.knockBackResist = 0.4f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.GoblinThief;
			AnimationType = NPCID.GoblinThief;
		}

		public override void AI()
		{
			// Throw gold coin projectiles every 2 seconds
			NPC.ai[1]++;
			if (NPC.ai[1] >= 120 && NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.ai[1] = 0;
				Player target = Main.player[NPC.target];
				Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 8f;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dir,
					ProjectileID.CoinPortal, NPC.damage / 3, 2f, Main.myPlayer);
			}

			// Gold dust trail — greedy little thing
			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, 0f, 100, default, 0.5f);
				dust.noGravity = true;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Steals gold (thematically — just applies Midas debuff equivalent)
			target.AddBuff(BuffID.Midas, 300);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneRockLayerHeight || spawnInfo.Player.ZoneDirtLayerHeight)
				return 0.04f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			// Goblins drop lots of gold!
			npcLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.GoldBar, 3, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 3, 1, 2));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.GoblinRebel"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 6; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GoldCoin);
		}
	}
}
