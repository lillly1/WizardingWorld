using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Pets.BabyDragon
{
	/// <summary>
	/// Dragon Egg (Hatched) — summons a Baby Hungarian Horntail light pet.
	/// Crafted from the Golden Egg dropped by the Horntail boss.
	/// "It's a dragon! Hagrid would be so proud."
	/// </summary>
	public class BabyDragonItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<BabyDragonProjectile>();
			Item.buffType = ModContent.BuffType<BabyDragonBuff>();
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.LightPurple;
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
				.AddIngredient(ModContent.ItemType<Items.Consumables.GoldenEgg>(), 1)
				.AddIngredient(ItemID.HellstoneBar, 5)
				.AddIngredient(ModContent.ItemType<Items.Consumables.EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
