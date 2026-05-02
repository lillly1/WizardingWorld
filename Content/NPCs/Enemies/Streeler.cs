using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Streeler — a giant magical snail that changes colour every hour.
	/// Leaves a trail of venomous slime behind it that poisons players.
	/// Slow but dangerous if you step in the trail.
	/// Underground enemy. Drops Streeler Venom (potion ingredient).
	/// </summary>
	public class Streeler : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 2;
		}

		public override void SetDefaults()
		{
			NPC.width = 24;
			NPC.height = 18;
			NPC.damage = 25;
			NPC.defense = 16; // Hard shell
			NPC.lifeMax = 100;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(silver: 15);
			NPC.knockBackResist = 0.2f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.BlueSlime;
		}

		public override void AI()
		{
			// Very slow
			NPC.velocity.X *= 0.8f;

			// Venomous slime trail
			if (Main.rand.NextBool(3))
			{
				// Colour changes — pick random bright colour
				int[] colors = { DustID.GreenTorch, DustID.PurpleTorch, DustID.OrangeTorch, DustID.PinkTorch };
				int dustType = colors[(int)(Main.GameUpdateCount / 300) % colors.Length];
				Dust dust = Dust.NewDustDirect(NPC.Bottom - new Vector2(4, 0), NPC.width, 4, dustType, 0f, 0f, 150, default, 0.5f);
				dust.noGravity = false;
				dust.velocity *= 0.1f;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Venom, 240);
			target.AddBuff(BuffID.Slow, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneDirtLayerHeight || spawnInfo.Player.ZoneRockLayerHeight)
				return 0.04f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 3, 8));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Streeler"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 4; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch);
		}
	}
}
