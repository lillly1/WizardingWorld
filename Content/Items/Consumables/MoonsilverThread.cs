using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>Moonsilver Thread -- woven from shack ward residue. Crafting material. Mod-original.</summary>
    public class MoonsilverThread : ModItem
    {
        public override void SetDefaults()
        { Item.width = 16; Item.height = 16; Item.maxStack = 999; Item.rare = ItemRarityID.LightPurple; Item.value = Item.sellPrice(silver: 85); }
    }
}
