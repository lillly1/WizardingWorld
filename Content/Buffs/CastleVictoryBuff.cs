using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>Castle Victory -- granted after winning the Battle of Hogwarts. +12% all damage, +10 defense, +6 life regen, +0.1 luck. 15 minutes.</summary>
    public class CastleVictoryBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.12f;
            player.statDefense += 10;
            player.lifeRegen += 6;
            player.luck += 0.1f;
        }
    }
}
