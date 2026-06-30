using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Fiendfyre Wand — endgame. Fires cursed flames that grow and home into enemies.</summary>
	public class FiendfyreWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 42;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 95;
			Item.knockBack = 4f;
			Item.mana = 20;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<FiendfyreProjectile>();
			Item.shootSpeed = 10f;
			Item.value = Item.buyPrice(gold: 50);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = WizardSoundStyles.Fiendfyre;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.HellstoneBar, 20)
				.AddIngredient(ItemID.SoulofFright, 10)
				.AddIngredient(ItemID.FragmentSolar, 10)
				.AddIngredient(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 30)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
