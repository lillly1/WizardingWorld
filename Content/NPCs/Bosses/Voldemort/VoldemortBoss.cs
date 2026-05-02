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

namespace WizardingWorld.Content.NPCs.Bosses.Voldemort
{
	/// <summary>
	/// Lord Voldemort — True final boss (post-Lunatic Cultist tier).
	/// Phase 1: Teleports and fires killing curses.
	/// Phase 2: Summons Death Eater minions, rapid spell barrage.
	/// Phase 3 (below 20%): Desperate — creates Horcrux shields, massive AoE attacks.
	/// </summary>
	[AutoloadBossHead]
	public class VoldemortBoss : ModNPC
	{
		public int Phase
		{
			get => (int)NPC.ai[0];
			set { NPC.ai[0] = value; NPC.netUpdate = true; }
		}

		public ref float AttackTimer => ref NPC.ai[1];
		public ref float TeleportTimer => ref NPC.ai[2];
		public ref float SpecialTimer => ref NPC.ai[3];
		public ref float ShieldTimer => ref NPC.localAI[0];

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
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 60;

			// Horcrux Hunt scaling — destroying Horcruxes weakens Voldemort. Canon Tier A.
			float power = Common.Systems.HorcruxHuntSystem.GetVoldemortPowerMultiplier();
			NPC.damage = (int)(80 * power);
			NPC.defense = (int)(40 * power);
			NPC.lifeMax = (int)(60000 * power);
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
			if (hpPercent < 0.20f && Phase < 2)
			{
				Phase = 2;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
			}
			else if (hpPercent < 0.55f && Phase < 1)
			{
				Phase = 1;
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
			}

			switch (Phase)
			{
				case 0: DoPhase1(player); break;
				case 1: DoPhase2(player); break;
				case 2: DoPhase3(player); break;
			}

			NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;

			// Dark aura
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 100, default, 1.2f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}

		private void DoPhase1(Player player)
		{
			AttackTimer++;
			TeleportTimer++;

			// Hover near player
			Vector2 targetPos = player.Center + new Vector2(NPC.direction * 250, -150);
			float speed = 6f;
			float inertia = 30f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Fire killing curse bolts
			if (AttackTimer >= 60 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 12f;
				// Green bolt (Avada Kedavra style)
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.CursedFlameHostile, NPC.damage / 3, 0f, Main.myPlayer);
			}

			// Teleport every 5 seconds
			if (TeleportTimer >= HorcruxHuntSystem.GetTeleportIntervalTicks(0) && Main.netMode != NetmodeID.MultiplayerClient)
			{
				TeleportTimer = 0;
				TeleportNearPlayer(player);
			}
		}

		private void DoPhase2(Player player)
		{
			AttackTimer++;
			TeleportTimer++;
			SpecialTimer++;

			// Faster and more aggressive
			Vector2 targetPos = player.Center + new Vector2(NPC.direction * 200, -120);
			float speed = 10f;
			float inertia = 20f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Rapid spell fire
			if (AttackTimer >= 30 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 14f;

				// Alternate between different hostile projectiles
				int projType = Main.rand.NextBool() ? ProjectileID.CursedFlameHostile : ProjectileID.ShadowBeamHostile;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					projType, NPC.damage / 3, 0f, Main.myPlayer);
			}

			// Teleport more frequently
			if (TeleportTimer >= HorcruxHuntSystem.GetTeleportIntervalTicks(1))
			{
				TeleportTimer = 0;
				if (Main.netMode != NetmodeID.MultiplayerClient)
					TeleportNearPlayer(player);
			}

			// Summon Death Eater minions
			if (SpecialTimer >= 300 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpecialTimer = 0;
				int summonCount = HorcruxHuntSystem.GetPhase2MinionCount();
				for (int i = 0; i < summonCount; i++)
				{
					Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(200, 200);
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y,
						ModContent.NPCType<NPCs.Enemies.DeathEater>());
				}
			}
		}

		private void DoPhase3(Player player)
		{
			AttackTimer++;
			TeleportTimer++;
			SpecialTimer++;

			// Desperate fury
			float speed = 14f;
			float inertia = 10f;
			Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;
			NPC.damage = (int)(NPC.defDamage * 1.5f);

			// Very rapid attacks
			if (AttackTimer >= 15 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				AttackTimer = 0;
				Vector2 fireDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 16f;
				fireDir = fireDir.RotatedByRandom(MathHelper.ToRadians(10));
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.CursedFlameHostile, NPC.damage / 3, 0f, Main.myPlayer);
			}

			// Rapid teleport
			if (TeleportTimer >= HorcruxHuntSystem.GetTeleportIntervalTicks(2))
			{
				TeleportTimer = 0;
				if (Main.netMode != NetmodeID.MultiplayerClient)
					TeleportNearPlayer(player);
			}

			// Ring of death bolts every 4 seconds
			if (SpecialTimer >= 240 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpecialTimer = 0;
				int boltCount = HorcruxHuntSystem.GetPhase3BoltCount();
				for (int i = 0; i < boltCount; i++)
				{
					float angle = MathHelper.TwoPi / boltCount * i;
					Vector2 ringDir = angle.ToRotationVector2() * 8f;
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, ringDir,
						ProjectileID.CursedFlameHostile, NPC.damage / 4, 0f, Main.myPlayer);
				}

				if (!HorcruxHuntSystem.HorcruxShieldDisabled)
					ShieldTimer = HallowsSystem.resurrectionStoneAwakened ? 60f : 120f;
			}

			if (ShieldTimer > 0)
			{
				ShieldTimer--;
				NPC.defense = NPC.defDefense + 25;
				if (Main.rand.NextBool(3))
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 50, default, 1.6f);
					dust.noGravity = true;
					dust.velocity *= 1.8f;
				}
			}
			else
			{
				NPC.defense = NPC.defDefense;
			}

			// Intense dark aura
			for (int i = 0; i < 4; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 50, default, 2f);
				dust.noGravity = true;
				dust.velocity *= 2f;
			}
		}

		private void TeleportNearPlayer(Player player)
		{
			// Teleport to a random position near the player
			Vector2 newPos = player.Center + Main.rand.NextVector2CircularEdge(300, 300);
			NPC.Center = newPos;
			NPC.netUpdate = true;

			// Teleport dust at old and new position
			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 50, default, 1.5f);
				dust.noGravity = true;
				dust.velocity *= 3f;
			}

			SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossLoot.Voldemort.VoldemortBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossLoot.Voldemort.VoldemortTrophy>(), 10));

			// Elder Wand — Voldemort canonically wielded it
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Wands.ElderWand>(), 1));

			// Dark Arts Tome — guaranteed, 5-10 stack
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.DarkArtsTome>(), 1, 5, 10));

			// Essence of Magic — 30-50
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 30, 50));

			// Super Healing Potions — 5-15
			npcLoot.Add(ItemDropRule.Common(ItemID.SuperHealingPotion, 1, 5, 15));

			// Gaunt's Ring — contains the Resurrection Stone, purifiable after Horcrux Hunt
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Accessories.GauntsRing>(), 1));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedVoldemort, -1);
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
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Voldemort"),
			});
		}
	}
}
