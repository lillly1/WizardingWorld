using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Quidditch Whistle -- starts an inter-house Quidditch match.
    /// Reusable item. Requires: daytime, on a broom mount, wearing house armor, post-Basilisk.
    /// Between matches, shows current Quidditch Cup standings.
    /// Mod-original: Madam Hooch's whistle to begin a match.
    /// </summary>
    public class QuidditchWhistle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.consumable = false;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 5);
            Item.UseSound = SoundID.Item35;
        }

        public override bool? UseItem(Player player)
        {
            if (Common.Systems.QuidditchCupSystem.matchActive)
            {
                // Show current match score
                string score = Language.GetTextValue("Mods.WizardingWorld.Quidditch.CurrentScore",
                    Common.Systems.QuidditchCupSystem.playerScore,
                    Common.Systems.QuidditchCupSystem.opponentScore);
                Main.NewText(score, new Color(255, 255, 200));
                return true;
            }

            if (Common.Systems.QuidditchCupSystem.quidditchCupAwarded)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.SeasonComplete"),
                    new Color(200, 200, 200));
                // Reset season for next cycle
                Common.Systems.QuidditchCupSystem.ResetSeason();
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.NewSeason"),
                    new Color(255, 215, 100));
                return true;
            }

            if (Common.Systems.QuidditchCupSystem.CanStartMatch(player))
            {
                Common.Systems.QuidditchCupSystem.StartMatch(player);
            }
            else
            {
                // Show standings
                Main.NewText(Common.Systems.QuidditchCupSystem.GetStandingsText(),
                    new Color(255, 215, 100));

                var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
                if (wp.houseSet <= 0 || wp.houseSet > 4)
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.NeedHouseArmor"),
                        new Color(255, 100, 100));
                else if (!player.mount.Active)
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.NeedBroom"),
                        new Color(255, 100, 100));
                else if (!Main.dayTime)
                    Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Quidditch.NeedDaytime"),
                        new Color(255, 100, 100));
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 3)
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
