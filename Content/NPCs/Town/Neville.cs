using System.Collections.Generic;
using Terraria; using Terraria.ID; using Terraria.Localization; using Terraria.ModLoader; using Terraria.Utilities;

namespace WizardingWorld.Content.NPCs.Town
{
    [AutoloadHead]
    public class Neville : ModNPC
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
            NPC.damage = 20; NPC.defense = 15; NPC.lifeMax = 280;
            NPC.HitSound = SoundID.NPCHit1; NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) =>
            Common.Systems.BattleOfHogwartsSystem.battlesWon >= 1;

        public override List<string> SetNPCNameList() => new() { "Neville Longbottom" };

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Neville.Standard1"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Neville.Standard2"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Neville.Standard3"));
            chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Neville.Standard4"));
            if (Common.Systems.HorcruxHuntSystem.naginiDefeated)
                chat.Add(Language.GetTextValue("Mods.WizardingWorld.Dialogue.Neville.NaginiDone"));
            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton) shopName = "Shop";
        }

        public override void AddShops()
        {
            new NPCShop(Type, "Shop")
                .Add<Items.Consumables.HorcruxTracker>()
                .Add<Items.Weapons.SwordOfGryffindor>()
                .Add<Items.Consumables.Potions.MandrakeRestorative>()
                .Add(ItemID.HealingPotion)
                .Register();
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback) { damage = 20; knockback = 5f; }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) { cooldown = 25; randExtraCooldown = 10; }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay) { projType = ProjectileID.WoodenBoomerang; attackDelay = 1; }
    }
}
