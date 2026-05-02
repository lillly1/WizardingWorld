using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.Troll
{
	public class TrollBag : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.BossBag[Type] = true;
			ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
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
			// Troll Club — 33% chance
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Weapons.TrollClub>(), 3));

			// Expert exclusive: Troll Hide — +8 defense, +10% knockback resistance
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TrollHide>(), 1));

			// Healing potions
			itemLoot.Add(ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 10));

			// Gold coins
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 1, 3));
		}
	}
}
