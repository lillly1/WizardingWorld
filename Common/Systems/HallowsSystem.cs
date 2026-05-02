using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Hallows System — world progression for the unique Deathly Hallows.
	///
	/// Responsibilities:
	/// - Tracks when the true Invisibility Cloak has been reclaimed
	/// - Tracks Gaunt's Ring purification into the Resurrection Stone
	/// - Resolves active Hallows on the player once item flags are set
	/// - Stores the one-time "Master of Death" world attunement event
	/// </summary>
	public class HallowsSystem : ModSystem
	{
		public static bool invisibilityCloakClaimed;
		public static bool resurrectionStoneAwakened;
		public static bool hallowsAttuned;

		public override void ClearWorld()
		{
			invisibilityCloakClaimed = false;
			resurrectionStoneAwakened = false;
			hallowsAttuned = false;
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["invisibilityCloakClaimed"] = invisibilityCloakClaimed;
			tag["resurrectionStoneAwakened"] = resurrectionStoneAwakened;
			tag["hallowsAttuned"] = hallowsAttuned;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			invisibilityCloakClaimed = tag.GetBool("invisibilityCloakClaimed");
			resurrectionStoneAwakened = tag.GetBool("resurrectionStoneAwakened");
			hallowsAttuned = tag.GetBool("hallowsAttuned");
		}

		public override void NetSend(BinaryWriter writer)
		{
			BitsByte flags = new();
			flags[0] = invisibilityCloakClaimed;
			flags[1] = resurrectionStoneAwakened;
			flags[2] = hallowsAttuned;
			writer.Write(flags);
		}

		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			invisibilityCloakClaimed = flags[0];
			resurrectionStoneAwakened = flags[1];
			hallowsAttuned = flags[2];
		}

		public static bool CanClaimInvisibilityCloak =>
			DownedBossSystem.downedDementorKing &&
			HorcruxHuntSystem.AllCoreHorcruxesDestroyed &&
			!invisibilityCloakClaimed;

		public static bool PlayerHasGauntsRing(Player player)
		{
			int ringType = ModContent.ItemType<Content.Items.Accessories.GauntsRing>();
			for (int i = 0; i < player.inventory.Length; i++)
			{
				if (!player.inventory[i].IsAir && player.inventory[i].type == ringType)
					return true;
			}

			for (int i = 0; i < player.armor.Length; i++)
			{
				if (!player.armor[i].IsAir && player.armor[i].type == ringType)
					return true;
			}

			return false;
		}

		public static bool CanPurifyGauntsRing(Player player) =>
			HorcruxHuntSystem.AllCoreHorcruxesDestroyed &&
			DownedBossSystem.downedVoldemort &&
			!resurrectionStoneAwakened &&
			PlayerHasGauntsRing(player);

		public static string GetDumbledoreGuidance(Player player)
		{
			if (!HorcruxHuntSystem.AllCoreHorcruxesDestroyed)
				return Language.GetTextValue("Mods.WizardingWorld.Hallows.GuidanceHorcruxes");

			if (!DownedBossSystem.downedDementorKing)
				return Language.GetTextValue("Mods.WizardingWorld.Hallows.GuidanceAzkaban");

			if (!invisibilityCloakClaimed)
				return Language.GetTextValue("Mods.WizardingWorld.Hallows.GuidanceCloak");

			if (!DownedBossSystem.downedVoldemort)
				return Language.GetTextValue("Mods.WizardingWorld.Hallows.GuidanceVoldemort");

			if (!PlayerHasGauntsRing(player) && !resurrectionStoneAwakened)
				return Language.GetTextValue("Mods.WizardingWorld.Hallows.GuidanceRing");

			if (CanPurifyGauntsRing(player))
				return Language.GetTextValue("Mods.WizardingWorld.Hallows.GuidancePurify");

			if (!hallowsAttuned)
				return Language.GetTextValue("Mods.WizardingWorld.Hallows.GuidanceUnite");

			return Language.GetTextValue("Mods.WizardingWorld.Hallows.GuidanceComplete");
		}

		public static bool TryClaimInvisibilityCloak(Player player)
		{
			if (!CanClaimInvisibilityCloak)
				return false;

			invisibilityCloakClaimed = true;
			player.QuickSpawnItem(player.GetSource_GiftOrReward(),
				ModContent.ItemType<Content.Items.Accessories.InvisibilityCloak>());

			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Hallows.CloakReveal1"), new Color(210, 210, 255));
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Hallows.CloakReveal2"), new Color(255, 215, 140));
			}

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);

			return true;
		}

		public static bool TryPurifyGauntsRing(Player player)
		{
			if (!CanPurifyGauntsRing(player))
				return false;

			int ringType = ModContent.ItemType<Content.Items.Accessories.GauntsRing>();
			bool ringRemoved = false;
			for (int i = 0; i < player.inventory.Length; i++)
			{
				if (!player.inventory[i].IsAir && player.inventory[i].type == ringType)
				{
					player.inventory[i].stack--;
					if (player.inventory[i].stack <= 0)
						player.inventory[i].TurnToAir();
					ringRemoved = true;
					break;
				}
			}

			for (int i = 0; i < player.armor.Length && !ringRemoved; i++)
			{
				if (!player.armor[i].IsAir && player.armor[i].type == ringType)
				{
					player.armor[i].stack--;
					if (player.armor[i].stack <= 0)
						player.armor[i].TurnToAir();
					break;
				}
			}

			resurrectionStoneAwakened = true;
			player.QuickSpawnItem(player.GetSource_GiftOrReward(),
				ModContent.ItemType<Content.Items.Accessories.ResurrectionStone>());

			var darkPlayer = player.GetModPlayer<Players.DarkArtsCorruptionPlayer>();
			darkPlayer.CleansCorruption(0.20f);

			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Hallows.StoneReveal1"), new Color(210, 210, 210));
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Hallows.StoneReveal2"), new Color(255, 215, 140));
			}

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);

			return true;
		}

		public static void ResolveActiveHallows(Player player, Players.WizardPlayer wizardPlayer)
		{
			wizardPlayer.hasDeathlyHallows =
				wizardPlayer.hasElderWand &&
				wizardPlayer.hasInvisibilityCloak &&
				wizardPlayer.hasResurrectionStone;

			if (!wizardPlayer.hasDeathlyHallows || hallowsAttuned)
				return;

			hallowsAttuned = true;

			if (Main.netMode != NetmodeID.Server)
			{
				for (int i = 0; i < 80; i++)
				{
					Dust dust = Dust.NewDustDirect(player.Center, 12, 12, DustID.GoldFlame, 0f, 0f, 50, default, 2.5f);
					dust.velocity = Main.rand.NextVector2Circular(10f, 10f);
					dust.noGravity = true;
				}

				for (int i = 0; i < 40; i++)
				{
					Dust dust = Dust.NewDustDirect(player.Center, 8, 8, DustID.WhiteTorch, 0f, -4f, 30, default, 2f);
					dust.velocity = Main.rand.NextVector2Circular(3f, 8f);
					dust.velocity.Y -= 5f;
					dust.noGravity = true;
				}

				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Hallows.MasterOfDeath1"), new Color(255, 215, 0));
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Hallows.MasterOfDeath2"), new Color(255, 255, 200));
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Hallows.MasterOfDeath3"), new Color(200, 200, 255));
			}

			var darkPlayer = player.GetModPlayer<Players.DarkArtsCorruptionPlayer>();
			darkPlayer.CleansCorruption(1.0f);

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);
		}
	}
}
