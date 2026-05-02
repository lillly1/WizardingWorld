using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>
    /// Cursed Opal Display -- a dangerous cursed necklace on display in Borgin's shop.
    /// Containment objective: destroy it carefully to contain its curse.
    /// Damages nearby players with dark energy. Canon-inspired: Katie Bell was nearly killed by one.
    /// </summary>
    public class CursedOpalDisplay : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
        }
        public override void SetDefaults()
        {
            NPC.width = 20; NPC.height = 20; NPC.damage = 20; NPC.defense = 8; NPC.lifeMax = 200;
            NPC.knockBackResist = 0f; NPC.noGravity = true; NPC.noTileCollide = true;
            NPC.friendly = false; NPC.aiStyle = -1; NPC.alpha = 40; NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit4; NPC.DeathSound = SoundID.NPCDeath14;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;
        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            if (!Common.Systems.KnockturnAlleySystem.missionActive) { NPC.active = false; return; }
            // Dark opal shimmer
            Lighting.AddLight(NPC.Center, 0.2f, 0.1f, 0.3f);
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.PurpleTorch, 0f, -0.5f, 100, default, 0.8f);
                d.noGravity = true; d.velocity *= 0.2f;
            }
            // Curse aura -- damages nearby players
            if (Main.GameUpdateCount % 90 == 0)
                foreach (Player p in Main.player)
                    if (p.active && !p.dead && Vector2.Distance(p.Center, NPC.Center) < 150f)
                    {
                        p.AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.DarkCurseDebuff>(), 120);
                        p.GetModPlayer<Common.Players.WizardPlayer>().AddDespair(0.03f, "the cursed opal necklace");
                    }
        }
        public override void OnKill()
        {
            var closest = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
            Common.Systems.KnockturnAlleySystem.OnObjectContained(closest);
            for (int i = 0; i < 15; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.Center, 6, 6, DustID.PurpleTorch, 0f, 0f, 50, default, 1.5f);
                d.velocity = Main.rand.NextVector2Circular(4f, 4f); d.noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Terraria.Localization.Language.GetTextValue("Mods.WizardingWorld.Knockturn.OpalContained"),
                    new Color(180, 150, 200));
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => true;
    }
}
