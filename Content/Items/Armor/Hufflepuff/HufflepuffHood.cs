using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Armor.Hufflepuff
{
	[AutoloadEquip(EquipType.Head)]
	public class HufflepuffHood : ModItem
	{
		public static LocalizedText SetBonusText { get; private set; }

		public override void SetStaticDefaults()
		{
			SetBonusText = this.GetLocalization("SetBonus");
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Orange;
			Item.defense = 7;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<HufflepuffRobes>()
				&& legs.type == ModContent.ItemType<HufflepuffLeggings>();
		}

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = SetBonusText.Value;
			// Hufflepuff: Loyalty — +8 defense, +4 life regen, thorns
			player.statDefense += 8;
			player.lifeRegen += 4;
			player.thorns = 0.3f;
			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
			wp.houseSet = 4;
		}

		public override void UpdateEquip(Player player)
		{
			player.statDefense += 3;
			player.lifeRegen += 1;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.Topaz, 3)
				.AddIngredient(ItemID.IronBar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
