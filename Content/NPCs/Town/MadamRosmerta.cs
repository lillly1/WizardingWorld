using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using WizardingWorld.Common.Systems;

namespace WizardingWorld.Content.NPCs.Town
{
	/// <summary>Madam Rosmerta — innkeeper of the Three Broomsticks tavern in Hogsmeade. Stocks Honeydukes sweets as tavern refreshments alongside her famous Butterbeer. Hogsmeade social anchor and Owl Post contact point. Canon-faithful.</summary>
	[AutoloadHead]
	public class MadamRosmerta : ModNPC
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
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.Guide, AffectionLevel.Like)
				.SetNPCAffection(NPCID.Merchant, AffectionLevel.Love);
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
			return Common.Systems.DownedBossSystem.downedBasilisk;
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>() { "Rosmerta" };
		}

		public override string GetChat()
		{
			WeightedRandom<string> chat = new();
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamRosmerta.Standard1"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamRosmerta.Standard2"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamRosmerta.Standard3"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamRosmerta.Standard4"));
			chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamRosmerta.Standard5"));

			if (!Main.dayTime)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamRosmerta.Night"));
			if (Common.Systems.DownedBossSystem.downedVoldemort)
				chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamRosmerta.PostVoldemort"), 2.0);

			return chat;
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamRosmerta.OwlPostButton");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (firstButton)
			{
				shop = ShopName;
			}
			else
			{
				Player player = Main.LocalPlayer;
				if (OwlPostSystem.requestCompleted)
				{
					Main.npcChatText = Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamRosmerta.OwlPostComplete");
				}
				else if (OwlPostSystem.TryCompleteRequest(player))
				{
					Main.npcChatText = Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamRosmerta.OwlPostSuccess");
				}
				else
				{
					Main.npcChatText = OwlPostSystem.GetCurrentRequestText();
				}
			}
		}

		public override void AddShops()
		{
			new NPCShop(Type, ShopName)
				.Add<Items.Consumables.Potions.Butterbeer>()
				.Add<Items.Consumables.FizzyWhizbee>()
				.Add<Items.Consumables.PepperImp>()
				.Add<Items.Consumables.SugarQuill>()
				.Add<Items.Consumables.AcidPop>()
				.Add<Items.Consumables.DrooblesBestBlowingGum>()
				.Add<Items.Consumables.ChocolateFrog>()
				.Add<Items.Consumables.PeppermintToad>()
				.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 18;
			knockback = 5f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 20;
			randExtraCooldown = 20;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.WaterGun;
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
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.MadamRosmerta"),
			});
		}
	}
}
