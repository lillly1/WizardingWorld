using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the Prophecy Shelf landmark tile.
    /// Department of Mysteries: Hall of Prophecy identity prop.
    /// </summary>
    public class ProphecyShelfItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.ProphecyShelf>());
            Item.width = 20;
            Item.height = 30;
            Item.value = Item.buyPrice(gold: 8);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 12)
                .AddIngredient(ItemID.Glass, 15)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
