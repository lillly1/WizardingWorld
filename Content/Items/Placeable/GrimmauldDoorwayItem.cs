using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the Grimmauld Doorway landmark tile.
    /// Grimmauld Place safehouse identity prop.
    /// </summary>
    public class GrimmauldDoorwayItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.GrimmauldDoorway>());
            Item.width = 20;
            Item.height = 30;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
                .AddIngredient(ItemID.IronBar, 10)
                .AddIngredient(ItemID.EbonwoodWall, 10)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
