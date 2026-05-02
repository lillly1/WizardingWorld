using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.Aragog
{
	public class AragogBag : ModItem
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
			// Spider Silk Weave — guaranteed
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Consumables.SpiderSilkWeave>(), 1, 8, 15));

			// Expert exclusive: Aragog's Fang — +15% crit, extra loot chance
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AragogsFang>(), 1));

			// Spider Fang (vanilla)
			itemLoot.Add(ItemDropRule.Common(ItemID.SpiderFang, 1, 5, 10));

			// Greater Healing potions
			itemLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 10));

			// Gold coins
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 5, 10));
		}
	}
}
