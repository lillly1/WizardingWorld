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
	/// <summary>Dragon Heartstring Wand — post-Skeletron. Casts Sectumsempra (dark slashing curse).</summary>
	public class DragonHeartstringWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 42;
			Item.knockBack = 5f;
			Item.mana = 12;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<SectumsempraProjectile>();
			Item.shootSpeed = 16f;
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = WizardSoundStyles.Sectumsempra;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Slight spread for a slashing feel
			float spread = MathHelper.ToRadians(5);
			velocity = velocity.RotatedByRandom(spread);
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BorealWood, 15)
				.AddIngredient(ItemID.Bone, 20)
				.AddIngredient(ItemID.Diamond, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
