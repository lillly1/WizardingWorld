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
	/// <summary>Hagrid — sells creature-related items, eggs, and materials.</summary>
	[AutoloadHead]
	public class Hagrid : ModNPC
	{
		public const string ShopName = "Shop";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 25;
			NPCID.Sets.ExtraFramesCount[Type] = 9;
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700;
			NPCID.Sets.AttackType[Type] = 3; // Melee swipe
			NPCID.Sets.AttackTime[Type] = 30;
			NPCID.Sets.AttackAverageChance[Type] = 15;
			NPCID.Sets.HatOffsetY[Type] = 4;

			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
				.SetBiomeAffection<JungleBiome>(AffectionLevel.Like)
				.SetBiomeAffection<HallowBiome>(AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love)
				.SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Like)
				.SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Dislike);
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = NPCAIStyleID.Passive;
			NPC.damage = 25;
			NPC.defense = 20;
			NPC.lifeMax = 400;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.3f;
			AnimationType = NPCID.Guide;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs)
		{
			return Common.Systems.DownedBossSystem.downedTroll
				|| Common.Systems.DownedBossSystem.downedBasilisk
				|| numTownNPCs >= 5;
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>() { "Rubeus", "Newt", "Charlie", "Kettleburn", "Silvanus" };
		}

		public override string GetChat()
		{
			WeightedRandom<string> chat = new();
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.Standard1"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.Standard2"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.Standard3"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.Standard4"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.Standard5"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.Standard6"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.Standard7"));

			// Context-aware dialogue based on boss progression
			if (Common.Systems.DownedBossSystem.downedBasilisk)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.PostBasilisk"));
			if (Common.Systems.DownedBossSystem.downedAragog)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.PostAragog"));
			if (Common.Systems.DownedBossSystem.downedHorntail)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.PostHorntail"));
			if (Common.Systems.DownedBossSystem.downedVoldemort)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.PostVoldemort"));
			if (!Main.dayTime)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.Night"));

			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Hagrid.Rare"), 0.1);
			return chat;
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (firstButton)
				shop = ShopName;
		}

		public override void AddShops()
		{
			new NPCShop(Type, ShopName)
				.Add<Items.Consumables.BasiliskSummonItem>(Common.Systems.WizardConditions.DownedBasilisk)
				.Add<Items.Consumables.HorntailSummonItem>(Common.Systems.WizardConditions.DownedAnyMechBoss)
				.Add<Pets.GoldenSnitch.GoldenSnitchItem>()
				.Add(ItemID.Cobweb, Condition.InBelowSurface)
				.Add(ItemID.SpiderFang, Condition.Hardmode)
				.Add(ItemID.Feather)
				.Add(ItemID.UnicornHorn, Condition.Hardmode)
				.Add<Items.Consumables.ForestLantern>(Common.Systems.WizardConditions.DownedBasilisk)
				.Add<Items.Consumables.WillowPassageToken>(Common.Systems.WizardConditions.DownedFenrir)
				.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 30;
			knockback = 8f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 15;
			randExtraCooldown = 10;
		}

		public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
		{
			itemWidth = 40;
			itemHeight = 40;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Hagrid"),
			});
		}
	}
}
