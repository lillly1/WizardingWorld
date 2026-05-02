using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>
    /// Ward Anchor -- stationary destructible objective for the castle ward defense.
    /// When destroyed, stabilizes the Hogwarts wards via HogwartsWardSystem.
    /// Canon-inspired: Hogwarts' protective enchantments are anchored by magical wards.
    /// </summary>
    public class WardAnchor : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // objective NPC, not a bestiary creature
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 24;
            NPC.damage = 0;
            NPC.defense = 10;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = 0;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1; // stationary
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.friendly = false;
        }

        public override void AI()
        {
            NPC.velocity = Vector2.Zero;

            if (Main.rand.NextBool(8))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.MagicMirror, 0f, -0.5f, 150, new Color(120, 160, 220), 0.8f);
                dust.noGravity = true;
            }

            Lighting.AddLight(NPC.Center, 0.25f, 0.35f, 0.5f);
        }

        public override void OnKill()
        {
            Common.Systems.HogwartsWardSystem.OnAnchorStabilized();

            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.MagicMirror, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f),
                    100, new Color(140, 180, 240), 1.4f);
                dust.noGravity = true;
            }
        }
    }
}
