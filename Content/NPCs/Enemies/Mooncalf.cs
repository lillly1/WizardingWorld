using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Mooncalf — a shy magical creature that only appears at night under a full moon.
	/// Performs intricate dances in the moonlight. Passive critter — flees if approached.
	/// Drops Mooncalf Dung (potent fertilizer / potion ingredient).
	/// One of the more charming Fantastic Beasts.
	/// </summary>
	public class Mooncalf : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
			Main.npcCatchable[Type] = true;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 20;
			NPC.height = 24;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 30;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(silver: 20);
			NPC.knockBackResist = 0.9f;
			NPC.aiStyle = NPCAIStyleID.Passive;
			AIType = NPCID.Bunny;
			AnimationType = NPCID.Bunny;
			NPC.friendly = true;
		}

		public override void AI()
		{
			// Moonlit dance particles — silver sparkles during full moon
			if (Main.moonPhase == 0 && !Main.dayTime && Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(NPC.position + new Vector2(0, -10), NPC.width, 8, DustID.SilverCoin, 0f, -1f, 100, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			// Only spawns at night during full moon on the surface
			if (!Main.dayTime && Main.moonPhase == 0 && spawnInfo.Player.ZoneOverworldHeight)
				return 0.08f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 2, 4));
			npcLoot.Add(ItemDropRule.Common(ItemID.FallenStar, 2, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Mooncalf"),
			});
		}
	}
}
