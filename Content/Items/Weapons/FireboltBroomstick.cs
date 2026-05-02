using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Firebolt — upgraded broomstick mount. Faster than Nimbus 2000.</summary>
	public class FireboltBroomstick : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(gold: 25);
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item79;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<Mounts.FireboltMount>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<NimbusBroomstick>(1)
				.AddIngredient(ItemID.SoulofFlight, 25)
				.AddIngredient(ItemID.HellstoneBar, 10)
				.AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
