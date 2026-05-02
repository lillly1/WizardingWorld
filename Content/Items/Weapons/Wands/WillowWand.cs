using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Willow wand — casts Expelliarmus (disarming bolt with high knockback).</summary>
	public class WillowWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 16;
			Item.knockBack = 7f;
			Item.mana = 7;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<ExpelliarmusProjectile>();
			Item.shootSpeed = 11f;
			Item.value = Item.buyPrice(gold: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item8;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 15)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ItemID.Sapphire, 2)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
