using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Basic starter wand. Casts Lumos (light) and Stupefy (stun bolt).</summary>
	public class OakWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 12;
			Item.knockBack = 3f;
			Item.mana = 6;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<StupefyProjectile>();
			Item.shootSpeed = 10f;
			Item.value = Item.buyPrice(silver: 50);
			Item.rare = ItemRarityID.White;
			Item.UseSound = WizardSoundStyles.Stupefy;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 15)
				.AddIngredient(ItemID.FallenStar, 3)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
