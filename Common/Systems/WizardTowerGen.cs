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
	/// Wizard Tower World Generation — spawns a small wizard tower structure
	/// on the surface during worldgen. Contains a chest with starter wizard loot.
	/// The tower is made of stone brick with a pointed roof, glowing windows,
	/// and a chest containing a wand, potions, and Essence of Magic.
	/// Only one tower spawns per world.
	/// </summary>
	public class WizardTowerGen : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			// Insert after "Micro Biomes" pass
			int index = tasks.FindIndex(t => t.Name.Equals("Micro Biomes"));
			if (index != -1)
			{
				tasks.Insert(index + 1, new WizardTowerPass("Wizard Tower", 50f));
			}
		}
	}

	public class WizardTowerPass : GenPass
	{
		public WizardTowerPass(string name, float loadWeight) : base(name, loadWeight) { }

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Building a Wizard Tower...";

			// Try to find a good surface location
			int attempts = 0;
			bool placed = false;

			while (!placed && attempts < 1000)
			{
				attempts++;
				int x = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
				int surfaceY = FindSurface(x);

				if (surfaceY <= 0 || surfaceY > Main.worldSurface)
					continue;

				// Check if area is flat enough (5 tiles wide)
				bool flat = true;
				for (int dx = -2; dx <= 2; dx++)
				{
					int sy = FindSurface(x + dx);
					if (Math.Abs(sy - surfaceY) > 2)
					{
						flat = false;
						break;
					}
				}

				if (!flat)
					continue;

				// Build the tower!
				BuildTower(x, surfaceY);
				placed = true;
			}
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

		private void BuildTower(int x, int groundY)
		{
			int towerWidth = 7;
			int towerHeight = 12;
			int halfW = towerWidth / 2;

			// Clear interior
			for (int dx = -halfW; dx <= halfW; dx++)
			{
				for (int dy = -towerHeight; dy <= 0; dy++)
				{
					int tx = x + dx;
					int ty = groundY + dy;
					if (WorldGen.InWorld(tx, ty, 10))
					{
						WorldGen.KillTile(tx, ty, false, false, true);
						WorldGen.KillWall(tx, ty, false);
					}
				}
			}

			// Build walls (stone brick)
			for (int dy = -towerHeight; dy <= 0; dy++)
			{
				int ty = groundY + dy;

				// Left wall
				WorldGen.PlaceTile(x - halfW, ty, TileID.GrayBrick, true, true);
				// Right wall
				WorldGen.PlaceTile(x + halfW, ty, TileID.GrayBrick, true, true);

				// Interior background wall
				for (int dx = -halfW + 1; dx < halfW; dx++)
				{
					int tx = x + dx;
					if (WorldGen.InWorld(tx, ty, 10))
						WorldGen.PlaceWall(tx, ty, WallID.GrayBrick, true);
				}
			}

			// Floor
			for (int dx = -halfW; dx <= halfW; dx++)
			{
				WorldGen.PlaceTile(x + dx, groundY, TileID.GrayBrick, true, true);
			}

			// Ceiling
			for (int dx = -halfW; dx <= halfW; dx++)
			{
				WorldGen.PlaceTile(x + dx, groundY - towerHeight, TileID.GrayBrick, true, true);
			}

			// Pointed roof (pyramid)
			for (int layer = 0; layer < 4; layer++)
			{
				int roofY = groundY - towerHeight - 1 - layer;
				int roofHalf = halfW - layer;
				for (int dx = -roofHalf; dx <= roofHalf; dx++)
				{
					WorldGen.PlaceTile(x + dx, roofY, TileID.WoodenBeam, true, true);
				}
			}

			// Place a chest with wizard loot inside the tower
			int chestX = x - 1;
			int chestY = groundY - 1;

			// Make sure there's floor under the chest
			WorldGen.PlaceTile(chestX, groundY, TileID.GrayBrick, true, true);
			WorldGen.PlaceTile(chestX + 1, groundY, TileID.GrayBrick, true, true);

			int chestIndex = WorldGen.PlaceChest(chestX, chestY, TileID.Containers, false, 0);
			if (chestIndex >= 0)
			{
				Chest chest = Main.chest[chestIndex];
				int slot = 0;

				// Starter wand
				chest.item[slot].SetDefaults(ModContent.ItemType<Content.Items.Weapons.Wands.WillowWand>());
				slot++;

				// Some potions
				chest.item[slot].SetDefaults(ModContent.ItemType<Content.Items.Consumables.Potions.Butterbeer>());
				chest.item[slot].stack = 5;
				slot++;

				chest.item[slot].SetDefaults(ModContent.ItemType<Content.Items.Consumables.ChocolateFrog>());
				chest.item[slot].stack = 3;
				slot++;

				// Essence of Magic starter
				chest.item[slot].SetDefaults(ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>());
				chest.item[slot].stack = 15;
				slot++;

				// Fallen Stars for crafting
				chest.item[slot].SetDefaults(ItemID.FallenStar);
				chest.item[slot].stack = 10;
				slot++;

				// Random accessory
				int[] possibleAccessories = {
					ModContent.ItemType<Content.Items.Accessories.Remembrall>(),
					ModContent.ItemType<Content.Items.Accessories.MaraudersMap>(),
					ModContent.ItemType<Content.Items.Accessories.SortingHat>(),
				};
				chest.item[slot].SetDefaults(possibleAccessories[WorldGen.genRand.Next(possibleAccessories.Length)]);
				slot++;

				// Daily Prophet
				chest.item[slot].SetDefaults(ModContent.ItemType<Content.Items.Consumables.DailyProphet>());
				slot++;

				// Gold coins
				chest.item[slot].SetDefaults(ItemID.GoldCoin);
				chest.item[slot].stack = 3;
			}

			// Place a torch inside
			WorldGen.PlaceTile(x, groundY - 5, TileID.Torches, true, true);
		}
	}
}
