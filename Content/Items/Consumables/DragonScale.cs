using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Dragon Scale — crafting material from the Hungarian Horntail.</summary>
	public class DragonScale : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(gold: 2);
		}
	}
}
