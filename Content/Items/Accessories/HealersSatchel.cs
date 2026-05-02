using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.Items.Accessories
{
    /// <summary>
    /// Healer's Satchel -- St Mungo's triage reward.
    /// +6 life regen, +5% endurance, debuff resistance (Venom/Confused immunity),
    /// reduced potion cooldown, passive despair relief.
    /// Supportive, not DPS. Mod-original.
    /// </summary>
    public class HealersSatchel : ModItem
    {
        public override void SetDefaults()
        { Item.width = 24; Item.height = 24; Item.accessory = true; Item.rare = ItemRarityID.LightRed; Item.value = Item.sellPrice(gold: 12); }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += 6;
            player.endurance += 0.05f;
            player.buffImmune[BuffID.Venom] = true;
            player.buffImmune[BuffID.Confused] = true;
            player.pStone = true; // Philosopher's Stone effect: reduced potion cooldown
            player.GetModPlayer<Common.Players.WizardPlayer>().RelieveDespair(0.002f);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Consumables.HealersSalts>(), 20)
                .AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
                .AddIngredient(ItemID.LifeCrystal, 1)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
