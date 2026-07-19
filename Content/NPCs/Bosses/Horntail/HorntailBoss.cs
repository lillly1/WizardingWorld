using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;

namespace WizardingWorld.Content.NPCs.Bosses.Horntail
{
	/// <summary>
	/// Hungarian Horntail — Hardmode dragon boss.
	/// Phase 1: Flies around, shoots fireballs.
	/// Phase 2: Enraged charges, tail swipe projectiles, fire breath.
	/// Phase 3 (below 25%): Desperate fury — rapid attacks, spawns fire rain.
	/// </summary>
	[AutoloadBossHead]
	public class HorntailBoss : ModNPC
	{
		public int Phase
		{
			get => (int)NPC.ai[0];
			set { NPC.ai[0] = value; NPC.netUpdate = true; }
		}

		public ref float AttackTimer => ref NPC.ai[1];
		public ref float MoveTimer => ref NPC.ai[2];
		public ref float SpecialTimer => ref NPC.ai[3];

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 8;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 148;
			NPC.height = 92;
			NPC.damage = 70;
			NPC.defense = 30;
			NPC.lifeMax = 28000;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 25);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = SoundID.NPCDeath10;

			if (!Main.dedServ)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/HorntailBoss");
		}

		public override void AI()
		{
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
				NPC.TargetClosest();

			Player player = Main.player[NPC.target];

			if (player.dead)
			{
				NPC.velocity.Y -= 0.1f;
				NPC.EncourageDespawn(10);
				return;
			}

			// Phase transitions
			float hpPercent = (float)NPC.life / NPC.lifeMax;
			if (hpPercent < 0.25f && Phase < 2)
				Phase = 2;
			else if (hpPercent < 0.6f && Phase < 1)
				Phase = 1;

			switch (Phase)
			{
				case 0: DoPhase1(player); break;
				case 1: DoPhase2(player); break;
				case 2: DoPhase3(player); break;
			}

			NPC.spriteDirection = NPC.direction;
			NPC.rotation = NPC.velocity.X * 0.02f;

			// Fire dust
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
				dust.noGravity = true;
			}
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;

			if (NPC.frameCounter >= (Phase >= 1 ? 6 : 7))
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += frameHeight;

				if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
					NPC.frame.Y = 0;
			}
		}

		private void DoPhase1(Player player)
		{
			AttackTimer++;
			MoveTimer++;

			// Fly above player
			Vector2 targetPos = player.Center + new Vector2(NPC.direction * 300, -200);
			float speed = 8f;
			float inertia = 25f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Shoot fireballs every 90 ticks
			if (AttackTimer >= 90 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 12f;
				WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.Horntail, NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.Fireball, NPC.damage / 3, 0f, Main.myPlayer);
			}

			// Switch sides occasionally
			if (MoveTimer > 180)
			{
				MoveTimer = 0;
				NPC.direction *= -1;
				NPC.netUpdate = true;
			}
		}

		private void DoPhase2(Player player)
		{
			AttackTimer++;

			float speed = 14f;
			float inertia = 12f;

			if (AttackTimer < 80)
			{
				// Aggressive chase
				Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
				NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;
				NPC.damage = NPC.defDamage;
			}
			else if (AttackTimer == 80)
			{
				// Fire breath — spread of fireballs
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 baseDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 10f;
					for (int i = -2; i <= 2; i++)
					{
						Vector2 spreadDir = baseDir.RotatedBy(MathHelper.ToRadians(i * 12));
						WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.Horntail, NPC.GetSource_FromAI(), NPC.Center, spreadDir,
							ProjectileID.Fireball, NPC.damage / 3, 0f, Main.myPlayer);
					}
				}

				SoundEngine.PlaySound(WizardSoundStyles.DragonRoar, NPC.Center);
			}
			else if (AttackTimer > 120)
			{
				AttackTimer = 0;
				NPC.netUpdate = true;
			}
		}

		private void DoPhase3(Player player)
		{
			AttackTimer++;
			SpecialTimer++;

			// Very aggressive pursuit
			float speed = 18f;
			float inertia = 8f;
			Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;
			NPC.damage = (int)(NPC.defDamage * 1.5f);

			// Rapid fireballs
			if (AttackTimer >= 40 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 14f;
				WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.Horntail, NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.Fireball, NPC.damage / 3, 0f, Main.myPlayer);
			}

			// Fire rain from sky every 3 seconds
			if (SpecialTimer >= 180 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpecialTimer = 0;
				for (int i = 0; i < 5; i++)
				{
					Vector2 spawnPos = player.Center + new Vector2(Main.rand.Next(-300, 300), -600);
					WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.Horntail, NPC.GetSource_FromAI(), spawnPos, new Vector2(0, 8f),
						ProjectileID.Fireball, NPC.damage / 4, 0f, Main.myPlayer);
				}
			}

			// Intense fire aura
			for (int i = 0; i < 3; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 50, default, 2f);
				dust.noGravity = true;
				dust.velocity *= 2f;
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.Horntail.HorntailBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.Horntail.HorntailTrophy>(), 10));

			// Dragon Scale (crafting material)
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.DragonScale>(), 1, 5, 10));

			// Golden Egg
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.GoldenEgg>(), 3));

			npcLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 15));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedHorntail, -1);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = ImmunityCooldownID.Bosses;
			return true;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Horntail"),
			});
		}
	}
}
