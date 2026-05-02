using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>Pensieve — grants full minimap reveal (like GPS), enemy detection, and night vision.</summary>
	public class Pensieve : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 28;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 12);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.accWatch = 3; // Full time display
			player.accDepthMeter = 1;
			player.accCompass = 1;
			player.detectCreature = true;
			player.nightVision = true;
			player.findTreasure = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.GPS, 1)
				.AddIngredient(ItemID.SilverBar, 10)
				.AddIngredient(ItemID.SoulofSight, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
