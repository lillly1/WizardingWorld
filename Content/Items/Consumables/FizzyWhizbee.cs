using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Fizzing Whizbee -- Honeydukes levitation sherbet balls.
    /// Grants a brief low-gravity jump boost.
    /// Canon-inspired: mentioned in the books as a popular Honeydukes sweet.
    /// </summary>
    public class FizzyWhizbee : ModItem
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
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 15);
            Item.buffType = BuffID.Featherfall;
            Item.buffTime = 18000; // 5 minutes
            Item.UseSound = SoundID.Item2;
        }
    }
}
