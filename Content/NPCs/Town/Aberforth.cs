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
	/// <summary>Aberforth Dumbledore — gruff barkeep of the Hog's Head. Sells resistance supplies and offers secret passage access. Canon-faithful: Order member, Hog's Head owner.</summary>
	[AutoloadHead]
	public class Aberforth : ModNPC
	{
		public const string ShopName = "Shop";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 25;
			NPCID.Sets.ExtraFramesCount[Type] = 9;
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700;
			NPCID.Sets.AttackType[Type] = 0; // Throwing
			NPCID.Sets.AttackTime[Type] = 90;
			NPCID.Sets.AttackAverageChance[Type] = 30;
			NPCID.Sets.HatOffsetY[Type] = 4;

			NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 1f, Direction = -1 };
			NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;

			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Love)
				.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.Guide, AffectionLevel.Like)
				.SetNPCAffection(NPCID.Wizard, AffectionLevel.Dislike);
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = NPCAIStyleID.Passive;
			NPC.damage = 16;
			NPC.defense = 16;
			NPC.lifeMax = 270;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			AnimationType = NPCID.Guide;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs) =>
			Common.Systems.DownedBossSystem.downedDementorKing;

		public override List<string> SetNPCNameList() =>
			new() { "Aberforth Dumbledore" };

		public override string GetChat()
		{
			WeightedRandom<string> chat = new();
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Aberforth.Standard1"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Aberforth.Standard2"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Aberforth.Standard3"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Aberforth.Standard4"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Aberforth.Standard5"));

			if (Common.Systems.HogwartsWardSystem.wardsDefended > 0)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Aberforth.Resistance"));

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
				return;
			}

			Main.npcChatText = Common.Systems.SecretPassageSystem.GetStatusText();
		}

		public override void AddShops()
		{
			new NPCShop(Type, ShopName)
				.Add<Items.Consumables.DAGalleon>()
				.Add<Items.Consumables.AzkabanWardSigil>()
				.Add<Items.Consumables.WillowPassageToken>()
				.Add<Items.Consumables.Potions.Butterbeer>()
				.Add(ItemID.HealingPotion)
				.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 16;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 30;
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
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Aberforth"),
			});
		}
	}
}
