using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Gringotts Vault Key -- activates a sanctioned vault retrieval mission.
    /// Reusable. Unlocks vault access on first use (post-Basilisk).
    /// Mod-original: inspired by Gringotts vault key system.
    /// </summary>
    public class GringottsVaultKey : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.consumable = false;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 5);
            Item.UseSound = SoundID.Item4;
        }

        public override bool? UseItem(Player player)
        {
            if (Common.Systems.GringottsVaultSystem.CanUnlock())
            {
                Common.Systems.GringottsVaultSystem.Unlock();
                return true;
            }
            if (Common.Systems.GringottsVaultSystem.CanStart())
            {
                Common.Systems.GringottsVaultSystem.StartMission(player);
                return true;
            }
            Main.NewText(Common.Systems.GringottsVaultSystem.GetStatusText(),
                new Color(200, 180, 100));
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 8)
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 10)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
