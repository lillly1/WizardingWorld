using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>Moonward Pendant -- Shrieking Shack reward accessory. +5 def, +4% endurance, Darkness/Jinxed immunity, +speed at night. Mod-original.</summary>
    public class MoonwardPendant : ModItem
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
            player.statDefense += 5;
            player.endurance += 0.04f;
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.JinxedDebuff>()] = true;

            if (!Main.dayTime)
                player.moveSpeed += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.MoonsilverThread>(), 20)
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
