using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>Star Trace -- celestial residue from the Centaur Skywatch expedition loop. Crafting material. Mod-original.</summary>
    public class StarTrace : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 10);
        }
    }
}
