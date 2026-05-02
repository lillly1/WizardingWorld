using Terraria; using Terraria.ModLoader;
namespace WizardingWorld.Content.Buffs
{
    /// <summary>Safehouse Resolve -- Grimmauld reward. +6 defense, +4 life regen, Jinxed immunity, -100 aggro. 10 min.</summary>
    public class SafehouseResolveBuff : ModBuff
    {
        public override void SetStaticDefaults() { Main.buffNoTimeDisplay[Type] = false; Main.debuff[Type] = false; }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 6;
            player.lifeRegen += 4;
            player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.JinxedDebuff>()] = true;
            player.aggro -= 100;
        }
    }
}
