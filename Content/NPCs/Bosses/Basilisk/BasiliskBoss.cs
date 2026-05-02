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

namespace WizardingWorld.Content.NPCs.Bosses.Basilisk
{
	/// <summary>
	/// Basilisk — Pre-Hardmode boss. Giant serpent from the Chamber of Secrets.
	/// Phase 1: Burrows underground and lunges at player.
	/// Phase 2: Exposed, fast charges + petrification gaze attack.
	/// </summary>
	[AutoloadBossHead]
	public class BasiliskBoss : ModNPC
	{
		// ai[0] = phase (0=burrowing, 1=exposed)
		// ai[1] = attack timer
		// ai[2] = charge direction X
		// ai[3] = charge direction Y

		public bool SecondPhase
		{
			get => NPC.ai[0] == 1f;
			set => NPC.ai[0] = value ? 1f : 0f;
		}

		public ref float AttackTimer => ref NPC.ai[1];

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
			NPC.width = 60;
			NPC.height = 60;
			NPC.damage = 40;
			NPC.defense = 16;
			NPC.lifeMax = 4500;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 10);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath10;

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

			// Phase transition at 50% HP
			if (!SecondPhase && NPC.life < NPC.lifeMax * 0.5f)
			{
				SecondPhase = true;
				NPC.netUpdate = true;

				// Roar effect
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

				// Burst of dust
				for (int i = 0; i < 40; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GreenTorch, 0f, 0f, 100, default, 2f);
					dust.velocity *= 4f;
					dust.noGravity = true;
				}
			}

			if (SecondPhase)
				DoSecondPhase(player);
			else
				DoFirstPhase(player);

			NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
		}

		private void DoFirstPhase(Player player)
		{
			// Burrowing serpent — circle around player and lunge periodically
			AttackTimer++;

			float orbitSpeed = 0.03f;
			float orbitRadius = 250f;

			if (AttackTimer < 120)
			{
				// Orbit around player
				float angle = AttackTimer * orbitSpeed;
				Vector2 targetPos = player.Center + new Vector2(
					(float)Math.Cos(angle) * orbitRadius,
					(float)Math.Sin(angle) * orbitRadius
				);

				float speed = 8f;
				float inertia = 20f;
				Vector2 direction = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
				NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

				NPC.damage = 0;
			}
			else if (AttackTimer == 120)
			{
				// Lunge at player
				Vector2 lungeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
				NPC.velocity = lungeDir * 16f;
				NPC.damage = NPC.defDamage;
				NPC.netUpdate = true;

				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
			}
			else if (AttackTimer > 150)
			{
				AttackTimer = 0;
				NPC.netUpdate = true;
			}

			// Green dust trail
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GreenTorch, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
			}
		}

		private void DoSecondPhase(Player player)
		{
			// Faster, more aggressive — direct charges with increasing speed
			AttackTimer++;

			float speedMultiplier = 1f + (1f - (float)NPC.life / NPC.lifeMax) * 0.5f;

			if (AttackTimer < 60)
			{
				// Chase player aggressively
				float speed = 12f * speedMultiplier;
				float inertia = 15f;
				Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
				NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
				NPC.damage = NPC.defDamage;
			}
			else if (AttackTimer == 60)
			{
				// Petrification gaze — shoot projectile at player
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 10f;
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dir,
						ProjectileID.CursedFlameHostile, NPC.damage / 2, 0f, Main.myPlayer);
				}
			}
			else if (AttackTimer > 90)
			{
				AttackTimer = 0;
				NPC.netUpdate = true;
			}

			// More intense green dust
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GreenTorch, 0f, 0f, 50, default, 1.5f);
				dust.noGravity = true;
				dust.velocity *= 0.5f;
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.Basilisk.BasiliskBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.Basilisk.BasiliskTrophy>(), 10));

			// Basilisk Fang (crafting material)
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Accessories.BasiliskFang>(), 1, 3, 5));

			// Sword of Gryffindor
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.SwordOfGryffindor>(), 3));

			// Vanilla drops
			npcLoot.Add(ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 10));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedBasilisk, -1);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = ImmunityCooldownID.Bosses;
			return true;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Basilisk"),
			});
		}
	}
}
