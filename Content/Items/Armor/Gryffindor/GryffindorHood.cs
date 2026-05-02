using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.Gryffindor
{
	[AutoloadEquip(EquipType.Head)]
	public class GryffindorHood : ModItem
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
			Item.defense = 5;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<GryffindorRobes>()
				&& legs.type == ModContent.ItemType<GryffindorLeggings>();
		}

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = SetBonusText.Value;
			// Gryffindor: Bravery — +15% damage, +10% melee speed
			player.GetDamage(DamageClass.Generic) += 0.15f;
			player.GetAttackSpeed(DamageClass.Melee) += 0.10f;
			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
			wp.houseSet = 1;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.08f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.Ruby, 3)
				.AddIngredient(ItemID.GoldBar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
