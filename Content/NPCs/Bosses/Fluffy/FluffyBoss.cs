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

namespace WizardingWorld.Content.NPCs.Bosses.Fluffy
{
	/// <summary>
	/// Fluffy — Three-Headed Dog. Post-Mechanical Bosses tier.
		/// Phase 1: Charges at player with 3 cycling attack patterns (bite, pounce, bark shockwave).
		/// Phase 2 (below 50%): All 3 heads attack simultaneously, faster charges, howl applies Jinxed.
		/// Phase 3 (below 20%): Desperate fury — continuous charging with multi-head bite and bark shockwaves.
		/// </summary>
	[AutoloadBossHead]
	public class FluffyBoss : ModNPC
	{
		// ai[0] = phase (0, 1, 2)
		// ai[1] = attack timer
		// ai[2] = head pattern index (0=bite, 1=pounce, 2=bark shockwave)
		// ai[3] = special timer (howl cooldown / fire breath timer)

		public int Phase
		{
			get => (int)NPC.ai[0];
			set { NPC.ai[0] = value; NPC.netUpdate = true; }
		}

		public ref float AttackTimer => ref NPC.ai[1];
		public ref float HeadPattern => ref NPC.ai[2];
		public ref float SpecialTimer => ref NPC.ai[3];

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 8;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 116;
			NPC.height = 90;
			NPC.damage = 60;
			NPC.defense = 25;
			NPC.lifeMax = 18000;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.value = Item.buyPrice(gold: 15);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath10;

			if (!Main.dedServ)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/FluffyBoss");
		}

