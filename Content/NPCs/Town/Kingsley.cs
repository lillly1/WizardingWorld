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
    /// Kingsley Shacklebolt -- Order of the Phoenix Auror, Ministry mission-giver.
    /// Appears after defeating Bellatrix. Sells mission items and Order supplies.
    /// Canon-faithful: Kingsley is a senior Auror and Order member who becomes Minister for Magic.
    /// </summary>
    [AutoloadHead]
    public class Kingsley : ModNPC
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

            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Love)
                .SetBiomeAffection<CorruptionBiome>(AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Guide, AffectionLevel.Like)
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Like);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 1f, Direction = -1 };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
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
            NPC.lifeMax = 300;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return Common.Systems.DownedBossSystem.downedBellatrix;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Kingsley Shacklebolt" };
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Kingsley.Standard1"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Kingsley.Standard2"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Kingsley.Standard3"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Kingsley.Standard4"));

            if (Common.Systems.ProphecyMissionSystem.missionsCompleted > 0)
                chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Kingsley.Veteran"));

            if (Common.Systems.GrimmauldPlaceSystem.safehouseRevealed)
                chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Kingsley.Grimmauld"));

            if (Common.Systems.GrimmauldPlaceSystem.maintenanceCompleted >= 3)
                chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Kingsley.Kreacher"));

            if (!Main.dayTime)
                chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Kingsley.Night"));

            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28"); // Shop
            button2 = Language.GetTextValue("Mods.WizardingWorld.Ministry.MissionButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = ShopName;
            }
            else
            {
                Main.npcChatText = Common.Systems.ProphecyMissionSystem.GetStatusText();
            }
        }

        public override void AddShops()
        {
            new NPCShop(Type, ShopName)
                .Add<Items.Consumables.MinistryVisitorBadge>()
                .Add<Items.Consumables.AzkabanBreachStone>()
                .Add<Items.Consumables.AzkabanWardSigil>()
                .Add<Items.Consumables.SecretKeeperNote>(new Condition(
                    "Mods.WizardingWorld.Conditions.PostDementorKing",
                    () => Common.Systems.DownedBossSystem.downedDementorKing))
                .Add(ItemID.SoulofNight, Condition.Hardmode)
                .Register();
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 5f;
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
                new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Kingsley"),
            });
        }
    }
}
