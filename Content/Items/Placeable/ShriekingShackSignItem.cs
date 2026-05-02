using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the Shrieking Shack Sign landmark tile.
    /// Shack identity marker near Hogsmeade.
    /// </summary>
    public class ShriekingShackSignItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.ShriekingShackSign>());
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
                .AddIngredient(ItemID.Cobweb, 10)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
