using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>Glade Dew -- purified essence from the Unicorn Glade expedition loop. Crafting material. Mod-original.</summary>
    public class GladeDew : ModItem
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
