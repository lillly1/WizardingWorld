using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>
	/// Red Oak Wand — known for quick reactions and adaptability.
	/// Casts Riddikulus — the anti-Boggart, anti-fear charm.
	/// Also cleanses fear and darkness effects on cast.
	/// </summary>
	public class RedOakWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 32;
			Item.knockBack = 4f;
			Item.mana = 10;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<RiddikulusProjectile>();
			Item.shootSpeed = 13f;
			Item.value = Item.buyPrice(gold: 4);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item8;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.RichMahogany, 15)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ItemID.Lens, 3)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
