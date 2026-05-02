using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Archmage's Crate — hardmode upgrade of the Wizard Crate.
	/// Contains better wizard loot: hardmode potions, rare accessories, more Essence.
	/// Caught while fishing in Hardmode (replaces some Wizard Crate catches).
	/// </summary>
	public class WizardCrateHardmode : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(gold: 5);
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			var source = player.GetSource_OpenItem(Type);

			// Guaranteed: lots of Essence
			player.QuickSpawnItem(source, ModContent.ItemType<EssenceOfMagic>(), Main.rand.Next(10, 25));

			// Hardmode potions (2 random)
			int[] hmPotions = {
				ModContent.ItemType<Potions.FelixFelicis>(),
				ModContent.ItemType<Potions.Veritaserum>(),
				ModContent.ItemType<Potions.Amortentia>(),
				ModContent.ItemType<Potions.WolfsbanePotion>(),
				ModContent.ItemType<Potions.PepperupPotion>(),
				ModContent.ItemType<Potions.SeekersReflexes>(),
				ModContent.ItemType<Potions.AquaFortis>(),
				ModContent.ItemType<Potions.DraconisElixir>(),
			};
			player.QuickSpawnItem(source, hmPotions[Main.rand.Next(hmPotions.Length)], Main.rand.Next(2, 4));
			player.QuickSpawnItem(source, hmPotions[Main.rand.Next(hmPotions.Length)], Main.rand.Next(1, 3));

			// 30% chance: rare accessory
			if (Main.rand.NextBool(3))
			{
				int[] accessories = {
					ModContent.ItemType<Accessories.WandHolster>(),
					ModContent.ItemType<Accessories.Sneakoscope>(),
					ModContent.ItemType<Accessories.Deluminator>(),
					ModContent.ItemType<Accessories.DragonScaleRing>(),
					ModContent.ItemType<Accessories.SpiderSilkCloak>(),
				};
				player.QuickSpawnItem(source, accessories[Main.rand.Next(accessories.Length)]);
			}

			// 15% chance: boss material
			if (Main.rand.NextBool(7))
			{
				int[] materials = {
					ModContent.ItemType<DragonScale>(),
					ModContent.ItemType<SpiderSilkWeave>(),
					ModContent.ItemType<CerberusFang>(),
					ModContent.ItemType<PhoenixAsh>(),
				};
				player.QuickSpawnItem(source, materials[Main.rand.Next(materials.Length)], Main.rand.Next(1, 3));
			}

			// Gold
			player.QuickSpawnItem(source, ItemID.GoldCoin, Main.rand.Next(3, 8));
		}
	}
}
