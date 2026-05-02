using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>
    /// Champion's Medallion -- crafted from boss materials.
    /// A powerful combat accessory representing wizard mastery.
    /// Previously named "Triwizard Cup" but renamed to avoid
    /// confusion with the actual Triwizard Tournament trophy.
    /// +15% all damage, +10% crit, +8 defense, +40 mana, +4 life regen.
    /// </summary>
    public class ChampionsMedallion : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(platinum: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Champion's reward — all-around excellence
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.GetCritChance(DamageClass.Generic) += 10;
            player.statDefense += 8;
            player.statManaMax2 += 40;
            player.lifeRegen += 4;
            player.moveSpeed += 0.10f;

            // Blue champion aura
            if (Main.rand.NextBool(8))
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.BlueTorch, 0f, -1f, 100, default, 0.7f);
                dust.noGravity = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 10)
                .AddIngredient(ModContent.ItemType<Consumables.GoldenEgg>(), 1)
                .AddIngredient(ModContent.ItemType<BasiliskFang>(), 5)
                .AddIngredient(ItemID.GoldBar, 30)
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddIngredient(ItemID.SoulofSight, 10)
                .AddIngredient(ItemID.SoulofFright, 10)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
