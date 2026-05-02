using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>Hufflepuff Feast Blessing -- loyalty and endurance. +10 defense, +6 life regen, +8% DR.</summary>
    public class HufflepuffFeastBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 10;
            player.lifeRegen += 6;
            player.endurance += 0.08f;
        }
    }
}
