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

namespace WizardingWorld.Content.NPCs.Bosses.Fenrir
{
	/// <summary>
	/// Fenrir Greyback — Blood Moon boss. Savage werewolf melee fighter.
	/// Phase 1: Fast ground charges and claw swipes, inflicts Bleeding.
	/// Phase 2 (below 50%): Full werewolf rage — faster, howl spawns Werewolf minions, inflicts Bleeding + BrokenArmor.
	/// Phase 3 (below 20%): Berserk — nearly constant charging, DarkCurse on hit, more werewolf spawns.
	/// </summary>
	[AutoloadBossHead]
	public class FenrirBoss : ModNPC
	{
		// ai[0] = phase (0, 1, 2)
		// ai[1] = attack timer
		// ai[2] = charge state sub-timer
		// ai[3] = howl/special timer

		public int Phase
		{
			get => (int)NPC.ai[0];
			set { NPC.ai[0] = value; NPC.netUpdate = true; }
		}

		public ref float AttackTimer => ref NPC.ai[1];
		public ref float ChargeTimer => ref NPC.ai[2];
		public ref float SpecialTimer => ref NPC.ai[3];

		private bool isCharging;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 8;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 58;
			NPC.height = 76;
			NPC.damage = 75;
			NPC.defense = 22;
			NPC.lifeMax = 25000;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.value = Item.buyPrice(gold: 20);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit6;
			NPC.DeathSound = SoundID.NPCDeath23;

			if (!Main.dedServ)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/FenrirBoss");
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

			// Phase transitions
			float hpPercent = (float)NPC.life / NPC.lifeMax;
			if (hpPercent < 0.20f && Phase < 2)
			{
				Phase = 2;
				SoundEngine.PlaySound(WizardSoundStyles.WerewolfHowl, NPC.Center);

				// Berserk dust burst
				for (int i = 0; i < 50; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2.5f);
					dust.velocity *= 5f;
					dust.noGravity = true;
				}
			}
			else if (hpPercent < 0.50f && Phase < 1)
			{
				Phase = 1;
				SoundEngine.PlaySound(WizardSoundStyles.WerewolfHowl, NPC.Center);

				for (int i = 0; i < 30; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
					dust.velocity *= 4f;
					dust.noGravity = true;
				}
			}

			switch (Phase)
			{
				case 0: DoPhase1(player); break;
				case 1: DoPhase2(player); break;
				case 2: DoPhase3(player); break;
			}

			NPC.spriteDirection = NPC.direction;
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
			AttackTimer++;
			ChargeTimer++;

			if (!isCharging)
			{
				// Run toward player on the ground
				float speed = 8f;
				NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;
				NPC.velocity.X = (NPC.velocity.X * 10f + NPC.direction * speed) / 11f;

				// Jump if blocked or player is above
				if (NPC.collideY && player.Center.Y < NPC.Center.Y - 100)
				{
					NPC.velocity.Y = -10f;
				}
				else if (NPC.collideX)
				{
					NPC.velocity.Y = -8f;
				}

				// Start a charge lunge periodically
				if (ChargeTimer >= 150)
				{
					ChargeTimer = 0;
					isCharging = true;
					Vector2 lungeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
					NPC.velocity = lungeDir * 18f;
					NPC.damage = (int)(NPC.defDamage * 1.3f);
					NPC.netUpdate = true;
					SoundEngine.PlaySound(SoundID.NPCHit6, NPC.Center);
				}
			}
			else
			{
				// Charging — maintain for a short burst
				AttackTimer++;
				if (AttackTimer >= 30)
				{
					isCharging = false;
					AttackTimer = 0;
					NPC.damage = NPC.defDamage;
					NPC.netUpdate = true;
				}
			}

