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
	/// Ebony Wand — the most powerful wood for combat magic.
	/// Casts Apparition (teleport to cursor) — the ultimate utility spell.
	/// "Ebony wands have an impressive appearance and are highly suited
	/// to all manner of combative magic."
	/// </summary>
	public class EbonyWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 38;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.autoReuse = false;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 1;
			Item.knockBack = 0f;
			Item.mana = 30;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<ApparitionProjectile>();
			Item.shootSpeed = 0f;
			Item.value = Item.buyPrice(gold: 15);
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item8;
		}

		public override bool CanUseItem(Player player)
		{
			// Can't use during Chaos State cooldown
			return !player.HasBuff(BuffID.ChaosState);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, 0, 0, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Ebonwood, 15)
				.AddIngredient(ItemID.SoulofLight, 5)
				.AddIngredient(ItemID.SoulofNight, 5)
				.AddIngredient(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 15)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
