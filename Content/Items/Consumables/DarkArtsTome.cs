using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Dark Arts Tome — crafting material from Bellatrix Lestrange. Used for dark magic items.</summary>
	public class DarkArtsTome : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.sellPrice(gold: 3);
		}
	}
}
