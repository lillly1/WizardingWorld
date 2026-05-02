using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Demiguise — a peaceful ape-like creature that can turn invisible.
	/// Passive critter until attacked. When threatened, turns invisible (alpha increases).
	/// Drops Demiguise Hair — used to weave Invisibility Cloaks.
	/// Rare forest critter, catchable.
	/// </summary>
	public class Demiguise : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
			Main.npcCatchable[Type] = true;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 24;
			NPC.height = 30;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 60;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(gold: 1);
			NPC.knockBackResist = 0.8f;
			NPC.aiStyle = NPCAIStyleID.Passive;
			AIType = NPCID.Bunny;
			AnimationType = NPCID.Bunny;
			NPC.friendly = true;
		}

		public override void AI()
		{
			// When hit (life < max), start turning invisible
			if (NPC.life < NPC.lifeMax)
			{
				// Fade to invisible over 2 seconds
				if (NPC.alpha < 220)
					NPC.alpha += 2;

				// Run away faster when invisible
				NPC.velocity.X *= 1.03f;
			}
			else
			{
				// Slowly become visible again when calm
				if (NPC.alpha > 0)
					NPC.alpha -= 1;
			}

			// Silvery shimmer dust when partially visible
			if (NPC.alpha < 150 && Main.rand.NextBool(10))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 0f, 0f, 150, default, 0.4f);
				dust.noGravity = true;
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneForest && Main.dayTime)
				return 0.02f;
			if (spawnInfo.Player.InModBiome<Biomes.ForbiddenForestBiome>())
				return 0.03f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			// Demiguise Hair — used to craft Invisibility-related items
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.DemiguiseHair>(), 1, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Demiguise"),
			});
		}
	}
}
