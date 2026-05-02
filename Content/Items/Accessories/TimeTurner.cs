using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>Time Turner — grants dodge (once per 10s) and +10% movement speed.</summary>
	public class TimeTurner : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 10);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
			wp.hasTimeTurner = true;
			player.moveSpeed += 0.10f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.GoldBar, 10)
				.AddIngredient(ItemID.SoulofLight, 10)
				.AddIngredient(ItemID.SoulofNight, 10)
				.AddIngredient(ModContent.ItemType<Consumables.GoldenEgg>(), 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
