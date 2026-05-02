using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Elm Wand — utility wand. Casts Reparo to mend magical wards and support structures.</summary>
	public class ElmWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.autoReuse = false;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 1;
			Item.knockBack = 0f;
			Item.mana = 25;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<ReparoProjectile>();
			Item.shootSpeed = 0f;
			Item.value = Item.buyPrice(gold: 8);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item8;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Only one Reparo field at a time
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == type)
					Main.projectile[i].Kill();
			}

			Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, 0, 0, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 15)
				.AddIngredient(ItemID.Chain, 2)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
