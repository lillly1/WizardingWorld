using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.Basilisk
{
	public class BasiliskBag : ModItem
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
			// Basilisk Fang — guaranteed
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Accessories.BasiliskFang>(), 1, 3, 5));

			// Sword of Gryffindor — 33% chance
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Weapons.SwordOfGryffindor>(), 3));

			// Expert exclusive: Basilisk Eye — grants Petrification immunity
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BasiliskEye>(), 1));

			// Essence of Magic
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Consumables.EssenceOfMagic>(), 1, 15, 25));

			// Healing potions
			itemLoot.Add(ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 10));

			// Gold coins
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 3, 7));
		}
	}
}
