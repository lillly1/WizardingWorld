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

namespace WizardingWorld.Content.NPCs.Bosses.Bellatrix
{
	/// <summary>
	/// Bellatrix Lestrange — Post-Plantera wizard duel boss.
	/// Phase 1: Hovers, fires Crucio/Stupefy/dark curse bolts, teleports every 5 seconds.
	/// Phase 2 (below 60%): Adds Fiendfyre, faster teleporting, screen text taunts.
	/// Phase 3 (below 25%): Rapid-fire spells, summons 2 Death Eater minions, Dark Curse on contact.
	/// </summary>
	[AutoloadBossHead]
	public class BellatrixBoss : ModNPC
	{
		// ai[0] = phase (0, 1, 2)
		// ai[1] = attack timer
		// ai[2] = teleport timer
		// ai[3] = special timer (Fiendfyre / minion spawn cooldown)

		public int Phase
		{
			get => (int)NPC.ai[0];
			set { NPC.ai[0] = value; NPC.netUpdate = true; }
		}

		public ref float AttackTimer => ref NPC.ai[1];
		public ref float TeleportTimer => ref NPC.ai[2];
		public ref float SpecialTimer => ref NPC.ai[3];

		private bool spawnedMinions;
		private int spellCycle; // Tracks which spell to fire next (0=Crucio, 1=Stupefy, 2=Dark curse)

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
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 60;
			NPC.damage = 70;
			NPC.defense = 30;
			NPC.lifeMax = 35000;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 25);
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
			if (hpPercent < 0.25f && Phase < 2)
			{
				Phase = 2;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
				TransitionDust(DustID.Shadowflame);

				// Taunt on phase 3 entry
				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BossDialogue.BellatrixPhase3"), Color.MediumPurple);
			}
			else if (hpPercent < 0.60f && Phase < 1)
			{
				Phase = 1;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
				TransitionDust(DustID.PurpleTorch);

				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BossDialogue.BellatrixPhase2"), Color.MediumPurple);
			}

			switch (Phase)
			{
				case 0: DoPhase1(player); break;
				case 1: DoPhase2(player); break;
				case 2: DoPhase3(player); break;
			}

			NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;
			NPC.rotation = NPC.velocity.X * 0.02f;

