using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Unbreakable Vow — binds the player's fate to their minions.
	/// +25% summon damage, +1 max minion, but player takes 15% more damage.
	/// High risk, high reward for summoner builds.
	/// "You must understand that one who breaks an Unbreakable Vow... dies."
	/// </summary>
	public class UnbreakableVow : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 10);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// Power boost
			player.GetDamage(DamageClass.Summon) += 0.25f;
			player.maxMinions += 1;

			// Cost — take more damage
			player.endurance -= 0.15f;

			// Binding chains visual
			if (Main.rand.NextBool(15))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.GoldCoin, 0f, 0f, 100, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SoulofMight, 5)
				.AddIngredient(ItemID.SoulofFright, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
