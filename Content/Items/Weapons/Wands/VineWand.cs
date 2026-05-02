using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Vine Wand — Hermione's wand. Casts Wingardium Leviosa and Aguamenti.</summary>
	public class VineWand : ModItem
	{
		private bool altSpell;

		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 38;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 25;
			Item.knockBack = 4f;
			Item.mana = 9;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<WingardiumProjectile>();
			Item.shootSpeed = 11f;
			Item.value = Item.buyPrice(gold: 3);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item8;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Alternate between Wingardium Leviosa and Aguamenti
			altSpell = !altSpell;
			if (altSpell)
			{
				Projectile.NewProjectile(source, position, velocity,
					ModContent.ProjectileType<AguamentiProjectile>(), damage, knockback, player.whoAmI);
				return false;
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.JungleSpores, 10)
				.AddIngredient(ItemID.Vine, 3)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
