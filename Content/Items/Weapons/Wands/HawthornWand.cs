using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Hawthorn Wand — Draco's wand. Fires Chain Stupefy that bounces between enemies.</summary>
	public class HawthornWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 38;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 38;
			Item.knockBack = 4f;
			Item.mana = 12;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<ChainStupefyProjectile>();
			Item.shootSpeed = 13f;
			Item.value = Item.buyPrice(gold: 8);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item8;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BorealWood, 15)
				.AddIngredient(ItemID.SoulofLight, 5)
				.AddIngredient(ItemID.SoulofNight, 5)
				.AddIngredient(ItemID.Diamond, 2)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
