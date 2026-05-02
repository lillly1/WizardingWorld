using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>
    /// Shopping Day -- Diagon Alley commerce buff.
    /// Increased luck, treasure sense, reduced NPC prices, coin attraction.
    /// 10 minutes. Canon-inspired: the excitement of a Diagon Alley shopping trip.
    /// </summary>
    public class ShoppingDayBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.luck += 0.15f;
            player.findTreasure = true;
            player.goldRing = true;
            player.discountAvailable = true; // 20% shop discount
        }
    }
}
