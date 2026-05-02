using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Enchanted Tadpole — Forbidden Forest fishing catch.
	/// Excellent bait (40 power) or can be consumed for Essence of Magic.
	/// </summary>
	public class EnchantedTadpole : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 12;
			Item.maxStack = 99;
			Item.bait = 40; // Excellent bait
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(silver: 15);
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.consumable = true;
			Item.UseSound = SoundID.Item2;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				player.QuickSpawnItem(player.GetSource_ItemUse(Item),
					ModContent.ItemType<EssenceOfMagic>(), Main.rand.Next(2, 5));
			}
			return true;
		}
	}
}
