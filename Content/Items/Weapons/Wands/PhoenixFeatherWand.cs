using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Phoenix Feather Wand — post-Eye of Cthulhu. Casts Incendio with a chance for double shot.</summary>
	public class PhoenixFeatherWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 38;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 30;
			Item.knockBack = 4f;
			Item.mana = 10;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<IncendioProjectile>();
			Item.shootSpeed = 14f;
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = WizardSoundStyles.Incendio;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// 25% chance to fire two bolts
			if (Main.rand.NextBool(4))
			{
				Vector2 offset = velocity.RotatedBy(MathHelper.ToRadians(10));
				Projectile.NewProjectile(source, position, offset, type, damage, knockback, player.whoAmI);
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 15)
				.AddIngredient(ItemID.HellstoneBar, 8)
				.AddIngredient(ItemID.Feather, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
