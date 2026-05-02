using Microsoft.Xna.Framework;
using Terraria; using Terraria.ID; using Terraria.ModLoader;
namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>Grimmauld Boggart -- fear manifestation in the safehouse. Weak to Riddikulus (5x). Canon: Boggart in Grimmauld wardrobe.</summary>
    public class GrimmauldBoggart : ModNPC
    {
        public override void SetStaticDefaults() { Main.npcFrameCount[Type] = 1; NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() { Hide = true }); }
        public override void SetDefaults()
        {
            NPC.width = 28; NPC.height = 36; NPC.damage = 25; NPC.defense = 10; NPC.lifeMax = 350;
            NPC.knockBackResist = 0.3f; NPC.noGravity = true; NPC.noTileCollide = true;
            NPC.friendly = false; NPC.aiStyle = NPCAIStyleID.Bat;
            NPC.alpha = 60; NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit54; NPC.DeathSound = SoundID.NPCDeath52;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;
        public override void AI()
        {
            if (!Common.Systems.GrimmauldPlaceSystem.missionActive) { NPC.active = false; return; }
            NPC.spriteDirection = NPC.direction;
            if (Main.rand.NextBool(3)) { Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Wraith, 0f, 0f, 100, default, 0.7f); d.noGravity = true; d.velocity *= 0.2f; }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Darkness, 180);
            target.AddBuff(BuffID.Slow, 120);
            target.GetModPlayer<Common.Players.WizardPlayer>().AddDespair(0.06f, "a Boggart's fear");
        }
        public override void OnKill()
        {
            var c = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
            Common.Systems.GrimmauldPlaceSystem.OnThreatCleared(c);
            for (int i = 0; i < 15; i++) { Dust d = Dust.NewDustDirect(NPC.Center, 4, 4, DustID.YellowStarDust, 0f, 0f, 50, default, 1.5f); d.velocity = Main.rand.NextVector2Circular(4f, 4f); d.noGravity = true; }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Microsoft.Xna.Framework.Vector2 position) => true;
    }
}
