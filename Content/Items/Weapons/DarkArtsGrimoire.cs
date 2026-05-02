using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Dark Arts Grimoire — crafted from Bellatrix's Dark Arts Tome.
	/// A spell book weapon that fires a spread of 3 dark curse bolts.
	/// High damage, applies Dark Curse. The knowledge of dark magic weaponized.
	/// </summary>
	public class DarkArtsGrimoire : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 72;
			Item.knockBack = 4f;
			Item.mana = 18;
			Item.noMelee = true;
			Item.shoot = ProjectileID.ShadowBeamFriendly;
			Item.shootSpeed = 14f;
			Item.value = Item.buyPrice(gold: 20);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item73;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Fire 3-bolt spread
			for (int i = -1; i <= 1; i++)
			{
				Vector2 spread = velocity.RotatedBy(MathHelper.ToRadians(i * 8));
				Projectile.NewProjectile(source, position, spread, type, damage, knockback, player.whoAmI);
			}
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DarkArtsTome>(), 5)
				.AddIngredient(ItemID.SpellTome, 1)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
