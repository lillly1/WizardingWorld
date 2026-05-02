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
	/// <summary>
	/// Fred and George Weasley — the Weasleys' Wizard Wheezes shopkeepers.
	/// Sells ALL Weasley products, joke items, and combat consumables.
	/// Appears after the Basilisk is defeated (they need customers with gold).
	/// Full of mischief — random dialogue about pranks and inventions.
	/// </summary>
	[AutoloadHead]
	public class FredAndGeorge : ModNPC
	{
		public const string ShopName = "Shop";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 25;
			NPCID.Sets.ExtraFramesCount[Type] = 9;
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700;
			NPCID.Sets.AttackType[Type] = 0; // Throwing
			NPCID.Sets.AttackTime[Type] = 30;
			NPCID.Sets.AttackAverageChance[Type] = 15;
			NPCID.Sets.HatOffsetY[Type] = 4;

			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
				.SetBiomeAffection<HallowBiome>(AffectionLevel.Love)
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Love)
				.SetNPCAffection(NPCID.Merchant, AffectionLevel.Like)
				.SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate);
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = NPCAIStyleID.Passive;
			NPC.damage = 15;
			NPC.defense = 15;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			AnimationType = NPCID.Guide;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs)
		{
			return Common.Systems.DownedBossSystem.downedBasilisk;
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>() { "Fred & George", "Gred & Forge", "The Weasley Twins", "Messrs Weasley" };
		}

		public override string GetChat()
		{
			WeightedRandom<string> chat = new();
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.Standard1"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.Standard2"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.Standard3"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.Standard4"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.Standard5"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.Standard6"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.Standard7"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.Standard8"));

			// Context-aware
			if (Common.Systems.DeathEaterInvasion.invasionActive)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.DuringInvasion"), 3.0);
			if (Common.Systems.DownedBossSystem.downedVoldemort)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.PostVoldemort"));
			if (Common.Systems.DownedBossSystem.downedUmbridge)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.PostUmbridge"));
			if (Main.bloodMoon)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.BloodMoon"));

			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.FredAndGeorge.Rare"), 0.1);
			return chat;
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28"); // Shop
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (firstButton)
				shop = ShopName;
		}

		public override void AddShops()
		{
			new NPCShop(Type, ShopName)
				// Weasley Combat Line
				.Add<Items.Weapons.WeasleyFireworks>()
				.Add<Items.Weapons.WeasleyBlazeBox>()
				.Add<Items.Weapons.DecoyDetonator>()
				.Add<Items.Weapons.Howler>()
				.Add<Items.Weapons.PeruvianDarknessPowder>()
				// Weasley Defense Line
				.Add<Items.Accessories.ShieldHat>()
				.Add<Items.Accessories.ExtendableEars>()
				.Add<Items.Accessories.Spectrespecs>()
				// Quidditch Line
				.Add<Items.Accessories.KeeperGloves>()
				.Add<Items.Weapons.ScreechingHowler>()
				// Sweets (Honeydukes partnership)
				.Add<Items.Consumables.BertieBottsBeans>()
				.Add<Items.Consumables.ChocolateFrog>()
				.Add<Items.Consumables.PeppermintToad>()
				.Add<Items.Consumables.ChocolateCauldron>()
				// Skiving Snackbox Line
				.Add<Items.Consumables.SkivingSnackbox>()
				.Add<Items.Consumables.PukingPastille>()
				.Add<Items.Consumables.NosebleedNougat>()
				// Stealth & Utility
				.Add<Items.Consumables.Potions.StealthDraught>(Condition.Hardmode)
				.Add<Items.Consumables.FlooPowder>()
				.Add<Items.Consumables.Portkey>()
				// Marauder's items (hardmode)
				.Add<Items.Accessories.PadfootAmulet>(Condition.Hardmode)
				.Add<Items.Accessories.ProngsCharm>(Condition.Hardmode)
				.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 20;
			knockback = 5f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 15;
			randExtraCooldown = 10;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ModContent.ProjectileType<Projectiles.WeasleyFireworkProjectile>();
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 10f;
			gravityCorrection = 1f;
			randomOffset = 3f;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.FredAndGeorge"),
			});
		}
	}
}
