using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WizardingWorld.Common.Systems
{
    /// <summary>
    /// Hogwarts Ward System -- tracks castle ward defense state.
    /// Ward anchors stabilized, defense unlocked status, and wards defended counter.
    /// Mod-original: represents the protective enchantments around Hogwarts.
    /// </summary>
    public class HogwartsWardSystem : ModSystem
    {
        public static bool wardDefenseUnlocked;
        public static int wardsDefended;

        public override void OnWorldLoad()
        {
            wardDefenseUnlocked = false;
            wardsDefended = 0;
        }

        public override void OnWorldUnload()
        {
            wardDefenseUnlocked = false;
            wardsDefended = 0;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["wardDefenseUnlocked"] = wardDefenseUnlocked;
            tag["wardsDefended"] = wardsDefended;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            wardDefenseUnlocked = tag.GetBool("wardDefenseUnlocked");
            wardsDefended = tag.GetInt("wardsDefended");
        }

        /// <summary>Called when a WardAnchor NPC is destroyed during ward defense.</summary>
        public static void OnAnchorStabilized()
        {
            wardsDefended++;
            if (!wardDefenseUnlocked)
                wardDefenseUnlocked = true;
        }
    }
}
