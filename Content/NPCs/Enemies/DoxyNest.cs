using Microsoft.Xna.Framework;
using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>Doxy Nest -- Grimmauld infestation node. Destroy to clear. Spawns Doxy swarms. Canon: Grimmauld had Doxy infestations.</summary>
    public class DoxyNest : ModNPC
    {
        public override void SetStaticDefaults() { Main.npcFrameCount[Type] = 1; NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() { Hide = true }); }
        public override void SetDefaults()
        {
            NPC.width = 24; NPC.height = 20; NPC.damage = 10; NPC.defense = 5; NPC.lifeMax = 200;
            NPC.knockBackResist = 0f; NPC.noGravity = true; NPC.noTileCollide = true;
            NPC.friendly = false; NPC.aiStyle = -1; NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit1; NPC.DeathSound = SoundID.NPCDeath1;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;
        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            if (!Common.Systems.GrimmauldPlaceSystem.missionActive) { NPC.active = false; return; }
            if (Main.rand.NextBool(4)) { Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GreenTorch, 0f, -0.3f, 80, default, 0.6f); d.noGravity = true; }
            // Spawn actual Doxy enemies nearby periodically
            if (Main.GameUpdateCount % 300 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int doxyType = ModContent.NPCType<Doxy>();
                if (NPC.CountNPCS(doxyType) < 6)
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-60, 60), (int)NPC.Center.Y, doxyType);
            }
        }
        public override void OnKill()
        {
            var c = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
            Common.Systems.GrimmauldPlaceSystem.OnThreatCleared(c);
            for (int i = 0; i < 12; i++) { Dust d = Dust.NewDustDirect(NPC.Center, 4, 4, DustID.GreenTorch, 0f, 0f, 50, default, 1.2f); d.velocity = Main.rand.NextVector2Circular(3f, 3f); d.noGravity = true; }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => true;
    }
}
