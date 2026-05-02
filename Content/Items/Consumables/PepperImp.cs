using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Pepper Imp -- Honeydukes fire-breathing sweets.
    /// Grants fire immunity and a small damage boost.
    /// Canon-inspired: makes you breathe fire in the books.
    /// </summary>
    public class PepperImp : ModItem
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
            Item.value = Item.buyPrice(silver: 25);
            Item.buffType = BuffID.Inferno;
            Item.buffTime = 14400; // 4 minutes
            Item.UseSound = SoundID.Item2;
        }
    }
}
