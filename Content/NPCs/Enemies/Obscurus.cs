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
	/// Obscurus — a dark parasitic force born from suppressed magic. Not a beast — a magical phenomenon.
	/// Canon-faithful classification: dark force, not a standard creature.
	/// </summary>
	public class Obscurus : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 40;
			NPC.damage = 70;
			NPC.defense = 25;
			NPC.lifeMax = 800;
			NPC.HitSound = SoundID.NPCHit36;
			NPC.DeathSound = SoundID.NPCDeath65;
			NPC.value = Item.buyPrice(gold: 2);
			NPC.knockBackResist = 0.05f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.aiStyle = -1;
			NPC.alpha = 60;
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];

			NPC.ai[0]++;

			// Erratic dark cloud movement
			float speed = 10f;
			float wobbleX = (float)Math.Sin(NPC.ai[0] * 0.07) * 5f;
			float wobbleY = (float)Math.Cos(NPC.ai[0] * 0.05) * 3f;
			Vector2 targetPos = target.Center + new Vector2(wobbleX * 20, wobbleY * 20 - 50);
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = Vector2.Lerp(NPC.velocity, dir, 0.05f);

			// Dark energy burst every 2 seconds
			if (NPC.ai[0] % 120 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				// Ring of dark bolts
				for (int i = 0; i < 6; i++)
				{
					float angle = MathHelper.TwoPi / 6 * i;
					Vector2 boltDir = angle.ToRotationVector2() * 6f;
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, boltDir,
						ProjectileID.ShadowBeamHostile, NPC.damage / 4, 0f, Main.myPlayer);
				}
			}

			// Dense dark particle cloud
			for (int i = 0; i < 4; i++)
			{
				Dust dust = Dust.NewDustDirect(
					NPC.position + Main.rand.NextVector2Circular(NPC.width, NPC.height),
					4, 4, DustID.Shadowflame, 0f, 0f, 100, default, 1.5f);
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(2f, 2f);
			}

			// Flickering alpha
			NPC.alpha = 40 + (int)(Math.Sin(NPC.ai[0] * 0.12) * 30);
			NPC.rotation += NPC.velocity.X * 0.01f;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.ShadowFlame, 300);
			target.AddBuff(BuffID.Darkness, 300);
			target.AddBuff(BuffID.Slow, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && !Main.dayTime && spawnInfo.Player.ZoneOverworldHeight && !NPC.AnyNPCs(Type))
				return 0.003f; // Very rare

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.SoulofNight, 1, 5, 10));
			npcLoot.Add(ItemDropRule.Common(ItemID.DarkShard, 2));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Accessories.DiademOfRavenclaw>(), 15));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Obscurus"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 15; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame);
		}
	}
}
