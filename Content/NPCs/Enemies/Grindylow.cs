using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Grindylow — aggressive underwater creature that drags players down.</summary>
	public class Grindylow : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 24;
			NPC.height = 24;
			NPC.damage = 25;
			NPC.defense = 8;
			NPC.lifeMax = 120;
			NPC.HitSound = SoundID.NPCHit3;
			NPC.DeathSound = SoundID.NPCDeath3;
			NPC.value = Item.buyPrice(silver: 15);
			NPC.knockBackResist = 0.5f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.Piranha;
			AnimationType = NPCID.Piranha;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Drag player down — slow + wet
			target.AddBuff(BuffID.Slow, 180);
			target.AddBuff(BuffID.Wet, 300);
			target.velocity.Y += 3f; // Drag downward
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Water && (spawnInfo.Player.ZoneDirtLayerHeight || spawnInfo.Player.ZoneBeach))
				return 0.08f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Coral, 3, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.Seashell, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.GrindylowTooth>(), 2, 1, 2));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Grindylow"),
			});
		}

		public override void OnKill()
		{
			if (Common.Systems.TriwizardTournamentSystem.taskActive &&
				Common.Systems.TriwizardTournamentSystem.currentTask == 2)
			{
				Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(),
					ModContent.ItemType<Items.Consumables.LakeRescueToken>());
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 6; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Water);
		}
	}
}
