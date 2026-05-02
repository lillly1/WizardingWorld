using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Acid Pop -- Honeydukes lollipop that burns a hole in your tongue.
    /// Grants armor penetration and minor damage boost.
    /// Canon-inspired: dangerous candy from Honeydukes.
    /// </summary>
    public class AcidPop : ModItem
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
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(silver: 20);
            Item.buffType = BuffID.Rage;
            Item.buffTime = 14400; // 4 minutes
            Item.UseSound = SoundID.Item2;
        }
    }
}
