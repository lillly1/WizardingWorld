using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Occamy — a serpentine winged creature from Fantastic Beasts.
	/// Choranaptyxic (grows/shrinks to fill available space).
	/// In-game: starts small, GROWS larger when hit (up to 2x size).
	/// Gets stronger as it grows. Drops silver eggshells (valuable).
	/// Sky/surface hardmode enemy.
	/// </summary>
	public class Occamy : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 30;
			NPC.height = 24;
			NPC.damage = 45;
			NPC.defense = 16;
			NPC.lifeMax = 350;
			NPC.HitSound = SoundID.NPCHit28;
			NPC.DeathSound = SoundID.NPCDeath31;
			NPC.value = Item.buyPrice(silver: 80);
			NPC.knockBackResist = 0.3f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Bat;
			AIType = NPCID.GiantBat;
		}

		public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			GrowOnHit();
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			GrowOnHit();
		}

		private void GrowOnHit()
		{
			// Choranaptyxic — grows when agitated!
			if (NPC.scale < 2.0f)
			{
				NPC.scale += 0.1f;
				NPC.damage = (int)(NPC.defDamage * NPC.scale);
				NPC.defense = (int)(16 * NPC.scale);

				// Growth dust burst
				for (int i = 0; i < 5; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 0f, 0f, 100, default, 0.8f);
					dust.noGravity = true;
					dust.velocity *= 1.5f;
				}
			}
		}

		public override void AI()
		{
			// Silver feather dust
			if (Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 0f, 0f, 100, default, 0.5f);
				dust.noGravity = true;
			}

			NPC.spriteDirection = NPC.direction;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode && (spawnInfo.Player.ZoneSkyHeight || (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime)))
				return 0.04f;
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			// Occamy eggshells are pure silver — very valuable
			npcLoot.Add(ItemDropRule.Common(ItemID.SilverCoin, 1, 20, 50));
			npcLoot.Add(ItemDropRule.Common(ItemID.SilverBar, 2, 3, 8));
			npcLoot.Add(ItemDropRule.Common(ItemID.Feather, 1, 2, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 2, 1, 3));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Occamy"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 6; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.SilverCoin);
		}
	}
}
