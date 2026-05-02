using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Peruvian Instant Darkness Powder — thrown cloud that blinds and confuses enemies in an area.</summary>
	public class PeruvianDarknessPowder : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.autoReuse = false;
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 0;
			Item.knockBack = 0f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.DarknessPowderProjectile>();
			Item.shootSpeed = 8f;
			Item.maxStack = 99;
			Item.consumable = true;
			Item.value = Item.buyPrice(silver: 10);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
		}

		public override void AddRecipes()
		{
			CreateRecipe(5)
				.AddIngredient(ItemID.SandBlock, 10)
				.AddIngredient(ItemID.Obsidian, 5)
				.AddIngredient(ItemID.FallenStar, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
