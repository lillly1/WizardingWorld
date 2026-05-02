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
	/// <summary>Dumbledore — appears after Azkaban's Despair. Guides the Hallows path and later sells top-tier items.</summary>
	[AutoloadHead]
	public class Dumbledore : ModNPC
	{
		public const string ShopName = "Shop";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 25;
			NPCID.Sets.ExtraFramesCount[Type] = 9;
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700;
			NPCID.Sets.AttackType[Type] = 2; // Magic
			NPCID.Sets.AttackTime[Type] = 60;
			NPCID.Sets.AttackAverageChance[Type] = 10;
			NPCID.Sets.HatOffsetY[Type] = 4;

			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
				.SetBiomeAffection<HallowBiome>(AffectionLevel.Like)
				.SetBiomeAffection<CorruptionBiome>(AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.Wizard, AffectionLevel.Love)
				.SetNPCAffection(NPCID.Guide, AffectionLevel.Like);
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = NPCAIStyleID.Passive;
			NPC.damage = 50;
			NPC.defense = 30;
			NPC.lifeMax = 500;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.3f;
			AnimationType = NPCID.Guide;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs)
		{
			return Common.Systems.DownedBossSystem.downedDementorKing;
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>() { "Albus", "Percival", "Gellert", "Nicolas" };
		}

		public override string GetChat()
		{
			WeightedRandom<string> chat = new();
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dumbledore.Standard1"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dumbledore.Standard2"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dumbledore.Standard3"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dumbledore.Standard4"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dumbledore.Standard5"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dumbledore.Standard6"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dumbledore.Standard7"));

			// Context-aware — Dumbledore reflects on what you've accomplished
			if (Common.Systems.DownedBossSystem.downedDementorKing)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dumbledore.PostDementorKing"));
			if (Main.bloodMoon)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dumbledore.BloodMoon"));
			if (Main.player[Main.myPlayer].statManaMax2 >= 300)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dumbledore.HighMana"));

			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Dumbledore.Rare"), 0.1);
			return chat;
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = Common.Systems.HallowsSystem.CanPurifyGauntsRing(Main.LocalPlayer)
				? "Purify Ring"
				: Common.Systems.HallowsSystem.CanClaimInvisibilityCloak
					? "Receive Cloak"
					: "Hallows";
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (firstButton)
			{
				shop = ShopName;
				return;
			}

			Player player = Main.LocalPlayer;

			if (Common.Systems.HallowsSystem.TryPurifyGauntsRing(player))
			{
				Main.npcChatText = "The ring was never the true prize. The stone within is older, stranger, and far more dangerous.";
				return;
			}

			if (Common.Systems.HallowsSystem.TryClaimInvisibilityCloak(player))
			{
				Main.npcChatText = "This cloak was never meant to be won like common loot. Guard it well.";
				return;
			}

			Main.npcChatText = Common.Systems.HallowsSystem.GetDumbledoreGuidance(player);
		}

		public override void AddShops()
		{
			new NPCShop(Type, ShopName)
				.Add<Items.Consumables.WizardsAlmanac>()
				.Add<Items.Consumables.PhoenixTear>()
				.Add(ItemID.SuperManaPotion)
				.Add(ItemID.SuperHealingPotion)
				.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 50;
			knockback = 6f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 15;
			randExtraCooldown = 10;
		}

		public override void TownNPCAttackMagic(ref float auraLightMultiplier)
		{
			auraLightMultiplier = 2f;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ModContent.ProjectileType<Projectiles.Spells.AvadaKedavraProjectile>();
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 14f;
			randomOffset = 1f;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Dumbledore"),
			});
		}
	}
}
