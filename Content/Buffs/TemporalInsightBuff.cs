using Terraria; using Terraria.ModLoader;
namespace WizardingWorld.Content.Buffs
{
    /// <summary>Temporal Insight -- Time Chamber reward. +8% all damage, +5% speed, danger sense. 8 minutes.</summary>
    public class TemporalInsightBuff : ModBuff
    {
        public override void SetStaticDefaults() { Main.buffNoTimeDisplay[Type] = false; Main.debuff[Type] = false; }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.08f;
            player.moveSpeed += 0.05f;
            player.dangerSense = true;
        }
    }
}
