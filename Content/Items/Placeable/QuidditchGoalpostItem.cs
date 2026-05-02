using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the Quidditch Goalpost landmark tile.
    /// Pitch boundary identity prop.
    /// </summary>
    public class QuidditchGoalpostItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.QuidditchGoalpost>());
            Item.width = 12;
            Item.height = 32;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
                .AddIngredient(ItemID.Wood, 20)
                .AddIngredient(ItemID.GoldBar, 3)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
