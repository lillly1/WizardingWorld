using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Soul Ash -- residue of despair left behind when Azkaban's dark wards are broken.
    /// Crafting material for anti-despair gear and Patronus-enhancing items.
    /// Mod-original material: represents purified remnants of Azkaban's dark magic.
    /// </summary>
    public class SoulAsh : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(silver: 50);
        }
    }
}
