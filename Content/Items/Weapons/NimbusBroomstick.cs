using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Nimbus 2000 — a flying broomstick mount item.</summary>
	public class NimbusBroomstick : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(gold: 10);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item79;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<Mounts.NimbusMount>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 30)
				.AddIngredient(ItemID.SoulofFlight, 15)
				.AddIngredient(ItemID.Feather, 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
