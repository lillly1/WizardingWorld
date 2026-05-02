using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.DementorKing
{
	public class DementorKingBag : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.BossBag[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = 99;
			Item.consumable = true;
			Item.rare = ItemRarityID.Expert;
		}

		public override bool CanRightClick() => true;

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			// Dementor's Shroud — guaranteed
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Consumables.DementorsShroud>(), 1, 8, 15));

			// Expert exclusive: Soul Siphon
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulSiphon>(), 1));

			// Essence of Magic — massive amount
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Consumables.EssenceOfMagic>(), 1, 30, 50));

			// Super healing potions
			itemLoot.Add(ItemDropRule.Common(ItemID.SuperHealingPotion, 1, 5, 15));

			// Gold coins
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 10, 20));
		}
	}
}
