using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Magical Imp — a small mischievous fire creature from wizarding folklore.
	/// Flies around throwing tiny fireballs. Not very dangerous individually
	/// but spawns in groups. Underworld/cavern enemy.
	/// Drops Imp Flames (crafting material for fire potions).
	/// </summary>
	public class MagicalImp : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 16;
			NPC.height = 20;
			NPC.damage = 25;
			NPC.defense = 6;
			NPC.lifeMax = 70;
			NPC.HitSound = SoundID.NPCHit5;
			NPC.DeathSound = SoundID.NPCDeath7;
			NPC.value = Item.buyPrice(silver: 10);
			NPC.knockBackResist = 0.7f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Bat;
			AIType = NPCID.CaveBat;
			NPC.lavaImmune = true;
		}

		public override void AI()
		{
			// Fire trail
			if (Main.rand.NextBool(5))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}

			// Throw tiny fireball every 3 seconds
			NPC.ai[1]++;
			if (NPC.ai[1] >= 180 && NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.ai[1] = 0;
				Player target = Main.player[NPC.target];
				Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 6f;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dir,
					ProjectileID.BallofFire, NPC.damage / 3, 0f, Main.myPlayer);
			}

			NPC.spriteDirection = NPC.direction;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.OnFire, 120);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneUnderworldHeight)
				return 0.08f;
			if (spawnInfo.Player.ZoneRockLayerHeight && !Main.dayTime)
				return 0.03f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.Torch, 1, 3, 8));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.ImpFlame>(), 2, 1, 2));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.MagicalImp"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 4; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch);
		}
	}
}
