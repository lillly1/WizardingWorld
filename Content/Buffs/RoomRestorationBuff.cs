using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
    /// <summary>
    /// Room of Requirement: Restoration manifestation.
    /// Relieves despair, boosts ward effectiveness, and enhances Reparo synergy.
    /// Active when the Room detects the player has been fighting dark/despair content.
    /// Mod-original: the Room provides what is needed.
    /// </summary>
    public class RoomRestorationBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var wp = player.GetModPlayer<Common.Players.WizardPlayer>();

            // Despair relief -- the Room soothes dark aftereffects
            wp.RelieveDespair(0.005f);

            // Protective restoration
            player.lifeRegen += 6;
            player.statDefense += 6;
            player.endurance += 0.06f;

            // Ward/Reparo synergy -- extends active ward buffs
            ExtendBuff(player, ModContent.BuffType<WardOfHopeBuff>(), 1);

            // Calming light
            Terraria.Lighting.AddLight(player.Center, 0.3f, 0.35f, 0.45f);
        }

        private static void ExtendBuff(Player player, int buffType, int amount)
        {
            int index = player.FindBuffIndex(buffType);
            if (index >= 0)
                player.buffTime[index] = System.Math.Min(player.buffTime[index] + amount, 60 * 60 * 5);
        }
    }
}
