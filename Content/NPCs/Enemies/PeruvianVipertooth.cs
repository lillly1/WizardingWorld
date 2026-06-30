using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Peruvian Vipertooth — the smallest and fastest of all dragon breeds.
	/// Copper-coloured, venomous, and extremely aggressive.
	/// Flies rapidly, bites with venomous fangs.
	/// Hardmode sky/surface enemy — the only dragon that isn't a boss.
	/// </summary>
	public class PeruvianVipertooth : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 36;
			NPC.height = 24;
			NPC.damage = 55;
			NPC.defense = 18;
			NPC.lifeMax = 350;
			NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = SoundID.NPCDeath10;
			NPC.value = Item.buyPrice(silver: 50);
			NPC.knockBackResist = 0.25f;
			NPC.noGravity = true;
			NPC.aiStyle = -1;
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			NPC.ai[0]++;

			// Extremely fast dive attacks
			if (NPC.ai[0] < 90)
			{
				// Circle above
				float angle = NPC.ai[0] * 0.05f;
				Vector2 orbitPos = target.Center + new Vector2((float)Math.Cos(angle) * 200f, -150f);
				float speed = 8f;
				Vector2 dir = (orbitPos - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
				NPC.velocity = Vector2.Lerp(NPC.velocity, dir, 0.06f);
				NPC.damage = NPC.defDamage / 2;
			}
			else if (NPC.ai[0] == 90)
			{
				// DIVE!
				Vector2 diveDir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 20f;
				NPC.velocity = diveDir;
				NPC.damage = (int)(NPC.defDamage * 1.5f);
				NPC.netUpdate = true;
				SoundEngine.PlaySound(WizardSoundStyles.DragonRoar, NPC.Center);
			}
			else if (NPC.ai[0] > 120)
			{
				NPC.ai[0] = 0;
				NPC.netUpdate = true;
			}

			// Copper-coloured fire dust
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CopperCoin, 0f, 0f, 100, default, 0.6f);
				dust.noGravity = true;
				dust.velocity = -NPC.velocity * 0.1f;
			}

			NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;
			NPC.rotation = NPC.velocity.ToRotation();
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Venom, 240);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && (spawnInfo.Player.ZoneSkyHeight || (spawnInfo.Player.ZoneOverworldHeight && !Main.dayTime)))
				return 0.03f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.DragonScale>(), 3, 1, 2));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 2, 4));
			npcLoot.Add(ItemDropRule.Common(ItemID.Feather, 2, 2, 5));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.PeruvianVipertooth"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 8; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CopperCoin);
		}
	}
}
