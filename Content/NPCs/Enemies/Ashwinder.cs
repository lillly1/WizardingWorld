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
	/// Ashwinder — thin grey serpent that spawns from unattended magical fires.
	/// Lives only an hour before collapsing to dust, but lays eggs that
	/// are intensely hot and valuable for Love Potions.
	/// Surface fire-biome enemy. Applies OnFire. Drops Ashwinder Egg.
	/// </summary>
	public class Ashwinder : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 30;
			NPC.height = 16;
			NPC.damage = 35;
			NPC.defense = 8;
			NPC.lifeMax = 120;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = Item.buyPrice(silver: 20);
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.DesertScorpionWalk;
			NPC.lavaImmune = true;
			NPC.buffImmune[BuffID.OnFire] = true;
		}

		public override void AI()
		{
			// Ember trail — the serpent leaves fire in its wake
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.Bottom - new Vector2(4, 0), NPC.width, 4, DustID.Torch, 0f, 0f, 100, default, 0.7f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}

			// Short lifespan — loses HP over time (crumbling to ash)
			NPC.ai[1]++;
			if (NPC.ai[1] % 120 == 0) // Every 2 seconds
				NPC.life -= 5;

			if (NPC.life <= 0)
			{
				NPC.life = 0;
				NPC.checkDead();
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.OnFire3, 240);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneUnderworldHeight)
				return 0.06f;
			if (spawnInfo.Player.ZoneDesert && !Main.dayTime)
				return 0.03f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.AshwinderEgg>(), 2, 1, 2));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 3, 1, 2));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Ashwinder"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 6; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch);
		}
	}
}
