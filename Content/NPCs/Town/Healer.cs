using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace WizardingWorld.Content.NPCs.Town
{
    [AutoloadHead]
    public class Healer : ModNPC
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
            NPC.townNPC = true; NPC.friendly = true;
            NPC.width = 18; NPC.height = 40; NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10; NPC.defense = 15; NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1; NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f; AnimationType = NPCID.Guide;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) =>
            Common.Systems.StMungosTriageSystem.hospitalUnlocked;

        public override List<string> SetNPCNameList() =>
            new() { "Healer Miriam", "Healer Augustus", "Healer Hippocrates" };

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Healer.Standard1"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Healer.Standard2"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Healer.Standard3"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Healer.Standard4"));
            if (Common.Systems.StMungosTriageSystem.missionsCompleted > 0)
                chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Healer.Veteran"));
            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = Language.GetTextValue("Mods.WizardingWorld.StMungos.TriageButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton) { shopName = "Shop"; }
            else { Main.npcChatText = Common.Systems.StMungosTriageSystem.GetStatusText(); }
        }

        public override void AddShops()
        {
            new NPCShop(Type, "Shop")
                .Add<Items.Consumables.StMungosPass>()
                .Add<Items.Consumables.Potions.PepperupPotion>()
                .Add<Items.Consumables.Potions.SkeleGro>()
                .Add<Items.Consumables.Potions.MandrakeRestorative>()
                .Add(ItemID.HealingPotion)
                .Add(ItemID.RestorationPotion)
                .Register();
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        { damage = 10; knockback = 3f; }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        { cooldown = 30; randExtraCooldown = 10; }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        { projType = ProjectileID.PurificationPowder; attackDelay = 1; }
    }
}
