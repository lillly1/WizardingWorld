using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Sphinx — ancient guardian creature. High HP, high defense, rare spawn.
	/// Walks slowly but charges when damaged. Drops rare wizard materials.
	/// Found in the desert during Hardmode — guardian of ancient wizard tombs.
	/// </summary>
	public class Sphinx : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 48;
			NPC.height = 40;
			NPC.damage = 60;
			NPC.defense = 35;
			NPC.lifeMax = 1200;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath10;
			NPC.value = Item.buyPrice(gold: 3);
			NPC.knockBackResist = 0.05f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.GraniteGolem;
			AnimationType = NPCID.GraniteGolem;
		}

		public override void AI()
		{
			// When below 50% HP: enrage — speed boost
			if (NPC.life < NPC.lifeMax * 0.5f)
			{
				NPC.velocity.X *= 1.05f;

				// Gold rage aura
				if (Main.rand.NextBool(4))
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, 0f, 100, default, 1.0f);
					dust.noGravity = true;
				}
			}

			// Stone dust from feet
			if (NPC.velocity.Y == 0 && NPC.velocity.X != 0 && Main.rand.NextBool(6))
			{
				Dust.NewDustDirect(NPC.Bottom, NPC.width, 4, DustID.Sand, 0f, -1f, 100, default, 1.0f);
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PetrifiedDebuff>(), 60); // 1 second petrify
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && spawnInfo.Player.ZoneDesert && !NPC.AnyNPCs(Type))
				return 0.02f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 5, 12));
			npcLoot.Add(ItemDropRule.Common(ItemID.GoldBar, 2, 3, 8));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Accessories.DiademOfRavenclaw>(), 20));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.GoldenEgg>(), 10));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Sphinx"),
			});
		}

		public override void OnKill()
		{
			if (Common.Systems.TriwizardTournamentSystem.taskActive &&
				Common.Systems.TriwizardTournamentSystem.currentTask == 3)
			{
				var closest = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
				Common.Systems.TriwizardTournamentSystem.OnMazeObstacleCleared(closest);
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sand);
		}
	}
}
