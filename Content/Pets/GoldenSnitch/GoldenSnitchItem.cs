using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.GoldenSnitch
{
	/// <summary>Golden Snitch — summons a Golden Snitch pet that follows you.</summary>
	public class GoldenSnitchItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.ZephyrFish);
			Item.shoot = ModContent.ProjectileType<GoldenSnitchProjectile>();
			Item.buffType = ModContent.BuffType<GoldenSnitchBuff>();
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
				.AddIngredient(ItemID.GoldBar, 10)
				.AddIngredient(ItemID.Feather, 5)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
