using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Lake Rescue Token -- collected during Task 2 (Great Lake Trial).
    /// Dropped by Merfolk and Grindylows during the trial.
    /// Collect 3 to complete the task.
    /// Mod-original: represents rescued hostage tokens from the lake bed.
    /// </summary>
    public class LakeRescueToken : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 10;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = 0;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool OnPickup(Player player)
        {
            Common.Systems.TriwizardTournamentSystem.OnRescueTokenCollected(player);
            return true; // consume into inventory
        }
    }
}
