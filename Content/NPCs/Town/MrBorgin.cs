using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace WizardingWorld.Content.NPCs.Town
{
    [AutoloadHead]
    public class MrBorgin : ModNPC
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
            NPC.width = 18; NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 14; NPC.defense = 12; NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return Common.Systems.KnockturnAlleySystem.knockturnUnlocked;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Mr Borgin" };
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MrBorgin.Standard1"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MrBorgin.Standard2"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MrBorgin.Standard3"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MrBorgin.Standard4"));
            if (Common.Systems.KnockturnAlleySystem.contractsCompleted > 0)
                chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.MrBorgin.Returning"));
            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = Language.GetTextValue("Mods.WizardingWorld.Knockturn.ContractButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton) { shopName = "Shop"; }
            else { Main.npcChatText = Common.Systems.KnockturnAlleySystem.GetStatusText(); }
        }

        public override void AddShops()
        {
            new NPCShop(Type, "Shop")
                .Add<Items.Consumables.KnockturnPass>()
                .Add<Items.Consumables.DarkArtsTome>()
                .Add(ItemID.SoulofNight, Condition.Hardmode)
                .Register();
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        { damage = 14; knockback = 3f; }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        { cooldown = 35; randExtraCooldown = 15; }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        { projType = ProjectileID.PoisonedKnife; attackDelay = 1; }
    }
}
