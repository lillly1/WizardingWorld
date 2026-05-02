using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Sugar Quill -- Honeydukes candy disguised as a quill.
    /// Grants mana regeneration boost.
    /// Canon-inspired: students suck on them during class.
    /// </summary>
    public class SugarQuill : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.consumable = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 10);
            Item.buffType = BuffID.ManaRegeneration;
            Item.buffTime = 21600; // 6 minutes
            Item.UseSound = SoundID.Item2;
        }
    }
}
