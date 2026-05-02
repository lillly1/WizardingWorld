using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Unicorn Blood — rare drop ONLY from the Forbidden Forest biome.
	/// Silvery-blue blood that sustains life but at a terrible cost.
	/// Used to craft the most powerful endgame items.
	/// "The blood of a unicorn will keep you alive, even if you are an inch from death,
	/// but at a terrible price."
	/// Cannot be used as a consumable — crafting material only.
	/// </summary>
	public class UnicornBlood : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 20;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 5);
		}
	}
}
