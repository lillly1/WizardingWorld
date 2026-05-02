using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>Tunnel Ward -- tunnel support ward. Stationary objective, applies Darkness+Slow. Mod-original.</summary>
    public class TunnelWard : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() { Hide = true });
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 24;
            NPC.damage = 12;
            NPC.defense = 12;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.friendly = false;
            NPC.aiStyle = -1;
            NPC.alpha = 50;
            NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;

        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            if (!Common.Systems.ShriekingShackSystem.missionActive)
            {
                NPC.active = false;
                return;
            }

            Lighting.AddLight(NPC.Center, 0.1f, 0.1f, 0.12f);

            // Periodic Darkness + Slow to nearby players every 2 seconds
            NPC.ai[0]++;
            if (NPC.ai[0] >= 120)
            {
                NPC.ai[0] = 0;
                foreach (Player p in Main.player)
                {
                    if (!p.active || p.dead) continue;
                    if (Vector2.Distance(p.Center, NPC.Center) < 150f)
                    {
                        p.AddBuff(BuffID.Darkness, 90);
                        p.AddBuff(BuffID.Slow, 60);
                    }
                }
            }

            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.Silver, 0f, -0.5f, 100, default, 0.7f);
                d.noGravity = true;
            }
        }

        public override void OnKill()
        {
            var c = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
            Common.Systems.ShriekingShackSystem.OnObjectiveCompleted(c);

            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.Center, 4, 4, DustID.Silver,
                    0f, 0f, 50, default, 1.5f);
                d.velocity = Main.rand.NextVector2Circular(4f, 4f);
                d.noGravity = true;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => true;
    }
}
