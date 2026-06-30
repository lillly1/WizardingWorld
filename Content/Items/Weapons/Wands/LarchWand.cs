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
	/// <summary>
	/// Larch Wand — versatile utility wand. Casts Finite Incantatem (debuff clear)
	/// and Accio (item attraction) on alternate uses.
	/// Larch is known for instilling confidence and courage.
	/// </summary>
	public class LarchWand : ModItem
	{
		private bool altCast;

		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.autoReuse = false;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 1;
			Item.knockBack = 0f;
			Item.mana = 20;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<FiniteIncantatemProjectile>();
			Item.shootSpeed = 0f;
			Item.value = Item.buyPrice(gold: 6);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = null;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			altCast = !altCast;

			if (altCast)
			{
				// Accio — item attraction
				Projectile.NewProjectile(source, player.Center, Vector2.Zero,
					ModContent.ProjectileType<AccioProjectile>(), 0, 0, player.whoAmI);
				SoundEngine.PlaySound(WizardSoundStyles.Accio, player.Center);
			}
			else
			{
				// Finite Incantatem — debuff clear
				Projectile.NewProjectile(source, player.Center, Vector2.Zero,
					ModContent.ProjectileType<FiniteIncantatemProjectile>(), 0, 0, player.whoAmI);
				SoundEngine.PlaySound(WizardSoundStyles.FiniteIncantatem, player.Center);
			}

			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 15)
				.AddIngredient(ItemID.PurificationPowder, 10)
				.AddIngredient(ItemID.FallenStar, 8)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
