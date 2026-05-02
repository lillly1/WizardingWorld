using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>Demiguise Weave Cloak — woven from Demiguise hair for ordinary stealth work.
	/// A lesser alternative to the true Invisibility Cloak.</summary>
	public class DemiguiseCloak : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 8);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.invis = true;
			player.aggro -= 200;
			player.moveSpeed += 0.05f;

			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
			wp.hasDemiguiseCloak = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DemiguiseHair>(), 15)
				.AddIngredient(ItemID.Silk, 20)
				.AddIngredient(ItemID.SoulofLight, 10)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 20)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
