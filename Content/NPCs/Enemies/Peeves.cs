using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Peeves — poltergeist prankster. Flies erratically, throws objects at players, and teleports.</summary>
	public class Peeves : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 24;
			NPC.height = 38;
			NPC.damage = 25;
			NPC.defense = 8;
			NPC.lifeMax = 180;
			NPC.HitSound = SoundID.NPCHit36;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = Item.buyPrice(silver: 20);
			NPC.knockBackResist = 0.1f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.aiStyle = -1;
			NPC.alpha = 80; // Semi-transparent ghost
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];

			NPC.ai[0]++;

			// Erratic flight pattern — zig-zag around player
			float wobble = (float)Math.Sin(NPC.ai[0] * 0.08f) * 4f;
			Vector2 targetPos = target.Center + new Vector2(wobble * 30f, -100 + wobble * 20f);
			float speed = 6f;
			float inertia = 20f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Throw objects at player periodically
			if (NPC.ai[0] % 90 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				Vector2 throwDir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 8f;
				// Throws random vanilla hostile projectile for variety
				int projType = Main.rand.NextBool() ? ProjectileID.RockGolemRock : ProjectileID.SandBallFalling;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, throwDir,
					projType, NPC.damage / 3, 2f, Main.myPlayer);
			}

			// Random teleport every 5-8 seconds
			if (NPC.ai[0] % 360 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.Center = target.Center + Main.rand.NextVector2CircularEdge(200, 200);
				NPC.netUpdate = true;

				for (int i = 0; i < 15; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Wraith, 0f, 0f, 100, default, 1.2f);
					dust.noGravity = true;
					dust.velocity *= 2f;
				}
			}

			// Ghost dust trail
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Wraith, 0f, 0f, 150, default, 0.7f);
				dust.noGravity = true;
			}

			// Flickering visibility (poltergeist effect)
			NPC.alpha = 80 + (int)(Math.Sin(NPC.ai[0] * 0.15) * 40);
			NPC.spriteDirection = NPC.Center.X < target.Center.X ? 1 : -1;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Confused, 120);
		}

		// Peeves is Hogwarts-specific — dungeon only (represents castle ruins)
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneDungeon)
				return 0.06f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.WetBomb, 2, 3, 8));
			npcLoot.Add(ItemDropRule.Common(ItemID.StinkPotion, 3, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.Confetti, 4, 5, 15));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Peeves"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 8; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Wraith);
		}
	}
}
