using Terraria; using Terraria.ModLoader;
namespace WizardingWorld.Content.Buffs
{
    /// <summary>Triage Resolved -- St Mungo's reward. +8 life regen, +5 defense, venom immunity. 10 min.</summary>
    public class TriageResolvedBuff : ModBuff
    {
        public override void SetStaticDefaults() { Main.buffNoTimeDisplay[Type] = false; Main.debuff[Type] = false; }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 8;
            player.statDefense += 5;
            player.buffImmune[Terraria.ID.BuffID.Venom] = true;
        }
    }
}
