using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Sneakoscope — dark detector. Lights up and whistles when danger is near.
	/// Grants: danger sense, hunter, and defense bonus when enemies are within 400px.
	/// </summary>
	public class Sneakoscope : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(gold: 4);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.dangerSense = true;

			// Check for nearby enemies
			bool dangerNear = false;
			bool bossNear = false;

			foreach (var npc in Main.ActiveNPCs)
			{
				if (!npc.friendly && npc.CanBeChasedBy())
				{
					float dist = Vector2.Distance(npc.Center, player.Center);
					if (dist < 400f)
					{
						dangerNear = true;
						if (npc.boss)
							bossNear = true;
					}
				}
			}

			if (dangerNear)
			{
				// Activate! Defense boost and detection when danger is close
				player.detectCreature = true;
				player.statDefense += 5;

				// Spinning/glowing red warning particles
				if (Main.rand.NextBool(4))
				{
					Dust dust = Dust.NewDustDirect(player.position + new Vector2(0, -20), 8, 8, DustID.LifeDrain, 0f, 0f, 100, default, 0.6f);
					dust.noGravity = true;
					dust.velocity = Main.rand.NextVector2Circular(1f, 1f);
				}
			}

			if (bossNear)
			{
				// Extra defense against bosses
				player.statDefense += 5;
				player.endurance += 0.05f;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Glass, 10)
				.AddIngredient(ItemID.GoldBar, 3)
				.AddIngredient(ItemID.Lens, 3)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
