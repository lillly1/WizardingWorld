using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Cursed Mummy — an ancient Egyptian wizard's undead guardian.
	/// Found in desert biomes. Applies Dark Curse on hit.
	/// Inspired by the ancient magical traditions of Egypt (Bill Weasley's domain).
	/// </summary>
	public class CursedMummy : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 24;
			NPC.height = 44;
			NPC.damage = 45;
			NPC.defense = 18;
			NPC.lifeMax = 300;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = Item.buyPrice(silver: 35);
			NPC.knockBackResist = 0.2f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.Mummy;
			AnimationType = NPCID.Mummy;
		}

		public override void AI()
		{
			// Curse aura — green-black particles
			if (Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, 0f, 0f, 100, default, 0.7f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Ancient curse — the real Dark Curse debuff
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.DarkCurseDebuff>(), 180);
			target.AddBuff(BuffID.Darkness, 300);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneDesert && !Main.dayTime)
				return 0.05f;
			if (Main.hardMode && spawnInfo.Player.ZoneDesert)
				return 0.03f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.AncientBattleArmorMaterial, 10));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Accessories.BasiliskFang>(), 20));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.CursedMummy"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 8; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sand);
		}
	}
}
