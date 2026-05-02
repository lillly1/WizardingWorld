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
	/// Centaur — forest-dwelling stargazer NPC.
	/// Sells divination/detection items and rare materials.
	/// Cryptic dialogue about the stars and fate.
	/// </summary>
	[AutoloadHead]
	public class Centaur : ModNPC
	{
		public const string ShopName = "Shop";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 25;
			NPCID.Sets.ExtraFramesCount[Type] = 9;
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700;
			NPCID.Sets.AttackType[Type] = 1; // Shooting (bow)
			NPCID.Sets.AttackTime[Type] = 60;
			NPCID.Sets.AttackAverageChance[Type] = 20;
			NPCID.Sets.HatOffsetY[Type] = 4;

			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
				.SetBiomeAffection<JungleBiome>(AffectionLevel.Like)
				.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
				.SetBiomeAffection<HallowBiome>(AffectionLevel.Hate)
				.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love)
				.SetNPCAffection(NPCID.WitchDoctor, AffectionLevel.Like)
				.SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Dislike);
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = NPCAIStyleID.Passive;
			NPC.damage = 20;
			NPC.defense = 20;
			NPC.lifeMax = 350;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.3f;
			AnimationType = NPCID.Guide;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs)
		{
			// Appears in worlds with forests after any boss is defeated
			return NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3;
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>() { "Firenze", "Bane", "Ronan", "Magorian", "Torvus" };
		}

		public override string GetChat()
		{
			WeightedRandom<string> chat = new();
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Centaur.Standard1"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Centaur.Standard2"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Centaur.Standard3"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Centaur.Standard4"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Centaur.Standard5"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Centaur.Standard6"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Centaur.Standard7"));

			// Moon phase dialogue — centaurs read the stars
			string moonKey = Main.moonPhase switch
			{
				0 => "FullMoon",
				4 => "NewMoon",
				_ => null,
			};
			if (moonKey != null)
				chat.Add(Language.GetTextValue($"Mods.WizardingWorld.Dialogue.Centaur.{moonKey}"));

			if (Main.bloodMoon)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Centaur.BloodMoon"), 3.0);

			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Centaur.Rare"), 0.1);
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
				.Add<Items.Accessories.Remembrall>()
				.Add<Items.Accessories.Sneakoscope>()
				.Add<Items.Accessories.MaraudersMap>()
				.Add<Items.Consumables.Potions.Veritaserum>(Condition.Hardmode)
				.Add(ItemID.Lens)
				.Add(ItemID.FallenStar)
				.Add(ItemID.CrystalBall, Condition.Hardmode)
				.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 25;
			knockback = 5f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 20;
			randExtraCooldown = 15;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.WoodenArrowHostile;
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 14f;
			gravityCorrection = 1f;
			randomOffset = 1f;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Centaur"),
			});
		}
	}
}
