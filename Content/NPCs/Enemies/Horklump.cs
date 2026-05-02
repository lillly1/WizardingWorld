using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Horklump — a fleshy pink mushroom-like creature. Barely qualifies as a beast.
	/// Sits still. Doesn't attack. Gnomish gardeners consider it a pest.
	/// The only known use is as Flobberworm food. Drops mushrooms.
	/// Passive underground critter — the most boring Fantastic Beast.
	/// </summary>
	public class Horklump : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 1;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 16;
			NPC.height = 14;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 10;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(copper: 10);
			NPC.knockBackResist = 1f;
			NPC.aiStyle = -1; // Does nothing. Just sits there.
			NPC.friendly = true;
		}

		public override void AI()
		{
			NPC.velocity = Vector2.Zero;
			if (NPC.velocity.Y < 5f) NPC.velocity.Y += 0.1f; // Gravity only
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneDirtLayerHeight || spawnInfo.Player.ZoneRockLayerHeight)
				return 0.03f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Mushroom, 1, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Horklump"),
			});
		}
	}
}
