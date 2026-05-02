using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>
    /// Prophecy Orb -- collectible objective during Department of Mysteries missions.
    /// Stationary, glowing, fragile. Touch to collect. Despawns when mission ends.
    /// Mod-original: represents prophecy spheres in the Hall of Prophecy.
    /// </summary>
    public class ProphecyOrb : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            { Hide = true });
        }

        public override void SetDefaults()
        {
            NPC.width = 16;
            NPC.height = 16;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.alpha = 50;
            NPC.npcSlots = 0f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;

        public override void AI()
        {
            NPC.velocity = Vector2.Zero;

            if (!Common.Systems.ProphecyMissionSystem.missionActive)
            {
                NPC.active = false;
                return;
            }

            // Ethereal blue glow
            Lighting.AddLight(NPC.Center, 0.3f, 0.3f, 0.6f);

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.BlueTorch, 0f, -0.5f, 100, default, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.2f;
            }

            // Gentle bobbing
            NPC.position.Y += (float)System.Math.Sin(Main.GameUpdateCount * 0.03f) * 0.3f;

            // Collect on player proximity
            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead) continue;
                if (Vector2.Distance(player.Center, NPC.Center) < 40f)
                {
                    Common.Systems.ProphecyMissionSystem.OnOrbCollected(player);
                    NPC.active = false;

                    // Collection effect
                    for (int i = 0; i < 15; i++)
                    {
                        Dust dust = Dust.NewDustDirect(NPC.Center, 4, 4,
                            DustID.BlueTorch, 0f, 0f, 50, default, 1.5f);
                        dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
                        dust.noGravity = true;
                    }
                    break;
                }
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => false;
    }
}
