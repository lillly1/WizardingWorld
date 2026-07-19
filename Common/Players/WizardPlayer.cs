using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Common.Players
{
	public class WizardPlayer : ModPlayer
	{
		// Spell power bonus (additive percentage)
		public float spellPowerBonus;

		// Special effects from accessories
		public bool hasElderWand;
		public bool hasInvisibilityCloak;
		public bool hasDemiguiseCloak;
		public bool hasResurrectionStone;
		public bool hasGauntsRing;
		public bool hasAzkabanWardensKey;
		public bool hasWardOfHope;
		public bool hasTimeTurner;
		public bool hasMaraudersMap;
		public bool hasDeathlyHallows;

		// Dodge cooldown for Time Turner
		public int timeTurnerCooldown;
		public int hallowsRespiteCooldown;

		// Patronus active state
		public bool patronusActive;
		public int patronusTimer;

		// Despair state — used by Azkaban and Dementor content.
		public float despair;
		public bool debugGodMode;

		// House affiliation for armor bonuses
		public int houseSet; // 0=none, 1=Gryffindor, 2=Slytherin, 3=Ravenclaw, 4=Hufflepuff, 5=DarkWizard

		// Wand Mastery — "The wand chooses the wizard"
		public Dictionary<int, int> wandMasteryXP = new(); // itemType -> XP
		public int activeWandType; // currently held wand type for tooltip display

		public override void ResetEffects()
		{
			spellPowerBonus = 0f;
			hasElderWand = false;
			hasInvisibilityCloak = false;
			hasDemiguiseCloak = false;
			hasResurrectionStone = false;
			hasGauntsRing = false;
			hasAzkabanWardensKey = false;
			hasWardOfHope = false;
			hasTimeTurner = false;
			hasMaraudersMap = false;
			hasDeathlyHallows = false;
			patronusActive = false;
			houseSet = 0;
		}

		public override void PostUpdateEquips()
		{
			// Apply spell power bonus
			if (spellPowerBonus > 0f)
				Player.GetDamage(ModContent.GetInstance<SpellDamage>()) += spellPowerBonus;

			// Time Turner dodge cooldown
			if (timeTurnerCooldown > 0)
				timeTurnerCooldown--;

			if (hallowsRespiteCooldown > 0)
				hallowsRespiteCooldown--;

			if (debugGodMode)
				ApplyDebugGodMode();

			// Patronus timer
			if (patronusTimer > 0)
			{
				patronusTimer--;
				patronusActive = patronusTimer > 0;
			}

			// Marauder's Map: reveal enemies
			if (hasMaraudersMap)
				Player.detectCreature = true;

			UpdateDespair();
			Common.Systems.HallowsSystem.ResolveActiveHallows(Player, this);

			// Deathly Hallows: MASTER OF DEATH — Canon Tier B.
			//
			// REDESIGNED: "Master of Death" means accepting death, not cheating it.
			// The canon interpretation is about wisdom and humility, not raw power.
			//
			// Effects are SURVIVAL and FATE oriented, not DPS:
			// - Immune to instant death, soul drain, fear, petrification
			// - Death prevention: resurrect once per day at 50% HP
			// - See all hidden things (traps, secrets, invisible enemies)
			// - Greatly reduced aggro (Death does not seek you)
			// - Corruption immunity (the Hallows purify)
			// - Moderate (not overwhelming) stat boosts
			//
			// Requires: Elder Wand in inventory + true Invisibility Cloak equipped + Resurrection Stone equipped
			if (hasDeathlyHallows)
			{
				// Survival — the core of the Master of Death identity
				Player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.PetrifiedDebuff>()] = true;
				Player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.JinxedDebuff>()] = true;
				Player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.DarkCurseDebuff>()] = true;
				Player.buffImmune[Terraria.ID.BuffID.Frozen] = true;
				Player.buffImmune[Terraria.ID.BuffID.Stoned] = true;
				Player.noKnockback = true;

				// Perception — see all hidden things
				Player.detectCreature = true;
				Player.dangerSense = true;
				Player.findTreasure = true;
				Player.nightVision = true;

				// Death's humility — reduced aggro, not increased power
				Player.aggro -= 600;

				// Corruption immunity — the Hallows purify
				var darkPlayer = Player.GetModPlayer<Players.DarkArtsCorruptionPlayer>();
				darkPlayer.CleansCorruption(0.002f); // Slow constant cleansing

				// Moderate stat boosts (NOT the overwhelming bonuses from before)
				Player.statDefense += 10;
				Player.lifeRegen += 4;
				Player.endurance += 0.08f;
				Player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.10f;

				// Subtle golden aura — dignified, not flashy
				if (Main.rand.NextBool(12))
				{
					Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height,
						Terraria.ID.DustID.GoldCoin, 0f, -0.5f, 100, default, 0.4f);
					dust.noGravity = true;
				}
			}
		}

		public override bool ConsumableDodge(Player.HurtInfo info)
		{
			if (debugGodMode)
			{
				Player.SetImmuneTimeForAllTypes(300);
				return true;
			}

			if (hasTimeTurner && timeTurnerCooldown <= 0)
			{
				timeTurnerCooldown = 600; // 10 second cooldown
				Player.SetImmuneTimeForAllTypes(Player.longInvince ? 120 : 80);

				// Visual effect — gold dust burst
				for (int i = 0; i < 20; i++)
				{
					Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, Terraria.ID.DustID.IchorTorch, 0f, 0f, 100, default, 1.5f);
					dust.velocity *= 2f;
					dust.noGravity = true;
				}

				return true;
			}

			if (hasDeathlyHallows && hallowsRespiteCooldown <= 0 && info.Damage >= Player.statLife)
			{
				hallowsRespiteCooldown = 60 * 60 * 24; // Roughly one Terraria day
				int rescuedLife = Math.Max(Player.statLifeMax2 / 2, 1);
				int healAmount = Math.Max(rescuedLife - Player.statLife, 0);
				Player.statLife = rescuedLife;
				if (healAmount > 0)
					Player.HealEffect(healAmount);
				Player.SetImmuneTimeForAllTypes(Player.longInvince ? 180 : 120);

				for (int i = 0; i < 30; i++)
				{
					Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, Terraria.ID.DustID.GoldFlame, 0f, -0.2f, 100, default, 1.4f);
					dust.velocity *= 1.8f;
					dust.noGravity = true;
				}

				if (Player.whoAmI == Main.myPlayer)
				Main.NewText(Common.Systems.HallowsSystem.HallowsText("DeathRespite", "The Hallows pull you back from the edge of death."), 255, 235, 180);

				return true;
			}

			return false;
		}

		public override bool FreeDodge(Player.HurtInfo info)
		{
			if (!debugGodMode)
				return false;

			Player.statLife = Player.statLifeMax2;
			Player.SetImmuneTimeForAllTypes(300);
			return true;
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (debugGodMode)
				modifiers.FinalDamage *= 0f;
		}

		public override void UpdateDead()
		{
			timeTurnerCooldown = 0;
			patronusTimer = 0;
			patronusActive = false;
			hallowsRespiteCooldown = 0;
			despair = Math.Max(0f, despair - 0.10f);
		}

		// --- Wand Mastery ---

		private void ApplyDebugGodMode()
		{
			Player.statLife = Player.statLifeMax2;
			Player.statMana = Player.statManaMax2;
			Player.immune = true;
			Player.immuneNoBlink = true;
			Player.immuneTime = Math.Max(Player.immuneTime, 300);
			Player.noKnockback = true;
			Player.noFallDmg = true;
			Player.lavaImmune = true;
			Player.fireWalk = true;
			Player.buffImmune[Terraria.ID.BuffID.OnFire] = true;
			Player.buffImmune[Terraria.ID.BuffID.OnFire3] = true;
			Player.buffImmune[Terraria.ID.BuffID.Burning] = true;
			Player.buffImmune[Terraria.ID.BuffID.Poisoned] = true;
			Player.buffImmune[Terraria.ID.BuffID.Venom] = true;
			Player.buffImmune[Terraria.ID.BuffID.Confused] = true;
			Player.buffImmune[Terraria.ID.BuffID.Cursed] = true;
			Player.buffImmune[Terraria.ID.BuffID.Darkness] = true;
			Player.buffImmune[Terraria.ID.BuffID.Blackout] = true;
			Player.buffImmune[Terraria.ID.BuffID.Bleeding] = true;
			Player.buffImmune[Terraria.ID.BuffID.BrokenArmor] = true;
			Player.buffImmune[Terraria.ID.BuffID.Frozen] = true;
			Player.buffImmune[Terraria.ID.BuffID.Stoned] = true;
			Player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.PetrifiedDebuff>()] = true;
			Player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.JinxedDebuff>()] = true;
			Player.buffImmune[ModContent.BuffType<Content.Buffs.Debuffs.DarkCurseDebuff>()] = true;
		}

		private void UpdateDespair()
		{
			float decay = 0.0007f;
			if (patronusActive)
				decay += 0.0025f;
			if (hasAzkabanWardensKey)
				decay += 0.0015f;
			if (hasWardOfHope)
				decay += 0.0015f;

			despair = Math.Max(0f, despair - decay);

			if (despair < 0.10f)
				return;

			// Low despair: unease, dimmed healing.
			if (despair >= 0.10f)
			{
				Player.lifeRegen -= 2;
			}

			// Medium despair: darkness, weakness, and creeping panic.
			if (despair >= 0.35f)
			{
				Player.moveSpeed -= 0.08f;
				Player.manaRegenDelay += 10;
				if (Main.rand.NextBool(180))
					Player.AddBuff(Terraria.ID.BuffID.Darkness, 60);
			}

			// High despair: crushing hopelessness.
			if (despair >= 0.65f)
			{
				Player.statDefense -= 6;
				Player.endurance -= 0.05f;
				if (Main.rand.NextBool(240))
					Player.AddBuff(Terraria.ID.BuffID.Slow, 90);
			}

			if (despair >= 0.85f && Main.rand.NextBool(300))
				Player.AddBuff(Terraria.ID.BuffID.Blackout, 60);

			if (despair >= 0.25f && Main.rand.NextBool(20))
			{
				Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, Terraria.ID.DustID.Shadowflame, 0f, 0f, 180, default, 0.4f + despair * 0.4f);
				dust.noGravity = true;
				dust.velocity *= 0.15f;
			}
		}

		public void AddDespair(float amount, string source = "despair")
		{
			float mitigation = 0f;
			if (patronusActive)
				mitigation += 0.45f;
			if (hasAzkabanWardensKey)
				mitigation += 0.25f;
			if (hasWardOfHope)
				mitigation += 0.20f;

			float finalAmount = amount * Math.Max(0.15f, 1f - mitigation);
			float old = despair;
			despair = Math.Clamp(despair + finalAmount, 0f, 1f);

			if (Player.whoAmI != Main.myPlayer)
				return;

			if (old < 0.35f && despair >= 0.35f)
				Main.NewText(Common.Systems.HallowsSystem.HallowsText("DespairMedium", "{0} presses against your thoughts.", source), 120, 120, 220);
			if (old < 0.65f && despair >= 0.65f)
				Main.NewText(Common.Systems.HallowsSystem.HallowsText("DespairHigh", "Despair closes in. Hold to your Patronus."), 180, 180, 255);
			if (old < 0.85f && despair >= 0.85f)
				Main.NewText(Common.Systems.HallowsSystem.HallowsText("DespairCritical", "The darkness is almost complete."), 220, 220, 255);
		}

		public void RelieveDespair(float amount)
		{
			despair = Math.Max(0f, despair - amount);
		}

		public int GetWandMasteryLevel(int itemType)
		{
			int xp = wandMasteryXP.TryGetValue(itemType, out int storedXp) ? storedXp : 0;
			if (xp >= 1000) return 3; // Mastered
			if (xp >= 400) return 2;  // Attuned
			if (xp >= 100) return 1;  // Familiar
			return 0;                  // New
		}

		public void AddWandMasteryXP(int itemType, int amount)
		{
			int current = wandMasteryXP.TryGetValue(itemType, out int storedXp) ? storedXp : 0;
			wandMasteryXP[itemType] = Math.Min(current + amount, 1500); // cap at 1500
		}

		public override void SaveData(TagCompound tag)
		{
			var masteryList = new List<TagCompound>();
			foreach (var kvp in wandMasteryXP)
			{
				var entry = new TagCompound();
				entry["type"] = kvp.Key;
				entry["xp"] = kvp.Value;
				masteryList.Add(entry);
			}
			tag["wandMastery"] = masteryList;
			tag["despair"] = despair;
			tag["hallowsRespiteCooldown"] = hallowsRespiteCooldown;
		}

		public override void LoadData(TagCompound tag)
		{
			wandMasteryXP.Clear();
			if (tag.ContainsKey("wandMastery"))
			{
				var list = tag.GetList<TagCompound>("wandMastery");
				foreach (var entry in list)
				{
					wandMasteryXP[entry.GetInt("type")] = entry.GetInt("xp");
				}
			}

			despair = tag.ContainsKey("despair") ? tag.GetFloat("despair") : 0f;
			hallowsRespiteCooldown = tag.ContainsKey("hallowsRespiteCooldown") ? tag.GetInt("hallowsRespiteCooldown") : 0;

			// Legacy migration: old Elder Wand variants now fold into the true Elder Wand mastery.
			int elderWand = ModContent.ItemType<Content.Items.Weapons.Wands.ElderWand>();
#pragma warning disable CS0618
			int legacyWandOfDestiny = ModContent.ItemType<Content.Items.Weapons.Wands.WandOfDestiny>();
			int legacyShadowElder = ModContent.ItemType<Content.Items.Weapons.Wands.ShadowElderWand>();
#pragma warning restore CS0618

			if (wandMasteryXP.TryGetValue(legacyWandOfDestiny, out int destinyXp))
			{
				int elderXp = wandMasteryXP.TryGetValue(elderWand, out int storedElderXp) ? storedElderXp : 0;
				wandMasteryXP[elderWand] = Math.Max(elderXp, destinyXp);
				wandMasteryXP.Remove(legacyWandOfDestiny);
			}

			if (wandMasteryXP.TryGetValue(legacyShadowElder, out int shadowXp))
			{
				int elderXp = wandMasteryXP.TryGetValue(elderWand, out int storedShadowMergedXp) ? storedShadowMergedXp : 0;
				wandMasteryXP[elderWand] = Math.Max(elderXp, shadowXp);
				wandMasteryXP.Remove(legacyShadowElder);
			}
		}
	}
}
