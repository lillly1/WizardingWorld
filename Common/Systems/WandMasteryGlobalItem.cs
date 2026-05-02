using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Wand Mastery — tracks usage and applies mastery bonuses to wands.
	///
	/// Mastery Levels:
	///   Level 0 (New): No bonus
	///   Level 1 (Familiar, 100 XP): -5% mana cost
	///   Level 2 (Attuned, 400 XP): -10% mana cost, +5% damage
	///   Level 3 (Mastered, 1000 XP): -15% mana cost, +10% damage, empowered visual
	///
	/// XP is gained by hitting enemies with the wand's projectiles (1 XP per hit).
	/// Canon-inspired: "The wand chooses the wizard" — loyalty through use.
	/// </summary>
	public class WandMasteryGlobalItem : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation)
		{
			return entity.DamageType == ModContent.GetInstance<SpellDamage>() && entity.damage > 1;
		}

		public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
		{
			var wp = player.GetModPlayer<Players.WizardPlayer>();
			int level = wp.GetWandMasteryLevel(item.type);

			if (level >= 2) damage += 0.05f;
			if (level >= 3) damage += 0.05f; // total +10% at level 3
		}

		public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
		{
			var wp = player.GetModPlayer<Players.WizardPlayer>();
			int level = wp.GetWandMasteryLevel(item.type);

			if (level >= 1) reduce -= 0.05f;
			if (level >= 2) reduce -= 0.05f;  // total -10% at level 2
			if (level >= 3) reduce -= 0.05f;  // total -15% at level 3
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (Main.LocalPlayer == null) return;

			var wp = Main.LocalPlayer.GetModPlayer<Players.WizardPlayer>();
			int level = wp.GetWandMasteryLevel(item.type);
			int xp = wp.wandMasteryXP.TryGetValue(item.type, out int storedItemXp) ? storedItemXp : 0;

			string levelName = level switch
			{
				0 => "New",
				1 => "Familiar",
				2 => "Attuned",
				3 => "Mastered",
				_ => "Unknown"
			};

			string color = level switch
			{
				0 => "AAAAAA",
				1 => "55FF55",
				2 => "5555FF",
				3 => "FFD700",
				_ => "FFFFFF"
			};

			int nextThreshold = level switch
			{
				0 => 100,
				1 => 400,
				2 => 1000,
				_ => 1000
			};

			string progressText = level < 3
				? $" ({xp}/{nextThreshold} XP)"
				: " (MAX)";

			tooltips.Add(new TooltipLine(Mod, "WandMastery",
				$"[c/{color}:Wand Mastery: {levelName}{progressText}]"));

			if (level >= 1)
				tooltips.Add(new TooltipLine(Mod, "MasteryBonus",
					$"[c/{color}:Mastery: -{level * 5}% mana cost{(level >= 2 ? $", +{(level - 1) * 5}% damage" : "")}]"));

			if (level >= 3)
			{
				if (item.type == ModContent.ItemType<Content.Items.Weapons.Wands.ElderWand>())
				{
					tooltips.Add(new TooltipLine(Mod, "MasteryLegend",
						"[c/FFD700:The Elder Wand now recognizes the old title: Wand of Destiny]"));
				}
				else if (item.type == ModContent.ItemType<Content.Items.Weapons.Wands.RedOakWand>())
				{
					tooltips.Add(new TooltipLine(Mod, "MasteryFear",
						"[c/FFD700:Mastery: Riddikulus spreads laughter to nearby fear-creatures]"));
				}
				else if (item.type == ModContent.ItemType<Content.Items.Weapons.Wands.UnicornHairWand>())
				{
					tooltips.Add(new TooltipLine(Mod, "MasteryPatronus",
						"[c/FFD700:Mastery: Patronus burns away despair and hunts dark creatures farther away]"));
				}
			}
		}
	}
}
