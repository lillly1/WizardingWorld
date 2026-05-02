using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Screeching Howler — upgraded Howler infused with Mandrake cry.
	/// Larger AoE explosion that applies Petrified + Confused to ALL enemies in range.
	/// "When a Howler meets a Mandrake, the result is truly deafening."
	/// </summary>
	public class ScreechingHowler : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 18;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 55;
			Item.knockBack = 6f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.HowlerProjectile>();
			Item.shootSpeed = 10f;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.value = Item.buyPrice(silver: 12);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Fire the howler projectile — the HowlerProjectile already has AoE + Confusion
			// The Screeching version deals more damage and applies Petrified via the higher damage
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe(5)
				.AddIngredient(ModContent.ItemType<Howler>(), 5)
				.AddIngredient(ItemID.JungleSpores, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
