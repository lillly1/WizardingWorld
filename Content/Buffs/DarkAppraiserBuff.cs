using Terraria; using Terraria.ModLoader;
namespace WizardingWorld.Content.Buffs
{
    /// <summary>Dark Appraiser -- Knockturn contract reward. Night vision, +6% damage, danger sense. 8 min.</summary>
    public class DarkAppraiserBuff : ModBuff
    {
        public override void SetStaticDefaults() { Main.buffNoTimeDisplay[Type] = false; Main.debuff[Type] = false; }
        public override void Update(Player player, ref int buffIndex)
        {
            player.nightVision = true;
            player.GetDamage(DamageClass.Generic) += 0.06f;
            player.dangerSense = true;
        }
    }
}
