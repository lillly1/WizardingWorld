using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>Gryffindor Feast Blessing -- courage and strength. +12% damage, +8 defense.</summary>
    public class GryffindorFeastBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.12f;
            player.statDefense += 8;
            player.lifeRegen += 2;
        }
    }
}
