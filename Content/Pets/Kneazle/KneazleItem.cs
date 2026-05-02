using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.Kneazle
{
	/// <summary>Cat Treats — summons a Kneazle magical cat pet. Grants enemy detection.</summary>
	public class KneazleItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<KneazleProjectile>();
			Item.buffType = ModContent.BuffType<KneazleBuff>();
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
				player.AddBuff(Item.buffType, 3600);

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Leather, 5)
				.AddIngredient(ItemID.Silk, 5)
				.AddIngredient(ItemID.FallenStar, 3)
				.AddIngredient(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
