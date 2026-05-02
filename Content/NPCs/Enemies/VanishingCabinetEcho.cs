using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>
    /// Vanishing Cabinet Echo -- an unstable paired transport artifact.
    /// Containment objective: seal it before it creates a breach.
    /// Periodically teleports nearby players randomly. Canon-inspired: Draco's Vanishing Cabinet plot.
    /// </summary>
    public class VanishingCabinetEcho : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
        }
        public override void SetDefaults()
        {
            NPC.width = 28; NPC.height = 40; NPC.damage = 10; NPC.defense = 15; NPC.lifeMax = 350;
            NPC.knockBackResist = 0f; NPC.noGravity = true; NPC.noTileCollide = true;
            NPC.friendly = false; NPC.aiStyle = -1; NPC.alpha = 50; NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit4; NPC.DeathSound = SoundID.NPCDeath14;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;
        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            if (!Common.Systems.KnockturnAlleySystem.missionActive) { NPC.active = false; return; }
            Lighting.AddLight(NPC.Center, -0.2f, -0.1f, 0.15f);
            // Unstable shimmer
            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustDirect(NPC.position - new Vector2(4, 4),
                    NPC.width + 8, NPC.height + 8, DustID.Shadowflame, 0f, -0.3f, 100, default, 1.0f);
                d.noGravity = true; d.velocity *= 0.2f;
            }
            // Cabinet teleport hazard -- displace nearby players
            if (Main.GameUpdateCount % 300 == 0) // every 5 seconds
                foreach (Player p in Main.player)
                    if (p.active && !p.dead && Vector2.Distance(p.Center, NPC.Center) < 200f)
                    {
                        p.Teleport(p.position + new Vector2(Main.rand.Next(-200, 200), Main.rand.Next(-100, 100)));
                        p.AddBuff(BuffID.Confused, 60);
                    }
            // Flickering alpha
            NPC.alpha = 30 + (int)(System.Math.Sin(Main.GameUpdateCount * 0.08f) * 25);
        }
        public override void OnKill()
        {
            var closest = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
            Common.Systems.KnockturnAlleySystem.OnObjectContained(closest);
            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.Center, 8, 8, DustID.Shadowflame, 0f, 0f, 50, default, 1.8f);
                d.velocity = Main.rand.NextVector2Circular(5f, 5f); d.noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Terraria.Localization.Language.GetTextValue("Mods.WizardingWorld.Knockturn.CabinetSealed"),
                    new Color(160, 140, 180));
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => true;
    }
}
