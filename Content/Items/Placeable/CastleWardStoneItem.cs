using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the Castle Ward Stone landmark tile.
    /// Battle of Hogwarts defense identity prop.
    /// </summary>
    public class CastleWardStoneItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.CastleWardStone>());
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
                .AddIngredient(ItemID.StoneBlock, 20)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
