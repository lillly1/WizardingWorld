using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>
    /// Castle Defense Buff -- active during the Battle of Hogwarts.
    /// +8 defense representing the animated castle statues and armor defending alongside the player.
    /// Mod-original: inspired by McGonagall's Piertotum Locomotor from Deathly Hallows.
    /// </summary>
    public class CastleDefenseBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 8;
        }
    }
}
