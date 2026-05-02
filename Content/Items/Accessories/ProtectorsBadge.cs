using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>
    /// Protector's Badge -- Hogwarts Defender reward.
    /// +8 defense, +6% endurance, Jinxed/DarkCurse immunity, +4 life regen, despair relief.
    /// Crafted from Battle of Hogwarts materials.
    /// </summary>
    public class ProtectorsBadge : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 8);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 8;
            player.endurance += 0.06f;
            player.lifeRegen += 4;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.DarkCurseDebuff>()] = true;

            // Despair relief
            var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
            if (wp.despair > 0f)
                wp.despair = System.Math.Max(0f, wp.despair - 0.002f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.CastleDefenseRune>(), 25)
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 20)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
