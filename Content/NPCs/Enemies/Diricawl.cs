using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Diricawl — a plump, fluffy bird that can teleport when threatened.
	/// Muggles know it as the Dodo — they think it's extinct because it keeps vanishing!
	/// Peaceful critter that teleports away when a player gets close.
	/// Catchable but very difficult due to teleporting.
	/// Drops Diricawl Feather (teleportation crafting ingredient).
	/// </summary>
	public class Diricawl : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
			Main.npcCatchable[Type] = true;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 20;
			NPC.height = 18;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 25;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(silver: 30);
			NPC.knockBackResist = 1f;
			NPC.aiStyle = NPCAIStyleID.Passive;
			AIType = NPCID.Bunny;
			AnimationType = NPCID.Bunny;
			NPC.friendly = true;
		}

		public override void AI()
		{
			// Teleport away when a player gets close!
			foreach (var player in Main.ActivePlayers)
			{
				if (Vector2.Distance(player.Center, NPC.Center) < 120f)
				{
					// Poof! Vanish dust
					for (int i = 0; i < 15; i++)
					{
						Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.MagicMirror, 0f, 0f, 50, default, 1.0f);
						dust.noGravity = true;
						dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
					}

					// Teleport to random nearby location
					Vector2 newPos = NPC.Center + Main.rand.NextVector2CircularEdge(300, 200);
					NPC.Center = newPos;
					NPC.netUpdate = true;
					break;
				}
			}

			// Plump fluffy dust
			if (Main.rand.NextBool(30))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Cloud, 0f, 0f, 100, default, 0.3f);
				dust.noGravity = true;
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneForest && Main.dayTime)
				return 0.03f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Feather, 1, 2, 4));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Diricawl"),
			});
		}
	}
}
