using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Weasley's Basic Blaze Box — Weasleys' Wizard Wheezes.
	/// Thrown item that creates an AoE fire burst on impact. Ignites enemies.
	/// </summary>
	public class WeasleyBlazeBox : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 35;
			Item.knockBack = 4f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.BlazeBoxProjectile>();
			Item.shootSpeed = 10f;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.value = Item.buyPrice(silver: 5);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
		}

		public override void AddRecipes()
		{
			CreateRecipe(10)
				.AddIngredient(ItemID.Torch, 10)
				.AddIngredient(ItemID.Gel, 5)
				.AddIngredient(ItemID.FallenStar, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
