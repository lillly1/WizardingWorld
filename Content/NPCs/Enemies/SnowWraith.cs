using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Snow Wraith — an ice-elemental magical spirit found in snow biomes.
	/// Freezes players on hit. Drops Essence of Magic and ice materials.
	/// Inspired by the frozen landscape around Durmstrang Institute.
	/// </summary>
	public class SnowWraith : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 26;
			NPC.height = 40;
			NPC.damage = 40;
			NPC.defense = 14;
			NPC.lifeMax = 220;
			NPC.HitSound = SoundID.NPCHit36;
			NPC.DeathSound = SoundID.NPCDeath39;
			NPC.value = Item.buyPrice(silver: 30);
			NPC.knockBackResist = 0.3f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Bat;
			AIType = NPCID.IceElemental;
			NPC.alpha = 60;
		}

		public override void AI()
		{
			// Ice crystal dust
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.IceTorch, 0f, 0f, 150, default, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}

			NPC.spriteDirection = NPC.direction;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Frozen, 60); // 1 second freeze
			target.AddBuff(BuffID.Chilled, 300);
			target.AddBuff(BuffID.Frostburn, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneSnow && !Main.dayTime)
				return 0.06f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.IceBlock, 1, 5, 15));
			npcLoot.Add(ItemDropRule.Common(ItemID.FrostCore, 15));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.SnowWraith"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 8; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.IceTorch);
		}
	}
}
