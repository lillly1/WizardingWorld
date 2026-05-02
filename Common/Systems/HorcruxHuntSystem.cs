using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using WizardingWorld.Content.Items.Accessories;
using WizardingWorld.Content.Items.Weapons;
using WizardingWorld.Content.Items.Weapons.Wands;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Horcrux Hunt System — Canon Tier A.
	///
	/// "You will have to find and destroy all the remaining Horcruxes." — Dumbledore
	///
	/// This is the mod's narrative endgame mechanic:
	/// - Players must DESTROY Horcrux accessory items using a Basilisk Fang or Sword of Gryffindor
	/// - Each destroyed Horcrux weakens Voldemort permanently
	/// - Voldemort's HP, damage, and defense scale down per Horcrux destroyed
	/// - Destroying all 4 Horcruxes makes Voldemort vulnerable to final defeat
	///   and unlocks purification of Gaunt's Ring into the Resurrection Stone
	///
	/// Horcrux destruction is PERMANENT per world (saved to world data).
	/// Destroyed Horcruxes cannot be re-obtained — they're gone forever.
	/// This creates genuine sacrifice: you lose powerful (but corrupting) accessories
	/// to make the final boss beatable.
	///
	/// Questline progression:
	/// 1. Obtain Horcrux accessories (crafting or drops)
	/// 2. Obtain a destruction tool (Basilisk Fang or Sword of Gryffindor)
	/// 3. Right-click each Horcrux to destroy it (requires destruction tool in inventory)
	/// 4. Destroying all 4 unlocks Gaunt's Ring purification → Resurrection Stone
	/// 5. Gather all 3 Deathly Hallows (Elder Wand, Invisibility Cloak, Resurrection Stone)
	/// 6. Hallows Attunement grants mastery over death — the ultimate reward
	///
	/// Scaling:
	/// 0 destroyed: Voldemort at 100% power (60,000 HP, near-impossible)
	/// 1 destroyed: 85% power (51,000 HP)
	/// 2 destroyed: 70% power (42,000 HP)
	/// 3 destroyed: 55% power (33,000 HP)
	/// 4 destroyed: 40% power (24,000 HP) — fair fight, as intended
	///
	/// "Neither can live while the other survives."
	/// </summary>
	public class HorcruxHuntSystem : ModSystem
	{
		public static int horcruxesDestroyed; // 0 to 4
		public static bool diaryDestroyed;
		public static bool locketDestroyed;
		public static bool cupDestroyed;
		public static bool diademDestroyed;
		public static bool naginiDefeated;

		public static bool AllCoreHorcruxesDestroyed =>
			diaryDestroyed && locketDestroyed && cupDestroyed && diademDestroyed;

		public override void ClearWorld()
		{
			horcruxesDestroyed = 0;
			diaryDestroyed = false;
			locketDestroyed = false;
			cupDestroyed = false;
			diademDestroyed = false;
			naginiDefeated = false;
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["horcruxesDestroyed"] = horcruxesDestroyed;
			tag["diaryDestroyed"] = diaryDestroyed;
			tag["locketDestroyed"] = locketDestroyed;
			tag["cupDestroyed"] = cupDestroyed;
			tag["diademDestroyed"] = diademDestroyed;
			tag["naginiDefeated"] = naginiDefeated;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			horcruxesDestroyed = tag.GetInt("horcruxesDestroyed");
			diaryDestroyed = tag.GetBool("diaryDestroyed");
			locketDestroyed = tag.GetBool("locketDestroyed");
			cupDestroyed = tag.GetBool("cupDestroyed");
			diademDestroyed = tag.GetBool("diademDestroyed");
			naginiDefeated = tag.GetBool("naginiDefeated");
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(horcruxesDestroyed);
			var flags = new BitsByte();
			flags[0] = diaryDestroyed;
			flags[1] = locketDestroyed;
			flags[2] = cupDestroyed;
			flags[3] = diademDestroyed;
			flags[4] = naginiDefeated;
			writer.Write(flags);
		}

		public override void NetReceive(BinaryReader reader)
		{
			horcruxesDestroyed = reader.ReadInt32();
			BitsByte flags = reader.ReadByte();
			diaryDestroyed = flags[0];
			locketDestroyed = flags[1];
			cupDestroyed = flags[2];
			diademDestroyed = flags[3];
			naginiDefeated = flags[4];
		}

		/// <summary>Get Voldemort's power multiplier based on Horcruxes destroyed.</summary>
		public static float GetVoldemortPowerMultiplier()
		{
			float multiplier = horcruxesDestroyed switch
			{
				0 => 1.00f,
				1 => 0.85f,
				2 => 0.70f,
				3 => 0.55f,
				_ => 0.40f, // 4 destroyed = 40% power
			};

			if (naginiDefeated)
				multiplier -= 0.05f;

			return Math.Max(0.35f, multiplier);
		}

		public static int GetTeleportIntervalTicks(int phase)
		{
			int baseTicks = phase switch
			{
				0 => 300,
				1 => 180,
				_ => 120
			};

			int reduction = horcruxesDestroyed * 18;
			if (naginiDefeated)
				reduction += 30;
			if (HallowsSystem.resurrectionStoneAwakened)
				reduction += 20;

			return Math.Min(baseTicks + reduction, baseTicks + 90);
		}

		public static int GetPhase2MinionCount()
		{
			int count = 3 - horcruxesDestroyed / 2;
			if (naginiDefeated)
				count--;

			return Math.Max(1, count);
		}

		public static int GetPhase3BoltCount()
		{
			int count = 12 - horcruxesDestroyed;
			if (naginiDefeated)
				count -= 2;
			if (HallowsSystem.resurrectionStoneAwakened)
				count -= 1;

			return Math.Max(6, count);
		}

		public static bool HorcruxShieldDisabled => AllCoreHorcruxesDestroyed;

		public static int GetPreparationScore()
		{
			int score = horcruxesDestroyed;
			if (naginiDefeated)
				score++;
			if (HallowsSystem.resurrectionStoneAwakened)
				score++;
			return score;
		}

		/// <summary>
		/// Checks whether the player has a valid Horcrux destruction tool in their inventory.
		/// Canon: Basilisk venom and goblin-made weapons impregnated with it can destroy Horcruxes.
		/// </summary>
		private static bool HasDestructionTool(Player player)
		{
			int fangType = ModContent.ItemType<BasiliskFang>();
			int swordType = ModContent.ItemType<SwordOfGryffindor>();

			for (int i = 0; i < player.inventory.Length; i++)
			{
				if (!player.inventory[i].IsAir &&
					(player.inventory[i].type == fangType || player.inventory[i].type == swordType))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Attempt to destroy a Horcrux via right-click interaction.
		/// Requires a Basilisk Fang or Sword of Gryffindor in the player's inventory.
		/// Returns true if the Horcrux was destroyed, false if the player lacks a destruction tool.
		/// </summary>
		public bool AttemptDestroyHorcrux(int horcruxItemType, Player player)
		{
			if (!HasDestructionTool(player))
				return false;

			// Determine which Horcrux this is and check if already destroyed
			string horcruxName = null;
			if (horcruxItemType == ModContent.ItemType<RiddlesDiary>())
			{
				if (diaryDestroyed) return true; // Already destroyed, consume silently
				horcruxName = "Diary";
			}
			else if (horcruxItemType == ModContent.ItemType<SlytherinsLocket>())
			{
				if (locketDestroyed) return true;
				horcruxName = "Locket";
			}
			else if (horcruxItemType == ModContent.ItemType<HufflepuffsCup>())
			{
				if (cupDestroyed) return true;
				horcruxName = "Cup";
			}
			else if (horcruxItemType == ModContent.ItemType<DiademOfRavenclaw>())
			{
				if (diademDestroyed) return true;
				horcruxName = "Diadem";
			}

			if (horcruxName == null)
				return false;

			// Remove the Horcrux from the player's inventory
			for (int i = 0; i < player.inventory.Length; i++)
			{
				if (!player.inventory[i].IsAir && player.inventory[i].type == horcruxItemType)
				{
					player.inventory[i].TurnToAir();
					break;
				}
			}

			// Perform the actual destruction
			DestroyHorcrux(horcruxName, player);
			return true;
		}

		/// <summary>Destroy a Horcrux. Called when player uses a Horcrux item at the Enchanting Table.</summary>
		public static void DestroyHorcrux(string horcruxName, Player player)
		{
			bool wasNew = false;

			switch (horcruxName)
			{
				case "Diary":
					if (!diaryDestroyed) { diaryDestroyed = true; wasNew = true; }
					break;
				case "Locket":
					if (!locketDestroyed) { locketDestroyed = true; wasNew = true; }
					break;
				case "Cup":
					if (!cupDestroyed) { cupDestroyed = true; wasNew = true; }
					break;
				case "Diadem":
					if (!diademDestroyed) { diademDestroyed = true; wasNew = true; }
					break;
			}

			if (!wasNew)
				return;

			horcruxesDestroyed++;

			// Dramatic destruction effect — dark energy bursting outward
			for (int i = 0; i < 50; i++)
			{
				Dust dust = Dust.NewDustDirect(player.Center, 8, 8, DustID.Shadowflame, 0f, 0f, 50, default, 2f);
				dust.velocity = Main.rand.NextVector2Circular(8f, 8f);
				dust.noGravity = true;
			}

			// Green soul-fragment escaping — the piece of Voldemort's soul screaming free
			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(player.Center, 8, 8, DustID.CursedTorch, 0f, -3f, 50, default, 1.5f);
				dust.velocity = Main.rand.NextVector2Circular(4f, 6f);
				dust.velocity.Y -= 4f;
				dust.noGravity = true;
			}

			if (Main.netMode != NetmodeID.Server)
			{
				// Horcrux-specific dramatic text
				string flavorText = horcruxName switch
				{
					"Diary" => Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.DiaryFlavor"),
					"Locket" => Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.LocketFlavor"),
					"Cup" => Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.CupFlavor"),
					"Diadem" => Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.DiademFlavor"),
					_ => Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.DefaultFlavor")
				};

				Main.NewText(flavorText, new Color(100, 255, 100));
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.SoulDestroyed", horcruxesDestroyed), new Color(255, 50, 50));
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.DarkLordWeakens", (int)(GetVoldemortPowerMultiplier() * 100)), new Color(200, 100, 100));

				if (AllCoreHorcruxesDestroyed)
				{
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.AllDestroyed"), new Color(255, 215, 0));
					if (DownedBossSystem.downedDementorKing)
						Main.NewText(Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.SeekCloak"), new Color(200, 200, 255));
					else
						Main.NewText(Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.EndureAzkaban"), new Color(180, 200, 255));
				}
			}

			// Cleanse player corruption — destroying Horcruxes is an act of light
			var darkPlayer = player.GetModPlayer<Players.DarkArtsCorruptionPlayer>();
			darkPlayer.CleansCorruption(0.15f);

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);
		}

		public static void MarkNaginiDefeated(Player player)
		{
			if (naginiDefeated)
				return;

			naginiDefeated = true;

			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.NaginiFalls"), new Color(180, 255, 180));
			}

			if (player != null)
			{
				var darkPlayer = player.GetModPlayer<Players.DarkArtsCorruptionPlayer>();
				darkPlayer.CleansCorruption(0.08f);
			}

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);
		}
	}
}
