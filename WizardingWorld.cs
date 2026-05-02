using Terraria.ModLoader;

namespace WizardingWorld
{
	public class WizardingWorld : Mod
	{
		public const string AssetPath = $"{nameof(WizardingWorld)}/";

		public enum MessageType : byte
		{
			SyncWizardPlayer
		}
	}
}
