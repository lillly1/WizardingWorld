using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Golden Egg — rare drop from Hungarian Horntail. Used to craft special items.</summary>
	public class GoldenEgg : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 10);
		}

		public override bool OnPickup(Player player)
		{
			if (Common.Systems.TriwizardTournamentSystem.taskActive &&
				Common.Systems.TriwizardTournamentSystem.currentTask == 1)
			{
				Common.Systems.TriwizardTournamentSystem.OnGoldenEggRetrieved(player);
			}
			return true;
		}
	}
}
