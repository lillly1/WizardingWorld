using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Dragon Scale Ring — a ring crafted from Horntail scales.
	/// Fire immunity + 8% spell damage + 5% all damage.
	/// Compact fire-resistant accessory for spell casters.
	/// </summary>
	public class DragonScaleRing : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.accessory = true;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(gold: 8);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.buffImmune[BuffID.OnFire] = true;
			player.buffImmune[BuffID.OnFire3] = true;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.08f;
			player.GetDamage(DamageClass.Generic) += 0.05f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 3)
				.AddIngredient(ItemID.GoldBar, 5)
				.AddIngredient(ItemID.Ruby, 1)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
