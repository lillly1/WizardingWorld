using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Boss Progression Compass — shows which wizard boss to fight next.
	/// Displays progression status in the tooltip dynamically.
	/// Also grants +3% damage as a small passive bonus.
	/// "It always points to your next great challenge."
	/// </summary>
	public class BossCompass : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(gold: 3);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Generic) += 0.03f;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string nextBoss = GetNextBoss();
			tooltips.Add(new TooltipLine(Mod, "NextBoss",
				Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextChallenge", nextBoss)));

			// Show full progression
			string progress = GetProgressString();
			tooltips.Add(new TooltipLine(Mod, "Progress",
				Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.ProgressTooltip", progress)));
			string naginiSuffix = Common.Systems.HorcruxHuntSystem.naginiDefeated
				? Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NaginiSuffix")
				: string.Empty;
			tooltips.Add(new TooltipLine(Mod, "Horcruxes",
				Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.HorcruxTooltip",
					Common.Systems.HorcruxHuntSystem.horcruxesDestroyed, naginiSuffix)));
		}

		private string GetNextBoss()
		{
			if (!DownedBossSystem.downedTroll) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextTroll");
			if (!DownedBossSystem.downedQuirrell) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextQuirrell");
			if (!DownedBossSystem.downedBasilisk) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextBasilisk");
			if (!DownedBossSystem.downedAragog) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextAragog");
			if (!DownedBossSystem.downedFluffy) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextFluffy");
			if (!DownedBossSystem.downedHorntail) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextHorntail");
			if (!DownedBossSystem.downedUmbridge) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextUmbridge");
			if (!DownedBossSystem.downedFenrir) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextFenrir");
			if (!DownedBossSystem.downedBellatrix) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextBellatrix");
			if (!DownedBossSystem.downedBartyCrouch) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextBartyCrouch");
			if (!DownedBossSystem.downedDementorKing) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextDementorKing");
			if (!DownedBossSystem.downedVoldemort)
				return Common.Systems.HorcruxHuntSystem.AllCoreHorcruxesDestroyed
					? Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextVoldemortReady")
					: Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextVoldemortBlocked");
			if (Common.Systems.HallowsSystem.CanPurifyGauntsRing(Main.LocalPlayer)) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextPurifyRing");
			if (Common.Systems.HallowsSystem.CanClaimInvisibilityCloak) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextClaimCloak");
			if (!Common.Systems.HallowsSystem.hallowsAttuned) return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.NextUniteHallows");
			return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.Completed");
		}

		private string GetProgressString()
		{
			int defeated = 0;
			if (DownedBossSystem.downedTroll) defeated++;
			if (DownedBossSystem.downedQuirrell) defeated++;
			if (DownedBossSystem.downedBasilisk) defeated++;
			if (DownedBossSystem.downedAragog) defeated++;
			if (DownedBossSystem.downedFluffy) defeated++;
			if (DownedBossSystem.downedHorntail) defeated++;
			if (DownedBossSystem.downedUmbridge) defeated++;
			if (DownedBossSystem.downedFenrir) defeated++;
			if (DownedBossSystem.downedBellatrix) defeated++;
			if (DownedBossSystem.downedBartyCrouch) defeated++;
			if (DownedBossSystem.downedVoldemort) defeated++;
			if (DownedBossSystem.downedDementorKing) defeated++;
			return Language.GetTextValue("Mods.WizardingWorld.Items.BossCompass.Dynamic.ProgressSummary", defeated);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Compass, 1)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
