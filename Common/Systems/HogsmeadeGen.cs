using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Hogsmeade Village — Canon Tier A.
	///
	/// Generates a small village structure on the surface during world creation.
	/// Contains 3-4 buildings representing:
	/// - Three Broomsticks (inn/rest point)
	/// - Honeydukes (sweet shop)
	/// - Weasleys' Wizard Wheezes branch (joke shop)
	/// - Shrieking Shack (nearby, boarded up, requires Alohomora)
	///
	/// The village provides a recognizable second hub distinct from the Wizard Tower.
	/// Placed on the opposite side of the world from spawn for exploration reward.
	/// </summary>
	public class HogsmeadeGen : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int index = tasks.FindIndex(t => t.Name.Equals("Micro Biomes"));
			if (index != -1)
			{
				tasks.Insert(index + 1, new HogsmeadePass("Hogsmeade Village", 40f));
			}
		}
	}

	public class HogsmeadePass : GenPass
	{
		public HogsmeadePass(string name, float loadWeight) : base(name, loadWeight) { }

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Building Hogsmeade Village...";

			// Place on the far side of the world from spawn
			int worldCenter = Main.maxTilesX / 2;
			int targetX;
			if (Main.spawnTileX < worldCenter)
				targetX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.65), (int)(Main.maxTilesX * 0.8));
			else
				targetX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2), (int)(Main.maxTilesX * 0.35));

			int surfaceY = FindSurface(targetX);
			if (surfaceY <= 0 || surfaceY > Main.worldSurface)
				return;

			// Build 3 small buildings in a row
			BuildBuilding(targetX - 20, surfaceY, 8, 6, "Three Broomsticks");
			BuildBuilding(targetX, surfaceY, 7, 5, "Honeydukes");
			BuildBuilding(targetX + 18, surfaceY, 7, 5, "Joke Shop");

			// Shrieking Shack — offset and elevated, looks abandoned
			int shackX = targetX - 40;
			int shackY = FindSurface(shackX);
			if (shackY > 0)
				BuildShriekingShack(shackX, shackY);

			// Place a signpost
			WorldGen.PlaceTile(targetX - 5, surfaceY - 1, TileID.Signs, true, true);
		}

		private int FindSurface(int x)
		{
			for (int y = 50; y < Main.worldSurface; y++)
			{
				if (Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType])
					return y;
			}
			return -1;
		}

		private void BuildBuilding(int x, int groundY, int width, int height, string name)
		{
			int halfW = width / 2;

			// Clear interior
			for (int dx = -halfW; dx <= halfW; dx++)
				for (int dy = -height; dy <= 0; dy++)
				{
					int tx = x + dx, ty = groundY + dy;
					if (WorldGen.InWorld(tx, ty, 10))
					{
						WorldGen.KillTile(tx, ty, false, false, true);
						WorldGen.KillWall(tx, ty, false);
					}
				}

			// Walls
			for (int dy = -height; dy <= 0; dy++)
			{
				WorldGen.PlaceTile(x - halfW, groundY + dy, TileID.WoodBlock, true, true);
				WorldGen.PlaceTile(x + halfW, groundY + dy, TileID.WoodBlock, true, true);
				for (int dx = -halfW + 1; dx < halfW; dx++)
					WorldGen.PlaceWall(x + dx, groundY + dy, WallID.Wood, true);
			}

			// Floor + ceiling
			for (int dx = -halfW; dx <= halfW; dx++)
			{
				WorldGen.PlaceTile(x + dx, groundY, TileID.WoodBlock, true, true);
				WorldGen.PlaceTile(x + dx, groundY - height, TileID.WoodBlock, true, true);
			}

			// Door
			WorldGen.PlaceTile(x, groundY - 1, TileID.ClosedDoor, true, true);

			// Torch inside
			WorldGen.PlaceTile(x, groundY - height + 2, TileID.Torches, true, true);
		}

		private void BuildShriekingShack(int x, int groundY)
		{
			// Smaller, darker, more decrepit
			BuildBuilding(x, groundY, 6, 5, "Shrieking Shack");

			// Board up the door with extra wood blocks (Alohomora needed conceptually)
			WorldGen.PlaceTile(x - 1, groundY - 1, TileID.WoodBlock, true, true);
			WorldGen.PlaceTile(x + 1, groundY - 1, TileID.WoodBlock, true, true);

			// Place cobwebs inside for creepy feel
			for (int dx = -2; dx <= 2; dx++)
				for (int dy = -4; dy <= -2; dy++)
					if (WorldGen.genRand.NextBool(3))
						WorldGen.PlaceTile(x + dx, groundY + dy, TileID.Cobweb, true, true);
		}
	}
}
