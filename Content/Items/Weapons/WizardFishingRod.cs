using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Wizard Fishing Rod — enchanted rod that increases wizard fish catch rates.
	/// 30% fishing power. Crafted at the Enchanting Table.
	/// Glows with magical energy, improving chances of catching wizard-specific fish.
	/// </summary>
	public class WizardFishingRod : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.autoReuse = true;
			Item.fishingPole = 30; // Decent fishing power
			Item.shootSpeed = 12f;
			Item.shoot = ProjectileID.BobberWooden; // Standard bobber
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
		}

		public override void HoldItem(Player player)
		{
			// Magical shimmer when holding
			if (Main.rand.NextBool(20))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, default, 0.3f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.FishingSeaweed, 1)
				.AddIngredient(ItemID.Wood, 10)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Alt recipe without fishing seaweed
			CreateRecipe()
				.AddIngredient(ItemID.ReinforcedFishingPole, 1)
				.AddIngredient(ItemID.FallenStar, 3)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
