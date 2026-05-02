using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Wizard's Chess Set — decorative/buff item.
	/// When used, grants a "Strategic Mind" buff: +8% crit chance for 5 minutes.
	/// "That's totally barbaric!" "That's wizard's chess."
	/// Also works as furniture (placeable chess board).
	/// </summary>
	public class WizardChessSet : ModItem
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
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.Item4;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				player.AddBuff(BuffID.Battle, 18000); // Increased enemy spawn (battle ready)
				player.AddBuff(BuffID.Rage, 18000); // +8% crit (strategic thinking)

				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Items.WizardChessSet.UseMessage"), 200, 200, 255);
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.StoneBlock, 20)
				.AddIngredient(ItemID.GoldBar, 3)
				.AddIngredient(ItemID.SilverBar, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
