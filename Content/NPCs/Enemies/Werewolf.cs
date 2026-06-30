using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>Werewolf — fast, powerful night enemy during full moon in Hardmode.</summary>
	public class Werewolf : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 6;
		}

		public override void SetDefaults()
		{
			NPC.width = 30;
			NPC.height = 48;
			NPC.damage = 65;
			NPC.defense = 22;
			NPC.lifeMax = 500;
			NPC.HitSound = SoundID.NPCHit6;
			NPC.DeathSound = WizardSoundStyles.WerewolfHowl;
			NPC.value = Item.buyPrice(silver: 60);
			NPC.knockBackResist = 0.2f;
			NPC.aiStyle = NPCAIStyleID.Fighter;
			AIType = NPCID.Werewolf;
			AnimationType = NPCID.Werewolf;
		}

		public override void AI()
		{
			// Rage dust during blood moon
			if (Main.bloodMoon && Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
			}

			// Faster during blood moon
			if (Main.bloodMoon)
				NPC.velocity.X *= 1.03f;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			// Lycanthropy-like effect
			target.AddBuff(BuffID.Bleeding, 300);
			target.AddBuff(BuffID.Weak, 180);

			// During blood moon, additional debuff
			if (Main.bloodMoon)
				target.AddBuff(BuffID.BrokenArmor, 180);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			// Full moon nights in hardmode, or blood moon
			if (Main.hardMode && !Main.dayTime && spawnInfo.Player.ZoneOverworldHeight)
			{
				if (Main.bloodMoon)
					return 0.15f;
				if (Main.moonPhase == 0) // Full moon
					return 0.08f;
			}

			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.MoonCharm, 15)); // Rare vanilla moon charm
			npcLoot.Add(ItemDropRule.Common(ItemID.AdhesiveBandage, 10));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.WizardingWorld.Bestiary.Werewolf"),
			});
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood);
		}
	}
}
