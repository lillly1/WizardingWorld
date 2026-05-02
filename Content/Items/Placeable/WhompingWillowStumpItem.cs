using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the Whomping Willow Stump landmark tile.
    /// Passage entrance identity prop near the Willow.
    /// </summary>
    public class WhompingWillowStumpItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.WhompingWillowStump>());
            Item.width = 28;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
                .AddIngredient(ItemID.Wood, 30)
                .AddIngredient(ItemID.Vine, 3)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
