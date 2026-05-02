using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Blackthorn Wand — Bellatrix's wand. Casts Impedimenta (AoE slow jinx).</summary>
	public class BlackthornWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 38;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 28;
			Item.useAnimation = 28;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 35;
			Item.knockBack = 3f;
			Item.mana = 14;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<ImpedimentaProjectile>();
			Item.shootSpeed = 12f;
			Item.value = Item.buyPrice(gold: 6);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item8;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SpookyWood, 15)
				.AddIngredient(ItemID.SoulofNight, 5)
				.AddIngredient(ItemID.Amethyst, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
