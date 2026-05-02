using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Knarl — looks exactly like a hedgehog, but extremely aggressive.
	/// Difference from a hedgehog: if you offer a Knarl food, it assumes
	/// you're trying to poison it and savages your garden.
	/// Small ground enemy that rages. Higher damage than you'd expect from its size.
	/// Forest/underground pre-hardmode.
	/// </summary>
	public class Knarl : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 18;
			NPC.height = 12;
			NPC.damage = 22; // Surprisingly vicious for its size
			NPC.defense = 8; // Spiny
			NPC.lifeMax = 45;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(silver: 5);
			NPC.knockBackResist = 0.6f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.Bunny; // Passive until provoked...
			AnimationType = NPCID.Bunny;
		}

		public override void AI()
		{
			// Rages when hit — switches to aggressive AI
			if (NPC.life < NPC.lifeMax)
			{
				NPC.aiStyle = NPCAIStyleID.Fighter;
				AIType = NPCID.Zombie;
				NPC.damage = NPC.defDamage * 2; // Double damage when angry!
				NPC.friendly = false;

				// Angry spiny particles
				if (Main.rand.NextBool(6))
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BorealWood, 0f, 0f, 100, default, 0.4f);
					dust.noGravity = true;
				}
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Spiny — thorns damage
			target.AddBuff(BuffID.Bleeding, 120);
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
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Knarl"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 4; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BorealWood);
		}
	}
}
