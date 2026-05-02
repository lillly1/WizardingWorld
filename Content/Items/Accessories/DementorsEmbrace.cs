using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Dementor's Embrace — crafted from the Dementor King's Shroud.
	/// Endgame dark accessory: +15% all damage, 5% life steal on all hits,
	/// but -30 max HP and Darkness visual. Enemies near you lose 3 defense.
	/// "To embrace the Dementor is to accept despair as power."
	/// </summary>
	public class DementorsEmbrace : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.accessory = true;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.sellPrice(gold: 20);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Generic) += 0.15f;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.10f;
			player.statLifeMax2 -= 30;

			// Dark aura — enemies lose defense nearby (handled via SoulSiphon pattern)
			if (!hideVisual)
			{
				// Cold dark particles
				if (Main.rand.NextBool(6))
				{
					Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Shadowflame, 0f, -0.5f, 180, default, 0.5f);
					dust.noGravity = true;
				}

				// Extinguish nearby light slightly
				Terraria.Lighting.AddLight(player.Center, -0.1f, -0.1f, -0.05f);
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DementorsShroud>(), 5)
				.AddIngredient(ItemID.SoulofNight, 15)
				.AddIngredient(ModContent.ItemType<Consumables.UnicornBlood>(), 2)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 25)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
