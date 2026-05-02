using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.DarkWizard
{
	[AutoloadEquip(EquipType.Head)]
	public class DarkWizardHood : ModItem
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
			Item.value = Item.sellPrice(gold: 8);
			Item.rare = ItemRarityID.Pink;
			Item.defense = 8;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<DarkWizardRobes>()
				&& legs.type == ModContent.ItemType<DarkWizardLeggings>();
		}

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = SetBonusText.Value;
			// Dark Wizard: +25% spell damage, +15% crit, life steal on spell hits
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.25f;
			player.GetCritChance(ModContent.GetInstance<SpellDamage>()) += 15;
			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
			wp.houseSet = 5;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.12f;
			player.statManaMax2 += 30;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 15)
				.AddIngredient(ItemID.SoulofNight, 10)
				.AddIngredient(ItemID.DarkShard, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
