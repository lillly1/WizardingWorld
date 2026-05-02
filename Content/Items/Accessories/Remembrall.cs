using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>Remembrall — glows red when enemies are near. Grants danger sense and spelunker effect.</summary>
	public class Remembrall : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(gold: 2);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.dangerSense = true;
			player.findTreasure = true;

			// Red glow when enemies are within 500 pixels
			bool nearEnemy = false;
			foreach (var npc in Main.ActiveNPCs)
			{
				if (!npc.friendly && npc.CanBeChasedBy()
					&& Vector2.Distance(npc.Center, player.Center) < 500f)
				{
					nearEnemy = true;
					break;
				}
			}

			if (nearEnemy && Main.rand.NextBool(5))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.LifeDrain, 0f, 0f, 100, default, 0.6f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Glass, 10)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ItemID.Ruby, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
