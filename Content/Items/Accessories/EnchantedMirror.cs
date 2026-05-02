using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Enchanted Hand Mirror — wizard's recall device.
	/// When equipped, pressing the mount hotkey recalls you home (like Magic Mirror).
	/// Also grants +3% spell damage passively.
	/// "Not quite the Mirror of Erised, but it'll get you home."
	/// </summary>
	public class EnchantedMirror : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 22;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(gold: 4);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(ModContent.GetInstance<DamageClasses.SpellDamage>()) += 0.03f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MagicMirror, 1)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			CreateRecipe()
				.AddIngredient(ItemID.IceMirror, 1)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
