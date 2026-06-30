using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>
	/// Alder Wand — early-game bridge wand between Oak (12 dmg) and Ash (22 dmg).
	/// Fires Stupefy like Oak but faster, with slightly higher damage.
	/// Cheap to craft — designed so players aren't stuck with Oak until Hellstone.
	/// Alder wood is "unyielding" and best for non-verbal magic.
	/// </summary>
	public class AlderWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 24; // Faster than Oak (30)
			Item.useAnimation = 24;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 17; // Between Oak (12) and Ash (22)
			Item.knockBack = 4f;
			Item.mana = 6;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<StupefyProjectile>();
			Item.shootSpeed = 11f;
			Item.value = Item.buyPrice(gold: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = WizardSoundStyles.Stupefy;
		}

		public override void AddRecipes()
		{
			// Easy recipe — no gems, no rare materials
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 15)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ItemID.IronBar, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
