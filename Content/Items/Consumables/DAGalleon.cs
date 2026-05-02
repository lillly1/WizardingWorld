using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
    /// <summary>
    /// D.A. Galleon -- enchanted coin used by Dumbledore's Army for communication.
    /// Activates DAMeetingBuff (8 min) and serves as DA membership token.
    /// Canon-faithful: Hermione enchanted Galleons with a Protean Charm for DA meetings.
    /// </summary>
    public class DAGalleon : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.consumable = false; // reusable token
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 5);
            Item.UseSound = SoundID.Item4;
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<Content.Buffs.DAMeetingBuff>(), 60 * 60 * 8); // 8 minutes
            Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.DAGalleonUse"),
                new Microsoft.Xna.Framework.Color(220, 200, 100));
            return true;
        }
    }
}
