using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>Sorting Hat — boosts the set bonus of any Hogwarts house armor currently worn.</summary>
	public class SortingHat : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(gold: 5);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();

			switch (wp.houseSet)
			{
				case 1: // Gryffindor — extra damage
					player.GetDamage(DamageClass.Generic) += 0.08f;
					break;
				case 2: // Slytherin — extra crit
					player.GetCritChance(DamageClass.Generic) += 6;
					break;
				case 3: // Ravenclaw — extra mana
					player.statManaMax2 += 30;
					player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.10f;
					break;
				case 4: // Hufflepuff — extra defense
					player.statDefense += 5;
					player.lifeRegen += 2;
					break;
				case 5: // Dark Wizard — extra dark power
					player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.12f;
					break;
				default:
					// No house armor — small generic bonus
					player.statDefense += 2;
					player.statManaMax2 += 10;
					break;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 15)
				.AddIngredient(ItemID.FallenStar, 10)
				.AddIngredient(ItemID.GoldBar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
