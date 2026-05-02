using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>Order Signet -- concealment and resistance. +5 def, -200 aggro, debuff resistance (Jinxed/Confused immunity), +4% endurance. Mod-original.</summary>
    public class OrderSignet : ModItem
    {
        public override void SetDefaults()
        { Item.width = 22; Item.height = 22; Item.accessory = true; Item.rare = ItemRarityID.LightPurple; Item.value = Item.sellPrice(gold: 14); }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 5;
            player.aggro -= 200;
            player.endurance += 0.04f;
            player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.JinxedDebuff>()] = true;
            player.buffImmune[Terraria.ID.BuffID.Confused] = true;
            player.GetModPlayer<Common.Players.WizardPlayer>().RelieveDespair(0.002f);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.ConcealmentThread>(), 20)
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
