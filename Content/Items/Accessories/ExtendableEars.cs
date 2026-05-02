using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>Extendable Ears — Weasley product. Widens enemy detection range and grants Hunter effect.</summary>
	public class ExtendableEars : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.accessory = true;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(gold: 3);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.detectCreature = true;
			player.dangerSense = true;
			player.InfoAccMechShowWires = true;

			// Slightly wider NPC aggro detection — makes enemies target you from farther
			// but you also see them first
			player.aggro += 50;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wire, 20)
				.AddIngredient(ItemID.Silk, 5)
				.AddIngredient(ItemID.FallenStar, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
