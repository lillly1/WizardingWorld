using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the Borgin Storefront landmark tile.
    /// Knockturn Alley identity prop (Borgin and Burkes).
    /// </summary>
    public class BorginStorefrontItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.BorginStorefront>());
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
                .AddIngredient(ItemID.EbonwoodWall, 10)
                .AddIngredient(ItemID.Bone, 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
