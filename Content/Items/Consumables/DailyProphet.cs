using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Daily Prophet — wizarding newspaper. When used, displays the current world state:
	/// which bosses have been defeated, current events, time of day, moon phase.
	/// Informational utility item. Not consumed on use.
	/// </summary>
	public class DailyProphet : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.maxStack = 1;
			Item.consumable = false; // Reusable
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(silver: 50);
			Item.UseSound = SoundID.Item1;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return true;

			var sb = new StringBuilder();
			sb.AppendLine("=== THE DAILY PROPHET ===");
			sb.AppendLine();

			// Time
			string timeOfDay = Main.dayTime ? "Daytime" : "Nighttime";
			string moonPhase = Main.moonPhase switch
			{
				0 => "Full Moon",
				1 => "Waning Gibbous",
				2 => "Third Quarter",
				3 => "Waning Crescent",
				4 => "New Moon",
				5 => "Waxing Crescent",
				6 => "First Quarter",
				_ => "Waxing Gibbous",
			};
			sb.AppendLine($"Current: {timeOfDay} | Moon: {moonPhase}");
			sb.AppendLine($"World Mode: {(Main.hardMode ? "Hardmode" : "Pre-Hardmode")}");
			sb.AppendLine();

			// Daily Challenge
			sb.AppendLine($"--- {Common.Systems.DailyChallengeSystem.GetChallengeText()} ---");
			sb.AppendLine();

			// Wizard boss status
			sb.AppendLine("--- Wizarding World Status ---");
			sb.AppendLine($"Basilisk: {(Common.Systems.DownedBossSystem.downedBasilisk ? "DEFEATED" : "At Large")}");
			sb.AppendLine($"Hungarian Horntail: {(Common.Systems.DownedBossSystem.downedHorntail ? "DEFEATED" : "At Large")}");
			sb.AppendLine($"He Who Must Not Be Named: {(Common.Systems.DownedBossSystem.downedVoldemort ? "VANQUISHED" : "STILL AT LARGE")}");

			// Events
			if (Common.Systems.DeathEaterInvasion.invasionActive)
				sb.AppendLine("!! DEATH EATER INVASION IN PROGRESS !!");
			if (Main.bloodMoon)
				sb.AppendLine("!! BLOOD MOON RISING — WEREWOLVES ACTIVE !!");
			if (Main.eclipse)
				sb.AppendLine("!! SOLAR ECLIPSE — DARK CREATURES EMPOWERED !!");

			// Forbidden Forest
			if (Common.Systems.DownedBossSystem.downedBasilisk)
				sb.AppendLine("The Forbidden Forest stirs at night...");

			sb.AppendLine();
			sb.AppendLine("\"All the news that's fit to print\"");

			// Display as chat message
			foreach (string line in sb.ToString().Split('\n'))
			{
				if (!string.IsNullOrWhiteSpace(line))
					Main.NewText(line, 200, 180, 140);
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Book, 1)
				.AddIngredient(ItemID.FallenStar, 1)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
