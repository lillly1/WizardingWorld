using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
    /// <summary>
    /// Scene effects for key Wizarding World locations and events.
    /// Each provides ambient atmosphere (music, screen tint) tied to
    /// existing system state fields — no new game logic, just presentation.
    /// </summary>

    // ──────────────────────────────────────────────
    //  1. Azkaban Despair Event
    // ──────────────────────────────────────────────

    /// <summary>
    /// Active during the Azkaban Despair event. Plays the Dementor King
    /// boss track and suppresses screen brightness to reinforce the
    /// oppressive dementor atmosphere already driven by the event system.
    /// </summary>
    public class AzkabanSceneEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/DementorKingBoss");

        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            return AzkabanDespairEvent.eventActive;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            // Cold, desaturated tint during Dementor assault
            player.ManageSpecialBiomeVisuals("WizardingWorld:AzkabanDespair", isActive);
        }
    }

    // ──────────────────────────────────────────────
    //  2. Battle of Hogwarts
    // ──────────────────────────────────────────────

    /// <summary>
    /// Active during the multi-phase Battle of Hogwarts siege.
    /// Plays the Voldemort boss track and applies a dark red-tinted
    /// atmosphere that intensifies during the breach phase.
    /// </summary>
    public class BattleSceneEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/VoldemortBoss");

        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            return BattleOfHogwartsSystem.battleActive;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            // War-torn darkness over the castle grounds
            player.ManageSpecialBiomeVisuals("WizardingWorld:BattleOfHogwarts", isActive);
        }
    }

    // ──────────────────────────────────────────────
    //  3. Gringotts Vault Descent
    // ──────────────────────────────────────────────

    /// <summary>
    /// Active during Gringotts vault retrieval missions. No custom music
    /// track exists for this yet, so it falls through to whatever biome
    /// music is playing. Applies a dim underground tint.
    /// </summary>
    public class GringottsDescentEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        public override bool IsSceneEffectActive(Player player)
        {
            return GringottsVaultSystem.missionActive;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            // Deep-vault darkness with faint gold shimmer feel
            player.ManageSpecialBiomeVisuals("WizardingWorld:GringottsDescent", isActive);
        }
    }

    // ──────────────────────────────────────────────
    //  4. Knockturn Alley
    // ──────────────────────────────────────────────

    /// <summary>
    /// Active during Knockturn Alley cursed-object containment missions.
    /// Dark, oppressive atmosphere befitting the shadiest corner of
    /// wizarding London.
    /// </summary>
    public class KnockturnAlleyEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        public override bool IsSceneEffectActive(Player player)
        {
            return KnockturnAlleySystem.missionActive;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            // Murky, oppressive dark-alley tint
            player.ManageSpecialBiomeVisuals("WizardingWorld:KnockturnAlley", isActive);
        }
    }

    // ──────────────────────────────────────────────
    //  5. Shrieking Shack
    // ──────────────────────────────────────────────

    /// <summary>
    /// Active during Shrieking Shack missions (Willow Suppression,
    /// Tunnel Stabilization, Shack Containment). Eerie atmosphere
    /// that intensifies during full moon nights. Uses the Fenrir boss
    /// track during full moon for extra tension.
    /// </summary>
    public class ShriekingShackEffect : ModSceneEffect
    {
        public override int Music
        {
            get
            {
                // During full moon, play Fenrir's track for lycanthropic tension;
                // otherwise fall through to ambient/biome music (-1 = no override)
                if (Main.moonPhase == 0)
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/FenrirBoss");

                return -1;
            }
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        public override bool IsSceneEffectActive(Player player)
        {
            return ShriekingShackSystem.missionActive;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            // Haunted, ghostly atmosphere — stronger during full moon
            player.ManageSpecialBiomeVisuals("WizardingWorld:ShriekingShack", isActive);
        }
    }
}
