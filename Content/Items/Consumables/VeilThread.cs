using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>Veil Thread -- dark gossamer from sealed Veil fractures. Crafting material. Mod-original.</summary>
    public class VeilThread : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16; Item.height = 16;
            Item.maxStack = 999; Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 1);
        }
    }
}
