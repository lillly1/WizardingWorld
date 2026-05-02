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
	/// Lethifold (Living Shroud) — one of the most dangerous creatures in the wizarding world.
	/// A dark cloak-like creature that smothers its prey. Resembles a black cloak
	/// half an inch thick gliding along the ground at night.
	/// Only a Patronus can repel it. Applies suffocation (rapid life drain).
	/// Rare nighttime surface enemy in Hardmode. The Dementor's cousin.
	/// </summary>
	public class Lethifold : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 36;
			NPC.height = 20;
			NPC.damage = 70;
			NPC.defense = 15;
			NPC.lifeMax = 600;
			NPC.HitSound = SoundID.NPCHit36;
			NPC.DeathSound = SoundID.NPCDeath39;
			NPC.value = Item.buyPrice(gold: 1);
			NPC.knockBackResist = 0.1f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.aiStyle = -1;
			NPC.alpha = 100; // Semi-transparent shadow
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];

			// Glide along the ground toward player — creeping shadow
			Vector2 groundTarget = target.Bottom + new Vector2(0, -10);
			float speed = 6f;
			float inertia = 15f;
			Vector2 dir = (groundTarget - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Dark shadow dust
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 200, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
			}

			// Reduce light around it
			Terraria.Lighting.AddLight(NPC.Center, -0.3f, -0.3f, -0.2f);

			NPC.alpha = 80 + (int)(Math.Sin(Main.GameUpdateCount * 0.08) * 30);
			NPC.rotation = NPC.velocity.X * 0.02f;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Suffocation — it smothers you
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.DarkCurseDebuff>(), 240);
			target.AddBuff(BuffID.Suffocation, 120);
			target.AddBuff(BuffID.Darkness, 300);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && !Main.dayTime && spawnInfo.Player.ZoneOverworldHeight && !NPC.AnyNPCs(Type))
				return 0.008f; // Very rare — one of the rarest enemies
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.SoulofNight, 1, 3, 6));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 3, 6));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.DementorsShroud>(), 5));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Lethifold"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame);
		}
	}
}
