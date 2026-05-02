using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Moody's Magical Eye — the famous spinning eye of Mad-Eye Moody.
	/// Sees through walls (Hunter effect), reveals treasure, night vision.
	/// CONSTANT VIGILANCE!
	/// </summary>
	public class MoodysEye : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 10);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.detectCreature = true;
			player.findTreasure = true;
			player.nightVision = true;
			player.dangerSense = true;
			player.InfoAccMechShowWires = true;

			// The eye sees ALL
			player.GetDamage(DamageClass.Generic) += 0.05f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Lens, 10)
				.AddIngredient(ItemID.SoulofSight, 10)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
