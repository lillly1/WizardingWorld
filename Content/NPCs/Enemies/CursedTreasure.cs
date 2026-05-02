using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>Cursed Treasure -- Gringotts anti-theft trap. Damages on proximity, Geminus Curse themed. Mod-original.</summary>
    public class CursedTreasure : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
        }
        public override void SetDefaults()
        {
            NPC.width = 24; NPC.height = 20; NPC.damage = 30; NPC.defense = 15; NPC.lifeMax = 200;
            NPC.knockBackResist = 0f; NPC.noGravity = true; NPC.noTileCollide = true;
            NPC.friendly = false; NPC.aiStyle = -1; NPC.alpha = 30; NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit4; NPC.DeathSound = SoundID.NPCDeath14;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;
        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            if (!Common.Systems.GringottsVaultSystem.missionActive) { NPC.active = false; return; }
            // Burning golden aura -- Geminus Curse (multiplying/burning treasure)
            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustDirect(NPC.position - new Vector2(4, 4), NPC.width + 8, NPC.height + 8,
                    DustID.Torch, 0f, -0.5f, 80, default, 1.0f);
                d.noGravity = true; d.velocity *= 0.3f;
            }
            Lighting.AddLight(NPC.Center, 0.4f, 0.2f, 0.05f);
            // Burn nearby players
            if (Main.GameUpdateCount % 60 == 0)
                foreach (Player p in Main.player)
                    if (p.active && !p.dead && Vector2.Distance(p.Center, NPC.Center) < 150f)
                        p.AddBuff(BuffID.OnFire, 120);
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => true;
    }
}
