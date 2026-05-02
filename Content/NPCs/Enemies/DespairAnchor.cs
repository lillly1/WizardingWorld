using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
    /// <summary>
    /// Despair Anchor -- a dark ward point that amplifies despair during the Azkaban event.
    /// Stationary. Destroying these reduces event intensity and grants progress.
    /// Mod-original content: represents Azkaban's corrupted ward infrastructure.
    /// </summary>
    public class DespairAnchor : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            { Hide = true }); // Not a real creature
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 32;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 500;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = 0;
            NPC.aiStyle = -1; // stationary
            NPC.immortal = false;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.OnFire] = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Common.Systems.AzkabanDespairEvent.eventActive) return 0f;
            // Rare spawn, max 3 at a time
            int count = NPC.CountNPCS(Type);
            if (count >= 3) return 0f;
            return 0.04f;
        }

        public override void AI()
        {
            // Stationary -- emit despair aura
            NPC.velocity = Vector2.Zero;

            // Dark pulsing visual
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(NPC.position - new Vector2(8, 8),
                    NPC.width + 16, NPC.height + 16,
                    DustID.Shadowflame, 0f, -1f, 100, default, 1.5f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }

            // Apply despair to nearby players
            if (Main.GameUpdateCount % 60 == 0) // every second
            {
                foreach (Player player in Main.player)
                {
                    if (!player.active || player.dead) continue;
                    if (Vector2.Distance(player.Center, NPC.Center) < 400f)
                    {
                        player.GetModPlayer<Common.Players.WizardPlayer>()
                            .AddDespair(0.05f, "a Despair Anchor");
                    }
                }
            }

            // Dim nearby light
            Lighting.AddLight(NPC.Center, -0.4f, -0.4f, -0.3f);
        }

        public override void OnKill()
        {
            // Destroying an anchor gives bonus event progress and relieves despair
            Common.Systems.AzkabanDespairEvent.OnAnchorDestroyed();

            // Visual burst
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.Center, 8, 8,
                    DustID.GoldFlame, 0f, 0f, 50, default, 2f);
                dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
                dust.noGravity = true;
            }

            if (Main.netMode != NetmodeID.Server)
                Main.NewText("A Despair Anchor has been destroyed! The darkness weakens.", new Color(200, 220, 255));

            // Relieve nearby players
            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead) continue;
                if (Vector2.Distance(player.Center, NPC.Center) < 600f)
                {
                    player.GetModPlayer<Common.Players.WizardPlayer>().RelieveDespair(0.10f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.SoulAsh>(), 1, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 1, 3, 5));
        }
    }
}
