using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace WizardingWorld.Content.NPCs.Town
{
    [AutoloadHead]
    public class GoblinTeller : ModNPC
    {
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
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 1f, Direction = -1 };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 36;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 15;
            NPC.defense = 18;
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
            return new List<string> { "Griphook", "Bogrod", "Ragnok" };
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.GoblinTeller.Standard1"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.GoblinTeller.Standard2"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.GoblinTeller.Standard3"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.GoblinTeller.Standard4"));
            if (Common.Systems.GringottsVaultSystem.runsCompleted > 0)
                chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.GoblinTeller.Returning"));
            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = Language.GetTextValue("Mods.WizardingWorld.Gringotts.VaultButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton) { shopName = "Shop"; }
            else { Main.npcChatText = Common.Systems.GringottsVaultSystem.GetStatusText(); }
        }

        public override void AddShops()
        {
            new NPCShop(Type, "Shop")
                .Add<Items.Consumables.GringottsVaultKey>()
                .Add(ItemID.GoldCoin, Condition.Hardmode)
                .Add(ItemID.PlatinumCoin, Condition.DownedMoonLord)
                .Register();
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        { damage = 15; knockback = 4f; }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        { cooldown = 30; randExtraCooldown = 15; }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        { projType = ProjectileID.GoldCoin; attackDelay = 1; }
    }
}
