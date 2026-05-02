using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Adds loading screen tips about the Wizarding World mod.
	/// These appear during world generation and loading screens.
	/// </summary>
	public class LoadingTips : ModSystem
	{
		public override void PostSetupContent()
		{
			// Tips are added via localization in the .hjson file
			// under the key Mods.WizardingWorld.LoadingTips.*
			// tModLoader will automatically pick them up
		}
	}
}
