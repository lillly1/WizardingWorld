using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.Ravenclaw
{
	[AutoloadEquip(EquipType.Head)]
	public class RavenclawHood : ModItem
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
			Item.defense = 3;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<RavenclawRobes>()
				&& legs.type == ModContent.ItemType<RavenclawLeggings>();
		}

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = SetBonusText.Value;
			// Ravenclaw: Wisdom — +20% spell damage, +40 max mana, mana regen
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.20f;
			player.statManaMax2 += 40;
			player.manaRegen += 5;
			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
			wp.houseSet = 3;
		}

		public override void UpdateEquip(Player player)
		{
			player.statManaMax2 += 20;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.Sapphire, 3)
				.AddIngredient(ItemID.CopperBar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
