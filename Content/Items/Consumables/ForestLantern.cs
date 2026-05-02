using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Forest Expedition Lantern -- Hagrid's enchanted lantern for deep forest expeditions.
    /// Reusable. Post-Basilisk, nighttime, in Forbidden Forest biome.
    /// </summary>
    public class ForestLantern : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
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
            if (Common.Systems.ForestExpeditionSystem.CanUnlock())
            {
                Common.Systems.ForestExpeditionSystem.Unlock();
                return true;
            }
            if (Common.Systems.ForestExpeditionSystem.CanStart(player))
            {
                Common.Systems.ForestExpeditionSystem.StartMission(player);
                return true;
            }
            Main.NewText(Common.Systems.ForestExpeditionSystem.GetStatusText(), new Color(80, 120, 80));
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 3)
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 8)
                .AddIngredient(ItemID.Torch, 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
