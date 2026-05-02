using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>Galleon Dust -- enchanted gold residue from Gringotts vaults. Crafting material. Mod-original.</summary>
    public class GalleonDust : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16; Item.height = 16; Item.maxStack = 999;
            Item.rare = ItemRarityID.Orange; Item.value = Item.sellPrice(gold: 1);
        }
    }
}
