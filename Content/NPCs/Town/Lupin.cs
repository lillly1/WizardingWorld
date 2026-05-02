using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace WizardingWorld.Content.NPCs.Town
{
	/// <summary>Remus Lupin — Defense Against the Dark Arts expert and passage network guide.</summary>
	[AutoloadHead]
	public class Lupin : ModNPC
	{
		public const string ShopName = "Shop";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 25;
			NPCID.Sets.ExtraFramesCount[Type] = 9;
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700;
			NPCID.Sets.AttackType[Type] = 0;
			NPCID.Sets.AttackTime[Type] = 90;
			NPCID.Sets.AttackAverageChance[Type] = 30;
			NPCID.Sets.HatOffsetY[Type] = 4;

			NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 1f, Direction = -1 };
			NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;

			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
				.SetBiomeAffection<HallowBiome>(AffectionLevel.Like)
				.SetBiomeAffection<CorruptionBiome>(AffectionLevel.Hate)
				.SetNPCAffection(NPCID.Guide, AffectionLevel.Like)
				.SetNPCAffection(NPCID.Clothier, AffectionLevel.Like);
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = NPCAIStyleID.Passive;
			NPC.damage = 18;
			NPC.defense = 18;
			NPC.lifeMax = 280;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			AnimationType = NPCID.Guide;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs) =>
			Common.Systems.DownedBossSystem.downedFenrir;

		public override List<string> SetNPCNameList() =>
			new() { "Remus Lupin" };

		public override string GetChat()
		{
			WeightedRandom<string> chat = new();
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Lupin.Standard1"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Lupin.Standard2"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Lupin.Standard3"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Lupin.Standard4"));

			if (Main.moonPhase == 0 && !Main.dayTime)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Lupin.FullMoon"));
			if (Common.Systems.ShriekingShackSystem.shackUnlocked)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Lupin.Shack"));

			return chat;
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = Language.GetTextValue("Mods.WizardingWorld.Passages.PassageButton");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
			if (firstButton)
			{
				shopName = ShopName;
			}
			else
			{
				Main.npcChatText = Common.Systems.SecretPassageSystem.GetStatusText();
			}
		}

		public override void AddShops()
		{
			new NPCShop(Type, ShopName)
				.Add<Items.Consumables.WillowPassageToken>()
				.Add<Items.Consumables.Potions.WolfsbanePotion>()
				.Add<Items.Accessories.MaraudersMap>()
				.Add<Items.Consumables.Potions.PepperupPotion>()
				.Add(ItemID.SoulofLight, Condition.Hardmode)
				.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 18;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 25;
			randExtraCooldown = 10;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.WoodenBoomerang;
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 10f;
			randomOffset = 2f;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Lupin"),
			});
		}
	}
}
