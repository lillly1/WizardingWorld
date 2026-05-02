using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Wizard Crate — rare hardmode fishing catch containing wizard loot.
	/// Right-click to open for random wizard items.
	/// </summary>
	public class WizardCrate : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			var source = player.GetSource_OpenItem(Type);

			// Always: Essence of Magic
			player.QuickSpawnItem(source, ModContent.ItemType<EssenceOfMagic>(), Main.rand.Next(5, 15));

			// Random potion
			int[] potions = {
				ModContent.ItemType<Potions.Butterbeer>(),
				ModContent.ItemType<Potions.FelixFelicis>(),
				ModContent.ItemType<Potions.PepperupPotion>(),
				ModContent.ItemType<Potions.WolfsbanePotion>(),
				ModContent.ItemType<Potions.Gillyweed>(),
			};
			player.QuickSpawnItem(source, potions[Main.rand.Next(potions.Length)], Main.rand.Next(2, 5));

			// 20% chance: random accessory
			if (Main.rand.NextBool(5))
			{
				int[] accessories = {
					ModContent.ItemType<Accessories.Remembrall>(),
					ModContent.ItemType<Accessories.SortingHat>(),
					ModContent.ItemType<Accessories.ExtendableEars>(),
					ModContent.ItemType<Accessories.MaraudersMap>(),
				};
				player.QuickSpawnItem(source, accessories[Main.rand.Next(accessories.Length)]);
			}

			// Gold
			player.QuickSpawnItem(source, ItemID.GoldCoin, Main.rand.Next(1, 4));

			// 10% chance: Fallen Stars
			if (Main.rand.NextBool(10))
				player.QuickSpawnItem(source, ItemID.FallenStar, Main.rand.Next(5, 15));
		}
	}
}
