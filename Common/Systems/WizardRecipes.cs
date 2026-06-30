using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Recipe groups and recipe modifications.
	/// Allows gold/platinum bars to be used interchangeably in wizard recipes.
	/// Also fixes the Enchanting Table to count as a vanilla Workbench.
	/// </summary>
	public class WizardRecipes : ModSystem
	{
		public override void AddRecipeGroups()
		{
			// Gold or Platinum bars interchangeable
			RecipeGroup goldGroup = new(() => "Gold/Platinum Bar",
				ItemID.GoldBar, ItemID.PlatinumBar);
			RecipeGroup.RegisterGroup("WizardingWorld:GoldBars", goldGroup);

			// Silver or Tungsten bars interchangeable
			RecipeGroup silverGroup = new(() => "Silver/Tungsten Bar",
				ItemID.SilverBar, ItemID.TungstenBar);
			RecipeGroup.RegisterGroup("WizardingWorld:SilverBars", silverGroup);

			// Iron or Lead bars interchangeable
			RecipeGroup ironGroup = new(() => "Iron/Lead Bar",
				ItemID.IronBar, ItemID.LeadBar);
			RecipeGroup.RegisterGroup("WizardingWorld:IronBars", ironGroup);

			// Copper or Tin bars
			RecipeGroup copperGroup = new(() => "Copper/Tin Bar",
				ItemID.CopperBar, ItemID.TinBar);
			RecipeGroup.RegisterGroup("WizardingWorld:CopperBars", copperGroup);

			// Corruption or Crimson creature remains for early boss summons
			RecipeGroup evilChunkGroup = new(() => "Rotten Chunk/Vertebra",
				ItemID.RottenChunk, ItemID.Vertebrae);
			RecipeGroup.RegisterGroup("WizardingWorld:EvilChunks", evilChunkGroup);

			// Any wood type
			RecipeGroup woodGroup = new(() => "Any Wood",
				ItemID.Wood, ItemID.BorealWood, ItemID.RichMahogany, ItemID.Ebonwood,
				ItemID.Shadewood, ItemID.Pearlwood, ItemID.SpookyWood, ItemID.DynastyWood,
				ItemID.PalmWood, ItemID.AshWood);
			RecipeGroup.RegisterGroup("WizardingWorld:AnyWood", woodGroup);
		}
	}
}
