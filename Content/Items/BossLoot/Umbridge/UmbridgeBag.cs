using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.Umbridge
{
	public class UmbridgeBag : ModItem
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
			// Umbridge's Quill -- guaranteed in bag
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Weapons.UmbridgesQuill>(), 1));

			// Expert exclusive: Ministry Badge
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MinistryBadge>(), 1));

			// Essence of Magic
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Consumables.EssenceOfMagic>(), 1, 20, 35));

			// Greater healing potions
			itemLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 15));

			// Gold coins
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 6, 12));
		}
	}
}
