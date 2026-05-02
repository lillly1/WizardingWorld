using Microsoft.Xna.Framework;
using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>Venom Wound Node -- creature injuries ward objective. Mod-original.</summary>
    public class VenomWoundNode : ModNPC
    {
        public override void SetStaticDefaults() { Main.npcFrameCount[Type] = 1; NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() { Hide = true }); }
        public override void SetDefaults()
        {
            NPC.width = 20; NPC.height = 20; NPC.damage = 15; NPC.defense = 10; NPC.lifeMax = 280;
            NPC.knockBackResist = 0f; NPC.noGravity = true; NPC.noTileCollide = true;
            NPC.friendly = false; NPC.aiStyle = -1; NPC.alpha = 40; NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit4; NPC.DeathSound = SoundID.NPCDeath14;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;
        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            if (!Common.Systems.StMungosTriageSystem.missionActive) { NPC.active = false; return; }
            Lighting.AddLight(NPC.Center, 0.1f, 0.3f, 0.1f);
            if (Main.rand.NextBool(3)) { Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GreenTorch, 0f, -0.5f, 100, default, 0.8f); d.noGravity = true; }
            if (Main.GameUpdateCount % 120 == 0)
                foreach (Player p in Main.player)
                    if (p.active && !p.dead && Vector2.Distance(p.Center, NPC.Center) < 150f)
                        p.AddBuff(BuffID.Venom, 60);
        }
        public override void OnKill()
        {
            var c = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
            Common.Systems.StMungosTriageSystem.OnNodeStabilized(c);
            for (int i = 0; i < 15; i++) { Dust d = Dust.NewDustDirect(NPC.Center, 4, 4, DustID.GreenTorch, 0f, 0f, 50, default, 1.5f); d.velocity = Main.rand.NextVector2Circular(4f, 4f); d.noGravity = true; }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => true;
    }
}
