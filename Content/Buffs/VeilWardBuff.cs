using Terraria; using Terraria.ModLoader;
namespace WizardingWorld.Content.Buffs
{
    /// <summary>Veil Ward -- Death Chamber reward. +8 defense, +6% endurance, despair resistance, darkness immunity. 10 minutes.</summary>
    public class VeilWardBuff : ModBuff
    {
        public override void SetStaticDefaults() { Main.buffNoTimeDisplay[Type] = false; Main.debuff[Type] = false; }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 8;
            player.endurance += 0.06f;
            player.buffImmune[Terraria.ID.BuffID.Darkness] = true;
            player.buffImmune[Terraria.ID.BuffID.Blackout] = true;
            player.GetModPlayer<Common.Players.WizardPlayer>().RelieveDespair(0.003f);
        }
    }
}
