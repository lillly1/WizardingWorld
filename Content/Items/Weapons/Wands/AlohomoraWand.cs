using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>
	/// This is a temporary wand for Alohomora until the wandlore system
	/// lets any wand cast any learned spell.
	/// For now, the Elm Wand (healing) is repurposed and Alohomora
	/// is accessible as a standalone castable item.
	///
	/// Alohomora Key — a magical lockpick disguised as a small golden key.
	/// Not a wand — it's a utility tool that casts Alohomora.
	/// "Standard Book of Spells, Chapter Seven."
	/// </summary>
	public class AlohomoraKey : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.autoReuse = false;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 1; // Minimal — it's utility, not combat
			Item.knockBack = 0f;
			Item.mana = 8;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<AlohomoraProjectile>();
			Item.shootSpeed = 10f;
			Item.value = Item.buyPrice(gold: 2);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item8;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.GoldBar, 3)
				.AddIngredient(ItemID.FallenStar, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
