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
	/// Phantom Stag — a spectral mockery of a true Patronus, twisted by the Hallow's wild magic.
	/// Not a real Patronus — a magical echo corrupted by overcharged positive energy.
	/// Fully original mod content.
	/// </summary>
	public class PhantomStag : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 34;
			NPC.height = 34;
			NPC.damage = 55;
			NPC.defense = 20;
			NPC.lifeMax = 400;
			NPC.HitSound = SoundID.NPCHit5;
			NPC.DeathSound = SoundID.NPCDeath7;
			NPC.value = Item.buyPrice(silver: 50);
			NPC.knockBackResist = 0.2f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.aiStyle = -1;
			NPC.alpha = 50;
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];

			// Erratic flight — like a corrupted animal spirit
			NPC.ai[0]++;
			float wobble = (float)Math.Sin(NPC.ai[0] * 0.06) * 3f;
			Vector2 targetPos = target.Center + new Vector2(wobble * 40f, -80 + wobble * 20f);
			float speed = 7f;
			float inertia = 18f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Fire prismatic bolt every 2 seconds
			if (NPC.ai[0] % 120 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				Vector2 fireDir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 10f;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.HallowBossRainbowStreak, NPC.damage / 3, 0f, Main.myPlayer);
			}

			// Rainbow/prismatic dust trail
			int[] colors = { DustID.PinkTorch, DustID.BlueTorch, DustID.YellowStarDust, DustID.PurpleTorch };
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
					colors[Main.rand.Next(colors.Length)], 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}

			NPC.spriteDirection = NPC.Center.X < target.Center.X ? 1 : -1;
			NPC.rotation = NPC.velocity.X * 0.03f;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Confused, 120);
			target.AddBuff(BuffID.ShadowFlame, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && spawnInfo.Player.ZoneHallow && !Main.dayTime)
				return 0.06f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.PixieDust, 1, 2, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.SoulofLight, 3, 1, 2));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.PhantomStag"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 10; i++)
			{
				int dustType = new[] { DustID.PinkTorch, DustID.BlueTorch, DustID.YellowStarDust }[Main.rand.Next(3)];
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);
			}
		}
	}
}
