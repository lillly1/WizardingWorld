using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Knockturn Pass -- shady access token for Knockturn Alley.
    /// Unlocks Knockturn on first use (post-Bellatrix + visited Diagon).
    /// Starts containment contracts on subsequent uses.
    /// Mod-original: the dark side-street adjacent to Diagon Alley.
    /// </summary>
    public class KnockturnPass : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; Item.height = 18;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 45; Item.useAnimation = 45;
            Item.consumable = false;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 5);
            Item.UseSound = SoundID.Item4;
        }

        public override bool? UseItem(Player player)
        {
            if (Common.Systems.KnockturnAlleySystem.CanUnlock())
            {
                Common.Systems.KnockturnAlleySystem.Unlock();
                return true;
            }
            if (Common.Systems.KnockturnAlleySystem.CanStart())
            {
                Common.Systems.KnockturnAlleySystem.StartMission(player);
                return true;
            }
            Main.NewText(Common.Systems.KnockturnAlleySystem.GetStatusText(),
                new Color(140, 120, 160));
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 10)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
