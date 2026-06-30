using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Dementor — Azkaban's dark guardians, appearing during times of great darkness.
	/// Canon-faithful: appear during Blood Moons, dark invasions, and in the Forbidden Forest.
	/// </summary>
	public class Dementor : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
			NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 1f };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.width = 30;
			NPC.height = 50;
			NPC.damage = 55;
			NPC.defense = 20;
			NPC.lifeMax = 400;
			NPC.HitSound = SoundID.NPCHit36;
			NPC.DeathSound = WizardSoundStyles.DementorScream;
			NPC.value = Item.buyPrice(silver: 50);
			NPC.knockBackResist = 0.3f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.aiStyle = NPCAIStyleID.Bat;
			AIType = NPCID.GiantBat;
		}

		public override void AI()
		{
			// Dark dust trail
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 150, default, 1.2f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}

			// Face movement direction
			NPC.spriteDirection = NPC.direction;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Life drain effect — darkness + mana sickness
			target.AddBuff(BuffID.Darkness, 300);
			target.AddBuff(BuffID.ManaSickness, 180);
			target.AddBuff(BuffID.Slow, 120);
			target.GetModPlayer<Common.Players.WizardPlayer>().AddDespair(0.18f, "A Dementor's chill");
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (!Main.hardMode)
				return 0f;

			// Blood Moon — times of great darkness
			if (Main.bloodMoon)
				return 0.08f;

			// Death Eater Invasion — dark forces unleashed
			if (Common.Systems.DeathEaterInvasion.invasionActive)
				return 0.08f;

			// Azkaban's Despair event — Dementors escape en masse
			if (Common.Systems.AzkabanDespairEvent.eventActive)
				return 0.15f;

			// Forbidden Forest at night — canon-faithful location
			if (!Main.dayTime && spawnInfo.Player.InModBiome<Biomes.ForbiddenForestBiome>())
				return 0.08f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.SoulofNight, 3, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.DarkShard, 10));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Dementor"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame);
			}
		}
	}
}
