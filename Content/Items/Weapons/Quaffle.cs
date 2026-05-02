using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Quaffle — throwable ball that bounces off surfaces and returns to the player.</summary>
	public class Quaffle : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 28;
			Item.knockBack = 5f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.QuaffleProjectile>();
			Item.shootSpeed = 14f;
			Item.value = Item.buyPrice(gold: 3);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Leather, 5)
				.AddIngredient(ItemID.IronBar, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
