using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Weasley's Wildfire Whiz-Bangs — fireworks that home into enemies and explode in color.</summary>
	public class WeasleyFireworks : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 40;
			Item.knockBack = 5f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.WeasleyFireworkProjectile>();
			Item.shootSpeed = 12f;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.value = Item.buyPrice(silver: 8);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item11;
		}

		public override void AddRecipes()
		{
			CreateRecipe(15)
				.AddIngredient(ItemID.Dynamite, 1)
				.AddIngredient(ItemID.FallenStar, 2)
				.AddIngredient(ItemID.Torch, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
