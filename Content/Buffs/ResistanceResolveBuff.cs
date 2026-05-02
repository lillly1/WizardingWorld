using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>
    /// Resistance Resolve Buff -- ward defense reward.
    /// +8 defense, +6% endurance, Jinxed immunity, +4 life regen, +0.05 luck. 10 minutes.
    /// Mod-original: earned through defending Hogwarts wards.
    /// </summary>
    public class ResistanceResolveBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 8;
            player.endurance += 0.06f;
            player.buffImmune[ModContent.BuffType<Debuffs.JinxedDebuff>()] = true;
            player.lifeRegen += 4;
            player.luck += 0.05f;
        }
    }
}
