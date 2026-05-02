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

namespace WizardingWorld.Content.NPCs.Bosses.Quirrell
{
	/// <summary>
	/// Professor Quirrell (with Voldemort on the back of his head) -- Pre-Hardmode boss, post-Eye of Cthulhu.
	/// Phase 1 (Quirrell): Nervous professor. Walks on ground, throws weak purple bolt projectiles. Occasionally stumbles.
	/// Phase 2 (below 60% HP -- Voldemort reveals): Flies, fires green killing curse bolts rapidly, much faster and aggressive. Defense increases to 20.
	/// Phase 3 (below 25% HP): Voldemort fully in control -- teleports, fires ring of cursed projectiles, applies Dark Curse on hit.
	/// </summary>
	[AutoloadBossHead]
	public class QuirrellBoss : ModNPC
	{
		// ai[0] = phase (0=Quirrell, 1=Voldemort revealed, 2=Voldemort full control)
		// ai[1] = attack timer
		// ai[2] = teleport timer / stumble timer
		// ai[3] = special timer (ring attack cooldown)

		public int Phase
		{
			get => (int)NPC.ai[0];
			set { NPC.ai[0] = value; NPC.netUpdate = true; }
		}

		public ref float AttackTimer => ref NPC.ai[1];
		public ref float SecondaryTimer => ref NPC.ai[2];
		public ref float SpecialTimer => ref NPC.ai[3];

		private bool stumbling;
		private int stumbleCount;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 6;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 56;
			NPC.damage = 35;
			NPC.defense = 12;
			NPC.lifeMax = 3500;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.value = Item.buyPrice(gold: 5);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;

			if (!Main.dedServ)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/BasiliskBoss");
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
			if (hpPercent < 0.25f && Phase < 2)
			{
				Phase = 2;
				NPC.defense = 20;
				NPC.noGravity = true;
				NPC.noTileCollide = true;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
				TransitionDust(DustID.CursedTorch);

				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BossDialogue.QuirrellPhase3"), Color.Green);
			}
			else if (hpPercent < 0.60f && Phase < 1)
			{
				Phase = 1;
				NPC.defense = 20;
				NPC.noGravity = true;
				NPC.noTileCollide = true;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
				TransitionDust(DustID.GreenTorch);

				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.BossDialogue.QuirrellPhase2"), Color.Green);
			}

			switch (Phase)
			{
				case 0: DoPhase1_Quirrell(player); break;
				case 1: DoPhase2_VoldemortRevealed(player); break;
				case 2: DoPhase3_VoldemortControl(player); break;
			}

			NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;

			if (Phase == 0)
				NPC.rotation = 0f;
			else
				NPC.rotation = NPC.velocity.X * 0.02f;
		}

		private void DoPhase1_Quirrell(Player player)
		{
			AttackTimer++;
			SecondaryTimer++;

			// Stumble mechanic -- occasionally stops moving
			if (!stumbling && SecondaryTimer >= 120 && Main.rand.NextBool(3))
			{
				stumbling = true;
				stumbleCount = 0;
				SecondaryTimer = 0;
			}

			if (stumbling)
			{
				stumbleCount++;
				NPC.velocity.X *= 0.9f;

				// Stumble lasts about 1 second
				if (stumbleCount >= 60)
				{
					stumbling = false;
					SecondaryTimer = 0;
				}
			}
			else
			{
				// Walk toward player on ground
				float speed = 3f;
				float inertia = 25f;
				float dirX = player.Center.X > NPC.Center.X ? 1f : -1f;
				NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + dirX * speed) / inertia;
			}

			// Apply gravity manually when not flying
			if (!NPC.noGravity)
			{
				NPC.velocity.Y += 0.3f;
				if (NPC.velocity.Y > 10f)
					NPC.velocity.Y = 10f;
			}

			// Fire weak purple bolts every 90 ticks
			if (AttackTimer >= 90 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 7f;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dir,
					ProjectileID.ShadowBeamHostile, NPC.damage / 3, 0f, Main.myPlayer);

				SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
			}

			// Nervous purple dust
			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch, 0f, 0f, 150, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}
		}

		private void DoPhase2_VoldemortRevealed(Player player)
		{
			AttackTimer++;
			SecondaryTimer++;

			// Fly toward player aggressively
			float speed = 9f;
			float inertia = 15f;
			Vector2 targetPos = player.Center + new Vector2(NPC.direction * 180, -80);
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Fire green killing curse bolts rapidly every 35 ticks
			if (AttackTimer >= 35 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 12f;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.CursedFlameHostile, NPC.damage / 2, 0f, Main.myPlayer);

				SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
			}

			// Occasional dash toward player every 3 seconds
			if (SecondaryTimer >= 180)
			{
				SecondaryTimer = 0;
				Vector2 dashDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
				NPC.velocity = dashDir * 16f;
				NPC.netUpdate = true;
			}

			// Green cursed aura
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GreenTorch, 0f, 0f, 80, default, 1.0f);
				dust.noGravity = true;
				dust.velocity *= 0.4f;
			}
		}

		private void DoPhase3_VoldemortControl(Player player)
		{
			AttackTimer++;
			SecondaryTimer++;
			SpecialTimer++;

			// Aggressive pursuit
			float speed = 12f;
			float inertia = 12f;
			Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;
			NPC.damage = (int)(NPC.defDamage * 1.3f);

			// Rapid green bolts every 25 ticks
			if (AttackTimer >= 25 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 14f;
				fireDir = fireDir.RotatedByRandom(MathHelper.ToRadians(10));
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.CursedFlameHostile, NPC.damage / 2, 0f, Main.myPlayer);
			}

			// Teleport every 2.5 seconds
			if (SecondaryTimer >= 150)
			{
				SecondaryTimer = 0;
				if (Main.netMode != NetmodeID.MultiplayerClient)
					TeleportNearPlayer(player);
			}

			// Ring of cursed projectiles every 4 seconds
			if (SpecialTimer >= 240 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpecialTimer = 0;
				int numProjectiles = 8;
				for (int i = 0; i < numProjectiles; i++)
				{
					float angle = MathHelper.TwoPi / numProjectiles * i;
					Vector2 ringDir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 8f;
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, ringDir,
						ProjectileID.CursedFlameHostile, NPC.damage / 3, 0f, Main.myPlayer);
				}

				SoundEngine.PlaySound(SoundID.Item45, NPC.Center);
			}

			// Intense green-black aura
			for (int i = 0; i < 3; i++)
			{
				int dustType = Main.rand.NextBool() ? DustID.GreenTorch : DustID.CursedTorch;
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 50, default, 1.5f);
				dust.noGravity = true;
				dust.velocity *= 2f;
			}
		}

		private void TeleportNearPlayer(Player player)
		{
			Vector2 newPos = player.Center + Main.rand.NextVector2CircularEdge(250, 250);
			NPC.Center = newPos;
			NPC.netUpdate = true;

			for (int i = 0; i < 25; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GreenTorch, 0f, 0f, 50, default, 1.5f);
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
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.Quirrell.QuirrellBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.Quirrell.QuirrellTrophy>(), 10));

			// Quirrell's Turban accessory -- 33% chance on normal
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.Quirrell.QuirrellsTurban>(), 3));

			// Healing potions
			npcLoot.Add(ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 10));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedQuirrell, -1);
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
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Quirrell"),
			});
		}
	}
}
