using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// Ministry Visitor Badge -- grants access to the Department of Mysteries mission.
    /// Reusable. Unlocks mission on first use (post-Bellatrix), starts mission on subsequent uses.
    /// Mod-original: inspired by the Ministry visitor entrance.
    /// </summary>
    public class MinistryVisitorBadge : ModItem
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
            Item.value = Item.buyPrice(gold: 8);
            Item.UseSound = SoundID.Item4;
        }

        public override bool? UseItem(Player player)
        {
            if (Common.Systems.ProphecyMissionSystem.CanUnlock())
            {
                Common.Systems.ProphecyMissionSystem.UnlockMission();
                return true;
            }

            // Check for chamber unlocks
            if (Common.Systems.TimeChamberSystem.CanUnlock())
            {
                Common.Systems.TimeChamberSystem.Unlock();
                return true;
            }
            if (Common.Systems.DeathChamberSystem.CanUnlock())
            {
                Common.Systems.DeathChamberSystem.Unlock();
                return true;
            }

            if (Common.Systems.ProphecyMissionSystem.missionActive)
            {
                Main.NewText(Common.Systems.ProphecyMissionSystem.GetStatusText(),
                    new Color(200, 180, 255));
                return true;
            }

            // Start chamber missions in rotation: Prophecy -> Time -> Death
            if (Common.Systems.TimeChamberSystem.CanStart() && Common.Systems.ProphecyMissionSystem.missionsCompleted > Common.Systems.TimeChamberSystem.runsCompleted)
            {
                Common.Systems.TimeChamberSystem.StartMission(player);
                return true;
            }
            if (Common.Systems.DeathChamberSystem.CanStart() && Common.Systems.TimeChamberSystem.runsCompleted > Common.Systems.DeathChamberSystem.runsCompleted)
            {
                Common.Systems.DeathChamberSystem.StartMission(player);
                return true;
            }

            if (Common.Systems.ProphecyMissionSystem.CanStartMission())
            {
                Common.Systems.ProphecyMissionSystem.StartMission(player);
                return true;
            }

            Main.NewText(Common.Systems.ProphecyMissionSystem.GetStatusText(),
                new Color(200, 200, 200));
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 5)
                .AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 15)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddTile<Tiles.EnchantingTable>()
                .Register();
        }
    }
}
