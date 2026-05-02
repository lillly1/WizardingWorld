using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Dueling Dummy — placeable training target. Test your spell damage!</summary>
	public class DuelingDummyItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 32;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.maxStack = 10;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(silver: 50);
			Item.UseSound = SoundID.Item1;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.MultiplayerClient)
			{
				// Spawn dummy at cursor position
				var pos = Main.MouseWorld;
				NPC.NewNPC(player.GetSource_ItemUse(Item), (int)pos.X, (int)pos.Y,
					ModContent.NPCType<NPCs.Enemies.DuelingDummy>());
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 20)
				.AddIngredient(ItemID.Silk, 5)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
