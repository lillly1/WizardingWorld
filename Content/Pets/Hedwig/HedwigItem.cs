using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.Hedwig
{
	/// <summary>Owl Treat — summons Hedwig, a snowy owl light pet.</summary>
	public class HedwigItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<HedwigProjectile>();
			Item.buffType = ModContent.BuffType<HedwigBuff>();
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
				.AddIngredient(ItemID.Feather, 10)
				.AddIngredient(ItemID.SnowBlock, 20)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
