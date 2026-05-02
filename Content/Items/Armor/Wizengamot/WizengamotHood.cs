using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.Wizengamot
{
	/// <summary>
	/// Wizengamot Hood — post-Moon Lord endgame spell armor.
	/// The robes of the Wizarding High Court — the pinnacle of magical authority.
	/// Set bonus: Arcane Authority — +30% spell damage, -20% mana cost,
	/// spells have +10% area of effect (simulated via projectile scale).
	/// </summary>
	[AutoloadEquip(EquipType.Head)]
	public class WizengamotHood : ModItem
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
			Item.value = Item.sellPrice(gold: 20);
			Item.rare = ItemRarityID.Red;
			Item.defense = 16;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<WizengamotRobes>()
				&& legs.type == ModContent.ItemType<WizengamotBoots>();
		}

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = SetBonusText.Value;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.30f;
			player.manaCost -= 0.20f;
			player.statManaMax2 += 60;
			player.manaRegen += 8;

			// Arcane authority aura
			if (Main.rand.NextBool(10))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.PurpleTorch, 0f, -0.5f, 50, default, 0.5f);
				dust.noGravity = true;
			}
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.15f;
			player.statManaMax2 += 40;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DementorsShroud>(), 5)
				.AddIngredient(ItemID.LunarBar, 8)
				.AddIngredient(ItemID.FragmentNebula, 10)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 20)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
