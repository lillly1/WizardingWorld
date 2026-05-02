using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Spider Silk Weave — crafting material dropped by Aragog.
	/// Magically reinforced Acromantula silk, useful for powerful armor and accessories.
	/// </summary>
	public class SpiderSilkWeave : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(silver: 50);
		}
	}
}
