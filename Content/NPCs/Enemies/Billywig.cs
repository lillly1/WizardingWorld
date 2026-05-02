using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Billywig — tiny Australian magical insect.
	/// Its sting causes giddiness and levitation. Extremely fast flyer.
	/// Dried Billywig stings are used in Fizzing Whizzbees.
	/// Surface daytime spawn, pre-hardmode.
	/// </summary>
	public class Billywig : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 14;
			NPC.height = 12;
			NPC.damage = 15;
			NPC.defense = 2;
			NPC.lifeMax = 30;
			NPC.HitSound = SoundID.NPCHit5;
			NPC.DeathSound = SoundID.NPCDeath7;
			NPC.value = Item.buyPrice(silver: 8);
			NPC.knockBackResist = 0.9f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Bat;
			AIType = NPCID.CaveBat;
		}

		public override void AI()
		{
			// Vivid blue sparkle — extremely fast
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BlueTorch, 0f, 0f, 100, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}

			NPC.spriteDirection = NPC.direction;

			// Speed up — Billywigs are incredibly fast
			NPC.velocity *= 1.01f;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Billywig sting — levitation! Player floats upward briefly
			target.velocity.Y = -12f;
			target.AddBuff(BuffID.Confused, 90); // Giddiness
			target.jump = 15; // Extra jump height

			// Blue sting particles
			for (int i = 0; i < 8; i++)
			{
				Dust dust = Dust.NewDustDirect(target.Center, 4, 4, DustID.BlueTorch, 0f, -2f, 50, default, 0.8f);
				dust.noGravity = true;
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.dayTime && spawnInfo.Player.ZoneOverworldHeight)
				return 0.06f;
			if (spawnInfo.Player.ZoneJungle)
				return 0.04f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.FallenStar, 5));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Billywig"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 4; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BlueTorch);
		}
	}
}
