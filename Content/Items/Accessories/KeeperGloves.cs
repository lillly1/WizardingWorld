using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Quidditch Keeper Gloves — the goalie's best friend.
	/// +8 defense, +10% knockback resistance, thorns (20% contact damage reflected).
	/// "You shall not pass... the Quaffle through these hoops."
	/// </summary>
	public class KeeperGloves : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(gold: 5);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statDefense += 8;
			player.noKnockback = true;
			player.thorns = 0.2f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Leather, 10)
				.AddIngredient(ItemID.IronBar, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
