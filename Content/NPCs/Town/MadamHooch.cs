using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace WizardingWorld.Content.NPCs.Town
{
    /// <summary>Madam Hooch — Quidditch referee and flying instructor NPC.</summary>
    [AutoloadHead]
    public class MadamHooch : ModNPC
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

            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            { Velocity = 1f, Direction = -1 };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;

            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Guide, AffectionLevel.Like)
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Dislike);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 12;
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
            return new List<string>() { "Madam Hooch" };
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamHooch.Standard1"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamHooch.Standard2"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamHooch.Standard3"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamHooch.Standard4"));

            if (Common.Systems.QuidditchCupSystem.matchesPlayedThisSeason > 0)
                chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamHooch.Season"));

            if (Common.Systems.QuidditchCupSystem.quidditchCupAwarded)
                chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MadamHooch.CupWon"));

            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = Language.GetTextValue("Mods.WizardingWorld.Quidditch.StandingsButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = ShopName;
            }
            else
            {
                Main.npcChatText = Common.Systems.QuidditchCupSystem.GetStandingsText();
            }
        }

        public override void AddShops()
        {
            new NPCShop(Type, ShopName)
                .Add<Items.Consumables.QuidditchWhistle>()
                .Add<Items.Weapons.Quaffle>()
                .Add<Items.Weapons.BeatersBat>()
                .Add(ItemID.FlyingCarpet, Condition.Hardmode)
                .Register();
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 18;
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
                new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.MadamHooch"),
            });
        }
    }
}
