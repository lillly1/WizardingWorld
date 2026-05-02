using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.Buffs
{
    /// <summary>Moonward Buff -- Shrieking Shack reward. +6 defense, +4 life regen, Darkness/Jinxed immunity, +5% speed. 10 min.</summary>
    public class MoonwardBuff : ModBuff
    {
        public override void SetStaticDefaults() { Main.buffNoTimeDisplay[Type] = false; Main.debuff[Type] = false; }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 6;
            player.lifeRegen += 4;
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.JinxedDebuff>()] = true;
            player.moveSpeed += 0.05f;
        }
    }
}
