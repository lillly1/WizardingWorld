using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>
    /// Veil Fracture -- sealable breach near the Veil in Death Chamber missions.
    /// Stationary, radiates dark energy, destroyed by damage to seal it.
    /// Mod-original: represents cracks in the boundary between life and death.
    /// </summary>
    public class VeilFracture : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
        }

        public override void SetDefaults()
        {
            NPC.width = 24; NPC.height = 32;
            NPC.damage = 15; NPC.defense = 10; NPC.lifeMax = 300;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true; NPC.noTileCollide = true;
            NPC.immortal = false; NPC.friendly = false;
            NPC.aiStyle = -1; NPC.alpha = 60; NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit54; NPC.DeathSound = SoundID.NPCDeath52;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;

        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            if (!Common.Systems.DeathChamberSystem.missionActive) { NPC.active = false; return; }

            Lighting.AddLight(NPC.Center, -0.3f, -0.2f, 0.1f);
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(NPC.position - new Vector2(4, 4),
                    NPC.width + 8, NPC.height + 8, DustID.Shadowflame, 0f, -0.5f, 100, default, 1.2f);
                dust.noGravity = true; dust.velocity *= 0.3f;
            }

            // Damage nearby players with despair
            if (Main.GameUpdateCount % 90 == 0)
                foreach (Player p in Main.player)
                    if (p.active && !p.dead && Vector2.Distance(p.Center, NPC.Center) < 200f)
                        p.GetModPlayer<Common.Players.WizardPlayer>().AddDespair(0.04f, "a Veil Fracture");
        }

        public override void OnKill()
        {
            var closest = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
            Common.Systems.DeathChamberSystem.OnFractureSealed(closest);

            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.Center, 8, 8, DustID.WhiteTorch, 0f, 0f, 50, default, 1.8f);
                d.velocity = Main.rand.NextVector2Circular(5f, 5f); d.noGravity = true;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => true;
    }
}
