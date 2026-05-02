using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>
    /// Defender's Signet -- reward accessory from castle ward defense.
    /// +6 defense, +5% endurance, Jinxed/Confused immunity, +0.05 moveSpeed.
    /// Crafted from CastleWardThread + EssenceOfMagic + Souls at EnchantingTable.
    /// Mod-original: represents earned trust through defending Hogwarts.
    /// </summary>
    public class DefendersSignet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 8);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 6;
            player.endurance += 0.05f;
            player.moveSpeed += 0.05f;
            player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.JinxedDebuff>()] = true;
            player.buffImmune[BuffID.Confused] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Content.Items.Consumables.CastleWardThread>(), 20)
                .AddIngredient(ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 15)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddTile<Content.Tiles.EnchantingTable>()
                .Register();
        }
    }
}
