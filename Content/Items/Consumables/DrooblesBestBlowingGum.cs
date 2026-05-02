using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Drooble's Best Blowing Gum -- Honeydukes gum that creates blue bubbles.
    /// Grants movement speed and jump boost.
    /// Canon-inspired: Dumbledore enjoys them, mentioned as a Chocolate Frog card favorite.
    /// </summary>
    public class DrooblesBestBlowingGum : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.consumable = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 12);
            Item.buffType = BuffID.Swiftness;
            Item.buffTime = 21600; // 6 minutes
            Item.UseSound = SoundID.Item2;
        }
    }
}
