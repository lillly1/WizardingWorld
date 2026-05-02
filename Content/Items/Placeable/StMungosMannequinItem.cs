using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the St Mungos Mannequin landmark tile.
    /// Hidden hospital entrance identity prop (Purge and Dowse Ltd).
    /// </summary>
    public class StMungosMannequinItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.StMungosMannequin>());
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient(ItemID.IronBar, 3)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
