using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.Buffs.Debuffs;

namespace WizardingWorld.Content.NPCs.Bosses.DementorKing
{
	/// <summary>
	/// Azkaban's Despair — Post-Golem flying boss.
	/// Phase 1: Hovers above player, dark energy waves, spawns Dementor minions. Extinguishes light.
	/// Phase 2 (below 60%): "The Kiss" — channels a beam draining HP. Faster, more Dementor spawns.
	/// Phase 3 (below 25%): "Soul Harvest" — dark vortex that pulls player in, Dementor rings, DarkCurse on all hits.
	/// Key mechanic: Having a Patronus active grants +30% damage against this boss.
	/// </summary>
	[AutoloadBossHead]
	public class DementorKingBoss : ModNPC
	{
		// ai[0] = phase (0, 1, 2)
		// ai[1] = attack timer
		// ai[2] = special timer (Kiss channel / vortex)
		// ai[3] = spawn timer

		public int Phase
		{
			get => (int)NPC.ai[0];
			set { NPC.ai[0] = value; NPC.netUpdate = true; }
		}

		public ref float AttackTimer => ref NPC.ai[1];
		public ref float SpecialTimer => ref NPC.ai[2];
		public ref float SpawnTimer => ref NPC.ai[3];

		private bool isChannelingKiss;
		private int kissTimer;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 6;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Darkness] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 60;
			NPC.height = 80;
			NPC.damage = 85;
			NPC.defense = 35;
			NPC.lifeMax = 45000;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 50);
			NPC.boss = true;
			NPC.npcSlots = 15f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit36;
			NPC.DeathSound = SoundID.NPCDeath65;

			if (!Main.dedServ)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/DementorKingBoss"); // post-Golem tier
		}

		public override void AI()
		{
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
				NPC.TargetClosest();

			Player player = Main.player[NPC.target];

			if (player.dead)
			{
				NPC.velocity.Y -= 0.2f;
				NPC.EncourageDespawn(10);
				return;
			}

			// Phase transitions
			float hpPercent = (float)NPC.life / NPC.lifeMax;
			if (hpPercent < 0.25f && Phase < 2)
			{
				Phase = 2;
				isChannelingKiss = false;
				kissTimer = 0;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

				// Soul Harvest entry burst
				for (int i = 0; i < 60; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 100, default, 2.5f);
					dust.velocity *= 6f;
					dust.noGravity = true;
				}
			}
			else if (hpPercent < 0.60f && Phase < 1)
			{
				Phase = 1;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

				for (int i = 0; i < 40; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 100, default, 2f);
					dust.velocity *= 4f;
					dust.noGravity = true;
				}
			}

			// Extinguish light in a massive radius — apply Darkness to nearby players
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player p = Main.player[i];
				if (p.active && !p.dead && Vector2.Distance(NPC.Center, p.Center) < 800f)
				{
					p.AddBuff(BuffID.Darkness, 60);
				}
			}

			switch (Phase)
			{
				case 0: DoPhase1(player); break;
				case 1: DoPhase2(player); break;
				case 2: DoPhase3(player); break;
			}

			NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;
			NPC.rotation = NPC.velocity.X * 0.02f;

			// Dark aura particles
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 100, default, 1.5f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}

		private void DoPhase1(Player player)
		{
			AttackTimer++;
			SpawnTimer++;

			// Hover above the player
			Vector2 targetPos = player.Center + new Vector2(0, -300);
			float speed = 7f;
			float inertia = 25f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Send waves of dark energy downward every 80 ticks
			if (AttackTimer >= 80 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;

				// Spread of dark bolts downward
				for (int i = -2; i <= 2; i++)
				{
					Vector2 fireDir = new Vector2(i * 3f, 10f);
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
						ProjectileID.ShadowBeamHostile, NPC.damage / 3, 0f, Main.myPlayer);
				}
			}

			// Spawn 2-3 Dementor minions every 8 seconds
			if (SpawnTimer >= 480 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpawnTimer = 0;
				int count = Main.rand.Next(2, 4);
				for (int i = 0; i < count; i++)
				{
					Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(250, 250);
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y,
						ModContent.NPCType<NPCs.Enemies.Dementor>());
				}
			}
		}

		private void DoPhase2(Player player)
		{
			AttackTimer++;
			SpecialTimer++;
			SpawnTimer++;

			if (!isChannelingKiss)
			{
				// Faster hover pursuit
				Vector2 targetPos = player.Center + new Vector2(NPC.direction * 200, -200);
				float speed = 12f;
				float inertia = 18f;
				Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
				NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

				// More frequent dark energy waves
				if (AttackTimer >= 50 && Main.netMode != NetmodeID.MultiplayerClient)
				{
					AttackTimer = 0;
					Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 12f;
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
						ProjectileID.ShadowBeamHostile, NPC.damage / 3, 0f, Main.myPlayer);

					// Side bolts
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir.RotatedBy(0.3f),
						ProjectileID.ShadowBeamHostile, NPC.damage / 4, 0f, Main.myPlayer);
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir.RotatedBy(-0.3f),
						ProjectileID.ShadowBeamHostile, NPC.damage / 4, 0f, Main.myPlayer);
				}

				// Begin "The Kiss" channel every 6 seconds
				if (SpecialTimer >= 360)
				{
					SpecialTimer = 0;
					isChannelingKiss = true;
					kissTimer = 0;
					NPC.velocity *= 0.2f;
					NPC.netUpdate = true;
					SoundEngine.PlaySound(SoundID.NPCDeath52, NPC.Center);
				}
			}
			else
			{
				// "The Kiss" — channel toward player for 3 seconds, rapidly draining HP
				kissTimer++;

				// Slow hover during channel
				NPC.velocity *= 0.95f;

				// Beam dust between boss and player
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 toPlayer = player.Center - NPC.Center;
					float dist = toPlayer.Length();
					Vector2 normDir = toPlayer / dist;

					// Visual beam dust
					for (int i = 0; i < (int)(dist / 20f); i++)
					{
						Vector2 dustPos = NPC.Center + normDir * (i * 20f);
						Dust dust = Dust.NewDustDirect(dustPos, 1, 1, DustID.Shadowflame, 0f, 0f, 150, default, 1.2f);
						dust.noGravity = true;
						dust.velocity = Vector2.Zero;
					}

					// Drain HP every 10 ticks (6 times per second for 3 seconds = 18 hits)
					if (kissTimer % 10 == 0 && dist < 600f)
					{
						player.Hurt(Terraria.DataStructures.PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage / 5, 0, cooldownCounter: ImmunityCooldownID.Bosses);
					}
				}

				if (kissTimer >= 180) // 3 seconds
				{
					isChannelingKiss = false;
					NPC.netUpdate = true;
				}
			}

			// More frequent Dementor spawns
			if (SpawnTimer >= 360 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpawnTimer = 0;
				int count = Main.rand.Next(2, 4);
				for (int i = 0; i < count; i++)
				{
					Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(300, 300);
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y,
						ModContent.NPCType<NPCs.Enemies.Dementor>());
				}
			}

			// Intense shadow dust
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 80, default, 1.8f);
				dust.noGravity = true;
				dust.velocity *= 0.5f;
			}
		}

		private void DoPhase3(Player player)
		{
			AttackTimer++;
			SpecialTimer++;
			SpawnTimer++;

			// "Soul Harvest" — create a vortex that pulls the player in
			NPC.damage = (int)(NPC.defDamage * 1.5f);

			// Hover and orbit
			float speed = 10f;
			float inertia = 15f;
			Vector2 targetPos = player.Center + new Vector2((float)Math.Cos(SpecialTimer * 0.02f) * 250, -200);
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Pull player toward the boss (vortex mechanic)
			float pullDist = Vector2.Distance(NPC.Center, player.Center);
			if (pullDist < 600f && pullDist > 50f)
			{
				Vector2 pullDir = (NPC.Center - player.Center).SafeNormalize(Vector2.Zero);
				float pullStrength = 2.5f * (1f - pullDist / 600f);
				player.velocity += pullDir * pullStrength;
			}

			// Rapid dark energy attacks
			if (AttackTimer >= 30 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 14f;
				fireDir = fireDir.RotatedByRandom(MathHelper.ToRadians(15));
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.ShadowBeamHostile, NPC.damage / 3, 0f, Main.myPlayer);
			}

			// Spawn Dementor rings around the boss every 5 seconds
			if (SpawnTimer >= 300 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpawnTimer = 0;

				// Ring of Dementors
				int ringCount = 4;
				for (int i = 0; i < ringCount; i++)
				{
					float angle = MathHelper.TwoPi / ringCount * i;
					Vector2 spawnPos = NPC.Center + angle.ToRotationVector2() * 300f;
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y,
						ModContent.NPCType<NPCs.Enemies.Dementor>());
				}

				SoundEngine.PlaySound(SoundID.NPCDeath52, NPC.Center);
			}

			// Screen darkens — heavy shadow dust everywhere
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position - new Vector2(100, 100), NPC.width + 200, NPC.height + 200,
					DustID.Shadowflame, 0f, 0f, 200, default, 2.5f);
				dust.noGravity = true;
				dust.velocity *= 2f;
			}

			// Vortex visual — spinning dark dust
			for (int i = 0; i < 3; i++)
			{
				float vortexAngle = SpecialTimer * 0.1f + i * MathHelper.TwoPi / 3;
				Vector2 vortexPos = NPC.Center + vortexAngle.ToRotationVector2() * 150f;
				Dust dust = Dust.NewDustDirect(vortexPos, 1, 1, DustID.Shadowflame, 0f, 0f, 100, default, 2f);
				dust.noGravity = true;
				dust.velocity = (NPC.Center - vortexPos).SafeNormalize(Vector2.Zero) * 3f;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			switch (Phase)
			{
				case 0:
					// Phase 1: Darkness + Mana Sickness
					target.AddBuff(BuffID.Darkness, 300);
					target.AddBuff(BuffID.ManaSickness, 300);
					target.GetModPlayer<Common.Players.WizardPlayer>().AddDespair(0.18f, "Azkaban's Despair");
					break;
				case 1:
					// Phase 2: same but longer
					target.AddBuff(BuffID.Darkness, 480);
					target.AddBuff(BuffID.ManaSickness, 480);
					target.GetModPlayer<Common.Players.WizardPlayer>().AddDespair(0.25f, "the Dementor's Kiss");
					break;
				case 2:
					// Phase 3: Dark Curse on all hits
					target.AddBuff(ModContent.BuffType<DarkCurseDebuff>(), 360);
					target.GetModPlayer<Common.Players.WizardPlayer>().AddDespair(0.35f, "Soul Harvest");
					break;
			}
		}

		public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
		{
			if (player.GetModPlayer<Common.Players.WizardPlayer>().patronusActive)
				modifiers.FinalDamage *= 1.30f;
		}

		public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
		{
			if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
				return;

			Player player = Main.player[projectile.owner];
			if (player.GetModPlayer<Common.Players.WizardPlayer>().patronusActive)
				modifiers.FinalDamage *= 1.30f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.DementorKing.DementorKingBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.DementorKing.DementorKingTrophy>(), 10));

			// Dementor's Shroud (endgame crafting material)
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.DementorsShroud>(), 1, 5, 10));

			// Essence of Magic — massive amount
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 30, 50));

			// Super healing potions
			npcLoot.Add(ItemDropRule.Common(ItemID.SuperHealingPotion, 1, 5, 15));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedDementorKing, -1);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = ImmunityCooldownID.Bosses;
			return true;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.DementorKing"),
			});
		}
	}
}
