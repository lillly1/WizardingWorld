using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Prophecy Dust -- residue from handled prophecy orbs.
    /// Crafting material for Order-aligned gear.
    /// Mod-original: represents the ethereal substance of shattered prophecies.
    /// </summary>
    public class ProphecyDust : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(silver: 75);
        }
    }
}
