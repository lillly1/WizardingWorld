using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Boggart — shapeshifter that copies nearby enemy types. Inflicts fear (Slow + Darkness).</summary>
	public class Boggart : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 24;
			NPC.height = 36;
			NPC.damage = 30;
			NPC.defense = 10;
			NPC.lifeMax = 200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = Item.buyPrice(silver: 25);
			NPC.knockBackResist = 0.4f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.Zombie;
			AnimationType = NPCID.Zombie;
		}

		public override void AI()
		{
			// Shimmer/morph effect — flickering alpha
			NPC.alpha = (int)(Math.Sin(Main.GameUpdateCount * 0.1) * 50 + 80);

			// Dark shimmer dust
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Wraith, 0f, 0f, 150, default, 1.0f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}

			// Periodically become more aggressive based on player proximity
			if (NPC.HasValidTarget)
			{
				Player target = Main.player[NPC.target];
				float dist = Vector2.Distance(target.Center, NPC.Center);
				if (dist < 150f)
				{
					// Close range — boost speed
					NPC.velocity.X *= 1.02f;
				}
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Fear effect — Jinxed + Darkness
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>(), 240);
			target.AddBuff(BuffID.Darkness, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneDirtLayerHeight || spawnInfo.Player.ZoneRockLayerHeight)
				return 0.04f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Lens, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.BlackLens, 10));
			npcLoot.Add(ItemDropRule.Common(ItemID.SoulofNight, 5, 1, 2));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Boggart"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 8; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Wraith);
		}
	}
}
