using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// St Mungo's Visitor Pass -- access to the hidden hospital.
    /// Reusable. Unlocks hospital (post-Horntail), starts triage missions.
    /// Canon-inspired: St Mungo's is concealed behind a derelict department store.
    /// </summary>
    public class StMungosPass : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20; Item.height = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 45; Item.useAnimation = 45;
            Item.consumable = false;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 5);
            Item.UseSound = SoundID.Item4;
        }

        public override bool? UseItem(Player player)
        {
            if (Common.Systems.StMungosTriageSystem.CanUnlock())
            {
                Common.Systems.StMungosTriageSystem.Unlock();
                return true;
            }
            if (Common.Systems.StMungosTriageSystem.CanStart())
            {
                Common.Systems.StMungosTriageSystem.StartMission(player);
                return true;
            }
            Main.NewText(Common.Systems.StMungosTriageSystem.GetStatusText(),
                new Color(180, 200, 180));
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 5)
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 10)
                .AddIngredient(ItemID.HealingPotion, 3)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
