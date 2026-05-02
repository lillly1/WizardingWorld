using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>
    /// Order of the Phoenix Badge -- earned through Ministry missions.
    /// Grants anti-dark protection and spell power.
    /// +8% spell damage, +5 defense, reduced Dark Curse duration, despair resistance.
    /// Mod-original: represents Order membership and commitment to the fight against dark magic.
    /// </summary>
    public class OrderBadge : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 12);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<Content.DamageClasses.SpellDamage>()) += 0.08f;
            player.statDefense += 5;
            player.endurance += 0.04f;

            // Reduce Dark Curse debuff duration
            player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.DarkCurseDebuff>()] = true;

            // Passive despair resistance
            player.GetModPlayer<Common.Players.WizardPlayer>().RelieveDespair(0.002f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.ProphecyDust>(), 15)
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 20)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
