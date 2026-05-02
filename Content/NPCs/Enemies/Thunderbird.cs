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
	/// Thunderbird — a massive magical bird from Fantastic Beasts.
	/// Creates storms, fires lightning. Spawns in the sky/space layer.
	/// Native to Arizona but found wherever storms gather.
	/// Can sense danger and create storms as a defense mechanism.
	/// Hardmode sky enemy.
	/// </summary>
	public class Thunderbird : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 50;
			NPC.height = 30;
			NPC.damage = 65;
			NPC.defense = 22;
			NPC.lifeMax = 500;
			NPC.HitSound = SoundID.NPCHit28;
			NPC.DeathSound = SoundID.NPCDeath31;
			NPC.value = Item.buyPrice(silver: 60);
			NPC.knockBackResist = 0.2f;
			NPC.noGravity = true;
			NPC.aiStyle = -1;
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			NPC.ai[0]++;

			// Swooping flight pattern — flies above then dives
			float flightY = target.Center.Y - 200 + (float)Math.Sin(NPC.ai[0] * 0.03) * 80f;
			Vector2 targetPos = new Vector2(target.Center.X + NPC.direction * 200, flightY);
			float speed = 9f;
			float inertia = 20f;
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Lightning strike every 3 seconds — bolt from bird to ground below
			if (NPC.ai[0] % 180 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				Vector2 boltDir = new Vector2(Main.rand.NextFloat(-2f, 2f), 12f);
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, boltDir,
					ProjectileID.CultistBossLightningOrb, NPC.damage / 3, 0f, Main.myPlayer);
			}

			// Switch sides periodically
			if (NPC.ai[0] % 300 == 0)
			{
				NPC.direction *= -1;
				NPC.netUpdate = true;
			}

			// Electric/storm dust
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 0.5f;
			}

			NPC.spriteDirection = NPC.direction;
			NPC.rotation = NPC.velocity.X * 0.02f;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Electrified, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && spawnInfo.Player.ZoneSkyHeight)
				return 0.06f;
			if (Main.hardMode && spawnInfo.Player.ZoneOverworldHeight && Main.raining)
				return 0.04f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Feather, 1, 3, 8));
			npcLoot.Add(ItemDropRule.Common(ItemID.SoulofFlight, 3, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 2, 4));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Thunderbird"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 8; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Electric);
		}
	}
}
