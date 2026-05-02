using Terraria;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>Ravenclaw Feast Blessing -- wisdom and wit. +15% spell damage, +60 max mana, +mana regen.</summary>
    public class RavenclawFeastBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.15f;
            player.statManaMax2 += 60;
            player.manaRegen += 25;
        }
    }
}
