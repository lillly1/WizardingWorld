using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>
	/// Cypress Wand — boss-utility wand. Casts Conjunctivitis Curse.
	/// Associated with nobility and sacrifice. Used by Krum against the dragon.
	/// Designed specifically for boss fights — long Jinxed duration weakens bosses.
	/// </summary>
	public class CypressWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 38;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 30;
			Item.knockBack = 2f;
			Item.mana = 15;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<ConjunctivitisProjectile>();
			Item.shootSpeed = 14f;
			Item.value = Item.buyPrice(gold: 6);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = WizardSoundStyles.Conjunctivitis;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.RichMahogany, 15)
				.AddIngredient(ItemID.Lens, 5)
				.AddIngredient(ItemID.SoulofSight, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
