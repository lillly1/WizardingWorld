using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Leaky Cauldron Token -- the gateway into Diagon Alley.
    /// Reusable. Grants Shopping Day buff and shows available Diagon Alley services.
    /// Gated at post-Basilisk (same as Gringotts access).
    /// Canon-inspired: the Leaky Cauldron is the hidden pub entrance to Diagon Alley.
    /// </summary>
    public class LeakyCauldronToken : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.consumable = false;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 3);
            Item.UseSound = SoundID.Item4;
        }

        public override bool CanUseItem(Player player)
        {
            return Common.Systems.DownedBossSystem.downedBasilisk;
        }

        public override bool? UseItem(Player player)
        {
            Common.Systems.DiagonAlleySystem.EnterDiagonAlley(player);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 3)
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
