using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.Buffs.Debuffs;

namespace WizardingWorld.Content.NPCs.Bosses.Umbridge
{
	/// <summary>
	/// Dolores Umbridge -- The Debuff Boss. Post-mech bosses, hardmode.
	/// She doesn't deal much damage directly. Instead she DEBUFFS you constantly.
	/// Phase 1: Hovers, fires pink Educational Decree projectiles (Jinxed), announces decrees every 5 seconds.
	/// Phase 2 (below 50%): "I WILL have order!" -- faster fire, Petrified on contact, summons Enchanted Sword enemies.
	/// Phase 3 (below 20%): Panic -- rapid teleporting, constant debuffs, blood quill projectiles (Bleeding).
	/// </summary>
	[AutoloadBossHead]
	public class UmbridgeBoss : ModNPC
	{
		// ai[0] = phase (0, 1, 2)
		// ai[1] = attack timer
		// ai[2] = decree timer (chat messages + debuff application)
		// ai[3] = teleport timer / special timer

		public int Phase
		{
			get => (int)NPC.ai[0];
			set { NPC.ai[0] = value; NPC.netUpdate = true; }
		}

		public ref float AttackTimer => ref NPC.ai[1];
		public ref float DecreeTimer => ref NPC.ai[2];
		public ref float SpecialTimer => ref NPC.ai[3];

		private bool spawnedSquad;
		private int decreeNumber = 24; // Start at Educational Decree #24

		// Debuffs to randomly apply via decrees
		private static readonly int[] DecreeDebuffs = new int[]
		{
			BuffID.Slow,
			BuffID.Weak,
			BuffID.Confused,
			BuffID.Darkness,
			BuffID.ManaSickness,
		};

		private static readonly string[] DecreeDebuffNames = new string[]
		{
			"speedy movement",
			"excessive strength",
			"clear thinking",
			"adequate lighting",
			"proper mana usage",
		};

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 6;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 56;
			NPC.damage = 40;
			NPC.defense = 35;
			NPC.lifeMax = 20000;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 20);
			NPC.boss = true;
			NPC.npcSlots = 12f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit36;
			NPC.DeathSound = SoundID.NPCDeath65;

			if (!Main.dedServ)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/BellatrixBoss");
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
			if (hpPercent < 0.20f && Phase < 2)
			{
				Phase = 2;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
				TransitionDust(DustID.PinkTorch);

				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BossDialogue.UmbridgePhase3"), Color.HotPink);
			}
			else if (hpPercent < 0.50f && Phase < 1)
			{
				Phase = 1;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
				TransitionDust(DustID.PinkTorch);

				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BossDialogue.UmbridgePhase2"), Color.HotPink);
			}

			switch (Phase)
			{
				case 0: DoPhase1(player); break;
				case 1: DoPhase2(player); break;
				case 2: DoPhase3(player); break;
			}

			NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;
			NPC.rotation = NPC.velocity.X * 0.02f;

