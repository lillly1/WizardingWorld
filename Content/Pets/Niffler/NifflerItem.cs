using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.Niffler
{
	/// <summary>Niffler Pouch — summons a coin-hunting Niffler pet.</summary>
	public class NifflerItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<NifflerProjectile>();
			Item.buffType = ModContent.BuffType<NifflerBuff>();
			Item.value = Item.buyPrice(gold: 8);
			Item.rare = ItemRarityID.LightRed;
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
				.AddIngredient(ItemID.GoldBar, 15)
				.AddIngredient(ItemID.Leather, 5)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
