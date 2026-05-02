using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>
    /// D.A. Meeting Buff -- coordination buff from Dumbledore's Army gatherings.
    /// +5% all damage, +4 defense, danger sense, +3 life regen. 8 minutes.
    /// Mod-original: represents the morale and training from DA meetings.
    /// </summary>
    public class DAMeetingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.statDefense += 4;
            player.dangerSense = true;
            player.lifeRegen += 3;
        }
    }
}
