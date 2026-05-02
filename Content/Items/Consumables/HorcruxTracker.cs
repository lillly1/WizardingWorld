using Microsoft.Xna.Framework;
using Terraria; using Terraria.ID; using Terraria.Localization; using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>Horcrux Tracker -- Dumbledore's wartime intelligence notes. Shows Horcrux Hunt state and battle readiness. Reusable.</summary>
    public class HorcruxTracker : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20; Item.height = 20; Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30; Item.useAnimation = 30; Item.consumable = false;
            Item.rare = ItemRarityID.Red; Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item4;
        }

        public override bool? UseItem(Player player)
        {
            if (Common.Systems.BattleOfHogwartsSystem.CanUnlock())
            {
                Common.Systems.BattleOfHogwartsSystem.Unlock();
                return true;
            }
            if (Common.Systems.BattleOfHogwartsSystem.CanStart())
            {
                Common.Systems.BattleOfHogwartsSystem.StartBattle(player);
                return true;
            }

            // Show Horcrux state
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.TrackerTitle"), new Color(255, 215, 100));
                Main.NewText($"  {(Common.Systems.HorcruxHuntSystem.diaryDestroyed ? "[X]" : "[ ]")} {Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.DiaryName")}", Common.Systems.HorcruxHuntSystem.diaryDestroyed ? new Color(100, 200, 100) : new Color(200, 100, 100));
                Main.NewText($"  {(Common.Systems.HorcruxHuntSystem.locketDestroyed ? "[X]" : "[ ]")} {Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.LocketName")}", Common.Systems.HorcruxHuntSystem.locketDestroyed ? new Color(100, 200, 100) : new Color(200, 100, 100));
                Main.NewText($"  {(Common.Systems.HorcruxHuntSystem.cupDestroyed ? "[X]" : "[ ]")} {Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.CupName")}", Common.Systems.HorcruxHuntSystem.cupDestroyed ? new Color(100, 200, 100) : new Color(200, 100, 100));
                Main.NewText($"  {(Common.Systems.HorcruxHuntSystem.diademDestroyed ? "[X]" : "[ ]")} {Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.DiademName")}", Common.Systems.HorcruxHuntSystem.diademDestroyed ? new Color(100, 200, 100) : new Color(200, 100, 100));
                Main.NewText($"  {(Common.Systems.HorcruxHuntSystem.naginiDefeated ? "[X]" : "[ ]")} {Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.NaginiName")}", Common.Systems.HorcruxHuntSystem.naginiDefeated ? new Color(100, 200, 100) : new Color(200, 100, 100));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Battle.VoldemortPower", (int)(Common.Systems.HorcruxHuntSystem.GetVoldemortPowerMultiplier() * 100)), new Color(200, 150, 150));
                Main.NewText(Common.Systems.BattleOfHogwartsSystem.GetStatusText(), new Color(200, 180, 140));
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 20)
                .AddIngredient(ItemID.Book, 1)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
