using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Skiving Snackbox — Weasleys' Wizard Wheezes signature product.
	/// Eat one to "fake sick" — becomes temporarily invisible and loses ALL aggro.
	/// Enemies completely ignore you for 5 seconds.
	/// Perfect panic button for boss fights. 15-second cooldown via Potion Sickness.
	/// </summary>
	public class SkivingSnackbox : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(silver: 30);
			Item.UseSound = SoundID.Item2;
			Item.potion = true; // Triggers potion sickness cooldown
			Item.healLife = 1; // Minimal heal to trigger potion mechanic
		}

		public override void OnConsumeItem(Player player)
		{
			// Fake death — temporary invincibility + invisibility
			player.SetImmuneTimeForAllTypes(300); // 5 seconds immune
			player.invis = true;
			player.aggro -= 9999; // Enemies completely ignore you

			// Sick visual
			for (int i = 0; i < 15; i++)
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.GreenTorch, 0f, -1f, 100, default, 1.0f);
				dust.noGravity = true;
			}

			if (player.whoAmI == Main.myPlayer)
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.SkivingSnackboxUse"), 150, 200, 100);
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ItemID.Mushroom, 3)
				.AddIngredient(ItemID.PinkGel, 2)
				.AddIngredient(ItemID.FallenStar, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
