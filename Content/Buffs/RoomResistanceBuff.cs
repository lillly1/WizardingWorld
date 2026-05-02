using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>
    /// Room of Requirement: Resistance HQ manifestation.
    /// +10 defense, +8% endurance, +6 life regen, Jinxed/DarkCurse immunity. 3 minutes.
    /// Active when the Room detects the player is a DA member who has defended the wards.
    /// Mod-original: the Room becomes a resistance headquarters.
    /// </summary>
    public class RoomResistanceBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 10;
            player.endurance += 0.08f;
            player.lifeRegen += 6;
            player.buffImmune[ModContent.BuffType<Debuffs.JinxedDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<Debuffs.DarkCurseDebuff>()] = true;

            // Warm resistance glow
            Terraria.Lighting.AddLight(player.Center, 0.35f, 0.3f, 0.2f);
        }
    }
}
