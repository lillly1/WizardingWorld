using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>Slytherin Feast Blessing -- cunning and ambition. +10% crit, +8% speed, +5% life steal on crits.</summary>
    public class SlytherinFeastBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetCritChance(DamageClass.Generic) += 10;
            player.moveSpeed += 0.08f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
        }
    }
}
