using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Castle Ward Thread -- crafting material dropped from ward defense activities.
    /// Used to craft the Defender's Signet accessory.
    /// Mod-original content tied to Hogwarts ward defense.
    /// </summary>
    public class CastleWardThread : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(silver: 90);
        }
    }
}
