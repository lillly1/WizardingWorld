using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>Cauldron Spill Node -- potion accidents ward objective. Mod-original.</summary>
    public class CauldronSpillNode : ModNPC
    {
        public override void SetStaticDefaults() { Main.npcFrameCount[Type] = 1; NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() { Hide = true }); }
        public override void SetDefaults()
        {
            NPC.width = 22; NPC.height = 22; NPC.damage = 10; NPC.defense = 6; NPC.lifeMax = 220;
            NPC.knockBackResist = 0f; NPC.noGravity = true; NPC.noTileCollide = true;
            NPC.friendly = false; NPC.aiStyle = -1; NPC.alpha = 30; NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit4; NPC.DeathSound = SoundID.NPCDeath14;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;
        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            if (!Common.Systems.StMungosTriageSystem.missionActive) { NPC.active = false; return; }
            Lighting.AddLight(NPC.Center, 0.2f, 0.3f, 0.1f);
            if (!Main.dedServ && Main.netMode != NetmodeID.Server)
            {
                if (NPC.localAI[1] == 0f)
                {
                    NPC.localAI[0] = Main.rand.Next(60, 240);
                    NPC.localAI[1] = 1f;
                }

                if (NPC.localAI[0] > 0f)
                {
                    NPC.localAI[0]--;
                }
                else if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && Vector2.DistanceSquared(Main.LocalPlayer.Center, NPC.Center) < 600f * 600f)
                {
                    SoundEngine.PlaySound(WizardSoundStyles.CauldronBubble, NPC.Center);
                    NPC.localAI[0] = Main.rand.Next(180, 420);
                }
            }
            if (Main.rand.NextBool(2)) { Dust d = Dust.NewDustDirect(NPC.position - new Vector2(4, 4), NPC.width + 8, NPC.height + 8, DustID.Torch, 0f, -1f, 80, default, 0.9f); d.noGravity = true; d.velocity *= 0.3f; }
            if (Main.GameUpdateCount % 120 == 0)
                foreach (Player p in Main.player)
                    if (p.active && !p.dead && Vector2.Distance(p.Center, NPC.Center) < 150f)
                        p.AddBuff(BuffID.Confused, 60);
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
