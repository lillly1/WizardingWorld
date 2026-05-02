using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Prongs' Charm — James Potter's stag Patronus charm.
	/// Boosts Patronus-related abilities and spell damage.
	/// +10% spell damage, +5% crit. Patronus Guardian deals +25% damage.
	/// "The ones that love us never really leave us."
	/// </summary>
	public class ProngsCharm : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 8);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.10f;
			player.GetCritChance(ModContent.GetInstance<SpellDamage>()) += 5;

			// Boost minion damage (enhances Patronus Guardian specifically)
			player.GetDamage(DamageClass.Summon) += 0.10f;

			// Silver stag light
			if (!hideVisual)
				Terraria.Lighting.AddLight(player.Center, 0.3f, 0.3f, 0.4f);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SoulofLight, 8)
				.AddIngredient(ItemID.UnicornHorn, 1)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
