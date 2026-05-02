using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Hippogriff Feather — summons a Hippogriff mount. Faster than Nimbus.</summary>
	public class HippogriffFeather : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(gold: 15);
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item79;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<Mounts.HippogriffMount>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SoulofFlight, 20)
				.AddIngredient(ItemID.Feather, 15)
				.AddIngredient(ItemID.UnicornHorn, 2)
				.AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
