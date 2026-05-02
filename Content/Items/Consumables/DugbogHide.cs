using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Dugbog Hide — tough, bark-like skin from the Dugbog.
	/// Used in stealth and camouflage items. Naturally blends with the environment.
	/// </summary>
	public class DugbogHide : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 8);
		}
	}
}
