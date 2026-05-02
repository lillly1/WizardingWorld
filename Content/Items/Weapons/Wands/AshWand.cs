using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Ash wand — casts Incendio (fire bolt). Faster than Oak/Willow.</summary>
	public class AshWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 22;
			Item.knockBack = 4f;
			Item.mana = 8;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<IncendioProjectile>();
			Item.shootSpeed = 12f;
			Item.value = Item.buyPrice(gold: 2);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item8;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.AshWood, 15)
				.AddIngredient(ItemID.Hellstone, 5)
				.AddIngredient(ItemID.Ruby, 2)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
