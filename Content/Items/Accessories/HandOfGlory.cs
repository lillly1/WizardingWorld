using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>
    /// Hand of Glory -- a withered hand that gives light only to the holder.
    /// Grants night vision, spelunker sense, and stealth (aggro reduction).
    /// In darkness, also grants +6% damage and +4 defense.
    /// Canon-faithful: "gives light only to the holder" -- used by Draco in Half-Blood Prince.
    /// </summary>
    public class HandOfGlory : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22; Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 15);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Light only to the holder
            player.nightVision = true;
            player.findTreasure = true;
            player.aggro -= 200;

            // Stronger in darkness
            if (!Main.dayTime || player.ZoneDungeon || player.ZoneCorrupt || player.ZoneCrimson)
            {
                player.GetDamage(DamageClass.Generic) += 0.06f;
                player.statDefense += 4;
            }

            // Subtle eerie light near the player only
            Terraria.Lighting.AddLight(player.Center, 0.15f, 0.1f, 0.2f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.CursedResidue>(), 20)
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
