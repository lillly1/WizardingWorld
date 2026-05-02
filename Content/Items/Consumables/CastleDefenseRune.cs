using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>Castle Defense Rune -- reward material from the Battle of Hogwarts siege event.</summary>
    public class CastleDefenseRune : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16; Item.height = 16;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(silver: 95);
        }
    }
}
