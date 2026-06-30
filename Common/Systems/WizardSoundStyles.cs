using Terraria.Audio;

namespace WizardingWorld.Common.Systems
{
	public static class WizardSoundStyles
	{
		private const string SpellPath = "WizardingWorld/Assets/Sounds/Spells/";
		private const string EnemyPath = "WizardingWorld/Assets/Sounds/Enemies/";
		private const string AmbientPath = "WizardingWorld/Assets/Sounds/Ambient/";

		private static SoundStyle Spell(string name, float volume = 0.75f, float pitchVariance = 0.08f, int maxInstances = 8)
		{
			return new SoundStyle(SpellPath + name)
			{
				Volume = volume,
				PitchVariance = pitchVariance,
				MaxInstances = maxInstances
			};
		}

		private static SoundStyle Enemy(string name, float volume = 0.8f, float pitchVariance = 0.06f, int maxInstances = 4)
		{
			return new SoundStyle(EnemyPath + name)
			{
				Volume = volume,
				PitchVariance = pitchVariance,
				MaxInstances = maxInstances
			};
		}

		private static SoundStyle Ambient(string name, float volume = 0.45f, float pitchVariance = 0.03f, int maxInstances = 3)
		{
			return new SoundStyle(AmbientPath + name)
			{
				Volume = volume,
				PitchVariance = pitchVariance,
				MaxInstances = maxInstances
			};
		}

		public static readonly SoundStyle Accio = Spell("Accio");
		public static readonly SoundStyle Aguamenti = Spell("Aguamenti", volume: 0.8f, maxInstances: 5);
		public static readonly SoundStyle Alohomora = Spell("Alohomora", volume: 0.7f, maxInstances: 4);
		public static readonly SoundStyle Apparition = Spell("Apparition", volume: 0.7f);
		public static readonly SoundStyle AvadaKedavra = Spell("AvadaKedavra", volume: 0.85f, pitchVariance: 0.04f);
		public static readonly SoundStyle Bombarda = Spell("Bombarda", volume: 0.85f, maxInstances: 5);
		public static readonly SoundStyle Conjunctivitis = Spell("Conjunctivitis", volume: 0.9f, maxInstances: 5);
		public static readonly SoundStyle Crucio = Spell("Crucio", volume: 0.8f, pitchVariance: 0.05f);
		public static readonly SoundStyle ExpectoPatronum = Spell("ExpectoPatronum", volume: 0.8f, pitchVariance: 0.04f);
		public static readonly SoundStyle Expelliarmus = Spell("Expelliarmus");
		public static readonly SoundStyle Fiendfyre = Spell("Fiendfyre", volume: 0.85f, maxInstances: 5);
		public static readonly SoundStyle FiniteIncantatem = Spell("FiniteIncantatem", volume: 0.7f);
		public static readonly SoundStyle Impedimenta = Spell("Impedimenta");
		public static readonly SoundStyle Incendio = Spell("Incendio", volume: 0.8f);
		public static readonly SoundStyle Levicorpus = Spell("Levicorpus");
		public static readonly SoundStyle Lumos = Spell("Lumos", volume: 0.65f);
		public static readonly SoundStyle Protego = Spell("Protego", volume: 0.75f, maxInstances: 4);
		public static readonly SoundStyle Reducto = Spell("Reducto", volume: 0.85f, maxInstances: 5);
		public static readonly SoundStyle Reparo = Spell("Reparo", volume: 0.7f, maxInstances: 4);
		public static readonly SoundStyle Riddikulus = Spell("Riddikulus");
		public static readonly SoundStyle Sectumsempra = Spell("Sectumsempra", volume: 1f, pitchVariance: 0.05f, maxInstances: 5);
		public static readonly SoundStyle Stupefy = Spell("Stupefy");
		public static readonly SoundStyle Wingardium = Spell("Wingardium", volume: 0.7f, maxInstances: 5);

		public static readonly SoundStyle DementorScream = Enemy("DementorScream", volume: 0.85f, pitchVariance: 0.04f);
		public static readonly SoundStyle DragonRoar = Enemy("DragonRoar", volume: 0.9f, pitchVariance: 0.04f);
		public static readonly SoundStyle GhostWail = Enemy("GhostWail", volume: 0.8f, pitchVariance: 0.04f);
		public static readonly SoundStyle SpiderHiss = Enemy("SpiderHiss");
		public static readonly SoundStyle TrollRoar = Enemy("TrollRoar", volume: 0.9f, pitchVariance: 0.04f);
		public static readonly SoundStyle WerewolfHowl = Enemy("WerewolfHowl", volume: 0.85f, pitchVariance: 0.04f);

		public static readonly SoundStyle CauldronBubble = Ambient("CauldronBubble", volume: 0.4f, maxInstances: 2);
		public static readonly SoundStyle ForestWind = Ambient("ForestWind", volume: 0.35f, maxInstances: 1);
		public static readonly SoundStyle MagicHum = Ambient("MagicHum", volume: 0.35f, maxInstances: 1);
		public static readonly SoundStyle OwlHoot = Ambient("OwlHoot", volume: 0.65f);
	}
}
