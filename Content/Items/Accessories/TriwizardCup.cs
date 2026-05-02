using System;
using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>
    /// Legacy Triwizard Cup stub.
    /// Renamed to Champion's Medallion to free the "Triwizard Cup" name
    /// for the actual tournament trophy.
    /// Old saves auto-convert to ChampionsMedallion.
    /// </summary>
    [Obsolete("Renamed to ChampionsMedallion")]
    public class TriwizardCup : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 1;
            Item.rare = Terraria.ID.ItemRarityID.Yellow;
            Item.value = 0;
        }

        public override bool CanUseItem(Player player) => false;

        public override void UpdateInventory(Player player)
        {
            Item.SetDefaults(ModContent.ItemType<ChampionsMedallion>());
        }
    }
}
