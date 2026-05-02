using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Azkaban Ward Sigil — a temporary ward circle for surviving the prison's despair.
	/// Provides light, defense, and resistance to despair buildup.
	/// </summary>
	public class AzkabanWardSigil : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.sellPrice(gold: 2);
			Item.UseSound = SoundID.Item29;
		}

		public override bool? UseItem(Player player)
		{
			player.AddBuff(ModContent.BuffType<Buffs.WardOfHopeBuff>(), 60 * 60 * 3);
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ModContent.ItemType<DementorsShroud>(), 2)
				.AddIngredient(ItemID.SoulofLight, 3)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 6)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
