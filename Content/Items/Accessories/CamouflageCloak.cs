using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Camouflage Cloak — crafted from Dugbog Hide + Demiguise Hair.
	/// Combines the Dugbog's natural camouflage with Demiguise invisibility.
	/// -400 aggro, +10% speed, invisibility when standing still for 2 seconds.
	/// "Nearly as good as an Invisibility Cloak. Nearly."
	/// </summary>
	public class CamouflageCloak : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(gold: 7);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.aggro -= 400;
			player.moveSpeed += 0.10f;

			// Invisibility when standing still
			if (player.velocity.Length() < 0.5f)
			{
				player.invis = true;
				player.aggro -= 500; // Extra stealth when still
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DugbogHide>(), 8)
				.AddIngredient(ModContent.ItemType<Consumables.DemiguiseHair>(), 5)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
