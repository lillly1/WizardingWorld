using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
    /// <summary>
    /// Places the Veil Arch landmark tile.
    /// Department of Mysteries: Death Chamber identity prop.
    /// </summary>
    public class VeilArchItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Landmarks.VeilArch>());
            Item.width = 28;
            Item.height = 36;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
                .AddIngredient(ItemID.StoneBlock, 30)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
