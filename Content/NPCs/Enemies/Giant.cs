using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Giant — massive humanoid. Extremely rare surface spawn at night during Hardmode.
	/// Very slow but devastating damage. Shakes the screen when it walks.
	/// Think Grawp or the giants Voldemort recruited for the Battle of Hogwarts.
	/// </summary>
	public class Giant : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 6;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 50;
			NPC.height = 80;
			NPC.damage = 90;
			NPC.defense = 30;
			NPC.lifeMax = 2000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = Item.buyPrice(gold: 5);
			NPC.knockBackResist = 0f; // Immovable
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.Zombie;
			AnimationType = NPCID.Zombie;
			NPC.npcSlots = 5f;
		}

		public override void AI()
		{
			// Very slow but menacing
			NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -2f, 2f);

			// Ground stomp effect when walking
			if (NPC.velocity.Y == 0 && NPC.velocity.X != 0)
			{
				// Screen shake for nearby players
				foreach (var player in Main.ActivePlayers)
				{
					if (Vector2.Distance(player.Center, NPC.Center) < 400f)
					{
						// Dust stomp
						if (Main.GameUpdateCount % 30 == 0)
						{
							for (int i = 0; i < 10; i++)
							{
								Dust dust = Dust.NewDustDirect(NPC.Bottom - new Vector2(20, 0), NPC.width + 40, 8, DustID.Stone, Main.rand.NextFloat(-3f, 3f), -2f, 100, default, 1.5f);
								dust.noGravity = false;
							}
						}
					}
				}
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Devastating slam — broken armor + stunned
			target.AddBuff(BuffID.BrokenArmor, 600);
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PetrifiedDebuff>(), 30); // Brief stun
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && !Main.dayTime && spawnInfo.Player.ZoneOverworldHeight && !NPC.AnyNPCs(Type))
				return 0.005f; // Extremely rare

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 8, 15));
			npcLoot.Add(ItemDropRule.Common(ItemID.StoneBlock, 1, 30, 80));
			npcLoot.Add(ItemDropRule.Common(ItemID.IronBar, 2, 5, 15));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Accessories.PhilosophersStone>(), 30));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Giant"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 15; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Stone);
		}
	}
}
