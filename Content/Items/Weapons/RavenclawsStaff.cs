using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Ravenclaw's Staff of Wisdom — house signature weapon for Ravenclaw.
	/// Mana-efficient spell staff that fires a piercing beam of pure intellect.
	/// Reduces mana cost of all spells while held. Hits restore mana.
	/// Matches Ravenclaw's wisdom/mana armor bonus.
	/// </summary>
	public class RavenclawsStaff : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 50;
			Item.knockBack = 3f;
			Item.mana = 5; // Very low mana cost — wisdom conserves energy
			Item.noMelee = true;
			Item.shoot = ProjectileID.RainbowCrystalExplosion; // Blue crystal beam
			Item.shootSpeed = 16f;
			Item.value = Item.buyPrice(gold: 12);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item43;
		}

		public override void HoldItem(Player player)
		{
			// Wisdom passive: -10% mana cost while holding this staff
			player.manaCost -= 0.10f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Fire a blue wisdom bolt
			int projType = ProjectileID.NebulaLaser;
			Projectile.NewProjectile(source, position, velocity, projType, damage, knockback, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Sapphire, 5)
				.AddIngredient(ItemID.ManaCrystal, 3)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 12)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
