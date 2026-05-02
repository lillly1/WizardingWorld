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
	/// <summary>Potions Master — sells potion ingredients and brews.</summary>
	[AutoloadHead]
	public class PotionsMaster : ModNPC
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
				.SetBiomeAffection<UndergroundBiome>(AffectionLevel.Love)
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
				.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.Wizard, AffectionLevel.Like)
				.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love)
				.SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Dislike);
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = NPCAIStyleID.Passive;
			NPC.damage = 10;
			NPC.defense = 15;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			AnimationType = NPCID.Guide;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs)
		{
			return numTownNPCs >= 4;
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>() { "Severus", "Horace", "Libatius", "Zygmunt", "Arsenius" };
		}

		public override string GetChat()
		{
			WeightedRandom<string> chat = new();
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.PotionsMaster.Standard1"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.PotionsMaster.Standard2"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.PotionsMaster.Standard3"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.PotionsMaster.Standard4"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.PotionsMaster.Standard5"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.PotionsMaster.Standard6"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.PotionsMaster.Standard7"));

			// Context-aware — reacts to player's current buffs
			Player player = Main.LocalPlayer;
			if (player.HasBuff(ModContent.BuffType<Buffs.FelixFelicisBuff>()))
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.PotionsMaster.SeeFelixBuff"));
			if (player.HasBuff(ModContent.BuffType<Buffs.FirewhiskeyBuff>()))
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.PotionsMaster.SeeFirewhiskeyBuff"));
			if (Common.Systems.DownedBossSystem.downedVoldemort)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.PotionsMaster.PostVoldemort"));

			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.PotionsMaster.Rare"), 0.1);
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
				.Add<Items.Consumables.Potions.Butterbeer>()
				.Add<Items.Consumables.Potions.FelixFelicis>(Condition.Hardmode)
				.Add<Items.Consumables.Potions.PolyjuicePotion>(Condition.Hardmode)
				.Add<Items.Consumables.Potions.WolfsbanePotion>(Condition.Hardmode)
				.Add(ItemID.Bottle)
				.Add(ItemID.Mushroom)
				.Add(ItemID.Daybloom)
				.Add(ItemID.Moonglow)
				.Add(ItemID.Deathweed, Condition.Hardmode)
				.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 15;
			knockback = 3f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 30;
			randExtraCooldown = 30;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.ToxicFlask;
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 8f;
			gravityCorrection = 2f;
			randomOffset = 2f;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.PotionsMaster"),
			});
		}
	}
}
