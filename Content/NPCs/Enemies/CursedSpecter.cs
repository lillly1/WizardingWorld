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
	/// Cursed Specter — a wizard's ghost trapped in the Corruption.
	/// Fires cursed flame bolts, phases through walls. Applies Dark Curse on hit.
	/// The spirit of a wizard who dabbled too deep in dark magic and was consumed.
	/// Hardmode Corruption/Crimson enemy.
	/// </summary>
	public class CursedSpecter : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 28;
			NPC.height = 44;
			NPC.damage = 60;
			NPC.defense = 18;
			NPC.lifeMax = 450;
			NPC.HitSound = SoundID.NPCHit36;
			NPC.DeathSound = SoundID.NPCDeath39;
			NPC.value = Item.buyPrice(silver: 50);
			NPC.knockBackResist = 0.15f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.aiStyle = -1;
			NPC.alpha = 80;
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];

			// Slow menacing pursuit
			float speed = 5f;
			float inertia = 25f;
			Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Fire cursed flame every 2.5 seconds
			NPC.ai[0]++;
			if (NPC.ai[0] >= 150 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.ai[0] = 0;
				Vector2 fireDir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 8f;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.CursedFlameHostile, NPC.damage / 3, 0f, Main.myPlayer);
			}

			// Purple-green corrupt dust
			if (Main.rand.NextBool(3))
			{
				int dustType = Main.rand.NextBool() ? DustID.CursedTorch : DustID.Shadowflame;
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 150, default, 0.7f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}

			// Flickering alpha
			NPC.alpha = 60 + (int)(Math.Sin(NPC.ai[0] * 0.1) * 30);
			NPC.spriteDirection = NPC.Center.X < target.Center.X ? 1 : -1;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.DarkCurseDebuff>(), 180);
			target.AddBuff(BuffID.CursedInferno, 240);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && (spawnInfo.Player.ZoneCorrupt || spawnInfo.Player.ZoneCrimson) && !Main.dayTime)
				return 0.06f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.SoulofNight, 2, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.CursedFlame, 4, 2, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.CursedSpecter"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame);
		}
	}
}
