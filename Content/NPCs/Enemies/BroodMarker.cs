using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>Brood Marker -- Acromantula nest node. Spawns spiders. Destroy to clear the den. Mod-original.</summary>
    public class BroodMarker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() { Hide = true });
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 22;
            NPC.damage = 15;
            NPC.defense = 10;
            NPC.lifeMax = 300;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.friendly = false;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
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

            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.Web, 0f, -0.3f, 80, default, 0.7f);
                d.noGravity = true;
            }

            if (Main.GameUpdateCount % 300 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int aType = ModContent.NPCType<Acromantula>();
                if (NPC.CountNPCS(aType) < 4)
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80),
                        (int)NPC.Center.Y, aType);
            }
        }

        public override void OnKill()
        {
            var c = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
            Common.Systems.ForestExpeditionSystem.OnObjectiveCompleted(c);

            for (int i = 0; i < 15; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.Center, 4, 4, DustID.Web,
                    0f, 0f, 50, default, 1.5f);
                d.velocity = Main.rand.NextVector2Circular(4f, 4f);
                d.noGravity = true;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => true;
    }
}
