using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// House Renown System — earn renown for your Hogwarts house through combat prowess.
	/// Not to be confused with canonical House Points (awarded by professors).
	/// Canon-inspired original mod content: represents your house's battlefield reputation.
	///
	/// REDESIGN: Renown is now earned from DIVERSE sources, not just kills:
	/// - Boss defeats (20% of renown)
	/// - Quidditch Snitch catches (15%)
	/// - Daily Challenge completion (15%)
	/// - Exploration/discovery (10%)
	/// - Potion brewing (10%)
	/// - Creature study (10%)
	/// - Spell combo execution (10%)
	/// - Enemy kills while in house armor (10% — reduced from 100%)
	///
	/// This better reflects Hogwarts where house points come from
	/// academic merit, bravery, and conduct — not grinding.
	///
	/// Milestones:
	/// 50 → House Initiate (minor buff)
	/// 150 → House Regular (major buff)
	/// 500 → House Champion (massive buff, resets renown)
	///
	/// Each house's buffs match their identity:
	/// - Gryffindor: Damage + Endurance
	/// - Slytherin: Crit + Speed
	/// - Ravenclaw: Mana + Spell Power
	/// - Hufflepuff: Defense + Regen
	/// </summary>
	public class HouseRenownSystem : ModPlayer
	{
		public int houseRenown;
		public int pointDecayTimer;
		private int lastMilestone; // 0=none, 1=minor, 2=major, 3=champion

		public override void ResetEffects()
		{
			// Decay timer
			pointDecayTimer++;
			if (pointDecayTimer >= 600 && houseRenown > 0) // Every 10 seconds
			{
				pointDecayTimer = 0;
				houseRenown--;
			}
		}

		public override void PostUpdate()
		{
			var wp = Player.GetModPlayer<Players.WizardPlayer>();
			if (wp.houseSet == 0)
				return; // No house armor — no renown system

			// Check milestones
			if (houseRenown >= 500 && lastMilestone < 3)
			{
				lastMilestone = 3;
				ApplyHouseChampionBuff(wp.houseSet);
				GreatHallSystem.AwardHousePoints(wp.houseSet, 50, Language.GetTextValue("Mods.WizardingWorld.GreatHall.SourceChampion"));
				houseRenown = 0; // Reset after House Champion
				lastMilestone = 0;

				if (Player.whoAmI == Main.myPlayer)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Renown.Champion"), 255, 215, 0);
			}
			else if (houseRenown >= 150 && lastMilestone < 2)
			{
				lastMilestone = 2;
				ApplyMajorBuff(wp.houseSet);

				if (Player.whoAmI == Main.myPlayer)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Renown.MajorBuff"), 200, 200, 100);
			}
			else if (houseRenown >= 50 && lastMilestone < 1)
			{
				lastMilestone = 1;
				ApplyMinorBuff(wp.houseSet);

				if (Player.whoAmI == Main.myPlayer)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Renown.MinorBuff"), 150, 200, 150);
			}

			// Reset milestone tracking if renown drops below thresholds
			if (houseRenown < 50)
				lastMilestone = 0;
			else if (houseRenown < 150)
				lastMilestone = Math.Min(lastMilestone, 1);
		}

		public void AddPoints(int amount)
		{
			houseRenown += amount;

			// Visual feedback
			if (Player.whoAmI == Main.myPlayer && amount > 0)
			{
				CombatText.NewText(Player.getRect(), new Color(255, 215, 0), $"+{amount} HR"); // HR = House Renown
			}
		}

		private void ApplyMinorBuff(int house)
		{
			switch (house)
			{
				case 1: // Gryffindor — Wrath (damage)
					Player.AddBuff(BuffID.Wrath, 18000);
					break;
				case 2: // Slytherin — Swiftness
					Player.AddBuff(BuffID.Swiftness, 18000);
					break;
				case 3: // Ravenclaw — Magic Power
					Player.AddBuff(BuffID.MagicPower, 18000);
					break;
				case 4: // Hufflepuff — Ironskin
					Player.AddBuff(BuffID.Ironskin, 18000);
					break;
				case 5: // Dark Wizard — Rage
					Player.AddBuff(BuffID.Rage, 18000);
					break;
			}
		}

		private void ApplyMajorBuff(int house)
		{
			switch (house)
			{
				case 1: // Gryffindor — Wrath + Endurance
					Player.AddBuff(BuffID.Wrath, 18000);
					Player.AddBuff(BuffID.Endurance, 18000);
					break;
				case 2: // Slytherin — Swiftness + Rage
					Player.AddBuff(BuffID.Swiftness, 18000);
					Player.AddBuff(BuffID.Rage, 18000);
					break;
				case 3: // Ravenclaw — Magic Power + Mana Regen
					Player.AddBuff(BuffID.MagicPower, 18000);
					Player.AddBuff(BuffID.ManaRegeneration, 18000);
					break;
				case 4: // Hufflepuff — Ironskin + Regen
					Player.AddBuff(BuffID.Ironskin, 18000);
					Player.AddBuff(BuffID.Regeneration, 18000);
					break;
				case 5: // Dark Wizard — Rage + Inferno
					Player.AddBuff(BuffID.Rage, 18000);
					Player.AddBuff(BuffID.Inferno, 18000);
					break;
			}
		}

		private void ApplyHouseChampionBuff(int house)
		{
			// The ultimate reward — ALL major buffs at once for 10 minutes
			Player.AddBuff(BuffID.Wrath, 36000);
			Player.AddBuff(BuffID.Rage, 36000);
			Player.AddBuff(BuffID.Endurance, 36000);
			Player.AddBuff(BuffID.Ironskin, 36000);
			Player.AddBuff(BuffID.Swiftness, 36000);
			Player.AddBuff(BuffID.MagicPower, 36000);
			Player.AddBuff(BuffID.ManaRegeneration, 36000);
			Player.AddBuff(BuffID.Regeneration, 36000);
			Player.AddBuff(BuffID.Lifeforce, 36000);
			Player.AddBuff(BuffID.Lucky, 36000);

			// Victory fireworks
			for (int i = 0; i < 50; i++)
			{
				int dustType = new[] { DustID.GoldCoin, DustID.RedTorch, DustID.BlueTorch, DustID.GreenTorch, DustID.YellowStarDust }[Main.rand.Next(5)];
				Dust dust = Dust.NewDustDirect(Player.Center + Main.rand.NextVector2Circular(40, 40), 8, 8, dustType, 0f, 0f, 50, default, 2f);
				dust.velocity = Main.rand.NextVector2Circular(6f, 8f);
				dust.velocity.Y -= 4f;
				dust.noGravity = true;
			}
		}
	}

	/// <summary>Hooks into enemy kills to award House Renown.</summary>
	public class HouseRenownOnKill : GlobalNPC
	{
		public override void OnKill(NPC npc)
		{
			if (npc.friendly || npc.townNPC || npc.lifeMax < 20)
				return;

			Player nearest = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
			if (nearest == null || !nearest.active)
				return;

			var wp = nearest.GetModPlayer<Players.WizardPlayer>();
			if (wp.houseSet == 0)
				return; // Must wear house armor

			var hps = nearest.GetModPlayer<HouseRenownSystem>();

			// Renown scales with enemy difficulty
			int points = 1;
			if (npc.boss)
				points = 50;
			else if (npc.lifeMax >= 1000)
				points = 5;
			else if (npc.lifeMax >= 300)
				points = 3;
			else if (npc.lifeMax >= 100)
				points = 2;

			hps.AddPoints(points);
		}
	}
}
