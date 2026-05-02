using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.Voldemort
{
	public class VoldemortBag : ModItem
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
			// Elder Wand — guaranteed
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Weapons.Wands.ElderWand>(), 1));

			// Expert exclusive: Fragment of Voldemort's Soul — ultimate dark accessory
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulFragment>(), 1));

			// Dark Arts Tome — 5-10 stack
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Consumables.DarkArtsTome>(), 1, 5, 10));

			// Essence of Magic — massive amount
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Consumables.EssenceOfMagic>(), 1, 50, 80));

			// Super healing potions
			itemLoot.Add(ItemDropRule.Common(ItemID.SuperHealingPotion, 1, 5, 15));

			// Gaunt's Ring — contains the Resurrection Stone, purifiable after Horcrux Hunt
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Accessories.GauntsRing>(), 1));

			// Platinum coins
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 10, 20));
		}
	}
}
