using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the Gringotts Facade landmark tile.
    /// Gringotts Wizarding Bank identity prop.
    /// </summary>
    public class GringottsFacadeItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.GringottsFacade>());
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.buyPrice(gold: 8);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 12)
                .AddIngredient(ItemID.MarbleBlock, 20)
                .AddIngredient(ItemID.GoldBar, 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
