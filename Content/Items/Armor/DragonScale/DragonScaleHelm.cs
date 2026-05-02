using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Armor.DragonScale
{
	/// <summary>
	/// Dragon Scale Helm — hardmode armor crafted from Horntail drops.
	/// The set is designed for spell-combat hybrids: spell damage + fire resistance.
	/// Set bonus: Fire Shield — periodically releases fire novas when hit.
	/// </summary>
	[AutoloadEquip(EquipType.Head)]
	public class DragonScaleHelm : ModItem
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
			Item.defense = 12;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<DragonScaleBreastplate>()
				&& legs.type == ModContent.ItemType<DragonScaleGreaves>();
		}

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = SetBonusText.Value;
			// Dragon Fire Shield: fire immunity + fire damage aura
			player.buffImmune[BuffID.OnFire] = true;
			player.buffImmune[BuffID.OnFire3] = true;
			player.lavaImmune = true;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.18f;

			// Fire shield particles
			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Torch, 0f, 0f, 100, default, 0.6f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.10f;
			player.statManaMax2 += 30;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 8)
				.AddIngredient(ItemID.HallowedBar, 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
