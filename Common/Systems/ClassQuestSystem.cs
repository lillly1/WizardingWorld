using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Class Quest System — Canon Tier B.
	///
	/// Hogwarts-style class progression. Players complete tasks themed around
	/// magical subjects to earn House Standing and unlock advanced content.
	///
	/// Subjects:
	/// - Defence Against the Dark Arts (DADA): defeat specific enemies, learn counter-spells
	/// - Potions: brew specific potions, discover recipes
	/// - Care of Magical Creatures: study/discover Fantastic Beasts
	/// - Charms: execute spell combos, use utility spells
	///
	/// Each subject has 5 tiers of tasks (Year 1-5 equivalents).
	/// Completing all tasks in a subject grants a mastery title + permanent buff.
	///
	/// Future: OWL/NEWT exam challenges, professor NPCs, class schedules.
	/// </summary>
	public class ClassQuestPlayer : ModPlayer
	{
		// Subject mastery levels (0-5 for each)
		public int dadaMastery;
		public int potionsMastery;
		public int creaturesMastery;
		public int charmsMastery;

		// Task completion tracking
		public HashSet<string> completedTasks = new();

		public void CompleteTask(string taskId, string subject)
		{
			if (completedTasks.Contains(taskId))
				return;

			completedTasks.Add(taskId);

			// Award House Standing
			var hps = Player.GetModPlayer<HouseRenownSystem>();
			hps.AddPoints(15);

			if (Player.whoAmI == Main.myPlayer)
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ClassQuest.TaskComplete", taskId), 100, 200, 255);

			// Check for mastery level up
			CheckMasteryProgress(subject);
		}

		private void CheckMasteryProgress(string subject)
		{
			int taskCount = 0;
			foreach (string task in completedTasks)
			{
				if (task.StartsWith(subject))
					taskCount++;
			}

			int newLevel = taskCount / 3; // Every 3 tasks = 1 mastery level
			newLevel = System.Math.Min(newLevel, 5);

			switch (subject)
			{
				case "DADA":
					if (newLevel > dadaMastery)
					{
						dadaMastery = newLevel;
						if (Player.whoAmI == Main.myPlayer)
							Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ClassQuest.DADAMastery", dadaMastery), 255, 100, 100);
					}
					break;
				case "Potions":
					if (newLevel > potionsMastery)
					{
						potionsMastery = newLevel;
						if (Player.whoAmI == Main.myPlayer)
							Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ClassQuest.PotionsMastery", potionsMastery), 100, 200, 100);
					}
					break;
				case "Creatures":
					if (newLevel > creaturesMastery)
					{
						creaturesMastery = newLevel;
						if (Player.whoAmI == Main.myPlayer)
							Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ClassQuest.CreaturesMastery", creaturesMastery), 200, 150, 100);
					}
					break;
				case "Charms":
					if (newLevel > charmsMastery)
					{
						charmsMastery = newLevel;
						if (Player.whoAmI == Main.myPlayer)
							Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ClassQuest.CharmsMastery", charmsMastery), 100, 150, 255);
					}
					break;
			}
		}

		public override void PostUpdateEquips()
		{
			// Mastery bonuses (permanent once earned)
			if (dadaMastery >= 3)
				Player.GetDamage(ModContent.GetInstance<Content.DamageClasses.SpellDamage>()) += 0.05f;
			if (dadaMastery >= 5)
				Player.endurance += 0.05f;

			if (potionsMastery >= 3)
				Player.lifeRegen += 2;
			if (potionsMastery >= 5)
				Player.pStone = true; // Reduced potion cooldown

			if (creaturesMastery >= 3)
				Player.maxMinions += 1;
			if (creaturesMastery >= 5)
				Player.detectCreature = true;

			if (charmsMastery >= 3)
				Player.manaCost -= 0.05f;
			if (charmsMastery >= 5)
				Player.statManaMax2 += 30;
		}

		public override void SaveData(TagCompound tag)
		{
			tag["dadaMastery"] = dadaMastery;
			tag["potionsMastery"] = potionsMastery;
			tag["creaturesMastery"] = creaturesMastery;
			tag["charmsMastery"] = charmsMastery;
			tag["completedTasks"] = new List<string>(completedTasks);
		}

		public override void LoadData(TagCompound tag)
		{
			dadaMastery = tag.GetInt("dadaMastery");
			potionsMastery = tag.GetInt("potionsMastery");
			creaturesMastery = tag.GetInt("creaturesMastery");
			charmsMastery = tag.GetInt("charmsMastery");
			if (tag.ContainsKey("completedTasks"))
				completedTasks = new HashSet<string>(tag.GetList<string>("completedTasks"));
		}
	}
}
