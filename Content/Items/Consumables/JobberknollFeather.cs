using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Jobberknoll Feather — key ingredient for Truth Serums and Memory Potions.
	/// Dropped by the Jobberknoll bird. The feather contains echoes of every
	/// sound the bird ever heard, making it ideal for truth-compelling magic.
	/// </summary>
	public class JobberknollFeather : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 18;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(silver: 15);
		}
	}
}
