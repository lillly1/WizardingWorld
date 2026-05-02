using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Grindylow Tooth — found while ocean fishing or dropped by Grindylows.
	/// Crafting ingredient for dark water-themed accessories.
	/// Sharp, brittle, and faintly magical.
	/// </summary>
	public class GrindylowTooth : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 14;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 10);
		}
	}
}
