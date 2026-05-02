using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Essence of Magic — the universal crafting material for the Wizarding World mod.
	/// Drops from ALL mod enemies. Used in mid-to-late game recipes.
	/// This is the "glue" material that ties the mod's crafting economy together.
	/// Think of it as concentrated magical energy left behind when magical creatures die.
	/// </summary>
	public class EssenceOfMagic : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 999;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 2);
		}
	}
}
