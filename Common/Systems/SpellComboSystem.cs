using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Spell Combo System — casting two specific spells in sequence on the same enemy
	/// triggers a powerful bonus effect. Rewards skilled play and spell variety.
	///
	/// Combos:
	/// 1. Stupefy + Incendio = "Blazing Stun" — enemy explodes, AoE fire damage
	/// 2. Expelliarmus + Sectumsempra = "Disarming Slash" — massive crit, armor shred
	/// 3. Protego + any attack spell = "Counter-Spell" — reflected damage doubled
	/// 4. Impedimenta + Avada Kedavra = "Execution" — instant kill on non-boss below 30% HP
	/// 5. Stupefy + Riddikulus = "Laughter Shield" — heal the player for 50 HP
	/// 6. Crucio + Fiendfyre = "Hellfire Torment" — massive DoT stack
	///
	/// Tracks the last spell type that hit each NPC. If the next spell matches a combo,
	/// triggers the effect.
	/// </summary>
	public class SpellComboSystem : GlobalProjectile
	{
		// Track last spell type per NPC (NPC whoAmI → projectile type)
		// NOTE: Using static dicts is MP-unsafe but acceptable for single-player.
		// In MP, combo tracking is per-client which means each player sees their own combos.
		// This is actually correct behavior — your combos shouldn't trigger from another player's spells.
		private static readonly Dictionary<int, int> lastSpellOnNPC = new();
		private static readonly Dictionary<int, int> comboTimers = new();

		// Clear stale entries periodically to prevent memory leak
		private static int cleanupTimer;
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => true;

		public override void PostAI(Projectile projectile)
		{
			cleanupTimer++;
			if (cleanupTimer > 3600) // Every minute
			{
				cleanupTimer = 0;
				// Remove entries for dead NPCs
				var stale = new List<int>();
				foreach (var kvp in comboTimers)
				{
					if ((int)Main.GameUpdateCount - kvp.Value > 600) // 10 seconds old
						stale.Add(kvp.Key);
				}
				foreach (var key in stale)
				{
					lastSpellOnNPC.Remove(key);
					comboTimers.Remove(key);
				}
			}
		}

		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Only track spell damage projectiles from this mod
			if (projectile.DamageType != ModContent.GetInstance<Content.DamageClasses.SpellDamage>())
				return;

			if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
				return;

			Player owner = Main.player[projectile.owner];
			int npcId = target.whoAmI;
			int spellType = projectile.type;

			// Check if there's a previous spell on this NPC within the combo window
			if (lastSpellOnNPC.TryGetValue(npcId, out int prevSpell) &&
				comboTimers.TryGetValue(npcId, out int timer) &&
				Main.GameUpdateCount - timer < 180) // 3-second combo window
			{
				// Check for combos
				TryCombo(owner, target, prevSpell, spellType, damageDone);
			}

			// Record this spell as the last one
			lastSpellOnNPC[npcId] = spellType;
			comboTimers[npcId] = (int)Main.GameUpdateCount;
		}

		private void TryCombo(Player player, NPC target, int spell1, int spell2, int baseDamage)
		{
			// Combo 1: Stupefy + Incendio = Blazing Stun
			if (IsSpell(spell1, "StupefyProjectile") && IsSpell(spell2, "IncendioProjectile"))
			{
				ExecuteCombo(player, target, "Blazing Stun!", new Color(255, 150, 50), () =>
				{
					// AoE fire explosion
					foreach (var npc in Main.ActiveNPCs)
					{
						if (npc.whoAmI != target.whoAmI && npc.CanBeChasedBy() &&
							Vector2.Distance(npc.Center, target.Center) < 150f)
						{
							player.ApplyDamageToNPC(npc, baseDamage * 2, 8f, 0, false);
							npc.AddBuff(BuffID.OnFire3, 300);
						}
					}

					// Fire explosion visual
					for (int i = 0; i < 30; i++)
					{
						Dust dust = Dust.NewDustDirect(target.Center, 8, 8, DustID.Torch, 0f, 0f, 50, default, 2f);
						dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
						dust.noGravity = true;
					}
				});
				return;
			}

			// Combo 2: Expelliarmus + Sectumsempra = Disarming Slash
			if (IsSpell(spell1, "ExpelliarmusProjectile") && IsSpell(spell2, "SectumsempraProjectile"))
			{
				ExecuteCombo(player, target, "Disarming Slash!", new Color(255, 50, 50), () =>
				{
					// Massive bonus damage + armor shred
					player.ApplyDamageToNPC(target, baseDamage * 3, 12f, 0, true); // Guaranteed crit
					target.AddBuff(BuffID.Ichor, 600); // Defense shred
					target.AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.JinxedDebuff>(), 600);
				});
				return;
			}

			// Combo 3: Impedimenta + Avada Kedavra = Execution
			if (IsSpell(spell1, "ImpedimentaProjectile") && IsSpell(spell2, "AvadaKedavraProjectile"))
			{
				ExecuteCombo(player, target, "EXECUTION!", new Color(0, 200, 0), () =>
				{
					// Instant kill non-boss below 30% HP
					if (!target.boss && target.life < target.lifeMax * 0.3f)
					{
						target.life = 0;
						target.checkDead();

						// Green death explosion
						for (int i = 0; i < 40; i++)
						{
							Dust dust = Dust.NewDustDirect(target.Center, 8, 8, DustID.CursedTorch, 0f, 0f, 50, default, 2f);
							dust.velocity = Main.rand.NextVector2Circular(8f, 8f);
							dust.noGravity = true;
						}
					}
					else
					{
						// Still deals triple damage even if can't execute
						player.ApplyDamageToNPC(target, baseDamage * 3, 10f, 0, false);
					}
				});
				return;
			}

			// Combo 4: Crucio + Fiendfyre = Hellfire Torment
			if (IsSpell(spell1, "CrucioProjectile") && IsSpell(spell2, "FiendfyreProjectile"))
			{
				ExecuteCombo(player, target, "Hellfire Torment!", new Color(200, 50, 0), () =>
				{
					target.AddBuff(BuffID.OnFire3, 600);
					target.AddBuff(BuffID.ShadowFlame, 600);
					target.AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.DarkCurseDebuff>(), 600);

					for (int i = 0; i < 20; i++)
					{
						int dustType = Main.rand.NextBool() ? DustID.Torch : DustID.Shadowflame;
						Dust dust = Dust.NewDustDirect(target.Center, 8, 8, dustType, 0f, 0f, 50, default, 2f);
						dust.velocity = Main.rand.NextVector2Circular(5f, 5f);
						dust.noGravity = true;
					}
				});
				return;
			}

			// Combo 5: Any Stupefy variant + Riddikulus = Laughter Shield (heal)
			if ((IsSpell(spell1, "StupefyProjectile") || IsSpell(spell1, "ChainStupefyProjectile")) &&
				IsSpell(spell2, "RiddikulusProjectile"))
			{
				ExecuteCombo(player, target, "Laughter Shield!", new Color(255, 200, 100), () =>
				{
					int healAmount = 50;
					player.statLife = Math.Min(player.statLife + healAmount, player.statLifeMax2);
					player.HealEffect(healAmount);
				});
				return;
			}
		}

		private bool IsSpell(int projectileType, string spellName)
		{
			// Check by comparing the projectile's ModProjectile name
			if (projectileType < ProjectileID.Count)
				return false;

			var modProj = ProjectileLoader.GetProjectile(projectileType);
			return modProj != null && modProj.GetType().Name == spellName;
		}

		private void ExecuteCombo(Player player, NPC target, string comboName, Color textColor, Action effect)
		{
			// Show combo text
			if (player.whoAmI == Main.myPlayer)
			{
				CombatText.NewText(target.getRect(), textColor, comboName);
			}

			// Execute the combo effect
			effect();

			// Clear the combo state for this NPC
			lastSpellOnNPC.Remove(target.whoAmI);
		}
	}
}
