using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.PygmyPuff
{
	/// <summary>Pygmy Puff — miniature Puffskein. Weasleys' Wizard Wheezes' best-selling pet.</summary>
	public class PygmyPuffItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<PygmyPuffProjectile>();
			Item.buffType = ModContent.BuffType<PygmyPuffBuff>();
			Item.value = Item.buyPrice(gold: 3);
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
				.AddIngredient(ItemID.PinkGel, 10)
				.AddIngredient(ItemID.Silk, 5)
				.AddIngredient(ItemID.FallenStar, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
