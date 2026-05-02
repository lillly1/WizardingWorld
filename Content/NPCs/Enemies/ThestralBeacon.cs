using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>Thestral Beacon -- moonlit guidance marker in the Thestral Clearing. Collect on proximity. Mod-original.</summary>
    public class ThestralBeacon : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() { Hide = true });
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 22;
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
            NPC.alpha = 60;
            NPC.npcSlots = 0f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;

        public override void AI()
        {
            NPC.velocity = Vector2.Zero;

            if (!Common.Systems.ForestExpeditionSystem.missionActive)
            {
                NPC.active = false;
                return;
            }

            Lighting.AddLight(NPC.Center, 0.15f, 0.15f, 0.25f);

            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.WhiteTorch, 0f, -0.5f, 100, default, 0.7f);
                d.noGravity = true;
                d.velocity *= 0.2f;
            }

            NPC.position.Y += (float)System.Math.Sin(Main.GameUpdateCount * 0.03f) * 0.3f;

            foreach (Player p in Main.player)
            {
                if (!p.active || p.dead) continue;
                if (Vector2.Distance(p.Center, NPC.Center) < 40f)
                {
                    Common.Systems.ForestExpeditionSystem.OnObjectiveCompleted(p);
                    NPC.active = false;
                    for (int i = 0; i < 12; i++)
                    {
                        Dust d = Dust.NewDustDirect(NPC.Center, 4, 4,
                            DustID.WhiteTorch, 0f, 0f, 50, default, 1.5f);
                        d.velocity = Main.rand.NextVector2Circular(4f, 4f);
                        d.noGravity = true;
                    }
                    break;
                }
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => false;
    }
}
