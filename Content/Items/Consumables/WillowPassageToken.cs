using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Willow Passage Token -- reusable access item for the Shrieking Shack missions.
    /// First use unlocks, subsequent uses start missions. Post-Fenrir, nighttime.
    /// </summary>
    public class WillowPassageToken : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.consumable = false;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 5);
            Item.UseSound = SoundID.Item4;
        }

        public override bool? UseItem(Player player)
        {
            if (Common.Systems.ShriekingShackSystem.CanUnlock())
            {
                Common.Systems.ShriekingShackSystem.Unlock();
                return true;
            }
            if (Common.Systems.ShriekingShackSystem.CanStart())
            {
                Common.Systems.ShriekingShackSystem.StartMission(player);
                return true;
            }
            Main.NewText(Common.Systems.ShriekingShackSystem.GetStatusText(), new Color(140, 130, 160));
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 10)
                .AddIngredient(ItemID.Chain, 3)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
