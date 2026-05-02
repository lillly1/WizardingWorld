using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Lightning Scar — vanity accessory. Harry's iconic forehead scar.
	/// Also provides a small functional bonus: warns of nearby bosses (danger sense).
	/// "The scar had not pained Harry for nineteen years. All was well."
	/// </summary>
	[AutoloadEquip(EquipType.Face)]
	public class LightningScar : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(gold: 2);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// The scar burns when evil is near
			player.dangerSense = true;

			// Small spell damage bonus — connection to Voldemort
			player.GetDamage(ModContent.GetInstance<DamageClasses.SpellDamage>()) += 0.03f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ItemID.GoldBar, 2)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
