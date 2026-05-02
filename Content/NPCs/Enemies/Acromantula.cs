using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Acromantula — giant magical spider. Spawns underground.</summary>
	public class Acromantula : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 30;
			NPC.damage = 35;
			NPC.defense = 14;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit29;
			NPC.DeathSound = SoundID.NPCDeath32;
			NPC.value = Item.buyPrice(silver: 30);
			NPC.knockBackResist = 0.4f;
			NPC.aiStyle = NPCAIStyleID.Spider;
			AIType = NPCID.WallCreeper;
			AnimationType = NPCID.WallCreeper;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Venomous bite
			target.AddBuff(BuffID.Venom, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneDirtLayerHeight || spawnInfo.Player.ZoneRockLayerHeight)
				return 0.06f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Cobweb, 1, 5, 15));
			npcLoot.Add(ItemDropRule.Common(ItemID.SpiderFang, 4, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Acromantula"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 8; i++)
			{
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood);
			}
		}
	}
}
