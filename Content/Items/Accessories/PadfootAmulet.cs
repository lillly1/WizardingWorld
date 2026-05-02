using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Padfoot's Amulet — Sirius Black's Animagus amulet.
	/// Grants wolf-like speed: +20% movement speed, +2 max run speed.
	/// Also grants stealth: -200 aggro, enemies less likely to target you.
	/// "Mischief managed."
	/// </summary>
	public class PadfootAmulet : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 8);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// Wolf speed
			player.moveSpeed += 0.20f;
			player.maxRunSpeed += 2f;

			// Stealth — the dog slips through unnoticed
			player.aggro -= 200;

			// Dark paw print dust occasionally
			if (!hideVisual && Main.rand.NextBool(20) && player.velocity.Length() > 3f)
			{
				Dust dust = Dust.NewDustDirect(player.Bottom, 8, 4, DustID.Smoke, 0f, 0f, 150, default, 0.5f);
				dust.noGravity = false;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SoulofNight, 8)
				.AddIngredient(ItemID.MoonCharm, 1)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
