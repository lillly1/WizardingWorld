using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;

namespace WizardingWorld.Content.NPCs.Bosses.Aragog
{
	/// <summary>
	/// Aragog — Early Hardmode boss. Giant Acromantula patriarch.
	/// Phase 1: Crawls toward player, periodically spawns 2-3 Acromantula minions.
	/// Phase 2 (below 40% HP): Web spit attack that slows, faster movement, more spider spawns.
	/// Key mechanic: Fight Aragog AND a swarm of Acromantulas.
	/// </summary>
	[AutoloadBossHead]
	public class AragogBoss : ModNPC
	{
		// ai[0] = phase (0 = crawling, 1 = frenzied)
		// ai[1] = attack timer
		// ai[2] = spawn timer
		// ai[3] = web spit cooldown

		public bool Frenzied
		{
			get => NPC.ai[0] == 1f;
			set => NPC.ai[0] = value ? 1f : 0f;
		}

		public ref float AttackTimer => ref NPC.ai[1];
		public ref float SpawnTimer => ref NPC.ai[2];
		public ref float WebCooldown => ref NPC.ai[3];

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
			NPC.width = 80;
			NPC.height = 60;
			NPC.damage = 45;
			NPC.defense = 18;
			NPC.lifeMax = 8000;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 15);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit29;
			NPC.DeathSound = SoundID.NPCDeath32;

			if (!Main.dedServ)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/AragogBoss");
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

			// Phase transition at 40% HP
			if (!Frenzied && NPC.life < NPC.lifeMax * 0.4f)
			{
				Frenzied = true;
				NPC.netUpdate = true;

				// Screech effect
				SoundEngine.PlaySound(WizardSoundStyles.SpiderHiss, NPC.Center);

				// Burst of dark dust
				for (int i = 0; i < 40; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Web, 0f, 0f, 100, default, 2f);
					dust.velocity *= 4f;
					dust.noGravity = true;
				}
			}

			if (Frenzied)
				DoFrenziedPhase(player);
			else
				DoCrawlingPhase(player);

			// Face the player
			NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;
			NPC.rotation = 0f;
		}

		private void DoCrawlingPhase(Player player)
		{
			AttackTimer++;
			SpawnTimer++;

			// Crawl toward player — spider-like movement
			float speed = 5f;
			float inertia = 25f;
			Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

			// Lunge attack periodically
			if (AttackTimer >= 100)
			{
				AttackTimer = 0;
				NPC.netUpdate = true;

				Vector2 lungeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
				NPC.velocity = lungeDir * 10f;
				NPC.damage = (int)(NPC.defDamage * 1.5f);

				SoundEngine.PlaySound(SoundID.NPCHit29, NPC.Center);
			}
			else
			{
				NPC.damage = NPC.defDamage;
			}

			// Spawn 2-3 Acromantula minions periodically
			if (SpawnTimer >= 180)
			{
				SpawnTimer = 0;
				SpawnAcromantulas(Main.rand.Next(2, 4));
			}

			// Web dust trail
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Web, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
			}
		}

		private void DoFrenziedPhase(Player player)
		{
			AttackTimer++;
			SpawnTimer++;
			WebCooldown++;

			// Faster crawling
			float speed = 8f;
			float inertia = 18f;
			Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

			// Faster lunge attacks
			if (AttackTimer >= 70)
			{
				AttackTimer = 0;
				NPC.netUpdate = true;

				Vector2 lungeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
				NPC.velocity = lungeDir * 14f;
				NPC.damage = (int)(NPC.defDamage * 2f);

				SoundEngine.PlaySound(SoundID.NPCHit29, NPC.Center);
			}
			else
			{
				NPC.damage = NPC.defDamage;
			}

			// Web spit attack — shoots sticky projectiles that slow the player
			if (WebCooldown >= 60)
			{
				WebCooldown = 0;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 webDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 10f;
					// Shoot 3 web projectiles in a spread
					for (int i = -1; i <= 1; i++)
					{
						Vector2 spreadDir = webDir.RotatedBy(MathHelper.ToRadians(10f * i));
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, spreadDir,
							ProjectileID.WebSpit, NPC.damage / 3, 2f, Main.myPlayer);
					}
				}

				SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
			}

			// Spawn more Acromantulas, more frequently
			if (SpawnTimer >= 120)
			{
				SpawnTimer = 0;
				SpawnAcromantulas(Main.rand.Next(3, 5));
			}

			// More intense web dust
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Web, 0f, 0f, 50, default, 1.5f);
				dust.noGravity = true;
				dust.velocity *= 0.5f;
			}
		}

		private void SpawnAcromantulas(int count)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;

			int acroType = ModContent.NPCType<NPCs.Enemies.Acromantula>();

			for (int i = 0; i < count; i++)
			{
				// Spawn near Aragog with some offset
				Vector2 spawnPos = NPC.Center + new Vector2(
					Main.rand.NextFloat(-100f, 100f),
					Main.rand.NextFloat(-50f, 50f)
				);

				int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, acroType);
				if (index < Main.maxNPCs)
				{
					Main.npc[index].netUpdate = true;
				}
			}

			SoundEngine.PlaySound(WizardSoundStyles.SpiderHiss, NPC.Center);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.Aragog.AragogBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.Aragog.AragogTrophy>(), 10));

			// Spider Silk Weave — crafting material
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.SpiderSilkWeave>(), 1, 5, 10));

			// Spider Fang (vanilla)
			npcLoot.Add(ItemDropRule.Common(ItemID.SpiderFang, 1, 5, 10));

			// Greater Healing potions
			npcLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 10));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedAragog, -1);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = ImmunityCooldownID.Bosses;
			return true;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.SpiderNest,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Aragog"),
			});
		}
	}
}
