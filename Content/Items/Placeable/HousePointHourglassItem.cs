using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the House Point Hourglass landmark tile.
    /// Great Hall identity prop.
    /// </summary>
    public class HousePointHourglassItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.HousePointHourglass>());
            Item.width = 20;
            Item.height = 30;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
                .AddIngredient(ItemID.Glass, 10)
                .AddIngredient(ItemID.GoldBar, 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
