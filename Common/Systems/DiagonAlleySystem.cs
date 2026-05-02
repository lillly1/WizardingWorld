using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WizardingWorld.Common.Systems
{
    /// <summary>
    /// Diagon Alley System -- manages the wizard shopping street hub identity.
    ///
    /// The Leaky Cauldron is the canonical gateway into Diagon Alley.
    /// Using a Leaky Cauldron Token grants access to the Diagon Alley experience:
    /// a temporary "shopping day" buff that enhances NPC shop prices and treasure finding.
    ///
    /// This system tracks Diagon Alley visits and provides street identity framing
    /// for the Gringotts vault system, Ollivander integration, and future shop expansion.
    ///
    /// Mod-original: compact hub abstraction of the Diagon Alley shopping experience.
    /// </summary>
    public class DiagonAlleySystem : ModSystem
    {
        public static bool diagonAlleyVisited;
        public static int totalVisits;
        public static int daysSinceLastVisit;

        public override void ClearWorld()
        {
            diagonAlleyVisited = false;
            totalVisits = 0;
            daysSinceLastVisit = 0;
        }

        public override void PreUpdateWorld()
        {
            // Track days
            if (Main.dayTime && Main.time < 100)
                daysSinceLastVisit++;
        }

        /// <summary>Called when the player uses the Leaky Cauldron Token to enter Diagon Alley.</summary>
        public static void EnterDiagonAlley(Player player)
        {
            diagonAlleyVisited = true;
            totalVisits++;
            daysSinceLastVisit = 0;

            // Discover One-Eyed Witch passage on first visit
            if (SecretPassageSystem.passagesDiscovered < 2)
                SecretPassageSystem.DiscoverPassage("One-Eyed Witch");

            // Shopping Day buff -- enhanced commerce for 10 minutes
            player.AddBuff(ModContent.BuffType<Content.Buffs.ShoppingDayBuff>(), 60 * 60 * 10);

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DiagonAlley.Welcome"),
                    new Color(220, 200, 120));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DiagonAlley.StreetDescription"),
                    new Color(200, 180, 100));

                // Show available services
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DiagonAlley.Gringotts"),
                    new Color(180, 160, 80));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DiagonAlley.Ollivanders"),
                    new Color(160, 140, 100));
                Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DiagonAlley.Wheezes"),
                    new Color(255, 150, 50));
            }

            // Celebration dust -- stepping through the wall
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center, 8, 8,
                    DustID.GoldCoin, 0f, -1f, 80, default, 1.0f);
                dust.velocity = Main.rand.NextVector2Circular(3f, 4f);
                dust.noGravity = true;
            }

            // Award House Points for visiting
            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0 && wp.houseSet <= 4)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 5,
                    Language.GetTextValue("Mods.WizardingWorld.DiagonAlley.SourceVisit"));
        }

        /// <summary>Gringotts cart descent framing -- called before vault mission starts.</summary>
        public static void CartDescentFraming(Player player)
        {
            if (Main.netMode == NetmodeID.Server) return;

            // Sequential framing messages using Transit keys for the descent experience
            Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Transit.GringottsDescend"),
                new Color(160, 140, 100));
            Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Transit.GringottsDeep"),
                new Color(140, 120, 80));
            Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Transit.GringottsArrive"),
                new Color(120, 100, 60));

            // Also show the original DiagonAlley cart lines for additional flavor
            Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DiagonAlley.CartDescent1"),
                new Color(160, 140, 100));
            Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DiagonAlley.CartDescent2"),
                new Color(140, 120, 80));
            Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DiagonAlley.CartDescent3"),
                new Color(120, 100, 60));

            // Speed boost as "cart acceleration"
            player.velocity.Y -= 2f;

            // Tunnel dust effect
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center, 6, 6,
                    DustID.Iron, Main.rand.NextFloat(-4f, 4f), -3f, 100, default, 0.8f);
                dust.noGravity = true;
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["da_visited"] = diagonAlleyVisited;
            tag["da_visits"] = totalVisits;
            tag["da_days"] = daysSinceLastVisit;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            diagonAlleyVisited = tag.GetBool("da_visited");
            totalVisits = tag.GetInt("da_visits");
            daysSinceLastVisit = tag.GetInt("da_days");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(diagonAlleyVisited);
            writer.Write(totalVisits);
        }

        public override void NetReceive(BinaryReader reader)
        {
            diagonAlleyVisited = reader.ReadBoolean();
            totalVisits = reader.ReadInt32();
        }
    }
}
