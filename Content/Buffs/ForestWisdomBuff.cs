using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>Forest Wisdom -- expedition reward. +6% damage, danger sense, night vision, +4 life regen. 10 min.</summary>
    public class ForestWisdomBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.06f;
            player.dangerSense = true;
            player.nightVision = true;
            player.lifeRegen += 4;
        }
    }
}
