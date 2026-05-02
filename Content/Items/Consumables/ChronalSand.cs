using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>Chronal Sand -- time-infused residue from stabilized hourglasses. Crafting material. Mod-original.</summary>
    public class ChronalSand : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16; Item.height = 16;
            Item.maxStack = 999; Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(silver: 80);
        }
    }
}
