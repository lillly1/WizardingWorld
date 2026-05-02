using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>Triwizard Champion -- 20 minutes of glory. +15% all damage, +10% speed, +luck.</summary>
    public class TriwizardChampionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.moveSpeed += 0.10f;
            player.luck += 0.3f;
        }
    }
}
