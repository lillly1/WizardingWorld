using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>
    /// Gaunt's Ring — a Horcrux containing the Resurrection Stone.
    /// Destroy all four Horcruxes to purify the ring and extract the Stone.
    ///
    /// "The stone had been cracked down the vertical line representing
    /// the Elder Wand. The Resurrection Stone had cracked." — Deathly Hallows
    ///
    /// Canon-faithful: The Resurrection Stone was set into the Gaunt family ring,
    /// which Voldemort turned into a Horcrux.
    /// </summary>
    public class GauntsRing : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<Common.Players.WizardPlayer>().hasGauntsRing = true;

            // Cursed ring — grants power but at a cost
            player.GetDamage(DamageClass.Generic) += 0.08f;
            player.statDefense -= 4;

            // Dark corruption: slowly drains life
            if (Main.GameUpdateCount % 120 == 0) // every 2 seconds
                player.statLife -= 1;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            int destroyed = Common.Systems.HorcruxHuntSystem.horcruxesDestroyed;

            tooltips.Add(new TooltipLine(Mod, "GauntCurse",
                Language.GetTextValue("Mods.WizardingWorld.Items.GauntsRing.Dynamic.GauntCurse")));
            tooltips.Add(new TooltipLine(Mod, "HorcruxProgress",
                Language.GetTextValue("Mods.WizardingWorld.Items.GauntsRing.Dynamic.HorcruxProgress", destroyed)));

            if (Common.Systems.HallowsSystem.resurrectionStoneAwakened)
            {
                tooltips.Add(new TooltipLine(Mod, "Awakened",
                    Language.GetTextValue("Mods.WizardingWorld.Items.GauntsRing.Dynamic.AwakenedStatus")));
            }
            else if (Common.Systems.HorcruxHuntSystem.AllCoreHorcruxesDestroyed)
            {
                tooltips.Add(new TooltipLine(Mod, "Purifiable",
                    Language.GetTextValue("Mods.WizardingWorld.Items.GauntsRing.Dynamic.PurifiableStatus")));
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "Locked",
                    Language.GetTextValue("Mods.WizardingWorld.Items.GauntsRing.Dynamic.LockedStatus")));
            }
        }
    }
}
