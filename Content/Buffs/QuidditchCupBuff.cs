using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>
    /// Quidditch Cup Victory -- awarded to the winning house.
    /// +10% all damage, +10% move speed, +lucky, 15 minutes.
    /// </summary>
    public class QuidditchCupBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.10f;
            player.moveSpeed += 0.10f;
            player.luck += 0.2f;
        }
    }
}
