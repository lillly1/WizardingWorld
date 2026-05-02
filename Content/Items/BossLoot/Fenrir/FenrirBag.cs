using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.Fenrir
{
	public class FenrirBag : ModItem
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
			// Werewolf Pelt — guaranteed
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Consumables.WerewolfPelt>(), 1, 8, 15));

			// Expert exclusive: Lycan's Bite Mark
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<LycansBiteMark>(), 1));

			// Essence of Magic
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Consumables.EssenceOfMagic>(), 1, 20, 35));

			// Greater healing potions
			itemLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 15));

			// Gold coins
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 5, 12));
		}
	}
}
