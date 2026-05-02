using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Common.Players
{
	/// <summary>
	/// Dark Arts Corruption System — Canon Tier B.
	///
	/// Tracks how deeply a player has delved into dark magic.
	/// Corruption accumulates from casting Unforgivable Curses and using Horcruxes.
	///
	/// Design principle: "Dark magic increases power but causes corruption,
	/// instability, reduced healing, social penalties, Ministry suspicion,
	/// or Patronus weakness."
	///
	/// Corruption Effects (progressive):
	/// 0.0 - 0.3: Minor dark aura visual, slight damage boost
	/// 0.3 - 0.5: Patronus effectiveness -25%, NPC prices +10%
	/// 0.5 - 0.7: Patronus effectiveness -50%, random Paranoia (Confused 2s), NPC prices +25%
	/// 0.7 - 0.9: Patronus nearly useless, healing reduced -30%, dark visual intensifies
	/// 0.9 - 1.0: Maximum corruption — enormous dark power but Patronus impossible,
	///            healing -50%, NPCs refuse to sell, Ministry Suspicion triggers events
	///
	/// Corruption decays slowly over time (0.001/sec) and can be cleansed
	/// by Patronus casting, Phoenix Tears, or visiting a "purification" source.
	/// </summary>
	public class DarkArtsCorruptionPlayer : ModPlayer
	{
		public float corruption; // 0.0 to 1.0
		public int ministrySuspicion; // 0 to 100
		private int corruptionTickTimer;
		private int paranoidTimer;

		public override void ResetEffects()
		{
			// Natural decay — corruption fades slowly
			corruptionTickTimer++;
			if (corruptionTickTimer >= 60 && corruption > 0) // Every second
			{
				corruptionTickTimer = 0;
				corruption = Math.Max(0f, corruption - 0.001f);
			}
		}

		public override void PostUpdateEquips()
		{
			if (corruption <= 0.01f)
				return;

			// === TIER 1: 0.0 - 0.3 — Minor dark presence ===
			if (corruption > 0.05f)
			{
				// Slight dark power boost
				Player.GetDamage(ModContent.GetInstance<SpellDamage>()) += corruption * 0.08f;

				// Dark aura visual (subtle)
				if (Main.rand.NextBool(30))
				{
					Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Shadowflame, 0f, 0f, 200, default, 0.3f);
					dust.noGravity = true;
					dust.velocity *= 0.1f;
				}
			}

			// === TIER 2: 0.3 - 0.5 — Patronus weakening begins ===
			if (corruption > 0.3f)
			{
				// Patronus damage reduction handled in PatronusGuardian via this field
				// NPC price increase handled in shop hooks
			}

			// === TIER 3: 0.5 - 0.7 — Paranoia and social penalty ===
			if (corruption > 0.5f)
			{
				// Random paranoia — brief Confused every ~45 seconds
				paranoidTimer++;
				if (paranoidTimer >= 2700 && Main.rand.NextBool(3))
				{
					paranoidTimer = 0;
					Player.AddBuff(BuffID.Confused, 120); // 2 seconds
				}

				// Stronger dark aura
				if (Main.rand.NextBool(12))
				{
					Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Shadowflame, 0f, 0f, 150, default, 0.5f);
					dust.noGravity = true;
				}
			}

			// === TIER 4: 0.7 - 0.9 — Healing reduced ===
			if (corruption > 0.7f)
			{
				// Reduce life regen
				Player.lifeRegen = (int)(Player.lifeRegen * (1f - corruption * 0.3f));
			}

			// === TIER 5: 0.9 - 1.0 — Maximum corruption ===
			if (corruption > 0.9f)
			{
				// Massive dark power
				Player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.20f;
				Player.GetDamage(DamageClass.Generic) += 0.10f;

				// But healing devastated
				Player.lifeRegen = (int)(Player.lifeRegen * 0.5f);

				// Intense dark visual
				if (Main.rand.NextBool(5))
				{
					Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Shadowflame, 0f, -0.5f, 100, default, 0.8f);
					dust.noGravity = true;
				}
			}
		}

		/// <summary>Add corruption from casting dark spells or using Horcruxes.</summary>
		public void AddCorruption(float amount, string source = "dark magic")
		{
			float old = corruption;
			corruption = Math.Clamp(corruption + amount, 0f, 1f);
			ministrySuspicion = Math.Min(ministrySuspicion + (int)(amount * 30), 100);

			// Threshold crossing notifications
			if (Player.whoAmI == Main.myPlayer)
			{
				if (old < 0.3f && corruption >= 0.3f)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DarkArts.Tier2Warning"), 150, 50, 50);
				if (old < 0.5f && corruption >= 0.5f)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DarkArts.Tier3Warning"), 200, 50, 50);
				if (old < 0.7f && corruption >= 0.7f)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DarkArts.Tier4Warning"), 255, 30, 30);
				if (old < 0.9f && corruption >= 0.9f)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DarkArts.Tier5Warning"), 255, 0, 0);
			}
		}

		/// <summary>Cleanse corruption (from Patronus casting, Phoenix Tears, etc.)</summary>
		public void CleansCorruption(float amount)
		{
			corruption = Math.Max(0f, corruption - amount);
			if (Player.whoAmI == Main.myPlayer && amount > 0.1f)
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.DarkArts.Cleansed"), 100, 200, 100);
		}

		/// <summary>Get Patronus effectiveness multiplier (1.0 = full, 0.0 = none).</summary>
		public float GetPatronusEffectiveness()
		{
			if (corruption < 0.3f) return 1.0f;
			if (corruption < 0.5f) return 0.75f;
			if (corruption < 0.7f) return 0.50f;
			if (corruption < 0.9f) return 0.25f;
			return 0.05f; // Nearly impossible at max corruption
		}

		public override void SaveData(TagCompound tag)
		{
			tag["corruption"] = corruption;
			tag["ministrySuspicion"] = ministrySuspicion;
		}

		public override void LoadData(TagCompound tag)
		{
			corruption = tag.GetFloat("corruption");
			ministrySuspicion = tag.GetInt("ministrySuspicion");
		}

		public override void UpdateDead()
		{
			// Death slightly reduces corruption (a reset)
			corruption = Math.Max(0f, corruption - 0.05f);
			paranoidTimer = 0;
		}
	}
}
