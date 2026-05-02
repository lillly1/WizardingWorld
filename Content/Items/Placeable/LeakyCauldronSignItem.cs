using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the Leaky Cauldron Sign landmark tile.
    /// Diagon Alley gateway identity prop.
    /// </summary>
    public class LeakyCauldronSignItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.LeakyCauldronSign>());
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
                .AddIngredient(ItemID.Wood, 15)
                .AddIngredient(ItemID.IronBar, 3)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
