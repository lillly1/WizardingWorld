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
	/// <summary>Unicorn Hair Wand — post-Wall of Flesh (Hardmode). Casts Expecto Patronum (homing guardian).</summary>
	public class UnicornHairWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 55;
			Item.knockBack = 3f;
			Item.mana = 20;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<PatronusProjectile>();
			Item.shootSpeed = 8f;
			Item.value = Item.buyPrice(gold: 20);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = WizardSoundStyles.ExpectoPatronum;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Only allow one Patronus at a time
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == type)
					Main.projectile[i].Kill();
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Pearlwood, 15)
				.AddIngredient(ItemID.UnicornHorn, 3)
				.AddIngredient(ItemID.SoulofLight, 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
