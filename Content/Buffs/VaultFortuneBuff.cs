using Terraria; using Terraria.ModLoader;
namespace WizardingWorld.Content.Buffs
{
    /// <summary>Vault Fortune -- Gringotts reward. +0.2 luck, coin attraction, +5% all damage. 8 minutes.</summary>
    public class VaultFortuneBuff : ModBuff
    {
        public override void SetStaticDefaults() { Main.buffNoTimeDisplay[Type] = false; Main.debuff[Type] = false; }
        public override void Update(Player player, ref int buffIndex)
        {
            player.luck += 0.2f;
            player.goldRing = true;
            player.GetDamage(DamageClass.Generic) += 0.05f;
        }
    }
}
