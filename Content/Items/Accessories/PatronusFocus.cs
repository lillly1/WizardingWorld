using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>
    /// Patronus Focus -- concentrates hope into a tangible ward against despair.
    /// Enhances Patronus duration and despair resistance.
    /// Crafted from Soul Ash + Azkaban materials.
    /// Mod-original: represents mastered anti-Dementor technique.
    /// </summary>
    public class PatronusFocus : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 15);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var wp = player.GetModPlayer<Common.Players.WizardPlayer>();

            // Constant anti-despair
            wp.RelieveDespair(0.003f);

            // Patronus enhancement
            if (wp.patronusActive)
            {
                // Extend Patronus timer (slow decay)
                if (wp.patronusTimer > 0 && Main.GameUpdateCount % 3 == 0)
                    wp.patronusTimer++;

                // Enhanced protection
                player.statDefense += 8;
                player.lifeRegen += 3;
                player.GetDamage(DamageClass.Generic) += 0.08f;
            }

            // Even without Patronus, provides hope
            player.statDefense += 4;
            player.endurance += 0.04f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.SoulAsh>(), 15)
                .AddIngredient(ModContent.ItemType<Consumables.DementorsShroud>(), 10)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 20)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
