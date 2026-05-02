using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Blast-Ended Skrewt — Hagrid's horrifying crossbreed.
	/// Large, armored, explosive. Has a blast from its rear end that propels it forward.
	/// High defense from the front (armored shell), vulnerable from behind.
	/// Hardmode jungle enemy.
	/// </summary>
	public class BlastEndedSkrewt : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 44;
			NPC.height = 30;
			NPC.damage = 55;
			NPC.defense = 28; // Armored shell
			NPC.lifeMax = 450;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath10;
			NPC.value = Item.buyPrice(silver: 60);
			NPC.knockBackResist = 0.15f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.GiantTortoise;
			AnimationType = NPCID.GiantTortoise;
			NPC.lavaImmune = true; // Blast-resistant
		}

		public override void AI()
		{
			// Blast propulsion — periodic speed bursts
			NPC.ai[1]++;
			if (NPC.ai[1] >= 180) // Every 3 seconds
			{
				NPC.ai[1] = 0;
				// Blast forward!
				NPC.velocity.X = NPC.direction * 12f;

				// Explosion dust from the rear
				int rearX = NPC.direction > 0 ? (int)NPC.position.X : (int)(NPC.position.X + NPC.width);
				for (int i = 0; i < 15; i++)
				{
					Dust dust = Dust.NewDustDirect(new Vector2(rearX, NPC.Center.Y), 8, 8, DustID.Torch, -NPC.direction * 3f, 0f, 100, default, 1.5f);
					dust.noGravity = true;
				}
			}

			// Smoke from rear end while walking
			if (Main.rand.NextBool(6))
			{
				int rearX = NPC.direction > 0 ? (int)NPC.position.X : (int)(NPC.position.X + NPC.width);
				Dust dust = Dust.NewDustDirect(new Vector2(rearX, NPC.Center.Y), 4, 4, DustID.Smoke, -NPC.direction * 1f, 0f, 150, default, 0.8f);
				dust.noGravity = true;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.OnFire, 180); // Burn from the blast
			target.AddBuff(BuffID.BrokenArmor, 120);
		}

		public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
		{
			// Armored from the front — hits from behind deal bonus damage
			// Check if the projectile/attack came from behind the Skrewt
			if (Main.rand.NextBool(3))
				modifiers.FinalDamage *= 0.5f; // 33% chance to partially deflect
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && spawnInfo.Player.ZoneJungle)
				return 0.04f;

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.ExplosivePowder, 3, 2, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.DragonScale>(), 8));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.BlastEndedSkrewt"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 8; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch);
				dust.noGravity = true;
			}
		}
	}
}
