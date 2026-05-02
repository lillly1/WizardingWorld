using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>
    /// Bludger -- autonomous iron ball that pursues players during Quidditch matches.
    /// Causes knockback and brief Slow debuff on hit. Does not kill -- disrupts.
    /// Despawns when the match ends.
    /// Canon-faithful: Bludgers are enchanted iron balls that try to unseat players.
    /// </summary>
    public class Bludger : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            { Hide = true });
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;
            NPC.damage = 25;
            NPC.defense = 30;
            NPC.lifeMax = 500;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = false; // Beaters can hit it away
            NPC.friendly = false;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit4;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;

        public override void AI()
        {
            // Despawn if match not active
            if (!Common.Systems.QuidditchCupSystem.matchActive)
            {
                NPC.active = false;
                return;
            }

            // Pursue the nearest mounted player
            Player target = null;
            float closest = 1200f;
            foreach (Player p in Main.player)
            {
                if (!p.active || p.dead || !p.mount.Active) continue;
                float dist = Vector2.Distance(NPC.Center, p.Center);
                if (dist < closest)
                {
                    closest = dist;
                    target = p;
                }
            }

            if (target != null)
            {
                Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                float speed = 6f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, dir * speed, 0.04f);
            }
            else
            {
                // Wander
                NPC.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                if (NPC.velocity.Length() > 4f)
                    NPC.velocity = Vector2.Normalize(NPC.velocity) * 4f;
            }

            // Iron ball visual
            NPC.rotation += NPC.velocity.Length() * 0.08f;
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.Iron, 0f, 0f, 80, default, 0.6f);
                dust.noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Slow, 120); // 2 seconds slow
            // Knock away from the Bludger after hit
            NPC.velocity = -NPC.velocity * 0.5f;
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            // Beater's Bat hits knock the Bludger away dramatically
            modifiers.Knockback *= 5f;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => false;
    }
}
