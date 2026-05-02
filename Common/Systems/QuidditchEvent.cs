using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Quidditch Golden Snitch Chase — mini-event system.
	/// A Golden Snitch spawns randomly during daytime and zips around erratically.
	/// If a player catches it (collides with it while holding a broomstick mount),
	/// they receive a massive reward: gold, Essence of Magic, and a temporary luck buff.
	/// The Snitch despawns after 60 seconds if not caught.
	/// Triggers randomly during daytime in Hardmode, ~1 in 10 minutes.
	/// </summary>
	public class QuidditchEvent : ModSystem
	{
		public static bool snitchActive;
		public static Vector2 snitchPosition;
		public static Vector2 snitchVelocity;
		public static float snitchTimer;
		public override void PreUpdateWorld()
		{
			if (snitchActive)
			{
				UpdateSnitch();
				return;
			}

			// Random chance to spawn snitch during daytime in Hardmode
			if (Main.dayTime && Main.hardMode && Main.rand.NextBool(36000)) // ~1 in 10 min
			{
				SpawnSnitch();
			}
		}

		public static void SpawnSnitch()
		{
			if (snitchActive)
				return;

			// Spawn near a random player
			Player target = Main.player[Main.rand.Next(Main.CurrentFrameFlags.ActivePlayersCount)];
			if (target == null || !target.active)
				return;

			snitchActive = true;
			snitchPosition = target.Center + new Vector2(Main.rand.Next(-300, 300), -200);
			snitchVelocity = Main.rand.NextVector2Circular(8f, 5f);
			snitchTimer = 3600; // 60 seconds

			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Events.QuidditchSnitch.Spawn"), 255, 215, 0);
			}
		}

		private static void UpdateSnitch()
		{
			snitchTimer--;

			if (snitchTimer <= 0)
			{
				// Snitch escaped
				snitchActive = false;
				if (Main.netMode != NetmodeID.Server)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Events.QuidditchSnitch.Escaped"), 200, 200, 200);
				return;
			}

			// Erratic movement — the Snitch is incredibly fast and unpredictable
			snitchVelocity += Main.rand.NextVector2Circular(1.5f, 1.5f);
			snitchVelocity = Vector2.Clamp(snitchVelocity, new Vector2(-12, -10), new Vector2(12, 10));
			snitchPosition += snitchVelocity;

			// Stay near players — don't fly off the map
			Player nearest = Main.player[Player.FindClosest(snitchPosition, 1, 1)];
			if (nearest != null && nearest.active)
			{
				float dist = Vector2.Distance(snitchPosition, nearest.Center);
				if (dist > 800f)
				{
					// Pull back toward player
					Vector2 pullDir = (nearest.Center - snitchPosition).SafeNormalize(Vector2.Zero) * 2f;
					snitchVelocity += pullDir;
				}
			}

			// Golden sparkle trail
			if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(snitchPosition, 8, 8, DustID.GoldCoin, 0f, 0f, 50, default, 1.0f);
				dust.noGravity = true;
				dust.velocity = snitchVelocity * -0.2f;
			}

			// Check for player collision — must be on a mount (broomstick!)
			foreach (var player in Main.ActivePlayers)
			{
				if (player.mount.Active && Vector2.Distance(player.Center, snitchPosition) < 40f)
				{
					CatchSnitch(player);
					return;
				}
			}

			// Warning at 10 seconds left
			if (snitchTimer == 600 && Main.netMode != NetmodeID.Server)
			{
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Events.QuidditchSnitch.Warning"), 255, 200, 50);
			}
		}

		private static void CatchSnitch(Player player)
		{
			// If a Quidditch match is active and in Snitch phase, delegate to the match system
			if (QuidditchCupSystem.matchActive && QuidditchCupSystem.matchPhase == 1)
			{
				QuidditchCupSystem.OnPlayerCatchesSnitch();
				snitchActive = false;
				return; // match handles rewards
			}

			snitchActive = false;

			// Reward!
			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Events.QuidditchSnitch.Caught", player.name), 255, 215, 0);
			}

			// Gold reward
			player.QuickSpawnItem(player.GetSource_GiftOrReward(), ItemID.GoldCoin, 5);

			// Essence reward
			player.QuickSpawnItem(player.GetSource_GiftOrReward(),
				ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), 15);

			// Luck buff
			player.AddBuff(BuffID.Lucky, 36000); // 10 minutes luck

			// Victory dust explosion
			for (int i = 0; i < 50; i++)
			{
				Dust dust = Dust.NewDustDirect(player.Center, 8, 8, DustID.GoldCoin, 0f, 0f, 50, default, 2f);
				dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
				dust.noGravity = true;
			}
		}
	}
}
