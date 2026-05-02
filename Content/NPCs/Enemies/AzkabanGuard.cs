using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Azkaban Guard — an elite Dementor variant. Larger, stronger, applies Dark Curse.
	/// Spawns rarely at night in Hardmode, or during Death Eater Invasions.
	/// Drains life on contact and applies the devastating Dark Curse debuff.
	/// Only the Patronus can truly counter them.
	/// </summary>
	public class AzkabanGuard : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 34;
			NPC.height = 56;
			NPC.damage = 75;
			NPC.defense = 30;
			NPC.lifeMax = 800;
			NPC.HitSound = SoundID.NPCHit36;
			NPC.DeathSound = SoundID.NPCDeath39;
			NPC.value = Item.buyPrice(gold: 1);
			NPC.knockBackResist = 0.1f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.aiStyle = -1;
			NPC.alpha = 60;
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];

			// Menacing slow pursuit — slower than normal Dementors but relentless
			float speed = 5f;
			float inertia = 25f;
			Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Cold aura — extinguish nearby light
			Lighting.AddLight(NPC.Center, -0.3f, -0.3f, -0.2f);

			// Dense dark frost particles
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 180, default, 1.3f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}

			// Ice crystal particles (Dementors bring cold)
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position + Main.rand.NextVector2Circular(NPC.width, NPC.height), 4, 4, DustID.IceTorch, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
			}

			NPC.spriteDirection = NPC.Center.X < target.Center.X ? 1 : -1;
			NPC.alpha = 50 + (int)(Math.Sin(Main.GameUpdateCount * 0.06) * 30);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// The Dementor's Kiss — Dark Curse + Darkness + life drain
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.DarkCurseDebuff>(), 300); // 5 seconds of Dark Curse
			target.AddBuff(BuffID.Darkness, 600);
			target.AddBuff(BuffID.ManaSickness, 300);
			target.GetModPlayer<Common.Players.WizardPlayer>().AddDespair(0.30f, "the Azkaban Guard's presence");

			// Life drain — heal the Dementor
			int drainAmount = Math.Min(hurtInfo.Damage / 3, 50);
			NPC.life = Math.Min(NPC.life + drainAmount, NPC.lifeMax);
			NPC.HealEffect(drainAmount);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && !Main.dayTime && spawnInfo.Player.ZoneOverworldHeight && !NPC.AnyNPCs(Type))
			{
				// Rare natural spawn, more common during invasion
				float chance = 0.01f;
				if (Common.Systems.DeathEaterInvasion.invasionActive)
					chance = 0.08f;

				// Azkaban's Despair event — frequent AzkabanGuard spawns
				if (Common.Systems.AzkabanDespairEvent.eventActive)
					chance = 0.12f;

				return chance;
			}

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.SoulofNight, 1, 3, 6));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 3, 8));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Accessories.PhilosophersStone>(), 50)); // Very rare

			// Azkaban Warden's Key — 1/20 drop chance
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Accessories.AzkabanWardensKey>(), 20));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.AzkabanGuard"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 12; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame);
				dust.noGravity = true;
			}
		}
	}
}
