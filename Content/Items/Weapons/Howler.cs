using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Howler — thrown screaming letter that homes and explodes with confusion.</summary>
	public class Howler : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 32;
			Item.knockBack = 4f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.HowlerProjectile>();
			Item.shootSpeed = 10f;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.value = Item.buyPrice(silver: 5);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
		}

		public override void AddRecipes()
		{
			CreateRecipe(10)
				.AddIngredient(ItemID.Book, 5)
				.AddIngredient(ItemID.Torch, 5)
				.AddIngredient(ItemID.FallenStar, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
