using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>
    /// Unstable Hourglass -- collectible objective in Time Chamber missions.
    /// Floats, glows golden, collected on proximity. Emits time-dust particles.
    /// Mod-original: represents unstable temporal artifacts.
    /// </summary>
    public class UnstableHourglass : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
        }

        public override void SetDefaults()
        {
            NPC.width = 16; NPC.height = 20;
            NPC.damage = 0; NPC.defense = 0; NPC.lifeMax = 1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true; NPC.noTileCollide = true;
            NPC.immortal = true; NPC.dontTakeDamage = true;
            NPC.friendly = true; NPC.aiStyle = -1;
            NPC.alpha = 50; NPC.npcSlots = 0f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;

        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            if (!Common.Systems.TimeChamberSystem.missionActive) { NPC.active = false; return; }

            Lighting.AddLight(NPC.Center, 0.4f, 0.35f, 0.1f);
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.GoldCoin, 0f, -0.5f, 100, default, 0.8f);
                dust.noGravity = true; dust.velocity *= 0.2f;
            }

            NPC.position.Y += (float)System.Math.Sin(Main.GameUpdateCount * 0.04f) * 0.4f;

            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead) continue;
                if (Vector2.Distance(player.Center, NPC.Center) < 40f)
                {
                    Common.Systems.TimeChamberSystem.OnHourglassCollected(player);
                    NPC.active = false;
                    for (int i = 0; i < 12; i++)
                    {
                        Dust d = Dust.NewDustDirect(NPC.Center, 4, 4, DustID.GoldCoin, 0f, 0f, 50, default, 1.5f);
                        d.velocity = Main.rand.NextVector2Circular(4f, 4f); d.noGravity = true;
                    }
                    break;
                }
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => false;
    }
}