			// Pink aura
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PinkTorch, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}

		private void DoPhase1(Player player)
		{
			AttackTimer++;
			DecreeTimer++;

			// Hover near player
			Vector2 targetPos = player.Center + new Vector2(NPC.direction * 300, -150);
			float speed = 6f;
			float inertia = 25f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Fire pink Educational Decree projectiles every 60 ticks
			if (AttackTimer >= 60 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 10f;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.PinkLaser, NPC.damage / 3, 0f, Main.myPlayer);

				SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
			}

			// Announce a decree every 5 seconds (300 ticks) and apply debuff
			if (DecreeTimer >= 300)
			{
				DecreeTimer = 0;
				AnnounceDecreeAndDebuff(player);
			}
		}

		private void DoPhase2(Player player)
		{
			AttackTimer++;
			DecreeTimer++;
			SpecialTimer++;

			// Faster hover, closer to player
			Vector2 targetPos = player.Center + new Vector2(NPC.direction * 220, -100);
			float speed = 9f;
			float inertia = 18f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Faster pink projectiles every 40 ticks
			if (AttackTimer >= 40 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 12f;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.PinkLaser, NPC.damage / 3, 0f, Main.myPlayer);

				// Occasionally fire a spread
				if (Main.rand.NextBool(3))
				{
					for (int i = -1; i <= 1; i += 2)
					{
						Vector2 spreadDir = fireDir.RotatedBy(MathHelper.ToRadians(i * 15));
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, spreadDir,
							ProjectileID.PinkLaser, NPC.damage / 4, 0f, Main.myPlayer);
					}
				}

				SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
			}

			// Faster decrees every 4 seconds
			if (DecreeTimer >= 240)
			{
				DecreeTimer = 0;
				AnnounceDecreeAndDebuff(player);
			}

			// Summon 2 Enchanted Sword enemies once as Inquisitorial Squad
			if (!spawnedSquad && Main.netMode != NetmodeID.MultiplayerClient)
			{
				spawnedSquad = true;
				for (int i = 0; i < 2; i++)
				{
					Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(200, 200);
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y,
						NPCID.EnchantedSword);
				}

				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BossDialogue.UmbridgeSummon"), Color.HotPink);
			}
		}

		private void DoPhase3(Player player)
		{
			AttackTimer++;
			DecreeTimer++;
			SpecialTimer++;

			// Panicked movement -- erratic and fast
			float speed = 13f;
			float inertia = 10f;
			Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Rapid blood quill projectiles (apply Bleeding) every 20 ticks
			if (AttackTimer >= 20 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 14f;
				fireDir = fireDir.RotatedByRandom(MathHelper.ToRadians(12));
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.PinkLaser, NPC.damage / 2, 0f, Main.myPlayer);
			}

			// Rapid teleporting every 2 seconds
			if (SpecialTimer >= 120)
			{
				SpecialTimer = 0;
				if (Main.netMode != NetmodeID.MultiplayerClient)
					TeleportNearPlayer(player);
			}

			// Constant debuff application every 3 seconds
			if (DecreeTimer >= 180)
			{
				DecreeTimer = 0;
				AnnounceDecreeAndDebuff(player);
			}

			// Intense pink aura
			for (int i = 0; i < 3; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PinkTorch, 0f, 0f, 50, default, 1.5f);
				dust.noGravity = true;
				dust.velocity *= 2f;
			}
		}

		private void AnnounceDecreeAndDebuff(Player player)
		{
			int debuffIndex = Main.rand.Next(DecreeDebuffs.Length);
			int debuffId = DecreeDebuffs[debuffIndex];
			string bannedThing = DecreeDebuffNames[debuffIndex];

			decreeNumber++;

			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BossDialogue.UmbridgeDecree", decreeNumber, bannedThing), Color.HotPink);
			}

			// Apply the debuff to all nearby players
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player p = Main.player[i];
					if (p.active && !p.dead && Vector2.Distance(NPC.Center, p.Center) < 1500f)
					{
						p.AddBuff(debuffId, 300); // 5 seconds
						// Also apply Jinxed
						p.AddBuff(ModContent.BuffType<JinxedDebuff>(), 180); // 3 seconds
					}
				}
			}
		}

		private void TeleportNearPlayer(Player player)
		{
			Vector2 newPos = player.Center + Main.rand.NextVector2CircularEdge(300, 300);
			NPC.Center = newPos;
			NPC.netUpdate = true;

			for (int i = 0; i < 25; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PinkTorch, 0f, 0f, 50, default, 1.5f);
				dust.noGravity = true;
				dust.velocity *= 3f;
			}

			SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
		}

		private void TransitionDust(int dustType)
		{
			for (int i = 0; i < 40; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 2f);
				dust.velocity *= 4f;
				dust.noGravity = true;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Always apply Jinxed
			target.AddBuff(ModContent.BuffType<JinxedDebuff>(), 240); // 4 seconds

			// Phase 2+: Petrified on contact
			if (Phase >= 1)
			{
				target.AddBuff(ModContent.BuffType<PetrifiedDebuff>(), 60); // 1 second stun
			}

			// Phase 3: Blood quill Bleeding
			if (Phase >= 2)
			{
				target.AddBuff(BuffID.Bleeding, 300); // 5 seconds
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.Umbridge.UmbridgeBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.Umbridge.UmbridgeTrophy>(), 10));

			// Umbridge's Quill weapon -- 33% chance
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.UmbridgesQuill>(), 3));

			// Greater healing potions
			npcLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 15));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedUmbridge, -1);
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
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Umbridge"),
			});
		}
	}
}
