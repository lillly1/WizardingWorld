using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Philosopher's Stone — endgame accessory crafted from all boss materials.
	/// Grants: +10% all damage, +8 life regen, enemies drop extra gold, reduced potion cooldown.
	/// The ultimate wizard's treasure.
	/// </summary>
	public class PhilosophersStone : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.sellPrice(gold: 50);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// Elixir of Life — powerful regen
			player.lifeRegen += 8;
			player.statLifeMax2 += 40;

			// Transmutation — enemies drop more gold
			player.goldRing = true;

			// Alchemical mastery
			player.GetDamage(DamageClass.Generic) += 0.10f;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.10f;

			// Faster potion recovery
			player.pStone = true; // Vanilla Philosopher's Stone effect

			// Golden aura
			if (Main.rand.NextBool(10))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.GoldCoin, 0f, -1f, 100, default, 0.6f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 5)
				.AddIngredient(ModContent.ItemType<Consumables.GoldenEgg>(), 1)
				.AddIngredient(ModContent.ItemType<BasiliskFang>(), 3)
				.AddIngredient(ItemID.GoldBar, 20)
				.AddIngredient(ItemID.LifeCrystal, 3)
				.AddIngredient(ItemID.ManaCrystal, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
