using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>Order Commendation -- awarded for completing a Ministry mission. +10% spell dmg, +4 def, +3 life regen.</summary>
    public class OrderCommendationBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(ModContent.GetInstance<Content.DamageClasses.SpellDamage>()) += 0.10f;
            player.statDefense += 4;
            player.lifeRegen += 3;
        }
    }
}
