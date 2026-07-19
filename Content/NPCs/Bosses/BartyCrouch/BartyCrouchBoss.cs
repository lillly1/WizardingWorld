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

namespace WizardingWorld.Content.NPCs.Bosses.BartyCrouch
{
	/// <summary>
	/// Barty Crouch Jr — post-Bellatrix + post-Plantera shapeshifter boss.
	/// Phase 1 (Disguise — "Moody"): Walks on ground, fires accurate single-target bolts. Drinks from flask to heal.
	/// Phase 2 (below 70% HP — true form): Flies, fires rapid dark curse projectiles, more aggressive.
	/// Phase 3 (below 30% HP): Teleports erratically, summons Death Eater minions, spread of killing curse bolts.
	/// </summary>
	[AutoloadBossHead]
	public class BartyCrouchBoss : ModNPC
	{
		public int Phase
		{
			get => (int)NPC.ai[0];
			set { NPC.ai[0] = value; NPC.netUpdate = true; }
		}

		public ref float AttackTimer => ref NPC.ai[1];
		public ref float SpecialTimer => ref NPC.ai[2];
		public ref float TeleportTimer => ref NPC.ai[3];

		private bool hasTransformed;
		private bool hasEnteredPhase3;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 8;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 46;
			NPC.height = 72;
			NPC.damage = 65;
			NPC.defense = 28;
			NPC.lifeMax = 30000;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.value = Item.buyPrice(gold: 25);
			NPC.boss = true;
			NPC.npcSlots = 12f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath65;

			if (!Main.dedServ)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VoldemortBoss");
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
			if (hpPercent < 0.30f && Phase < 2)
			{
				Phase = 2;
				if (!hasEnteredPhase3)
				{
					hasEnteredPhase3 = true;
					// "The Dark Lord will rise again!"
					SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
					TeleportTimer = 0;

					// Dark burst effect
					for (int i = 0; i < 40; i++)
					{
						Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 50, default, 2f);
						dust.noGravity = true;
						dust.velocity *= 4f;
					}
				}
			}
			else if (hpPercent < 0.70f && Phase < 1)
			{
				Phase = 1;
				if (!hasTransformed)
				{
					hasTransformed = true;
					TransformToTrueForm();
				}
			}

			switch (Phase)
			{
				case 0: DoPhase1_Disguise(player); break;
				case 1: DoPhase2_TrueForm(player); break;
				case 2: DoPhase3_Desperate(player); break;
			}

			NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;
		}

		/// <summary>Phase 1 — Disguised as "Moody". Walks on ground, fires bolts, drinks flask.</summary>
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

		private void DoPhase1_Disguise(Player player)
		{
			AttackTimer++;
			SpecialTimer++;

			// Walk toward player on ground
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			float walkSpeed = 3f;
			float dirX = player.Center.X > NPC.Center.X ? 1f : -1f;
			NPC.velocity.X = dirX * walkSpeed;

			// Fire accurate single-target bolts
			if (AttackTimer >= 90 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 10f;
				WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.BartyCrouch, NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.CursedFlameHostile, NPC.damage / 3, 0f, Main.myPlayer);
				SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
			}

			// Drink from flask — heal 500 HP every 8 seconds
			if (SpecialTimer >= 480 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpecialTimer = 0;
				int healAmount = 500;
				NPC.life = Math.Min(NPC.life + healAmount, NPC.lifeMax);
				NPC.HealEffect(healAmount, true);

				// Flask dust (green potion particles)
				for (int i = 0; i < 15; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(5, 10), 10, 10, DustID.GreenTorch, 0f, -1f, 100, default, 1.2f);
					dust.noGravity = true;
					dust.velocity *= 0.5f;
				}
			}

			// Subtle dust (disguise shimmer)
			if (Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.MagicMirror, 0f, 0f, 150, default, 0.4f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
			}
		}

		/// <summary>Dramatic transformation — "I'll show you what a faithful servant can do!"</summary>
		private void TransformToTrueForm()
		{
			SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

			// Switch music to Voldemort theme
			if (!Main.dedServ)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VoldemortBoss");

			// Massive dust burst — transformation reveal
			for (int i = 0; i < 60; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 50, default, 2.5f);
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
			}
			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch, 0f, 0f, 50, default, 1.8f);
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
			}

			// Now flies
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.netUpdate = true;
		}

		/// <summary>Phase 2 — True form. Flies, rapid dark curses, more aggressive.</summary>
		private void DoPhase2_TrueForm(Player player)
		{
			AttackTimer++;
			SpecialTimer++;

			// Fly and hover near player
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			Vector2 targetPos = player.Center + new Vector2(NPC.direction * 220, -140);
			float speed = 10f;
			float inertia = 18f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Rapid dark curse projectiles
			if (AttackTimer >= 40 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 13f;
				int projType = Main.rand.NextBool() ? ProjectileID.CursedFlameHostile : ProjectileID.ShadowBeamHostile;
				WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.BartyCrouch, NPC.GetSource_FromAI(), NPC.Center, fireDir,
					projType, NPC.damage / 3, 0f, Main.myPlayer);
			}

			// Crucio-style attack — burst of homing bolts every 5 seconds
			if (SpecialTimer >= 300 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpecialTimer = 0;
				for (int i = -1; i <= 1; i++)
				{
					Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 11f;
					fireDir = fireDir.RotatedBy(MathHelper.ToRadians(i * 15));
					WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.BartyCrouch, NPC.GetSource_FromAI(), NPC.Center, fireDir,
						ProjectileID.ShadowBeamHostile, NPC.damage / 3, 0f, Main.myPlayer);
				}
				SoundEngine.PlaySound(SoundID.Item103, NPC.Center);
			}

			// Dark aura
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 100, default, 1.2f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}

		/// <summary>Phase 3 — Desperate. Teleports, summons Death Eaters, spread of killing curse bolts.</summary>
		private void DoPhase3_Desperate(Player player)
		{
			AttackTimer++;
			SpecialTimer++;
			TeleportTimer++;

			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.damage = (int)(NPC.defDamage * 1.5f);

			// Aggressive pursuit
			float speed = 14f;
			float inertia = 12f;
			Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Spread of killing curse bolts
			if (AttackTimer >= 25 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 baseDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 14f;
				for (int i = -2; i <= 2; i++)
				{
					Vector2 spreadDir = baseDir.RotatedBy(MathHelper.ToRadians(i * 10));
					WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.BartyCrouch, NPC.GetSource_FromAI(), NPC.Center, spreadDir,
						ProjectileID.CursedFlameHostile, NPC.damage / 3, 0f, Main.myPlayer);
				}
			}

			// Teleport erratically every 2 seconds
			if (TeleportTimer >= 120 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				TeleportTimer = 0;
				TeleportNearPlayer(player);
			}

			// Summon 2 Death Eater minions every 6 seconds
			if (SpecialTimer >= 360 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpecialTimer = 0;
				for (int i = 0; i < 2; i++)
				{
					Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(200, 200);
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, NPCID.Necromancer);
				}
			}

			// Intense dark aura
			for (int i = 0; i < 3; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 50, default, 2f);
				dust.noGravity = true;
				dust.velocity *= 2f;
			}
		}

		private void TeleportNearPlayer(Player player)
		{
			Vector2 newPos = player.Center + Main.rand.NextVector2CircularEdge(300, 300);
			NPC.Center = newPos;
			NPC.netUpdate = true;

			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 50, default, 1.5f);
				dust.noGravity = true;
				dust.velocity *= 3f;
			}

			SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Dark Curse on contact in phase 3
			if (Phase >= 2)
			{
				target.AddBuff(ModContent.BuffType<DarkCurseDebuff>(), 300);
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.BartyCrouch.BartyCrouchBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.BartyCrouch.BartyCrouchTrophy>(), 10));

			// Polyjuice Flask — expert accessory (also drops normally at lower rate)
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.BartyCrouch.PolyjuiceFlask>(), 3));

			// Essence of Magic
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 20, 35));

			npcLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 15));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedBartyCrouch, -1);
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
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.BartyCrouchBoss"),
			});
		}
	}
}
