using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>
	/// Rowan Wand — protective wood, good for defensive and destructive charms.
	/// Casts Bombarda — explosive curse that destroys terrain.
	/// The mining wand — essential utility for wizard players.
	/// </summary>
	public class RowanWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 30;
			Item.knockBack = 8f;
			Item.mana = 12;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<BombardaProjectile>();
			Item.shootSpeed = 12f;
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = WizardSoundStyles.Bombarda;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 15)
				.AddIngredient(ItemID.Dynamite, 5)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
