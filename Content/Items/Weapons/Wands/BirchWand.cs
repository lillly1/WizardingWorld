using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>
	/// Birch Wand — flexible and whippy. Casts Levicorpus (anti-gravity jinx).
	/// Great crowd control — launches non-boss enemies skyward.
	/// </summary>
	public class BirchWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 28;
			Item.knockBack = 10f; // High KB fits the spell
			Item.mana = 10;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<LevicorpusProjectile>();
			Item.shootSpeed = 13f;
			Item.value = Item.buyPrice(gold: 4);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item8;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BorealWood, 15)
				.AddIngredient(ItemID.Cloud, 10)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
