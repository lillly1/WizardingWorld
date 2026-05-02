using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Nimbus 2001 — the ultimate broomstick. Faster than everything.
	/// "Malfoy's father bought the whole team Nimbus 2001s."
	/// Post-Moon Lord mount crafted from Firebolt + Lunar materials.
	/// Grants +8% damage while flying fast (aerial superiority).
	/// </summary>
	public class Nimbus2001Broomstick : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(gold: 50);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item79;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<Mounts.Nimbus2001Mount>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<FireboltBroomstick>(1)
				.AddIngredient(ItemID.LunarBar, 10)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 30)
				.AddIngredient(ModContent.ItemType<Consumables.UnicornBlood>(), 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
