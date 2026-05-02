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
	/// Fire Crab — a large tortoise-like creature with a jewelled shell.
	/// Shoots flames from its rear end when threatened.
	/// Native to Fiji. Drops Fire Crab Shell (valuable crafting material).
	/// Hardmode underground/cavern enemy.
	/// </summary>
	public class FireCrab : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 32;
			NPC.height = 22;
			NPC.damage = 45;
			NPC.defense = 22; // Armored shell
			NPC.lifeMax = 280;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath10;
			NPC.value = Item.buyPrice(silver: 35);
			NPC.knockBackResist = 0.15f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.GraniteGolem;
			AnimationType = NPCID.GraniteGolem;
			NPC.lavaImmune = true;
		}

		public override void AI()
		{
			// Fire blast from rear every 3 seconds
			NPC.ai[1]++;
			if (NPC.ai[1] >= 180 && NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.ai[1] = 0;
				// Fire backwards (opposite to facing direction)
				Vector2 fireDir = new Vector2(-NPC.direction * 8f, -2f);
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.Fireball, NPC.damage / 3, 0f, Main.myPlayer);

				// Fire burst visual
				for (int i = 0; i < 8; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.Center + new Vector2(-NPC.direction * 16, 0), 8, 8, DustID.Torch, -NPC.direction * 3f, 0f, 50, default, 1.5f);
					dust.noGravity = true;
				}
			}

			// Jewelled shell sparkle
			if (Main.rand.NextBool(10))
			{
				int dustType = new[] { DustID.GemRuby, DustID.GemSapphire, DustID.GemEmerald, DustID.GemTopaz }[Main.rand.Next(4)];
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 0.4f);
				dust.noGravity = true;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.OnFire3, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && spawnInfo.Player.ZoneRockLayerHeight)
				return 0.04f;
			if (spawnInfo.Player.ZoneUnderworldHeight)
				return 0.05f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.DragonScale>(), 4, 1, 2));
			npcLoot.Add(ItemDropRule.Common(ItemID.Ruby, 5));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.FireCrab"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 6; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch);
		}
	}
}
