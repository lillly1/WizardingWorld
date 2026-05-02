using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Nagini — Voldemort's serpent Horcrux. Mini-boss that spawns in Hardmode.
	/// Fast slithering, venomous lunges, and summons smaller snakes.
	/// </summary>
	public class Nagini : ModNPC
	{
		public ref float AttackTimer => ref NPC.ai[0];
		public ref float LungeTimer => ref NPC.ai[1];

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 6;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 30;
			NPC.damage = 55;
			NPC.defense = 20;
			NPC.lifeMax = 3000;
			NPC.knockBackResist = 0.1f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 8);
			NPC.npcSlots = 5f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath10;

			if (!Main.dedServ)
				Music = MusicID.Boss1;
		}

		public override void AI()
		{
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
				NPC.TargetClosest();

			Player player = Main.player[NPC.target];

			if (player.dead)
			{
				NPC.velocity.Y += 0.1f;
				NPC.EncourageDespawn(10);
				return;
			}

			AttackTimer++;
			LungeTimer++;

			// Phase 1: Slithering chase
			float speed = 8f + (1f - (float)NPC.life / NPC.lifeMax) * 6f; // Gets faster as HP drops
			float inertia = 18f;
			Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Lunge attack every 3 seconds
			if (LungeTimer >= 180)
			{
				LungeTimer = 0;
				Vector2 lungeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
				NPC.velocity = lungeDir * 18f;
				NPC.netUpdate = true;

				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
			}

			// Spit venom projectile every 2 seconds
			if (AttackTimer >= 120 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 10f;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.VenomFang, NPC.damage / 3, 0f, Main.myPlayer);
			}

			// Venom dust trail
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GreenTorch, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}

			NPC.rotation = NPC.velocity.ToRotation();
			NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Venom, 300);
			target.AddBuff(BuffID.Poisoned, 600);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && !Main.dayTime && spawnInfo.Player.ZoneOverworldHeight
				&& !NPC.AnyNPCs(Type))
				return 0.005f; // Very rare natural spawn

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.VoldemortSummonItem>(), 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Accessories.SlytherinsLocket>(), 8));
			npcLoot.Add(ItemDropRule.Common(ItemID.VialofVenom, 1, 5, 15));
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = ImmunityCooldownID.Bosses;
			return true;
		}

		public override void OnKill()
		{
			Player player = NPC.target >= 0 && NPC.target < Main.maxPlayers ? Main.player[NPC.target] : null;
			Common.Systems.HorcruxHuntSystem.MarkNaginiDefeated(player);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Nagini"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GreenTorch);
		}
	}
}
