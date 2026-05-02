using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Dementor's Shroud — endgame crafting material from the Dementor King.</summary>
	public class DementorsShroud : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.sellPrice(gold: 5);
		}
	}
}
