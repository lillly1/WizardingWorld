using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Standard Book of Spells — the mod's in-game reference guide.
	/// When used, displays a paginated guide to all wands, organized by tier.
	/// Shows what spell each wand casts, key crafting materials, and progression tips.
	/// Sold by Ollivander. Reusable.
	/// </summary>
	public class SpellBook : ModItem
	{
		private int currentPage;
		private const int MaxPages = 4;

		private static void ShowLine(string key, byte r, byte g, byte b) =>
			Main.NewText(Language.GetTextValue($"Mods.WizardingWorld.Items.SpellBook.Guide.{key}"), r, g, b);

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.maxStack = 1;
			Item.consumable = false;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item1;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return true;

			currentPage = (currentPage + 1) % (MaxPages + 1);

			switch (currentPage)
			{
				case 0:
					ShowOverview();
					break;
				case 1:
					ShowPage1_EarlyWands();
					break;
				case 2:
					ShowPage2_MidWands();
					break;
				case 3:
					ShowPage3_LateWands();
					break;
				case 4:
					ShowPage4_Utility();
					break;
			}

			return true;
		}

		private void ShowOverview()
		{
			ShowLine("OverviewTitle", 255, 215, 0);
			ShowLine("OverviewAuthor", 200, 180, 140);
			ShowLine("OverviewBrowse", 180, 180, 180);
			ShowLine("OverviewPage1", 150, 200, 150);
			ShowLine("OverviewPage2", 200, 200, 150);
			ShowLine("OverviewPage3", 200, 150, 150);
			ShowLine("OverviewPage4", 150, 150, 200);
		}

		private void ShowPage1_EarlyWands()
		{
			ShowLine("Page1Title", 150, 200, 150);
			ShowLine("Page1Line1", 200, 200, 200);
			ShowLine("Page1Line2", 150, 150, 150);
			ShowLine("Page1Line3", 200, 200, 200);
			ShowLine("Page1Line4", 150, 150, 150);
			ShowLine("Page1Line5", 200, 200, 200);
			ShowLine("Page1Line6", 150, 150, 150);
			ShowLine("Page1Line7", 200, 200, 200);
			ShowLine("Page1Line8", 200, 200, 200);
			ShowLine("Page1Line9", 200, 200, 200);
			ShowLine("Page1Line10", 200, 200, 200);
		}

		private void ShowPage2_MidWands()
		{
			ShowLine("Page2Title", 200, 200, 150);
			ShowLine("Page2Line1", 200, 200, 200);
			ShowLine("Page2Line2", 150, 150, 150);
			ShowLine("Page2Line3", 200, 200, 200);
			ShowLine("Page2Line4", 200, 200, 200);
			ShowLine("Page2Line5", 200, 200, 200);
			ShowLine("Page2Line6", 200, 200, 200);
			ShowLine("Page2Line7", 200, 200, 200);
			ShowLine("Page2Line8", 150, 150, 150);
			ShowLine("Page2Line9", 200, 200, 200);
		}

		private void ShowPage3_LateWands()
		{
			ShowLine("Page3Title", 200, 150, 150);
			ShowLine("Page3Line1", 200, 200, 200);
			ShowLine("Page3Line2", 150, 150, 150);
			ShowLine("Page3Line3", 255, 100, 100);
			ShowLine("Page3Line4", 150, 150, 150);
			ShowLine("Page3Line5", 255, 150, 50);
			ShowLine("Page3Line6", 150, 150, 150);
			ShowLine("Page3Line7", 255, 215, 0);
			ShowLine("Page3Line8", 150, 150, 150);
			ShowLine("Page3Line9", 180, 180, 200);
		}

		private void ShowPage4_Utility()
		{
			ShowLine("Page4Title", 150, 150, 200);
			ShowLine("Page4Line1", 200, 200, 200);
			ShowLine("Page4Line2", 200, 200, 200);
			ShowLine("Page4Line3", 200, 200, 200);
			Main.NewText("", 200, 200, 200);
			ShowLine("Page4Tip1", 255, 215, 0);
			ShowLine("Page4Tip2", 255, 215, 0);
			ShowLine("Page4Tip3", 255, 215, 0);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Book, 1)
				.AddIngredient(ItemID.FallenStar, 3)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
