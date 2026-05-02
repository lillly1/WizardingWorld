using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>
    /// Triwizard Cup -- the actual tournament grand prize.
    /// Earned by completing all three tasks of the Triwizard Tournament.
    /// Not craftable -- awarded by the tournament system.
    /// +12% all damage, +8% crit, +6 defense, +4 life regen.
    /// </summary>
    public class ChampionsTrophy : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.12f;
            player.GetCritChance(DamageClass.Generic) += 8;
            player.statDefense += 6;
            player.lifeRegen += 4;
        }
    }
}
