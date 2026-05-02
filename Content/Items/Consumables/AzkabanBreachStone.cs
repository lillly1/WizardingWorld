using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Azkaban Breach Stone — triggers the Azkaban's Despair event.
    /// A fragment of Azkaban's anti-apparition ward, cracked by dark magic.
    /// </summary>
    public class AzkabanBreachStone : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.consumable = true;
            Item.maxStack = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 5);
            Item.UseSound = SoundID.Item119;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && NPC.downedGolemBoss && !Common.Systems.AzkabanDespairEvent.eventActive;
        }

        public override bool? UseItem(Player player)
        {
            Common.Systems.AzkabanDespairEvent.StartEvent();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DementorsShroud>(), 10)
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 15)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
