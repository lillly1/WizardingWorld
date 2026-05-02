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
	/// <summary>Dobby — free house-elf NPC. Sells utility items and socks.</summary>
	[AutoloadHead]
	public class Dobby : ModNPC
	{
		public const string ShopName = "Shop";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 25;
			NPCID.Sets.ExtraFramesCount[Type] = 9;
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700;
			NPCID.Sets.AttackType[Type] = 0; // Throwing
			NPCID.Sets.AttackTime[Type] = 60;
			NPCID.Sets.AttackAverageChance[Type] = 30;
			NPCID.Sets.HatOffsetY[Type] = 4;

			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
				.SetBiomeAffection<HallowBiome>(AffectionLevel.Like)
				.SetBiomeAffection<CorruptionBiome>(AffectionLevel.Hate)
				.SetNPCAffection(NPCID.Guide, AffectionLevel.Love)
				.SetNPCAffection(NPCID.Clothier, AffectionLevel.Like);
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 34;
			NPC.aiStyle = NPCAIStyleID.Passive;
			NPC.damage = 8;
			NPC.defense = 5;
			NPC.lifeMax = 150;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.7f;
			AnimationType = NPCID.Guide;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs)
		{
			return numTownNPCs >= 6 || Common.Systems.DownedBossSystem.downedBasilisk;
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>() { "Dobby", "Winky", "Kreacher", "Hokey", "Tippy" };
		}

		public override string GetChat()
		{
			WeightedRandom<string> chat = new();
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dobby.Standard1"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dobby.Standard2"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dobby.Standard3"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dobby.Standard4"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dobby.Standard5"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dobby.Standard6"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dobby.Standard7"));

			// Context-aware
			if (Common.Systems.DownedBossSystem.downedVoldemort)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dobby.PostVoldemort"), 2.0);
			if (!Main.dayTime)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dobby.Night"));
			if (Common.Systems.DownedBossSystem.downedBasilisk)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dobby.PostBasilisk"));

			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dobby.Rare"), 0.1);
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
				.Add(ItemID.Torch)
				.Add(ItemID.Rope)
				.Add(ItemID.HealingPotion)
				.Add(ItemID.ManaPotion)
				.Add(ItemID.RecallPotion)
				.Add(ItemID.WormholePotion)
				.Add<Items.Accessories.SortingHat>()
				.Add<Items.Consumables.Potions.Butterbeer>()
				.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 15;
			knockback = 6f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 20;
			randExtraCooldown = 20;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.Shuriken; // Throws things
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
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Dobby"),
			});
		}
	}
}
