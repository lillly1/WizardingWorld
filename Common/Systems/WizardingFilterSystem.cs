using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
    /// <summary>
    /// Registers screen shader filters for scene effects.
    /// Each filter key must match the string passed to ManageSpecialBiomeVisuals
    /// in WizardingSceneEffects.cs. Without registration here, those calls are no-ops.
    ///
    /// Uses vanilla "FilterMiniTower" shader pass (a simple color overlay) so no
    /// custom .fx files are needed. Color and opacity are set per filter.
    /// </summary>
    public class WizardingFilterSystem : ModSystem
    {
        public override void Load()
        {
            // Client-side only — dedicated servers have no rendering
            if (Main.dedServ)
                return;

            // Azkaban Despair: cold desaturated blue-gray, heavy oppression
            Filters.Scene["WizardingWorld:AzkabanDespair"] = new Filter(
                new ScreenShaderData("FilterMiniTower")
                    .UseColor(0.25f, 0.25f, 0.45f)
                    .UseOpacity(0.35f),
                EffectPriority.High);

            // Battle of Hogwarts: dark reddish war atmosphere
            Filters.Scene["WizardingWorld:BattleOfHogwarts"] = new Filter(
                new ScreenShaderData("FilterMiniTower")
                    .UseColor(0.5f, 0.2f, 0.15f)
                    .UseOpacity(0.3f),
                EffectPriority.High);

            // Gringotts Descent: dim underground gold-tinted darkness
            Filters.Scene["WizardingWorld:GringottsDescent"] = new Filter(
                new ScreenShaderData("FilterMiniTower")
                    .UseColor(0.35f, 0.3f, 0.15f)
                    .UseOpacity(0.25f),
                EffectPriority.Medium);

            // Knockturn Alley: murky oppressive green-gray
            Filters.Scene["WizardingWorld:KnockturnAlley"] = new Filter(
                new ScreenShaderData("FilterMiniTower")
                    .UseColor(0.2f, 0.25f, 0.2f)
                    .UseOpacity(0.3f),
                EffectPriority.Medium);

            // Shrieking Shack: eerie ghostly blue-white
            Filters.Scene["WizardingWorld:ShriekingShack"] = new Filter(
                new ScreenShaderData("FilterMiniTower")
                    .UseColor(0.3f, 0.3f, 0.5f)
                    .UseOpacity(0.2f),
                EffectPriority.Medium);
        }
    }

    /// <summary>
    /// Spawns ambient particles during active scene effects to reinforce
    /// atmosphere beyond just screen tinting. Runs per-tick on client only.
    /// </summary>
    public class WizardingAmbientSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            if (Main.dedServ || Main.netMode == NetmodeID.Server)
                return;

            Player player = Main.LocalPlayer;
            if (player == null || !player.active || player.dead)
                return;

            // Azkaban: drifting cold blue particles + frost motes
            if (AzkabanDespairEvent.eventActive && Main.rand.NextBool(4))
            {
                Vector2 pos = player.Center + Main.rand.NextVector2Circular(400f, 300f);
                Dust d = Dust.NewDustDirect(pos, 2, 2, DustID.IceTorch, 0f, -0.5f, 180, default, 0.4f);
                d.noGravity = true;
                d.velocity *= 0.3f;
            }

            // Battle of Hogwarts: ember/ash particles + occasional red spark
            if (BattleOfHogwartsSystem.battleActive)
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 pos = player.Center + Main.rand.NextVector2Circular(500f, 400f);
                    Dust d = Dust.NewDustDirect(pos, 2, 2, DustID.Torch, 0f, -1f, 150, default, 0.5f);
                    d.noGravity = true;
                    d.velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-1.5f, -0.3f));
                }
                // Occasional dark smoke
                if (Main.rand.NextBool(8))
                {
                    Vector2 pos = player.Center + Main.rand.NextVector2Circular(600f, 400f);
                    Dust d = Dust.NewDustDirect(pos, 4, 4, DustID.Smoke, 0f, -0.3f, 200, default, 1.2f);
                    d.noGravity = true;
                }
            }

            // Gringotts: gold dust motes drifting upward in vault
            if (GringottsVaultSystem.missionActive && Main.rand.NextBool(6))
            {
                Vector2 pos = player.Center + Main.rand.NextVector2Circular(300f, 200f);
                Dust d = Dust.NewDustDirect(pos, 2, 2, DustID.GoldCoin, 0f, -0.2f, 100, default, 0.3f);
                d.noGravity = true;
                d.velocity *= 0.2f;
            }

            // Knockturn Alley: sinister dark wisps
            if (KnockturnAlleySystem.missionActive && Main.rand.NextBool(6))
            {
                Vector2 pos = player.Center + Main.rand.NextVector2Circular(350f, 250f);
                Dust d = Dust.NewDustDirect(pos, 2, 2, DustID.Shadowflame, 0f, 0f, 200, default, 0.3f);
                d.noGravity = true;
                d.velocity = Main.rand.NextVector2Circular(0.3f, 0.3f);
            }

            // Shrieking Shack: ghostly wraith wisps, stronger during full moon
            if (ShriekingShackSystem.missionActive && Main.rand.NextBool(Main.moonPhase == 0 ? 3 : 7))
            {
                Vector2 pos = player.Center + Main.rand.NextVector2Circular(300f, 200f);
                Dust d = Dust.NewDustDirect(pos, 2, 2, DustID.Wraith, 0f, -0.4f, 150, default, 0.5f);
                d.noGravity = true;
                d.velocity *= 0.3f;
            }
        }
    }
}
