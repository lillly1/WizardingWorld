using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Wandlore System — Canon Tier B.
	///
	/// "The wand chooses the wizard." — Ollivander
	///
	/// Every wand has a Wood and Core that influence its behavior:
	/// - Wood determines spell school affinity (which spells get bonus damage)
	/// - Core determines casting style (crit chance, stability, burst potential)
	///
	/// This system tags existing wands without replacing them.
	/// Future: wand allegiance, mastery, personalized wand ceremony.
	/// </summary>
	public enum WandWood
	{
		Oak,        // Balanced, faithful. +5% all spell damage.
		Willow,     // Healing affinity. +10% Episkey/healing effectiveness.
		Ash,        // Fire affinity. +10% Incendio/fire spell damage.
		Holly,      // Defensive. +10% Protego/counter effectiveness. Harry's wand.
		Vine,       // Charms affinity. +10% utility spell effectiveness. Hermione's wand.
		Yew,        // Dark Arts affinity. +10% dark spell damage, -5% Patronus. Voldemort's wand.
		Elder,      // Raw power. +15% all damage. Fragile allegiance. The Deathstick.
		Elm,        // Dignified repair magic. +15% ward/utility restoration.
		Hawthorn,   // Conflicted. +8% all, best in hands of a skilled caster. Draco's wand.
		Blackthorn, // Dark warrior. +10% jinx/curse damage. Bellatrix's wand.
		Cedar,      // Perception. +10% detection/light spell effectiveness.
		Cypress,    // Nobility. +10% boss-fight effectiveness (damage vs bosses).
		Birch,      // Adaptable. +8% all, faster spell switching.
		Larch,      // Confidence. +10% utility spell effectiveness.
		RedOak,     // Quick reactions. +10% counter-spell effectiveness.
		Rowan,      // Protection. +10% defensive/barrier spell effectiveness.
		Alder,      // Steadfast. +5% all, bonus when at full HP.
		Ebony,      // Combat mastery. +10% all combat spells. Most powerful non-Elder wood.
	}

	public enum WandCore
	{
		PhoenixFeather,   // Reliable, versatile. +4% crit. Good mana regen. Loyal.
		DragonHeartstring, // Powerful, burst-oriented. +8% crit. Higher damage variance. Accident-prone.
		UnicornHair,       // Faithful, stable. +2% crit. Cannot be turned dark easily. -20% dark spell damage.
		ThestralTailHair,  // Rare, mastery of death. +5% crit. Bonus vs undead. Only for Elder Wand.
		BasiliskFang,      // Original. Venomous. +10% poison/venom damage. Tier C.
	}

	public enum SpellSchool
	{
		Charms,          // Wingardium, Accio, Lumos, general utility
		Defense,         // Protego, Expelliarmus, Finite Incantatem, Riddikulus
		DarkArts,        // Sectumsempra, Crucio, Avada Kedavra, Fiendfyre
		Transfiguration, // Future: object transformation spells
		Healing,         // Episkey, Vulnera Sanentur
		Utility,         // Alohomora, Apparition, Reparo (object repair)
		Summoning,       // Expecto Patronum, Avis, creature summons
		Fire,            // Incendio, Fiendfyre (also DarkArts), Bombarda
		CrowdControl,    // Impedimenta, Levicorpus, Stupefy, Petrificus Totalus
	}

	/// <summary>Registry mapping projectile types to their spell school.</summary>
	public class SpellSchoolRegistry : ModSystem
	{
		private static readonly System.Collections.Generic.Dictionary<int, SpellSchool> registry = new();

		public static void Register(int projectileType, SpellSchool school)
		{
			registry[projectileType] = school;
		}

		public static SpellSchool GetSchool(int projectileType)
		{
			return registry.TryGetValue(projectileType, out SpellSchool school) ? school : SpellSchool.Charms;
		}

		public override void PostSetupContent()
		{
			// Register all existing spell projectiles
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.StupefyProjectile>(), SpellSchool.CrowdControl);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.ExpelliarmusProjectile>(), SpellSchool.Defense);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.IncendioProjectile>(), SpellSchool.Fire);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.PatronusProjectile>(), SpellSchool.Summoning);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.SectumsempraProjectile>(), SpellSchool.DarkArts);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.AvadaKedavraProjectile>(), SpellSchool.DarkArts);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.ProtegoProjectile>(), SpellSchool.Defense);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.CrucioProjectile>(), SpellSchool.DarkArts);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.ReductoProjectile>(), SpellSchool.Charms);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.ImpedimentaProjectile>(), SpellSchool.CrowdControl);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.LevicorpusProjectile>(), SpellSchool.CrowdControl);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.BombardaProjectile>(), SpellSchool.Fire);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.FiendfyreProjectile>(), SpellSchool.DarkArts);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.EpiskeyProjectile>(), SpellSchool.Healing);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.ReparoProjectile>(), SpellSchool.Utility);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.RiddikulusProjectile>(), SpellSchool.Defense);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.FiniteIncantatemProjectile>(), SpellSchool.Defense);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.AccioProjectile>(), SpellSchool.Charms);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.ChainStupefyProjectile>(), SpellSchool.CrowdControl);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.ConjunctivitisProjectile>(), SpellSchool.CrowdControl);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.LumosMaximaProjectile>(), SpellSchool.Utility);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.WingardiumProjectile>(), SpellSchool.Charms);
			Register(ModContent.ProjectileType<Content.Projectiles.Spells.AguamentiProjectile>(), SpellSchool.Charms);
		}
	}
}
