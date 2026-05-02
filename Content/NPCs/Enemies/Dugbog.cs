using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Dugbog — a marsh-dwelling creature that resembles a dead piece of wood
	/// while stationary, but ambushes with a toothed maw when approached.
	/// Lies completely still until a player gets close, then lunges.
	/// Swamp/jungle ground enemy. Drops wood + Essence.
	/// </summary>
	public class Dugbog : ModNPC
	{
		private bool ambushing;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 30;
			NPC.height = 16;
			NPC.damage = 35;
			NPC.defense = 10;
			NPC.lifeMax = 150;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(silver: 15);
			NPC.knockBackResist = 0.3f;
			NPC.aiStyle = -1;
		}

		public override void AI()
		{
			if (!ambushing)
			{
				// Disguised as a log — completely still
				NPC.velocity = Vector2.Zero;
				if (NPC.velocity.Y < 5f)
					NPC.velocity.Y += 0.2f; // Gravity

				// Check for nearby players
				foreach (var player in Main.ActivePlayers)
				{
					if (Vector2.Distance(player.Center, NPC.Center) < 100f)
					{
						ambushing = true;
						NPC.netUpdate = true;

						// Surprise! Burst of wood dust
						for (int i = 0; i < 10; i++)
						{
							Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BorealWood, 0f, 0f, 100, default, 1.0f);
							dust.velocity = Main.rand.NextVector2Circular(3f, 2f);
							dust.noGravity = true;
						}
						break;
					}
				}
			}
			else
			{
				// Ambush mode — chase aggressively
				NPC.TargetClosest();
				Player target = Main.player[NPC.target];
				float speed = 6f;
				Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * speed;
				NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, dir.X, 0.1f);
				if (NPC.velocity.Y < 5f)
					NPC.velocity.Y += 0.2f;

				NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Bleeding, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneJungle || spawnInfo.Player.ZoneForest)
				return 0.03f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Wood, 1, 5, 15));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 3, 1, 2));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.DugbogHide>(), 2, 1, 2));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Dugbog"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 6; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BorealWood);
		}
	}
}
