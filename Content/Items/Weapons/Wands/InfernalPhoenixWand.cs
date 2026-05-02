using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>
	/// Infernal Phoenix Wand — upgraded Phoenix Feather Wand using Dragon Scale.
	/// Fires Incendio in a 5-bolt spread with guaranteed fire. Hardmode upgrade.
	/// "The phoenix feather, tempered by dragonfire."
	/// </summary>
	public class InfernalPhoenixWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 52;
			Item.knockBack = 4f;
			Item.mana = 12;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<IncendioProjectile>();
			Item.shootSpeed = 15f;
			Item.value = Item.buyPrice(gold: 15);
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item8;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// 5-bolt fire spread
			for (int i = -2; i <= 2; i++)
			{
				Vector2 spread = velocity.RotatedBy(MathHelper.ToRadians(i * 6));
				Projectile.NewProjectile(source, position, spread, type, damage, knockback, player.whoAmI);
			}
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<PhoenixFeatherWand>(), 1)
				.AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
