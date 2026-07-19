using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;

namespace WizardingWorld.Content.NPCs.Bosses.Troll
{
	/// <summary>
	/// Mountain Troll — Pre-Eye of Cthulhu intro boss.
	/// Phase 1: Walks slowly and swings its club. Stomps create shockwave dust.
	/// Phase 2 (below 50% HP): Enraged — faster movement, throws rocks as hostile projectiles.
	/// </summary>
	[AutoloadBossHead]
	public class TrollBoss : ModNPC
	{
		// ai[0] = phase (0 = walking, 1 = enraged)
		// ai[1] = attack timer
		// ai[2] = stomp timer
		// ai[3] = rock throw cooldown

		public bool Enraged
		{
			get => NPC.ai[0] == 1f;
			set => NPC.ai[0] = value ? 1f : 0f;
		}

		public ref float AttackTimer => ref NPC.ai[1];
		public ref float StompTimer => ref NPC.ai[2];
		public ref float RockCooldown => ref NPC.ai[3];

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
			NPC.width = 88;
			NPC.height = 104;
			NPC.damage = 22;
			NPC.defense = 6;
			NPC.lifeMax = 1500;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 3);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;

			if (!Main.dedServ)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/TrollBoss");
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

			// Phase transition at 50% HP
			if (!Enraged && NPC.life < NPC.lifeMax * 0.5f)
			{
				Enraged = true;
				NPC.netUpdate = true;

				// Roar effect
				SoundEngine.PlaySound(WizardSoundStyles.TrollRoar, NPC.Center);

				// Burst of brown/stone dust to show rage
				for (int i = 0; i < 30; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Dirt, 0f, 0f, 100, default, 2f);
					dust.velocity *= 3f;
					dust.noGravity = true;
				}
			}

			if (Enraged)
				DoEnragedPhase(player);
			else
				DoWalkingPhase(player);

			// Face the player
			NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;
			NPC.rotation = 0f;
		}

		public override void FindFrame(int frameHeight)
		{
			int frameSpeed = Enraged ? 6 : 8;
			NPC.frameCounter++;

			if (NPC.frameCounter >= frameSpeed)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += frameHeight;

				if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
					NPC.frame.Y = 0;
			}
		}

		private void DoWalkingPhase(Player player)
		{
			AttackTimer++;
			StompTimer++;

			// Slow walk toward player
			float speed = 2.4f;
			float inertia = 30f;
			Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

			// Club swing — lunges at player periodically
			if (AttackTimer >= 90)
			{
				AttackTimer = 0;
				NPC.netUpdate = true;

				// Lunge toward player for club swing
				Vector2 lungeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
				NPC.velocity = lungeDir * 6.5f;
				NPC.damage = (int)(NPC.defDamage * 1.35f); // Club hits harder

				SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
			}
			else
			{
				NPC.damage = NPC.defDamage;
			}

			// Stomp shockwave — periodic dust effect
			if (StompTimer >= 60)
			{
				StompTimer = 0;
				CreateStompDust();
			}
		}

		private void DoEnragedPhase(Player player)
		{
			AttackTimer++;
			StompTimer++;
			RockCooldown++;

			// Faster movement
			float speed = 4.5f;
			float inertia = 20f;
			Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

			// Faster club swings
			if (AttackTimer >= 60)
			{
				AttackTimer = 0;
				NPC.netUpdate = true;

				Vector2 lungeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
				NPC.velocity = lungeDir * 8.5f;
				NPC.damage = (int)(NPC.defDamage * 1.6f);

				SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
			}
			else
			{
				NPC.damage = NPC.defDamage;
			}

			// Throw rocks at the player
			if (RockCooldown >= 80)
			{
				RockCooldown = 0;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 rockDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 8f;
					WizardingBossAttackVisuals.SpawnProjectile(WizardBossAttackStyle.Troll, NPC.GetSource_FromAI(), NPC.Center, rockDir,
						ProjectileID.BoulderStaffOfEarth, NPC.damage / 3, 3f, Main.myPlayer);
				}

				SoundEngine.PlaySound(SoundID.Item69, NPC.Center);
			}

			// More frequent stomp dust
			if (StompTimer >= 40)
			{
				StompTimer = 0;
				CreateStompDust();
			}

			// Ambient angry dust
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
			}
		}

		private void CreateStompDust()
		{
			// Shockwave of stone/dirt dust on the ground below the troll
			for (int i = 0; i < 15; i++)
			{
				Vector2 dustVel = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-1f, 0.5f));
				Dust dust = Dust.NewDustDirect(
					NPC.BottomLeft, NPC.width, 4,
					DustID.Stone, dustVel.X, dustVel.Y, 80, default, 1.5f);
				dust.noGravity = false;
			}

			SoundEngine.PlaySound(SoundID.Item14, NPC.Center); // Explosion-like stomp sound
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.Troll.TrollBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.Troll.TrollTrophy>(), 10));

			// Troll Club — 33% chance
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.TrollClub>(), 3));

			// Healing potions
			npcLoot.Add(ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 10));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedTroll, -1);
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);
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
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Troll"),
			});
		}
	}
}
