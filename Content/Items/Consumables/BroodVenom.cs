using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>Brood Venom -- toxic extract from the Acromantula Nest expedition loop. Crafting material. Mod-original.</summary>
    public class BroodVenom : ModItem
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
