using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Shield Hat — Weasleys' Wizard Wheezes product.
	/// 15% chance to negate incoming damage completely.
	/// The ultimate trolling accessory from Fred and George.
	/// </summary>
	public class ShieldHat : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(gold: 6);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statDefense += 3;

			// The hat's shield enchantment is handled via ModPlayer dodge
			player.GetModPlayer<Common.Players.WizardPlayer>().spellPowerBonus += 0.03f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.CobaltShield, 1)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