			// Blood dust trail
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
			}
		}

		private void DoPhase2(Player player)
		{
			AttackTimer++;
			ChargeTimer++;
			SpecialTimer++;

			float speed = 12f;
			NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;

			if (!isCharging)
			{
				NPC.velocity.X = (NPC.velocity.X * 8f + NPC.direction * speed) / 9f;

				// More aggressive jumping
				if (NPC.collideY && player.Center.Y < NPC.Center.Y - 60)
				{
					NPC.velocity.Y = -12f;
				}
				else if (NPC.collideX)
				{
					NPC.velocity.Y = -10f;
				}

				// Faster, more frequent charges
				if (ChargeTimer >= 100)
				{
					ChargeTimer = 0;
					isCharging = true;
					Vector2 lungeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
					NPC.velocity = lungeDir * 22f;
					NPC.damage = (int)(NPC.defDamage * 1.5f);
					NPC.netUpdate = true;
					SoundEngine.PlaySound(SoundID.NPCHit6, NPC.Center);
				}
			}
			else
			{
				AttackTimer++;
				if (AttackTimer >= 25)
				{
					isCharging = false;
					AttackTimer = 0;
					NPC.damage = NPC.defDamage;
					NPC.netUpdate = true;
				}
			}

			// Howl attack — spawn werewolf minions every 6 seconds
			if (SpecialTimer >= 360 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpecialTimer = 0;
				SoundEngine.PlaySound(WizardSoundStyles.WerewolfHowl, NPC.Center);

				int wolfCount = Main.rand.Next(3, 5); // 3-4 werewolves
				for (int i = 0; i < wolfCount; i++)
				{
					Vector2 spawnPos = NPC.Center + new Vector2(Main.rand.Next(-200, 200), -50);
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, NPCID.Werewolf);
				}

				// Howl dust burst
				for (int i = 0; i < 20; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, -2f, 150, default, 1.5f);
					dust.noGravity = true;
				}
			}

			// Blood dust
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 80, default, 1.3f);
				dust.noGravity = true;
			}
		}

		private void DoPhase3(Player player)
		{
			AttackTimer++;
			ChargeTimer++;
			SpecialTimer++;

			// Berserk — nearly constant charging
			float speed = 16f;
			NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;
			NPC.damage = (int)(NPC.defDamage * 1.8f);

			if (!isCharging)
			{
				NPC.velocity.X = (NPC.velocity.X * 6f + NPC.direction * speed) / 7f;

				if (NPC.collideY && player.Center.Y < NPC.Center.Y - 40)
				{
					NPC.velocity.Y = -14f;
				}
				else if (NPC.collideX)
				{
					NPC.velocity.Y = -12f;
				}

				// Very frequent charges
				if (ChargeTimer >= 60)
				{
					ChargeTimer = 0;
					isCharging = true;
					Vector2 lungeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
					NPC.velocity = lungeDir * 26f;
					NPC.netUpdate = true;
					SoundEngine.PlaySound(SoundID.NPCHit6, NPC.Center);
				}
			}
			else
			{
				AttackTimer++;
				if (AttackTimer >= 20)
				{
					isCharging = false;
					AttackTimer = 0;
					NPC.netUpdate = true;
				}
			}

			// More frequent werewolf spawns — every 4 seconds
			if (SpecialTimer >= 240 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpecialTimer = 0;
				SoundEngine.PlaySound(WizardSoundStyles.WerewolfHowl, NPC.Center);

				int wolfCount = Main.rand.Next(3, 5);
				for (int i = 0; i < wolfCount; i++)
				{
					Vector2 spawnPos = NPC.Center + new Vector2(Main.rand.Next(-250, 250), -50);
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, NPCID.Werewolf);
				}
			}

			// Intense blood aura
			for (int i = 0; i < 3; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 50, default, 2f);
				dust.noGravity = true;
				dust.velocity *= 1.5f;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			switch (Phase)
			{
				case 0:
					// Phase 1: Bleeding
					target.AddBuff(BuffID.Bleeding, 300);
					break;
				case 1:
					// Phase 2: Bleeding + Broken Armor
					target.AddBuff(BuffID.Bleeding, 420);
					target.AddBuff(BuffID.BrokenArmor, 300);
					break;
				case 2:
					// Phase 3: Dark Curse
					target.AddBuff(ModContent.BuffType<DarkCurseDebuff>(), 300);
					break;
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.Fenrir.FenrirBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.Fenrir.FenrirTrophy>(), 10));

			// Werewolf Pelt (crafting material)
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.WerewolfPelt>(), 1, 5, 10));

			// Essence of Magic
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 20, 35));

			// Healing potions
			npcLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 15));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedFenrir, -1);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = ImmunityCooldownID.Bosses;
			return true;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Fenrir"),
			});
		}
	}
}
