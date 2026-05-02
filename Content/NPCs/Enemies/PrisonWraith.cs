using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>
    /// Prison Wraith -- spectral remains of condemned prisoners.
    /// Spawns during Azkaban's Despair event. Weaker than Dementors but appears in groups.
    /// Mod-original content inspired by Azkaban's dark history.
    /// </summary>
    public class PrisonWraith : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            { Velocity = 1f });
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 40;
            NPC.damage = 40;
            NPC.defense = 12;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath52;
            NPC.value = Item.buyPrice(silver: 30);
            NPC.aiStyle = NPCAIStyleID.Bat;
            NPC.alpha = 80;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.buffImmune[BuffID.Poisoned] = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.hardMode) return 0f;
            if (Common.Systems.AzkabanDespairEvent.eventActive) return 0.12f;
            return 0f;
        }

        public override void AI()
        {
            // Ghostly trail
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.Wraith, 0f, 0f, 100, default, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
            NPC.spriteDirection = NPC.direction;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Darkness, 180);
            target.AddBuff(BuffID.Slow, 120);
            target.GetModPlayer<Common.Players.WizardPlayer>().AddDespair(0.08f, "a Prison Wraith's touch");
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15;
            if (NPC.frameCounter >= 4) NPC.frameCounter = 0;
            NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.SoulAsh>(), 2));
            npcLoot.Add(ItemDropRule.Common(ItemID.SoulofNight, 3));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.Wraith, 0f, 0f, 50, default, 1.2f);
                dust.noGravity = true;
                dust.velocity *= 2f;
            }
        }
    }
}
