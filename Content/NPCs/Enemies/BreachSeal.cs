using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>Breach Seal -- Phase 2 breach objective for the Battle of Hogwarts. Mod-original.</summary>
    public class BreachSeal : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() { Hide = true });
        }

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 24;
            NPC.damage = 0;
            NPC.defense = 6;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.friendly = false;
            NPC.aiStyle = -1;
            NPC.alpha = 30;
            NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;

        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            if (!Common.Systems.BattleOfHogwartsSystem.battleActive)
            {
                NPC.active = false;
                return;
            }

            Lighting.AddLight(NPC.Center, 0.5f, 0.15f, 0.1f);

            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustDirect(NPC.position - new Vector2(4, 4), NPC.width + 8, NPC.height + 8,
                    DustID.Shadowflame, 0f, -0.6f, 80, default, 0.8f);
                d.noGravity = true;
                d.velocity *= 0.3f;
            }

            if (Main.rand.NextBool(5))
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.RedTorch, 0f, -0.5f, 100, default, 0.7f);
                d.noGravity = true;
            }

            if (Main.GameUpdateCount % 120 == 0)
            {
                foreach (Player p in Main.player)
                {
                    if (p.active && !p.dead && Vector2.Distance(p.Center, NPC.Center) < 200f)
                        p.AddBuff(BuffID.Darkness, 90);
                }
            }
        }

        public override void OnKill()
        {
            var closest = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
            Common.Systems.BattleOfHogwartsSystem.OnObjectiveCompleted(closest);

            for (int i = 0; i < 15; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.Center, 4, 4, DustID.Shadowflame,
                    0f, 0f, 50, default, 1.5f);
                d.velocity = Main.rand.NextVector2Circular(4f, 4f);
                d.noGravity = true;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => true;
    }
}
