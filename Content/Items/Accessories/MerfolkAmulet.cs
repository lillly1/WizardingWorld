using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Merfolk Amulet — crafted from Merfolk Scales (fishing catch).
	/// Grants gills, flipper, and +10% damage while submerged.
	/// The underwater warrior's accessory.
	/// </summary>
	public class MerfolkAmulet : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(gold: 6);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.gills = true;
			player.accFlipper = true;

			// Damage bonus while underwater
			if (player.wet)
			{
				player.GetDamage(DamageClass.Generic) += 0.10f;
				player.moveSpeed += 0.15f;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.MerfolkScale>(), 10)
				.AddIngredient(ItemID.SharkFin, 3)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
