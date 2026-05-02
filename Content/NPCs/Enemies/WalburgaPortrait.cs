using Microsoft.Xna.Framework;
using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>Walburga Portrait -- cursed shrieking portrait in Grimmauld. Applies Jinxed in area. Canon: permanently stuck to the wall.</summary>
    public class WalburgaPortrait : ModNPC
    {
        public override void SetStaticDefaults() { Main.npcFrameCount[Type] = 1; NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() { Hide = true }); }
        public override void SetDefaults()
        {
            NPC.width = 24; NPC.height = 32; NPC.damage = 20; NPC.defense = 12; NPC.lifeMax = 300;
            NPC.knockBackResist = 0f; NPC.noGravity = true; NPC.noTileCollide = true;
            NPC.friendly = false; NPC.aiStyle = -1; NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit4; NPC.DeathSound = SoundID.NPCDeath14;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;
        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            if (!Common.Systems.GrimmauldPlaceSystem.missionActive) { NPC.active = false; return; }
            if (Main.rand.NextBool(3)) { Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch, 0f, -0.5f, 100, default, 0.9f); d.noGravity = true; }
            // Shrieking curse aura
            if (Main.GameUpdateCount % 90 == 0)
                foreach (Player p in Main.player)
                    if (p.active && !p.dead && Vector2.Distance(p.Center, NPC.Center) < 200f)
                    {
                        p.AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.JinxedDebuff>(), 120);
                        p.GetModPlayer<Common.Players.WizardPlayer>().AddDespair(0.02f, "Walburga's shrieking");
                    }
        }
        public override void OnKill()
        {
            var c = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
            Common.Systems.GrimmauldPlaceSystem.OnThreatCleared(c);
            if (Main.netMode != NetmodeID.Server)
                Main.NewText(Terraria.Localization.Language.GetTextValue("Mods.WizardingWorld.Grimmauld.PortraitSilenced"), new Color(140, 130, 160));
            for (int i = 0; i < 15; i++) { Dust d = Dust.NewDustDirect(NPC.Center, 4, 4, DustID.PurpleTorch, 0f, 0f, 50, default, 1.5f); d.velocity = Main.rand.NextVector2Circular(4f, 4f); d.noGravity = true; }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => true;
    }
}
