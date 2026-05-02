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
	/// <summary>Ollivander — wand shop NPC. Sells basic wands and spell components.</summary>
	[AutoloadHead]
	public class Ollivander : ModNPC
	{
		public const string ShopName = "Shop";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 25;
			NPCID.Sets.ExtraFramesCount[Type] = 9;
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700;
			NPCID.Sets.AttackType[Type] = 2; // Magic attack
			NPCID.Sets.AttackTime[Type] = 90;
			NPCID.Sets.AttackAverageChance[Type] = 30;
			NPCID.Sets.HatOffsetY[Type] = 4;

			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
				.SetBiomeAffection<UndergroundBiome>(AffectionLevel.Like)
				.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.Wizard, AffectionLevel.Love)
				.SetNPCAffection(NPCID.Merchant, AffectionLevel.Like)
				.SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Dislike);
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
			// Spawns after any player has a wand
			foreach (var player in Main.ActivePlayers)
			{
				foreach (var item in player.inventory)
				{
					if (item.active && item.DamageType == ModContent.GetInstance<DamageClasses.SpellDamage>())
						return true;
				}
			}

			return numTownNPCs >= 3;
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>() { "Garrick", "Mykew", "Antioch", "Cadmus", "Ignotus", "Salazar" };
		}

		public override string GetChat()
		{
			WeightedRandom<string> chat = new();
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Ollivander.Standard1"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Ollivander.Standard2"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Ollivander.Standard3"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Ollivander.Standard4"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Ollivander.Standard5"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Ollivander.Standard6"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Ollivander.Standard7"));

			// Context-aware dialogue
			if (Common.Systems.DownedBossSystem.downedVoldemort)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Ollivander.PostVoldemort"));
			if (Common.Systems.DownedBossSystem.downedBasilisk && !Main.hardMode)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Ollivander.PostBasilisk"));
			if (Main.hardMode)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Ollivander.Hardmode"));

			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Ollivander.Rare"), 0.1);
			return chat;
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28"); // "Shop"
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (firstButton)
				shop = ShopName;
		}

		public override void AddShops()
		{
			new NPCShop(Type, ShopName)
				.Add<Items.Weapons.Wands.OakWand>()
				.Add<Items.Weapons.Wands.WillowWand>()
				.Add<Items.Weapons.Wands.AshWand>(Condition.DownedEyeOfCthulhu)
				.Add<Items.Placeable.EnchantingTableItem>()
				.Add(ItemID.FallenStar)
				.Add(ItemID.ManaCrystal)
				.Add(ItemID.ManaPotion)
				.Add(ItemID.Book)
				.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 20;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 30;
			randExtraCooldown = 30;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ModContent.ProjectileType<Projectiles.Spells.StupefyProjectile>();
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 12f;
			randomOffset = 2f;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Ollivander"),
			});
		}
	}
}