			// Dark aura
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}

		private void DoPhase1(Player player)
		{
			AttackTimer++;
			TeleportTimer++;

			// Hover near player — maintain offset position
			Vector2 targetPos = player.Center + new Vector2(NPC.direction * 250, -150);
			float speed = 7f;
			float inertia = 25f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Fire alternating spell projectiles every 70 ticks
			if (AttackTimer >= 70 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				FireSpell(player);
			}

			// Teleport every 5 seconds (300 ticks)
			if (TeleportTimer >= 300)
			{
				TeleportTimer = 0;
				if (Main.netMode != NetmodeID.MultiplayerClient)
					TeleportNearPlayer(player);
			}
		}

		private void DoPhase2(Player player)
		{
			AttackTimer++;
			TeleportTimer++;
			SpecialTimer++;

			// Faster hover
			Vector2 targetPos = player.Center + new Vector2(NPC.direction * 200, -120);
			float speed = 10f;
			float inertia = 18f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Faster spell fire every 45 ticks
			if (AttackTimer >= 45 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				FireSpell(player);
			}

			// Faster teleport every 3 seconds
			if (TeleportTimer >= 180)
			{
				TeleportTimer = 0;
				if (Main.netMode != NetmodeID.MultiplayerClient)
					TeleportNearPlayer(player);
			}

			// Fiendfyre attack every 4 seconds — large fire projectiles
			if (SpecialTimer >= 240 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpecialTimer = 0;
				Vector2 baseDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 10f;

				for (int i = -1; i <= 1; i++)
				{
					Vector2 fireDir = baseDir.RotatedBy(MathHelper.ToRadians(i * 20));
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
						ProjectileID.InfernoHostileBolt, NPC.damage / 2, 0f, Main.myPlayer);
				}

				SoundEngine.PlaySound(SoundID.Item45, NPC.Center);
			}

			// Occasional cackle taunts
			if (Main.rand.NextBool(600) && Main.netMode != NetmodeID.Server)
			{
				string[] taunts = new[]
				{
					Language.GetTextValue("Mods.WizardingWorld.BossDialogue.BellatrixTaunt1"),
					Language.GetTextValue("Mods.WizardingWorld.BossDialogue.BellatrixTaunt2"),
					Language.GetTextValue("Mods.WizardingWorld.BossDialogue.BellatrixTaunt3"),
					Language.GetTextValue("Mods.WizardingWorld.BossDialogue.BellatrixTaunt4"),
				};
				Main.NewText(taunts[Main.rand.Next(taunts.Length)], Color.MediumPurple);
			}
		}

		private void DoPhase3(Player player)
		{
			AttackTimer++;
			TeleportTimer++;
			SpecialTimer++;

			// Very aggressive pursuit
			float speed = 14f;
			float inertia = 10f;
			Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;
			NPC.damage = (int)(NPC.defDamage * 1.3f);

			// Rapid-fire spells every 20 ticks
			if (AttackTimer >= 20 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				FireSpell(player);

				// Also fire a Fiendfyre bolt on every other spell cast
				if (spellCycle % 2 == 0)
				{
					Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 12f;
					fireDir = fireDir.RotatedByRandom(MathHelper.ToRadians(15));
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
						ProjectileID.InfernoHostileBolt, NPC.damage / 2, 0f, Main.myPlayer);
				}
			}

			// Rapid teleport every 2 seconds
			if (TeleportTimer >= 120)
			{
				TeleportTimer = 0;
				if (Main.netMode != NetmodeID.MultiplayerClient)
					TeleportNearPlayer(player);
			}

			// Summon 2 Death Eater minions once
			if (!spawnedMinions && Main.netMode != NetmodeID.MultiplayerClient)
			{
				spawnedMinions = true;
				for (int i = 0; i < 2; i++)
				{
					Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(200, 200);
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y,
						ModContent.NPCType<NPCs.Enemies.DeathEater>());
				}

				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BossDialogue.BellatrixSummon"), Color.MediumPurple);
			}

			// Intense dark aura
			for (int i = 0; i < 3; i++)
			{
				int dustType = Main.rand.NextBool() ? DustID.PurpleTorch : DustID.Shadowflame;
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 50, default, 1.5f);
				dust.noGravity = true;
				dust.velocity *= 2f;
			}
		}

		private void FireSpell(Player player)
		{
			Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 12f;

			int projType;
			switch (spellCycle % 3)
			{
				case 0:
					// Crucio — red bolt, DoT (use cursed flame hostile as red-ish bolt)
					projType = ProjectileID.CursedFlameHostile;
					break;
				case 1:
					// Stupefy — stun bolt (use shadow beam for visual variety)
					projType = ProjectileID.ShadowBeamHostile;
					break;
				default:
					// Dark curse bolt
					projType = ProjectileID.DeathLaser;
					break;
			}

			Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
				projType, NPC.damage / 3, 0f, Main.myPlayer);

			spellCycle++;
		}

		private void TeleportNearPlayer(Player player)
		{
			Vector2 newPos = player.Center + Main.rand.NextVector2CircularEdge(300, 300);
			NPC.Center = newPos;
			NPC.netUpdate = true;

			// Teleport dust
			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch, 0f, 0f, 50, default, 1.5f);
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
			// Phase 3: Dark Curse debuff on contact
			if (Phase >= 2)
			{
				target.AddBuff(ModContent.BuffType<DarkCurseDebuff>(), 180); // 3 seconds
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.Bellatrix.BellatrixBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.Bellatrix.BellatrixTrophy>(), 10));

			// Dark Arts Tome (crafting material)
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.DarkArtsTome>(), 1, 3, 7));

			// Greater healing potions
			npcLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 15));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedBellatrix, -1);
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
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Bellatrix"),
			});
		}
	}
}
