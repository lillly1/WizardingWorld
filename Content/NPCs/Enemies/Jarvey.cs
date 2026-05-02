using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Jarvey — a magical ferret-like creature that hurls insults.
	/// Resembles an overgrown ferret. Can speak (rudely).
	/// Occasionally shouts rude things in chat when near players.
	/// Underground forest/surface enemy. Mostly annoying, slightly dangerous.
	/// </summary>
	public class Jarvey : ModNPC
	{
		private static readonly string[] insults = {
			"Oi! Yer mother was a Flobberworm!",
			"Call yourself a wizard? I've seen better magic from a Muggle!",
			"Nice hat. Did a troll sit on it?",
			"You smell worse than a Stinksap!",
			"I've met smarter Blast-Ended Skrewts!",
			"Go boil yer head, ya great lump!",
		};

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 22;
			NPC.height = 14;
			NPC.damage = 18;
			NPC.defense = 4;
			NPC.lifeMax = 60;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(silver: 8);
			NPC.knockBackResist = 0.7f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.Bunny;
			AnimationType = NPCID.Bunny;
		}

		public override void AI()
		{
			// Shout insults at nearby players
			NPC.ai[1]++;
			if (NPC.ai[1] >= 600 && Main.netMode != NetmodeID.Server) // Every 10 seconds
			{
				NPC.ai[1] = 0;
				foreach (var player in Main.ActivePlayers)
				{
					if (Vector2.Distance(player.Center, NPC.Center) < 300f)
					{
						string insult = insults[Main.rand.Next(insults.Length)];
						Main.NewText($"Jarvey: \"{insult}\"", 200, 150, 50);
						break;
					}
				}
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneForest || spawnInfo.Player.ZoneDirtLayerHeight)
				return 0.04f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 4));
			npcLoot.Add(ItemDropRule.Common(ItemID.Leather, 3, 1, 2));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Jarvey"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 4; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BorealWood);
		}
	}
}
