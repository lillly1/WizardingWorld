using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Decoy Detonator — Weasleys' Wizard Wheezes. Thrown device that
	/// draws enemy aggro, then explodes dealing damage + Jinxed debuff.
	/// Brilliant tactical tool.
	/// </summary>
	public class DecoyDetonator : ModItem
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
			Item.damage = 45;
			Item.knockBack = 6f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.DecoyDetonatorProjectile>();
			Item.shootSpeed = 8f;
			Item.maxStack = 99;
			Item.consumable = true;
			Item.value = Item.buyPrice(silver: 15);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
		}

		public override void AddRecipes()
		{
			CreateRecipe(5)
				.AddIngredient(ItemID.Dynamite, 1)
				.AddIngredient(ItemID.Wire, 5)
				.AddIngredient(ItemID.GoldBar, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
