using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Shows a message when the player enters/exits the Forbidden Forest biome.
	/// Also tracks biome state for the ambient system.
	/// </summary>
	public class BiomeMessages : ModPlayer
	{
		private bool wasInForest;

		public override void PostUpdate()
		{
			bool inForest = Player.InModBiome<Content.Biomes.ForbiddenForestBiome>();

			if (inForest && !wasInForest && Player.whoAmI == Main.myPlayer)
			{
				// Entered the Forbidden Forest
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Forest.Entered"), 100, 150, 80);

				if (DownedBossSystem.downedVoldemort)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Forest.EnteredPostVoldemort"), 150, 50, 50);
				else if (DownedBossSystem.downedHorntail)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Forest.EnteredPostHorntail"), 180, 100, 50);
			}
			else if (!inForest && wasInForest && Player.whoAmI == Main.myPlayer)
			{
				// Left the Forbidden Forest
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Forest.Exited"), 150, 180, 150);
			}

			wasInForest = inForest;
		}
	}
}
