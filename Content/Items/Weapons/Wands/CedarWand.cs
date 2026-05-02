using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Cedar Wand — utility wand. Casts Lumos Maxima (massive light + undead damage).</summary>
	public class CedarWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.autoReuse = false;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 15;
			Item.knockBack = 2f;
			Item.mana = 15;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<LumosMaximaProjectile>();
			Item.shootSpeed = 0f;
			Item.value = Item.buyPrice(gold: 4);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item8;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Only one Lumos Maxima at a time
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == type)
					Main.projectile[i].Kill();
			}

			Projectile.NewProjectile(source, player.Center + new Vector2(0, -60), Vector2.Zero, type, damage, knockback, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.RichMahogany, 15)
				.AddIngredient(ItemID.FallenStar, 10)
				.AddIngredient(ItemID.Topaz, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
