using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Thestral Reins — summons a Thestral mount.
	/// "Only visible to those who have witnessed death."
	/// Requires Basilisk defeated to craft. Grants night vision + detection while riding.
	/// </summary>
	public class ThestralReins : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 22;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(gold: 8);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item79;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<Mounts.ThestralMount>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Leather, 10)
				.AddIngredient(ItemID.SoulofNight, 8)
				.AddIngredient(ItemID.Feather, 10)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
