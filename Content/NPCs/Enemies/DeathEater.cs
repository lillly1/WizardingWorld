using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Death Eater — cloaked dark wizard. Flies and shoots curses. Hardmode night enemy.</summary>
	public class DeathEater : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 30;
			NPC.height = 46;
			NPC.damage = 60;
			NPC.defense = 25;
			NPC.lifeMax = 500;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = Item.buyPrice(silver: 75);
			NPC.knockBackResist = 0.2f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.aiStyle = -1;
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];

			// Float toward player with hover offset
			float speed = 5f;
			float inertia = 30f;
			Vector2 targetPos = target.Center + new Vector2(NPC.direction * 200, -80);
			Vector2 dir = (targetPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + dir) / inertia;

			// Shoot curses
			NPC.ai[0]++;
			if (NPC.ai[0] >= 120 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.ai[0] = 0;
				Vector2 fireDir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 10f;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireDir,
					ProjectileID.CursedFlameHostile, NPC.damage / 3, 0f, Main.myPlayer);
				NPC.netUpdate = true;
			}

			// Dark aura
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
			}

			NPC.spriteDirection = NPC.Center.X < target.Center.X ? 1 : -1;
			NPC.rotation = NPC.velocity.X * 0.02f;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.DarkCurseDebuff>(), 120);
			target.AddBuff(BuffID.Darkness, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && !Main.dayTime && spawnInfo.Player.ZoneOverworldHeight)
				return 0.05f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.SoulofNight, 2, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.DarkShard, 6));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.VoldemortSummonItem>(), 20));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.DeathEater"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame);
		}
	}
}
