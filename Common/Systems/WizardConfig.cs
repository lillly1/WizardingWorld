using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// ModConfig — lets players configure spawn rates, damage scaling, and event frequency.
	/// Accessible from the mod config menu in-game.
	/// </summary>
	public class WizardConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[Header("SpawnRates")]
		[Range(0.1f, 5.0f)]
		[DefaultValue(1.0f)]
		[Increment(0.1f)]
		public float EnemySpawnMultiplier { get; set; }

		[Range(0f, 5.0f)]
		[DefaultValue(1.0f)]
		[Increment(0.1f)]
		public float CritterSpawnMultiplier { get; set; }

		[Header("Difficulty")]
		[Range(0.5f, 5.0f)]
		[DefaultValue(1.0f)]
		[Increment(0.25f)]
		public float BossHealthMultiplier { get; set; }

		[Range(0.5f, 3.0f)]
		[DefaultValue(1.0f)]
		[Increment(0.1f)]
		public float SpellDamageMultiplier { get; set; }

		[Header("Events")]
		[Range(0f, 5.0f)]
		[DefaultValue(1.0f)]
		[Increment(0.5f)]
		public float InvasionChanceMultiplier { get; set; }

		[Range(0.5f, 5.0f)]
		[DefaultValue(1.0f)]
		[Increment(0.25f)]
		public float EssenceDropMultiplier { get; set; }
	}
}
