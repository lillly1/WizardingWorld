using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>
    /// Quidditch Hoop -- goal target for Quaffle scoring.
    /// 3 hoops spawn per match at the opposing end.
    /// The Quaffle scores when it hits a hoop (via OnHitByProjectile).
    /// Stationary, semi-transparent, indestructible during matches.
    /// Mod-original: represents the three goal posts at each end of the Quidditch pitch.
    /// </summary>
    public class QuidditchHoop : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            { Hide = true });
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 48;
            NPC.damage = 0;
            NPC.defense = 9999;
            NPC.lifeMax = 1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.friendly = false;
            NPC.aiStyle = -1;
            NPC.alpha = 100;
            NPC.npcSlots = 0f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f; // never natural spawn

        public override void AI()
        {
            NPC.velocity = Vector2.Zero;

            // Despawn if match is not active
            if (!Common.Systems.QuidditchCupSystem.matchActive)
            {
                NPC.active = false;
                return;
            }

            // Golden ring visual
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.GoldCoin, 0f, 0f, 100, default, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.1f;
            }

            // Light
            Lighting.AddLight(NPC.Center, 0.4f, 0.35f, 0.1f);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = 0;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position)
        {
            return false; // no health bar
        }
    }
}
