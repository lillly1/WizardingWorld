using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>Goblin Ledger -- enchanted Gringotts ledger. +luck, coin attraction, +5% all damage, treasure sense. Mod-original.</summary>
    public class GoblinLedger : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22; Item.height = 22; Item.accessory = true;
            Item.rare = ItemRarityID.Orange; Item.value = Item.sellPrice(gold: 10);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.luck += 0.15f;
            player.goldRing = true;
            player.findTreasure = true;
            player.GetDamage(DamageClass.Generic) += 0.05f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.GalleonDust>(), 20)
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
                .AddIngredient(ItemID.GoldBar, 10)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