		public override void AI()
		{
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
				NPC.TargetClosest();

			Player player = Main.player[NPC.target];

			if (player.dead)
			{
				NPC.velocity.X *= 0.95f;
				NPC.EncourageDespawn(10);
				return;
			}

			// Music weakness — holding the Enchanted Flute makes Fluffy sluggish and drowsy.
			if (player.HeldItem.type == ModContent.ItemType<Items.Consumables.FluffySummonItem>())
			{
				NPC.velocity *= 0.92f;
				if (Main.rand.NextBool(8))
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, -0.4f, 100, default, 0.8f);
					dust.noGravity = true;
				}
			}

			// Phase transitions
			float hpPercent = (float)NPC.life / NPC.lifeMax;
			if (hpPercent < 0.20f && Phase < 2)
			{
				Phase = 2;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
				TransitionDust(DustID.Smoke);
			}
			else if (hpPercent < 0.50f && Phase < 1)
			{
				Phase = 1;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
				TransitionDust(DustID.Blood);
			}

			switch (Phase)
			{
				case 0: DoPhase1(player); break;
				case 1: DoPhase2(player); break;
				case 2: DoPhase3(player); break;
			}

			if (player.HeldItem.type == ModContent.ItemType<Items.Consumables.FluffySummonItem>())
				NPC.damage = (int)(NPC.damage * 0.85f);

			// Face the player
			NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;
			NPC.rotation = NPC.velocity.X * 0.01f;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;

			if (NPC.frameCounter >= (Phase >= 1 ? 6 : 8))
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += frameHeight;

				if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
					NPC.frame.Y = 0;
			}
		}

		private void DoPhase1(Player player)
		{
			// 3 different attack patterns cycling: bite, pounce, bark shockwave
			// Each "head" attacks independently via timer offsets
			AttackTimer++;

			int pattern = (int)HeadPattern % 3;

			if (pattern == 0)
				DoBiteAttack(player, 10f, 100);
			else if (pattern == 1)
				DoPounceAttack(player, 14f, 120);
			else
				DoBarkShockwave(player, 90);
		}

		private void DoPhase2(Player player)
		{
			// All 3 heads attack simultaneously, faster charges, occasional howl
			AttackTimer++;
			SpecialTimer++;

			// Faster charge at player
			float speed = 12f;
			float inertia = 12f;
			Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * speed;
			dir.Y = Math.Min(dir.Y, 2f); // Keep mostly horizontal since ground-based

			NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + dir.X) / inertia;

			// Apply gravity manually
			if (NPC.velocity.Y < 10f)
				NPC.velocity.Y += 0.3f;

			NPC.damage = NPC.defDamage;

			// All 3 heads attack together every 80 ticks — bark bursts and hurled debris, not elemental breath.
			if (AttackTimer >= 80 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;

				Vector2 baseDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

				// Head 1 — direct snapping lunge debris
				WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.Fluffy, NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -20),
					baseDir * 12f, ProjectileID.BoulderStaffOfEarth, NPC.damage / 3, 0f, Main.myPlayer);

				// Head 2 — high bark burst
				WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.Fluffy, NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -30),
					baseDir.RotatedBy(-0.25f) * 11f, ProjectileID.DD2OgreSmash, NPC.damage / 3, 0f, Main.myPlayer);

				// Head 3 — low bark burst
				WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.Fluffy, NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -10),
					baseDir.RotatedBy(0.25f) * 11f, ProjectileID.DD2OgreSmash, NPC.damage / 3, 0f, Main.myPlayer);

				SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);
			}

			// Howl every 6 seconds — applies Jinxed to all nearby players
			if (SpecialTimer >= 360)
			{
				SpecialTimer = 0;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

				// Apply Jinxed debuff to all nearby players within 600 pixels
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player p = Main.player[i];
					if (p.active && !p.dead && Vector2.Distance(NPC.Center, p.Center) < 600f)
					{
						p.AddBuff(ModContent.BuffType<JinxedDebuff>(), 300); // 5 seconds
					}
				}

				// Howl dust ring
				for (int i = 0; i < 50; i++)
				{
					float angle = MathHelper.TwoPi / 50 * i;
					Vector2 dustVel = angle.ToRotationVector2() * 6f;
					Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.PurpleTorch, dustVel.X, dustVel.Y, 100, default, 1.5f);
					dust.noGravity = true;
				}
			}

			// Blood dust trail
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 1.2f);
				dust.noGravity = true;
			}
		}

		private void DoPhase3(Player player)
		{
			// Desperate fury — continuous charging with multi-head bite and bark shockwaves
			AttackTimer++;
			SpecialTimer++;

			// Very aggressive ground pursuit
			float speed = 16f;
			float inertia = 8f;
			Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * speed;

			NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + dir.X) / inertia;

			// Apply gravity
			if (NPC.velocity.Y < 10f)
				NPC.velocity.Y += 0.3f;

			NPC.damage = (int)(NPC.defDamage * 1.5f);

			// Multi-head bite attack — 3 boulder projectiles in a spread every 30 ticks
			if (AttackTimer >= 30 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;

				Vector2 baseDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

				for (int head = -1; head <= 1; head++)
				{
					Vector2 offset = new Vector2(0, head * 15 - 15);
					Vector2 biteDir = baseDir.RotatedBy(head * 0.20f) * 14f;
					WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.Fluffy, NPC.GetSource_FromAI(), NPC.Center + offset, biteDir,
						ProjectileID.BoulderStaffOfEarth, NPC.damage / 3, 2f, Main.myPlayer);
				}

				SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center); // Bite sound
			}

			// Bark shockwave — dust ring that pushes players back every 90 ticks
			if (SpecialTimer >= 90)
			{
				SpecialTimer = 0;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

				// Visual dust ring
				for (int i = 0; i < 40; i++)
				{
					float angle = MathHelper.TwoPi / 40 * i;
					Vector2 dustVel = angle.ToRotationVector2() * 7f;
					Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.Smoke, dustVel.X, dustVel.Y, 100, default, 2.5f);
					dust.noGravity = true;
				}

				// Push back all nearby players
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player p = Main.player[i];
					if (p.active && !p.dead)
					{
						float dist = Vector2.Distance(NPC.Center, p.Center);
						if (dist < 400f && dist > 10f)
						{
							Vector2 pushDir = (p.Center - NPC.Center).SafeNormalize(Vector2.Zero);
							float pushStrength = 12f * (1f - dist / 400f);
							p.velocity += pushDir * pushStrength;
						}
					}
				}
			}

			// Occasional jump at player if they are above
			if (AttackTimer == 15 && player.Center.Y < NPC.Center.Y - 100)
			{
				NPC.velocity.Y = -12f;
				NPC.netUpdate = true;
			}

			// Blood and dust trail (no fire)
			for (int i = 0; i < 3; i++)
			{
				int dustType = Main.rand.NextBool() ? DustID.Smoke : DustID.Blood;
				Dust dust2 = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 50, default, 1.8f);
				dust2.noGravity = true;
				dust2.velocity *= 1.5f;
			}
		}

		private void DoBiteAttack(Player player, float chargeSpeed, int cycleLength)
		{
			// Head 1: Bite — charge directly at player, snap at close range
			if (AttackTimer < cycleLength - 20)
			{
				float inertia = 15f;
				Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * chargeSpeed;
				NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + dir.X) / inertia;

				if (NPC.velocity.Y < 10f)
					NPC.velocity.Y += 0.3f;

				NPC.damage = NPC.defDamage;
			}
			else if (AttackTimer >= cycleLength)
			{
				AttackTimer = 0;
				HeadPattern = 1; // Cycle to pounce
				NPC.netUpdate = true;
			}
		}

		private void DoPounceAttack(Player player, float pounceSpeed, int cycleLength)
		{
			// Head 2: Pounce — leap at player
			if (AttackTimer == 1)
			{
				// Launch toward player
				Vector2 pounceDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
				NPC.velocity.X = pounceDir.X * pounceSpeed;
				NPC.velocity.Y = -8f; // Jump upward
				NPC.damage = (int)(NPC.defDamage * 1.2f);
				NPC.netUpdate = true;

				SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);
			}
			else
			{
				// Apply gravity during pounce
				if (NPC.velocity.Y < 10f)
					NPC.velocity.Y += 0.35f;
			}

			if (AttackTimer >= cycleLength)
			{
				AttackTimer = 0;
				HeadPattern = 2; // Cycle to bark
				NPC.netUpdate = true;
			}
		}

		private void DoBarkShockwave(Player player, int cycleLength)
		{
			// Head 3: Bark shockwave — stop and fire a spread of projectiles
			if (AttackTimer < 30)
			{
				// Slow down, preparing to bark
				NPC.velocity.X *= 0.92f;
				if (NPC.velocity.Y < 10f)
					NPC.velocity.Y += 0.3f;

				NPC.damage = 0;
			}
			else if (AttackTimer == 30 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				// Bark shockwave — spread of projectiles in a forward arc
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

				Vector2 baseDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
				for (int i = -3; i <= 3; i++)
				{
					Vector2 spreadDir = baseDir.RotatedBy(MathHelper.ToRadians(i * 15)) * 8f;
					WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.Fluffy, NPC.GetSource_FromAI(), NPC.Center, spreadDir,
						ProjectileID.DD2OgreSmash, NPC.damage / 4, 0f, Main.myPlayer);
				}

				// Dust shockwave ring
				for (int i = 0; i < 30; i++)
				{
					float angle = MathHelper.TwoPi / 30 * i;
					Vector2 dustVel = angle.ToRotationVector2() * 5f;
					Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.Smoke, dustVel.X, dustVel.Y, 100, default, 2f);
					dust.noGravity = true;
				}
			}
			else if (AttackTimer >= cycleLength)
			{
				AttackTimer = 0;
				HeadPattern = 0; // Cycle back to bite
				NPC.netUpdate = true;
			}
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

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.Fluffy.FluffyBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.Fluffy.FluffyTrophy>(), 10));

			// Cerberus Fang (crafting material)
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.CerberusFang>(), 1, 3, 7));

			// Greater healing potions
			npcLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 15));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedFluffy, -1);
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
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Fluffy"),
			});
		}
	}
}
