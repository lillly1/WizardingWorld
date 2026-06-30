using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Holly Wand — Harry's wand. Casts Protego (shield) + Expelliarmus alternating.</summary>
	public class HollyWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 38;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 28;
			Item.knockBack = 5f;
			Item.mana = 10;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<ExpelliarmusProjectile>();
			Item.shootSpeed = 12f;
			Item.value = Item.buyPrice(gold: 3);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = null;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Alternate: Expelliarmus and Protego shield
			if (player.altFunctionUse == 2 || Main.rand.NextBool(4))
			{
				// Cast Protego — only if none active
				bool hasShield = false;
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI
						&& Main.projectile[i].type == ModContent.ProjectileType<ProtegoProjectile>())
					{
						hasShield = true;
						break;
					}
				}

				if (!hasShield)
				{
					Projectile.NewProjectile(source, player.Center, Vector2.Zero,
						ModContent.ProjectileType<ProtegoProjectile>(), 0, 0f, player.whoAmI);
					SoundEngine.PlaySound(WizardSoundStyles.Protego, player.Center);
				}

				return false;
			}

			SoundEngine.PlaySound(WizardSoundStyles.Expelliarmus, player.Center);
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 15)
				.AddIngredient(ItemID.FallenStar, 8)
				.AddIngredient(ItemID.Feather, 3)
				.AddIngredient(ItemID.Ruby, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
