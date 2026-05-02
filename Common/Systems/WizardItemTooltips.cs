using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Adds a [Wizarding World] tag to all mod items' tooltips.
	/// Helps players identify which items belong to this mod in their inventory.
	/// Also adds tier indicators for wands based on damage.
	/// </summary>
	public class WizardItemTooltips : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation)
		{
			return entity.ModItem != null && entity.ModItem.Mod == ModContent.GetInstance<WizardingWorld>();
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			// Add mod tag at the bottom
			tooltips.Add(new TooltipLine(Mod, "ModTag", "[c/9B59B6:Wizarding World]"));

			// Add wand tier for wand items
			if (item.ModItem != null && item.ModItem.GetType().Namespace?.Contains("Wands") == true)
			{
				string tier = GetWandTier(item.damage);
				tooltips.Add(new TooltipLine(Mod, "WandTier", $"[c/888888:Wand Tier: {tier}]"));
			}
		}

		private string GetWandTier(int damage)
		{
			if (damage >= 140) return "Legendary";
			if (damage >= 90) return "Master";
			if (damage >= 50) return "Advanced";
			if (damage >= 30) return "Intermediate";
			if (damage >= 15) return "Basic";
			return "Starter";
		}
	}
}
