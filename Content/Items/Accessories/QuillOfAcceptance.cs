using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Quill of Acceptance — the magical quill that writes the names of magical children.
	/// +8% spell damage, +5% crit. Spells have +5 armor penetration.
	/// "The Quill of Acceptance has sat in Hogwarts since the school was founded."
	/// </summary>
	public class QuillOfAcceptance : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 8);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.08f;
			player.GetCritChance(ModContent.GetInstance<SpellDamage>()) += 5;
			player.GetArmorPenetration(ModContent.GetInstance<SpellDamage>()) += 5;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Feather, 5)
				.AddIngredient(ItemID.GoldBar, 5)
				.AddIngredient(ItemID.SoulofSight, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
