using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>Forest Warden's Badge -- expedition reward. +6 def, +5% endurance, danger sense, night vision in forest. Crafted from all 3 forest materials. Mod-original.</summary>
    public class ForestWarden : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 6;
            player.endurance += 0.05f;
            player.dangerSense = true;

            if (player.InModBiome<Content.Biomes.ForbiddenForestBiome>())
            {
                player.nightVision = true;
                player.moveSpeed += 0.08f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.GladeDew>(), 10)
                .AddIngredient(ModContent.ItemType<Consumables.StarTrace>(), 10)
                .AddIngredient(ModContent.ItemType<Consumables.BroodVenom>(), 10)
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
