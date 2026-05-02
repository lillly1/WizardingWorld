using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Wizard Info Display — shows wizard boss kill progress in the info accessories panel.
	/// Appears as "Wizard Bosses: X/12" next to GPS, compass, etc.
	/// Requires any mod accessory to be equipped (or the Boss Compass specifically).
	/// </summary>
	public class WizardInfoDisplay : InfoDisplay
	{
		public override bool Active()
		{
			// Show when player has any wizard accessory equipped
			Player player = Main.LocalPlayer;
			var wp = player.GetModPlayer<Players.WizardPlayer>();
			return wp.houseSet > 0 || wp.hasMaraudersMap || wp.hasTimeTurner || wp.hasInvisibilityCloak;
		}

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			int defeated = 0;
			if (DownedBossSystem.downedTroll) defeated++;
			if (DownedBossSystem.downedQuirrell) defeated++;
			if (DownedBossSystem.downedBasilisk) defeated++;
			if (DownedBossSystem.downedAragog) defeated++;
			if (DownedBossSystem.downedFluffy) defeated++;
			if (DownedBossSystem.downedHorntail) defeated++;
			if (DownedBossSystem.downedUmbridge) defeated++;
			if (DownedBossSystem.downedFenrir) defeated++;
			if (DownedBossSystem.downedBellatrix) defeated++;
			if (DownedBossSystem.downedBartyCrouch) defeated++;
			if (DownedBossSystem.downedVoldemort) defeated++;
			if (DownedBossSystem.downedDementorKing) defeated++;

			if (HallowsSystem.hallowsAttuned)
			{
				displayColor = new Color(255, 215, 0); // Gold
				return "Master of Death";
			}

			displayColor = new Color(155, 89, 182); // Purple
			return $"Wizard Bosses: {defeated}/12";
		}
	}
}
