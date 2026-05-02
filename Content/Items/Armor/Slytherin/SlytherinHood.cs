using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.Slytherin
{
	[AutoloadEquip(EquipType.Head)]
	public class SlytherinHood : ModItem
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
			Item.defense = 4;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<SlytherinRobes>()
				&& legs.type == ModContent.ItemType<SlytherinLeggings>();
		}

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = SetBonusText.Value;
			// Slytherin: Cunning — +12% crit chance, 5% life steal on crits
			player.GetCritChance(DamageClass.Generic) += 12;
			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
			wp.houseSet = 2;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetCritChance(ModContent.GetInstance<SpellDamage>()) += 8;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.Emerald, 3)
				.AddIngredient(ItemID.SilverBar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
