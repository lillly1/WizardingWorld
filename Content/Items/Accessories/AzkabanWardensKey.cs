using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>
    /// Azkaban Warden's Key — dropped during the Azkaban's Despair event.
    /// Grants resistance to despair effects and boosts Patronus power.
    ///
    /// "Even the darkest night will end and the sun will rise."
    /// </summary>
    public class AzkabanWardensKey : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 12);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
            wp.hasAzkabanWardensKey = true;
            wp.RelieveDespair(0.004f);

            // Anti-despair: immune to Darkness and life drain
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.DarkCurseDebuff>()] = true;

            // Patronus boost
            if (wp.patronusActive)
            {
                player.GetDamage(DamageClass.Generic) += 0.12f;
                player.lifeRegen += 4;
            }

            // General defensive boost
            player.statDefense += 6;
            player.endurance += 0.05f;
        }
    }
}
