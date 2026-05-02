using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Merfolk — underwater wizard-world merpeople. Aggressive in their territory.
	/// Wields tridents, fast swimmers. Spawns in oceans and underground water.
	/// Has a ranged water-bolt attack.
	/// </summary>
	public class Merfolk : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 24;
			NPC.height = 38;
			NPC.damage = 35;
			NPC.defense = 12;
			NPC.lifeMax = 200;
			NPC.HitSound = SoundID.NPCHit3;
			NPC.DeathSound = SoundID.NPCDeath3;
			NPC.value = Item.buyPrice(silver: 30);
			NPC.knockBackResist = 0.4f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.BloodFeeder;
		}

		public override void AI()
		{
			// Water bubble trail
			if (NPC.wet && Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Water, 0f, -1f, 100, default, 0.7f);
				dust.noGravity = true;
			}

			// Shoot water bolt every 3 seconds when target is visible
			NPC.ai[1]++;
			if (NPC.ai[1] >= 180 && NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.ai[1] = 0;
				Player target = Main.player[NPC.target];
				Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 8f;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dir,
					ProjectileID.WaterBolt, NPC.damage / 3, 0f, Main.myPlayer);
			}

			NPC.spriteDirection = NPC.direction;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Wet, 600);
			target.AddBuff(BuffID.Slow, 120);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Water)
			{
				if (spawnInfo.Player.ZoneBeach)
					return 0.10f;
				if (spawnInfo.Player.ZoneRockLayerHeight)
					return 0.05f;
			}

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Coral, 2, 2, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.Trident, 15));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.Potions.Gillyweed>(), 8));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Merfolk"),
			});
		}

		public override void OnKill()
		{
			if (Common.Systems.TriwizardTournamentSystem.taskActive &&
				Common.Systems.TriwizardTournamentSystem.currentTask == 2)
			{
				Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(),
					ModContent.ItemType<Items.Consumables.LakeRescueToken>());
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 8; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Water);
		}
	}
}
