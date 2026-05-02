using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Nosebleed Nougat — Weasleys' Wizard Wheezes.
	/// Eat one to trade HP for mana: lose 50 HP, instantly restore 200 mana.
	/// Also grants 30 seconds of boosted mana regen.
	/// For mana-hungry spell casters who don't mind bleeding.
	/// </summary>
	public class NosebleedNougat : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(silver: 15);
			Item.UseSound = SoundID.Item3;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return true;

			// Cost: lose 50 HP
			player.statLife -= 50;
			if (player.statLife < 1)
				player.statLife = 1;

			// Gain: restore 200 mana + regen buff
			player.statMana += 200;
			if (player.statMana > player.statManaMax2)
				player.statMana = player.statManaMax2;

			player.ManaEffect(200);
			player.AddBuff(BuffID.ManaRegeneration, 1800); // 30 seconds

			// Blood nosebleed visual
			for (int i = 0; i < 8; i++)
			{
				Dust dust = Dust.NewDustDirect(player.position + new Vector2(player.width / 2, 8), 4, 4, DustID.Blood, Main.rand.NextFloat(-1f, 1f), 1f, 100, default, 0.8f);
				dust.noGravity = false;
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(5)
				.AddIngredient(ItemID.Mushroom, 2)
				.AddIngredient(ItemID.Deathweed, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
