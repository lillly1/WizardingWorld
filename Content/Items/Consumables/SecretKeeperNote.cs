using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Secret Keeper's Note -- reveals Number Twelve, Grimmauld Place.
    /// Reusable. First use reveals safehouse; subsequent uses start maintenance.
    /// Gated: post-Dementor King + at least 1 Department mission completed.
    /// Canon-inspired: the address is revealed by a Secret Keeper.
    /// </summary>
    public class SecretKeeperNote : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20; Item.height = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 45; Item.useAnimation = 45;
            Item.consumable = false;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 8);
            Item.UseSound = SoundID.Item4;
        }

        public override bool? UseItem(Player player)
        {
            if (Common.Systems.GrimmauldPlaceSystem.CanReveal())
            {
                Common.Systems.GrimmauldPlaceSystem.RevealSafehouse();
                return true;
            }
            if (Common.Systems.GrimmauldPlaceSystem.CanStart())
            {
                Common.Systems.GrimmauldPlaceSystem.StartMission(player);
                return true;
            }
            Main.NewText(Common.Systems.GrimmauldPlaceSystem.GetStatusText(),
                new Color(140, 130, 120));
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 15)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
