using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Great Hall Bell -- summons a Great Hall experience.
    /// When a feast is available (every 7 in-game days) attending grants a house-aligned buff.
    /// Between feasts, shows current House Cup standings.
    /// Mod-original: represents the Hogwarts Great Hall gathering tradition.
    /// </summary>
    public class GreatHallBell : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.consumable = false;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.Item4;
        }

        public override bool? UseItem(Player player)
        {
            if (Common.Systems.GreatHallSystem.feastAvailable &&
                !Common.Systems.GreatHallSystem.feastAttendedToday)
            {
                int result = Common.Systems.GreatHallSystem.AttendFeast(player);
                if (result < 0)
                {
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.GreatHall.NeedHouseArmor"),
                        new Color(255, 100, 100));
                }
            }
            else if (Common.Systems.GreatHallSystem.feastAttendedToday)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.GreatHall.AlreadyAttended"),
                    new Color(200, 200, 200));
            }
            else
            {
                // No feast available -- show standings
                Main.NewText(Common.Systems.GreatHallSystem.GetStandingsText(),
                    new Color(255, 215, 100));
                string nextFeast = Language.GetTextValue("Mods.WizardingWorld.GreatHall.NextFeast",
                    7 - Common.Systems.GreatHallSystem.daysSinceLastFeast);
                Main.NewText(nextFeast, new Color(200, 200, 200));
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 5)
                .AddIngredient(ItemID.Bell, 1)
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 10)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
