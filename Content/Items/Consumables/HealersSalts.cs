using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>Healer's Salts -- purified remedy residue from St Mungo's triage. Crafting material. Mod-original.</summary>
    public class HealersSalts : ModItem
    {
        public override void SetDefaults()
        { Item.width = 16; Item.height = 16; Item.maxStack = 999; Item.rare = ItemRarityID.LightRed; Item.value = Item.sellPrice(silver: 80); }
    }
}
